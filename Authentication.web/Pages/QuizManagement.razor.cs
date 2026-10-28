using Authentication.web.Dialogs;
using Authentication.web.Model;
using Authentication.web.Services;
using Authentication.web.Shared;
using Authentication.web.utility;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using MudBlazor;
using QuizApp.Entities.Base_DTO;
using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;
using static MudBlazor.CategoryTypes;

namespace Authentication.web.Pages
{
    public partial class QuizManagement
    {
        private IEnumerable<ListQuizDTO> Quiz = new List<ListQuizDTO>();
        private string _searchString;
        private bool _sortNameByLength;
        private HashSet<ListQuizDTO> QuizSelected = new();
        private List<string> _events = new();
        public string txtsnakSuccess = "<div>suppression réussie</div>";
        public string txtsnakError = "<div>Problème de suppression</div>";

        private HubConnection hubConnection;


        [Inject]
        public IQuizService _quizService { get; set; }
        [Inject]
        public NavigationManager _navigationManager { get; set; }
        protected override async Task OnInitializedAsync()
        {
            //hubConnection = new HubConnectionBuilder().WithUrl(_navigationManager.ToAbsoluteUri("/notificationHub")).Build();
            //await hubConnection.StartAsync();
            Quiz = await _quizService.ListeQuiz();
        }

        private Func<ListQuizDTO, object> _sortBy => x =>
        {
            if (_sortNameByLength)
                return x.titre.Length;
            else
                return x.titre;
        };
        // quick filter - filter gobally across multiple columns with the same input
        private Func<ListQuizDTO, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;

            if (!string.IsNullOrWhiteSpace(x.titre) && x.titre.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (!string.IsNullOrWhiteSpace(x.categorieTitre) && x.categorieTitre.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (!string.IsNullOrWhiteSpace(x.duree_quiz.ToString()) && x.duree_quiz.ToString().Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;


            //if ($"{x.rowNumber} {x.adresse} {x.email}".Contains(_searchString))
            //    return true;

            return false;
        };
        void RowClicked(DataGridRowClickEventArgs<ListQuizDTO> args)
        {
            _events.Insert(0, $"Event = RowClick, Index = {args.RowIndex}, Data = {System.Text.Json.JsonSerializer.Serialize(args.Item)}");
        }
        void SelectedItemsChanged(HashSet<ListQuizDTO> items)
        {
            QuizSelected = items;
            _events.Insert(0, $"Event = SelectedItemsChanged, data = {System.Text.Json.JsonSerializer.Serialize(items)}");
        }
        // « Assigné à un utilisateur » n'est proposé que si aucun quiz sélectionné n'est incomplet.
        private bool CanAssignSelection => !QuizSelected.Any(q => !q.isComplet);

        private const string AssignedMessage =
            "Le quiz « {0} » est assigné à {1} utilisateur(s) : détachez-le (bouton « Retirer quiz ») pour pouvoir le modifier.";

        private async Task ShowAlert(string message)
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true, FullWidth = true };
            var parameters = new DialogParameters();
            parameters.Add("AlertMessage", message);
            parameters.Add("ShowActions", false);
            await dialogService.ShowAsync<AlertBox>("Action impossible", parameters, options);
        }

        // Compteurs (questions rattachées, utilisateurs assignés) à jour après chaque action.
        private async Task ReloadQuiz()
        {
            Quiz = await _quizService.ListeQuiz();
            QuizSelected.Clear();
        }

        protected async Task BindEntitytoQuiz(Entity entity)
        {

            var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true, FullWidth = true, MaxWidth = MaxWidth.Large };
            var parameters = new DialogParameters();


            if (QuizSelected.Count > 1)
            {
                parameters.Add("AlertMessage", "Select only one Item!!!");
                await dialogService.ShowAsync<AlertBox>("error", parameters, options);
                QuizSelected.Clear();
                return;

            }
            else if (QuizSelected.Count == 1)
            {
                var quiz = QuizSelected.First();

                // Un quiz assigné ne se modifie plus : ses étudiants passeraient un autre test.
                if (entity == Entity.QUESTION && quiz.isAssigne)
                {
                    await ShowAlert(string.Format(AssignedMessage, quiz.titre, quiz.nbUtilisateursAssignes));
                    return;
                }

                // Seul un quiz complet peut être assigné (contrôle aussi fait par l'API).
                if (entity == Entity.USER && !quiz.isComplet)
                {
                    await ShowAlert($"Le quiz « {quiz.titre} » est incomplet ({quiz.nbQuestionsRattachees}/{quiz.nbr_questions} questions) : il ne peut pas être assigné.");
                    return;
                }

                parameters.Add("QuizId", quiz.Id);
                parameters.Add("bindto", entity);
                var dialogresult = await dialogService.ShowAsync<BindEntityToQuizDlg>("", parameters, options);
                var result = await dialogresult.Result;
                await ReloadQuiz();
                return;
            }
            else
            {
                parameters.Add("AlertMessage", "You need To Select a Quiz!!!");
                await dialogService.ShowAsync<AlertBox>("error", parameters, options);
                QuizSelected.Clear();
                return;
            }
            QuizSelected.Clear();

        }

        protected async Task EditQuiz(string IdQuiz)
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true, FullWidth = true, MaxWidth = MaxWidth.Large };
            var parameters = new DialogParameters();


            if (QuizSelected.Count > 1)
            {
                parameters.Add("AlertMessage", "Select only one Item!!!");
                await dialogService.ShowAsync<AlertBox>("error", parameters, options);
                QuizSelected.Clear();
                return;

            }
            else if (QuizSelected.Count == 1)
            {
                var quiz = QuizSelected.First();
                if (quiz.isAssigne)
                {
                    await ShowAlert(string.Format(AssignedMessage, quiz.titre, quiz.nbUtilisateursAssignes));
                    return;
                }

                parameters.Add("QuizId", quiz.Id);
                parameters.Add("bindto", Entity.QUESTION);
                parameters.Add("IsEditMode", true);
                var dialogresult = await dialogService.ShowAsync<BindEntityToQuizDlg>("", parameters, options);
                var result = await dialogresult.Result;
                await ReloadQuiz();
                return;
            }
            else
            {
                parameters.Add("AlertMessage", "You need To Select a Quiz!!!");
                await dialogService.ShowAsync<AlertBox>("error", parameters, options);
                QuizSelected.Clear();
                return;
            }
            QuizSelected.Clear();
        }
        private async Task DeleteQuiz()
        {
            if (QuizSelected.Count == 0)
            {
                var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true, FullWidth = true };
                var parameters = new DialogParameters();
                parameters.Add("AlertMessage", "You need To Select a Quiz!!!");
                await dialogService.ShowAsync<AlertBox>("error", parameters, options);
                QuizSelected.Clear();
                return;
            }

            // Supprimer un quiz assigné effacerait aussi les assignations de ses étudiants.
            var assigned = QuizSelected.Where(q => q.isAssigne).ToList();
            if (assigned.Any())
            {
                await ShowAlert(assigned.Count == 1
                    ? string.Format(AssignedMessage, assigned[0].titre, assigned[0].nbUtilisateursAssignes)
                    : $"Ces quiz sont assignés à des utilisateurs : {string.Join(", ", assigned.Select(q => $"« {q.titre} »"))}. Détachez-les (bouton « Retirer quiz ») pour pouvoir les supprimer.");
                return;
            }

            foreach (var item in QuizSelected)
            {
                Response response = await _quizService.DeleteQuiz(item.Id);
                if (response.status)
                {
                    SnackbarService.Add
                        (txtsnakSuccess, Severity.Success
                        );

                }
                else
                {
                    SnackbarService.Add
                        (txtsnakError, Severity.Warning
                        );
                }
            }


            Quiz = await _quizService.ListeQuiz();
            QuizSelected.Clear();

        }
        // Liste les utilisateurs qui possèdent le quiz sélectionné, pour l'en détacher.
        private async Task UnassignQuiz()
        {
            if (QuizSelected.Count != 1)
            {
                await ShowAlert(QuizSelected.Count == 0 ? "Sélectionnez un quiz." : "Sélectionnez un seul quiz.");
                return;
            }

            var quiz = QuizSelected.First();
            var parameters = new DialogParameters();
            parameters.Add("QuizId", quiz.Id);
            parameters.Add("QuizTitre", quiz.titre);
            var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true, FullWidth = true, MaxWidth = MaxWidth.Medium };

            var dialog = await dialogService.ShowAsync<UnassignQuizDlg>("", parameters, options);
            await dialog.Result;
            await ReloadQuiz();
        }

        private void DetailedQuiz(string IdQuiz)
        {
            _navigationManager.NavigateTo($"/QuizVisulizer/{IdQuiz}");
        }

        private void HandleRowDoubleClick(BaseListDTO item)
        {
            _navigationManager.NavigateTo($"/QuizVisulizer/{item.Id}"); // Your logic for handling the double-click event
        }
    }

}
