using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;
using Authentication.web.Model;
using Authentication.web.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Linq;
using Authentication.web.utility;
using AutoMapper;
using Microsoft.AspNetCore.SignalR.Client;

namespace Authentication.web.Dialogs
{
    public partial class BindEntityToQuizDlg

    {
        [Inject]
        private IMapper _mapper { get; set; }
        public string dialogTitle { get; set; } = "Bind user to quiz";

        private IEnumerable<User> Users = new List<User>();

        private List<QuestionItem> _questionItems = new();
        private List<string> _boundQuestionIds = new();
        private string _searchString;

        [Inject]
        private IAdministrationService adminService { get; set; }

        [Inject]
        private IQuizService QuizService { get; set; }

        [Inject]
        private IQuestionService QuestionService { get; set; }

        [Inject]
        private IDialogService dialogService { get; set; }
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        public MudDataGrid<User> UsersGrid { get; set; }

        [Parameter]
        public Entity bindto { get; set; }

        [Parameter]
        public string QuizId { get; set; } = "";

        [Parameter]
        public bool IsEditMode { get; set; } = false;
        private HubConnection hubConnection { get; set; }
        protected override async Task OnInitializedAsync()
        {
            hubConnection = new HubConnectionBuilder()
                   .WithUrl("https://localhost:7284/notificationshub")
                   .Build();
            await hubConnection.StartAsync();

            if (bindto == Entity.USER)
            {
                var users = await adminService.GetUsers();
                Users = users.Where(usr => usr.role.Contains("User") && usr.role.Count == 1);
            }
            else
            {
                var BindedQuestions = await QuestionService.GetQuestionsByQuizId(QuizId);
                var FullQuestions = await QuestionService.GetQuestions();
                _boundQuestionIds = BindedQuestions.Select(q => q.Id).ToList();
                _questionItems = FullQuestions
                    .Select(q => new QuestionItem(q, _boundQuestionIds.Contains(q.Id)))
                    .ToList();
            }
        }

        private bool MatchesSearch(QuestionItem item)
        {
            return string.IsNullOrWhiteSpace(_searchString)
                || item.Question.questionText.Contains(_searchString, StringComparison.OrdinalIgnoreCase);
        }

        private void ItemUpdated(MudItemDropInfo<QuestionItem> dropItem)
        {
            dropItem.Item._Identifier = dropItem.DropzoneIdentifier;
        }

        private async Task ShowQuestionDetail(QuestionItem item)
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = false, FullWidth = true };
            var parameters = new DialogParameters
            {
                ["AlertMessage"] = item.Question.questionText,
                ["ShowActions"] = false
            };
            await dialogService.ShowAsync<Authentication.web.Shared.AlertBox>("Détail de la question", parameters, options);
        }

        private async Task OnValidChanges()
        {
            var toBind = _questionItems.Where(i => i._Identifier == "Assigned" && !_boundQuestionIds.Contains(i.Question.Id));
            var toUnbind = _questionItems.Where(i => i._Identifier == "Available" && _boundQuestionIds.Contains(i.Question.Id));

            foreach (var item in toBind)
            {
                var response = await QuizService.BindQuizToQuestion(QuizId, item.Question.Id);
                SnackbarService.Add(response.content, response.status ? Severity.Success : Severity.Warning);
            }

            foreach (var item in toUnbind)
            {
                var response = await QuizService.UnbindQuizFromQuestion(QuizId, item.Question.Id);
                SnackbarService.Add(response.content, response.status ? Severity.Success : Severity.Warning);
            }

            MudDialog.Close(DialogResult.Ok(true));
        }

        private async Task Addclicked()
        {
            if (UsersGrid.SelectedItems != null)
            {
                var selectedItems = UsersGrid.SelectedItems.ToList();
                foreach (var Item in selectedItems)
                {
                    var response = await QuizService.BindQuizToUser(new QuizUserDTO { UserId = Item.id, QuizId = QuizId });
                    if (response.status)
                    {
                        MudDialog.Close(DialogResult.Ok(true));
                        SnackbarService.Add
                       (response.content, Severity.Success
                       );
                    }
                    else
                    {
                        SnackbarService.Add
                               (response.content, Severity.Warning
                               );
                    }
                }

                await hubConnection.SendAsync("SendMessage", "", "");
            }
        }
    }
}
