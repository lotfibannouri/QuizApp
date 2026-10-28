using Authentication.web.Dialogs;
using Authentication.web.Services;
using Authentication.web.Shared.Questions;
using Authentication.web.Shared.Test;
using Authentication.web.utility;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using MudBlazor;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.Proctoring_DTO;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using QuizApp.Entities.Conception_Entities.DTO.ReportDTO;
using System;
using System.Security.Claims;
using System.Timers;
namespace Authentication.web.Pages
{
    public partial class QuizTestVisuliser : IAsyncDisposable
    {
        /// <summary>Intervalle entre deux photos de surveillance. Unique point de réglage.</summary>
        private const int SnapshotIntervalSeconds = 20;

        /// <summary>Fréquence d'analyse de l'image pour la détection de présence.</summary>
        private const int FaceCheckIntervalMs = 2000;

        /// <summary>
        /// Durée d'absence continue au-delà de laquelle un incident est signalé. Assez long
        /// pour absorber les faux négatifs (étudiant qui se penche, contre-jour), assez court
        /// pour que sortir du champ soit détecté rapidement.
        /// </summary>
        private const int FaceMissingThresholdSeconds = 5;

        [Parameter]
        public string QuizId { get; set; }

        [Inject]
        public IQuestionService _questionService { get; set; }
        [Inject]
        public IQuizService _quizService { get; set; }
        [Inject]
        public IReportService _reportService { get; set; }
        [Inject]
        public IDialogService DialogService { get; set; }
        [Inject]
        public IProctoringService _proctoringService { get; set; }
        [Inject]
        public AuthenticationStateProvider _authenticationStateProvider { get; set; }
        [Inject]
        public NavigationManager _navigationManager { get; set; }

        private bool _started = false;
        private bool _cameraReady = false;
        private string _cameraError = string.Empty;
        private string _reportError = string.Empty;
        private string _userId = string.Empty;
        private Guid _sessionId = Guid.NewGuid();

        // Enregistrés avec le résultat : début réel du test et soumission par le minuteur.
        private DateTime? _startedAt;
        private bool _autoSubmitted;
        private int _secondsSinceSnapshot = 0;
        private DotNetObjectReference<QuizTestVisuliser>? _dotNetRef;
        private bool _cameraStopped = false;
        private bool _previewAttached = false;
        private bool _faceWatchStarted = false;

        /// <summary>
        /// Presence de l'etudiant devant la camera, pilotee par le module JS.
        /// true par defaut : tant que la detection n'a rien dit, on ne soupconne personne.
        /// Determinant quand la surveillance est indisponible (FaceWatchUnavailable) : sans
        /// cela le bandeau d'avertissement resterait affiche en permanence sans raison.
        /// </summary>
        private bool _faceVisible = true;
        private int _secondsOutOfFrame = 0;

        /// <summary>
        /// Module ES scripts/proctoringInterop.js, importe a la demande plutot que charge par
        /// une balise dans index.html : un index.html en cache ne peut plus desactiver la
        /// surveillance a notre insu.
        /// </summary>
        private IJSObjectReference? _proctoringModule;

        public List<ListQuestionDTO> _questions { get; set; }

        public Dictionary<int, IQuestionPersist> questionPersists { get; set; }

        public RenderFragment QuestionListRF { get; set; }

        private TimeSpan totalTime;
        private TimeSpan remainingTime;
        private System.Timers.Timer timer;

        public bool IsTestFinished { get; private set; } = false;

        private QuizReportRequestDTO _reportRequest;
        private QuizScoreSummaryDTO _scoreSummary;

        private int currentIndex = 0;
        public int CurrentQuestionNumber => currentIndex + 1;
        public int TotalQuestions => _questions?.Count ?? 0;
        public bool IsFirstQuestion => currentIndex == 0;
        public bool IsLastQuestion => currentIndex >= TotalQuestions - 1;

        public void NextQuestion()
        {
            if (IsTestFinished)
                return;

            if (currentIndex < TotalQuestions - 1)
                currentIndex++;
        }

        public void PreviousQuestion()
        {
            if (IsTestFinished)
                return;

            if (currentIndex > 0)
                currentIndex--;
        }

        protected override async Task OnInitializedAsync()
        {
            if (!string.IsNullOrEmpty(QuizId))
            {
                var quiz = await _quizService.GetQuizById(QuizId);
                totalTime = TimeSpan.FromMinutes(quiz.duree_quiz);
                _questions = await _questionService.GetQuestionsByQuizId(QuizId);
                questionPersists = new Dictionary<int, IQuestionPersist>();
            }
            QuestionListRF = buildQuestionRF();
            remainingTime = totalTime;

            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            _userId = authState.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            // Le chronomètre ne démarre plus ici : il est lancé par StartTest(), une fois la
            // caméra active, pour que surveillance et épreuve aient la même durée de vie.
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            // Seul moment où l'élément <video> existe dans le DOM pour y rattacher le flux.
            if (firstRender)
            {
                await RequestCamera();
                return;
            }

            // Au démarrage du test, Blazor détruit le <video> de l'écran de consentement et
            // en crée un nouveau : il faut y rebrancher le flux, sinon l'aperçu reste noir et
            // les captures restent figées sur la dernière image de l'ancien élément.
            if (_started && !IsTestFinished && !_previewAttached && _cameraReady)
            {
                _previewAttached = await AttachPreview();

                // La détection ne démarre qu'une fois l'épreuve lancée et l'aperçu rebranché :
                // surveiller l'écran de consentement n'a pas de sens, et le détecteur doit
                // analyser le <video> définitif, pas celui qui vient d'être détruit.
                if (_previewAttached && !_faceWatchStarted)
                    await StartFaceWatch();
            }
        }

        private async Task StartFaceWatch()
        {
            if (_proctoringModule == null)
                return;

            _faceWatchStarted = true;

            CameraStartResult result;
            try
            {
                result = await _proctoringModule.InvokeAsync<CameraStartResult>(
                    "startFaceWatch", FaceCheckIntervalMs, FaceMissingThresholdSeconds);
            }
            catch (JSException ex)
            {
                result = new CameraStartResult { Ok = false, Reason = ex.Message };
            }

            // Une surveillance inactive doit laisser une trace : sans cet incident, une session
            // sans alerte serait indiscernable d'une session réellement sans anomalie.
            if (!result.Ok)
                await LogIncident("FaceWatchUnavailable", result.Reason);
        }

        private async Task<bool> AttachPreview()
        {
            if (_proctoringModule == null)
                return false;

            try
            {
                return await _proctoringModule.InvokeAsync<bool>("attach", "proctoring-preview");
            }
            catch (JSException)
            {
                // Élément pas encore présent : un nouveau rendu réessaiera.
                return false;
            }
        }

        private async Task RequestCamera()
        {
            _dotNetRef ??= DotNetObjectReference.Create(this);

            CameraStartResult result;
            try
            {
                _proctoringModule ??= await JSRuntime.InvokeAsync<IJSObjectReference>(
                    "import", "./scripts/proctoringInterop.js");

                result = await _proctoringModule.InvokeAsync<CameraStartResult>(
                    "start", "proctoring-preview", _dotNetRef);
            }
            catch (JSException ex)
            {
                // Module de surveillance introuvable ou en échec au chargement : on informe
                // au lieu de casser le rendu. L'import échoue ici de façon explicite, alors
                // qu'une balise <script> manquante échouait silencieusement.
                result = new CameraStartResult { Ok = false, Reason = "script de surveillance non chargé (" + ex.Message + ")" };
            }

            _cameraReady = result.Ok;
            _cameraError = result.Ok
                ? string.Empty
                : $"Accès à la caméra impossible ({result.Reason}). Autorisez la caméra, rechargez la page avec Ctrl+F5, puis réessayez : le test ne peut pas démarrer sans elle.";

            if (!result.Ok)
                await LogIncident("CameraDenied", result.Reason);

            StateHasChanged();
        }

        private void GoBack() => _navigationManager.NavigateTo("/StDashBoard");

        private void StartTest()
        {
            if (!_cameraReady || _started)
                return;

            _started = true;
            _startedAt = DateTime.Now;

            timer = new System.Timers.Timer(1000); // 1 second interval
            timer.Elapsed += TimerElapsed;
            timer.Start();
        }

        [JSInvokable]
        public async Task OnTabBlur(int seconds)
        {
            if (!_started || IsTestFinished)
                return;

            await LogIncident("TabBlur", $"Onglet quitté pendant {seconds} s");
        }

        /// <summary>
        /// Appelé par le module JS quand aucun visage n'est détecté depuis
        /// <see cref="FaceMissingThresholdSeconds"/> secondes. Un seul appel par épisode
        /// d'absence : le module ne rappelle qu'après le retour de l'étudiant dans le champ.
        /// </summary>
        [JSInvokable]
        public async Task OnFaceMissing(int seconds)
        {
            if (!_started || IsTestFinished)
                return;

            await LogIncident("NoFace", $"Aucun visage détecté pendant {seconds} s");
        }

        /// <summary>
        /// Appelé par le module JS à chaque changement de présence devant la caméra, et à
        /// chaque analyse tant que l'étudiant est hors champ. Sert uniquement l'affichage :
        /// l'enregistrement de l'incident reste le rôle de <see cref="OnFaceMissing"/>.
        /// </summary>
        [JSInvokable]
        public async Task OnPresenceChanged(bool present, int secondsMissing)
        {
            if (!_started || IsTestFinished)
                return;

            _faceVisible = present;
            _secondsOutOfFrame = present ? 0 : secondsMissing;

            await InvokeAsync(StateHasChanged);
        }

        private async Task LogIncident(string type, string? details)
        {
            if (string.IsNullOrEmpty(_userId))
                return;

            try
            {
                await _proctoringService.LogIncident(new ProctoringIncidentDTO
                {
                    QuizId = QuizId,
                    UserId = _userId,
                    SessionId = _sessionId.ToString(),
                    CapturedAt = DateTime.Now,
                    Type = type,
                    Details = details
                });
            }
            catch
            {
                // Un incident de surveillance non transmis ne doit jamais interrompre l'épreuve.
            }
        }

        private async Task CaptureSnapshot()
        {
            if (string.IsNullOrEmpty(_userId))
                return;

            try
            {
                if (_proctoringModule == null)
                    return;

                var image = await _proctoringModule.InvokeAsync<string?>("capture", 320);
                if (string.IsNullOrEmpty(image))
                    return;

                await _proctoringService.SaveSnapshot(new ProctoringSnapshotDTO
                {
                    QuizId = QuizId,
                    UserId = _userId,
                    SessionId = _sessionId.ToString(),
                    CapturedAt = DateTime.Now,
                    ImageBase64 = image
                });
            }
            catch
            {
                // Idem : une capture perdue ne doit pas faire échouer le passage du test.
            }
        }

        private async Task StopCamera()
        {
            if (_cameraStopped)
                return;

            _cameraStopped = true;
            try
            {
                if (_proctoringModule != null)
                    await _proctoringModule.InvokeVoidAsync("stop");
            }
            catch
            {
                // Le circuit JS peut déjà être parti (onglet fermé) : rien à rattraper ici.
            }
        }

        public async Task FinishTest()
        {
            if (IsTestFinished)
                return;

            IsTestFinished = true;
            timer?.Stop();
            await StopCamera();

            // Le bandeau ne doit pas survivre a l'epreuve : la surveillance vient d'etre
            // arretee, un avertissement hors champ n'aurait plus de sens sur l'ecran de resultat.
            _faceVisible = true;
            _secondsOutOfFrame = 0;

            StateHasChanged();

            _reportRequest = await BuildReportRequest();
            // Calcule le score côté serveur et enregistre le résultat du passage. Si
            // l'enregistrement échoue, l'étudiant doit quand même voir son score : on se
            // rabat sur le calcul seul, comme avant l'existence de la table des résultats.
            try
            {
                _scoreSummary = await _reportService.SubmitResult(_reportRequest);
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"Enregistrement du résultat impossible : {ex.Message}");
                _scoreSummary = await _reportService.GetScoreSummary(_reportRequest);
            }
            StateHasChanged();

            var parameters = new DialogParameters();
            parameters.Add("Score", _scoreSummary.Score);
            parameters.Add("MaxScore", _scoreSummary.MaxScore);
            parameters.Add("Percentage", _scoreSummary.Percentage);
            parameters.Add("Mention", _scoreSummary.Mention);
            parameters.Add("IsPassed", _scoreSummary.Mention != "Échoué");
            parameters.Add("OnViewReport", EventCallback.Factory.Create(this, ViewReport));

            var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true, FullWidth = true };

            await DialogService.ShowAsync<QuizResultDialog>("Résultat du test", parameters, options);
        }

        private async Task<QuizReportRequestDTO> BuildReportRequest()
        {
            var listScoreRequest = new List<ScoreRequestDTO>();
            foreach (var question in questionPersists.Values)
            {
                if (question is TestMultichk multichk)
                {
                    foreach (var item in multichk.chkdItems)
                    {
                        var mapped = multichk.Question.reponses.FirstOrDefault(x => x.Body == item.text);
                        item.color = mapped.IsAnswer == item.ischecked ? Color.Success : Color.Error;
                    }

                    var request = new ScoreRequestDTO
                    {
                        QuestionId = multichk.Question.Id,
                        Answers = multichk.chkdItems.Select(item => new AnswerSubmissionDTO { Body = item.text, IsChecked = item.ischecked }).ToList()
                    };
                    listScoreRequest.Add(request);
                }
                else if (question is TestTrue_False True_false)
                {
                    foreach(var item in True_false.chkdItems)
                    {
                        var mapped = True_false.Question.reponses.FirstOrDefault(x => x.Body == item.text);
                        item.color = mapped.IsAnswer == item.ischecked ? Color.Success : Color.Error;
                    }
                    var request = new ScoreRequestDTO
                    {
                        QuestionId = True_false.Question.Id,
                        Answers = True_false.BuildAnswers()
                    };
                    listScoreRequest.Add(request);

                }
                else if (question is TestCoding coding)
                {
                    var request = new ScoreRequestDTO { QuestionId = coding.Question.Id };
                    try
                    {
                        var (code, output) = await coding.GetSubmission();
                        request.SubmittedCode = code;
                        request.SubmittedOutput = output;
                    }
                    catch
                    {
                        // Une question Code défaillante (éditeur non initialisé, échec d'exécution)
                        // ne doit pas empêcher l'envoi du score des autres questions.
                    }
                    listScoreRequest.Add(request);
                }
            }

            return new QuizReportRequestDTO
            {
                ListScoreRequest = listScoreRequest,
                QuizId = this.QuizId,
                UserId = _userId,
                // Même identifiant que les captures de surveillance de ce passage.
                SessionId = _sessionId.ToString(),
                StartedAt = _startedAt,
                IsAutoSubmitted = _autoSubmitted
            };
        }

        public async Task ViewReport()
        {
            if (_reportRequest == null)
                return;

            var report = await _reportService.GetReport(_reportRequest);

            try
            {
                await JSRuntime.InvokeVoidAsync("openPdf", report);
            }
            catch (JSException ex)
            {
                // Meme precaution que pour la surveillance : si le script d'ouverture du PDF
                // n'est pas charge (cache navigateur anterieur, fichier en echec), on informe
                // l'etudiant au lieu de laisser remonter une erreur brute en fin d'epreuve.
                _reportError = $"Impossible d'ouvrir le rapport ({ex.Message}). Rechargez la page avec Ctrl+F5, puis reessayez.";
                StateHasChanged();
            }
        }

        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (IsTestFinished)
            {
                timer?.Stop();
                return;
            }

            remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds(1));

            // La cadence des photos réutilise ce tick d'une seconde plutôt qu'un second
            // timer : un seul objet à arrêter et à libérer.
            _secondsSinceSnapshot++;
            if (_secondsSinceSnapshot >= SnapshotIntervalSeconds)
            {
                _secondsSinceSnapshot = 0;
                InvokeAsync(async () => await CaptureSnapshot());
            }

            if (remainingTime.TotalSeconds <= 0)
            {
                timer.Stop();
                TimerCompleted();
            }

            InvokeAsync(StateHasChanged);
        }

        private void TimerCompleted()
        {
           _autoSubmitted = true;
           InvokeAsync(async () => await FinishTest());
        }

        // Aucun composant du projet ne libérait ses ressources : sans cela, quitter la page
        // en cours de test laissait le timer tourner et laisserait désormais la webcam allumée.
        public async ValueTask DisposeAsync()
        {
            timer?.Stop();
            timer?.Dispose();

            await StopCamera();

            if (_proctoringModule != null)
            {
                try
                {
                    await _proctoringModule.DisposeAsync();
                }
                catch (JSDisconnectedException)
                {
                    // Onglet ferme : le runtime JS n'est plus joignable, rien a liberer.
                }
                _proctoringModule = null;
            }

            _dotNetRef?.Dispose();
            _dotNetRef = null;
        }

        /// <summary>Résultat de proctoring.start côté JS.</summary>
        public class CameraStartResult
        {
            public bool Ok { get; set; }
            public string? Reason { get; set; }
        }


        protected RenderFragment buildQuestionRF() => builder =>
        {
            for (int i = 0; i < _questions.Count; i++)
            {
                var question = _questions[i];
                var isVisible = i == currentIndex;

                builder.OpenElement(10, "div");
                builder.AddAttribute(11, "style", isVisible ? "" : "display:none");

                var questionIndex = i;
                switch (question.type)
                {
                    case "Choix Multiple":
                        {

                            builder.OpenComponent(0, typeof(TestMultichk));
                            builder.AddAttribute(1, "Question", question);

                            builder.AddComponentReferenceCapture(2, capturedRef =>
                            {
                                questionPersists[questionIndex] = (TestMultichk)capturedRef;
                            });

                            builder.CloseComponent();
                        }
                        break;
                    case "Vrai/Faux":
                        {

                            builder.OpenComponent(0, typeof(TestTrue_False));
                            builder.AddAttribute(1, "Question", question);

                            builder.AddComponentReferenceCapture(2, capturedRef =>
                            {
                                questionPersists[questionIndex] = (TestTrue_False)capturedRef;
                            });

                            builder.CloseComponent();
                        }
                        break;

                    case "Codage":
                        {
                            builder.OpenComponent(0, typeof(TestCoding));
                            builder.AddAttribute(1, "Question", question);

                            builder.AddComponentReferenceCapture(2, capturedRef =>
                            {
                                questionPersists[questionIndex] = (TestCoding)capturedRef;
                            });

                            builder.CloseComponent();
                        }
                        break;

                    default:
                        break;
                }

                builder.CloseElement();
            }

        };
    }
}
