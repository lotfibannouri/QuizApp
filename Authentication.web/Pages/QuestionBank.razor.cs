using Authentication.web.Model;
using Authentication.web.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;

namespace Authentication.web.Pages
{
    public partial class QuestionBank
    {
        [Inject]
        public NavigationManager _navigationManager { get; set; }

        private IEnumerable<ListQuestionDTO> Questions = new List<ListQuestionDTO>();
        private string _searchString;
        private HashSet<ListQuestionDTO> QuestionsSelected = new();

        protected override async Task OnInitializedAsync()
        {
            Questions = await questionService.GetQuestions();
        }

        private Func<ListQuestionDTO, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;

            if (!string.IsNullOrWhiteSpace(x.questionText) && x.questionText.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (!string.IsNullOrWhiteSpace(x.type) && x.type.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (!string.IsNullOrWhiteSpace(x.categorieTitre) && x.categorieTitre.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.note.ToString().Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        };

        void SelectedItemsChanged(HashSet<ListQuestionDTO> items)
        {
            QuestionsSelected = items;
        }

        private async Task EditQuestion()
        {
            var optionsAlertBox = new DialogOptions { CloseOnEscapeKey = true };
            var parametersAlertBox = new DialogParameters();

            if (QuestionsSelected.Count > 1)
            {
                parametersAlertBox.Add("AlertMessage", "Il faut choisir une seule question!");
                await dialogService.ShowAsync<AlertBox>("Alert", parametersAlertBox, optionsAlertBox);
                return;
            }
            else if (QuestionsSelected.Count == 0)
            {
                parametersAlertBox.Add("AlertMessage", "Il faut choisir une question!");
                await dialogService.ShowAsync<AlertBox>("Alert", parametersAlertBox, optionsAlertBox);
                return;
            }

            _navigationManager.NavigateTo($"/QuestionManagement/{QuestionsSelected.First().Id}");
        }

        private async Task DeleteQuestion()
        {
            if (QuestionsSelected.Count == 0)
            {
                var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true, FullWidth = true };
                var parameters = new DialogParameters();
                parameters.Add("AlertMessage", "Il faut choisir une question!");
                await dialogService.ShowAsync<AlertBox>("Alert", parameters, options);
                QuestionsSelected.Clear();
                return;
            }

            foreach (var item in QuestionsSelected)
            {
                Response response = await questionService.DeleteQuestion(item.Id);

                // La suppression peut être refusée par le serveur quand la question est
                // assignée à un quiz : on affiche le message renvoyé, pas un texte générique.
                if (response.status)
                    SnackbarService.Add($"<div>{response.content}</div>", Severity.Success);
                else
                    SnackbarService.Add($"<div>{response.content}</div>", Severity.Warning);
            }

            Questions = await questionService.GetQuestions();
            QuestionsSelected.Clear();
        }
    }
}
