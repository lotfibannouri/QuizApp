using Authentication.web.Dialogs;
using Authentication.web.Services;
using Authentication.web.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;

namespace Authentication.web.Pages
{
    public partial class QuizManagement
    {
        private IEnumerable<ListQuizDTO> Quiz = new List<ListQuizDTO>();
        private string _searchString;
        private bool _sortNameByLength;
        private HashSet<ListQuizDTO> QuizSelected = new();
        private List<string> _events = new();
        [Inject]
        public IQuizService _quizService { get; set; }
        protected override async Task OnInitializedAsync()
        {

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

            if (!string.IsNullOrWhiteSpace(x.niv_deficulte.ToString()) && x.niv_deficulte.ToString().Contains(_searchString, StringComparison.OrdinalIgnoreCase))
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
        }
        protected async Task BindUsertoQuiz()
        {

            var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true, FullWidth = true };
            var parameters = new DialogParameters();
            if (QuizSelected.Count>1)
            {
                parameters.Add("AlertMessage", "Select only one Item!!!");
                await dialogService.ShowAsync<AlertBox>("error", parameters,options);
                return;
            }
            else
            {
                parameters.Add("QuizId", QuizSelected.FirstOrDefault().Id);
                var dialogresult = await dialogService.ShowAsync<BindUserToQuizDlg>("", parameters, options);
                var result = await dialogresult.Result;

            }
            
        }

    }

        
}
