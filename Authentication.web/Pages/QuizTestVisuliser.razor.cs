using Authentication.web.Dialogs;
using Authentication.web.Services;
using Authentication.web.Shared.Test;
using Authentication.web.utility;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using MudBlazor;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using QuizApp.Entities.Conception_Entities.DTO.ReportDTO;
using System;
using System.Timers;
namespace Authentication.web.Pages
{
    public partial class QuizTestVisuliser
    {
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

            timer = new System.Timers.Timer(1000); // 1 second interval
            timer.Elapsed += TimerElapsed;
            timer.Start();

            

        }
        public async Task FinishTest()
        {
            if (IsTestFinished)
                return;

            IsTestFinished = true;
            timer?.Stop();
            StateHasChanged();

            _reportRequest = await BuildReportRequest();
            _scoreSummary = await _reportService.GetScoreSummary(_reportRequest);
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

            return new QuizReportRequestDTO { ListScoreRequest = listScoreRequest, QuizId = this.QuizId };
        }

        public async Task ViewReport()
        {
            if (_reportRequest == null)
                return;

            var report = await _reportService.GetReport(_reportRequest);
            await JSRuntime.InvokeVoidAsync("openPdf", report);
        }

        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (IsTestFinished)
            {
                timer?.Stop();
                return;
            }

            remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds(1));

            if (remainingTime.TotalSeconds <= 0)
            {
                timer.Stop();
                TimerCompleted();
            }

            InvokeAsync(StateHasChanged);
        }

        private void TimerCompleted()
        {
           InvokeAsync(async () => await FinishTest());
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
