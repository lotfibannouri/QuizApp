﻿using Authentication.web.Dialogs;
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
            QuizSelected = items;
            _events.Insert(0, $"Event = SelectedItemsChanged, data = {System.Text.Json.JsonSerializer.Serialize(items)}");
        }
        protected async Task BindEntitytoQuiz(Entity entity)
        {

            var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true, FullWidth = true };
            var parameters = new DialogParameters();
            
            
            if (QuizSelected.Count > 1)
            {
                parameters.Add("AlertMessage", "Select only one Item!!!");
                await dialogService.ShowAsync<AlertBox>("error", parameters, options);
                await hubConnection.SendAsync("SendMessage", "parent", "usertest", "aaa", "bbb");
                QuizSelected.Clear();
                return;

            }
            else if (QuizSelected.Count == 1)
            {
                parameters.Add("QuizId", QuizSelected.FirstOrDefault().Id);
                parameters.Add("bindto", entity);
                var dialogresult = await dialogService.ShowAsync<BindEntityToQuizDlg>("", parameters, options);
                var result = await dialogresult.Result;
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


        public async Task AddQuiz()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true, FullWidth = true };
            var dialogresult = await dialogService.ShowAsync<QuizDialog>("Création Quiz", options);
            var result = await dialogresult.Result;
            Quiz = await _quizService.ListeQuiz();
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
