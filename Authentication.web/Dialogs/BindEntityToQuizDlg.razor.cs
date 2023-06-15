using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;
using Authentication.web.Model;
using Authentication.web.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Reflection;
using System.Linq;
using Authentication.web.utility;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using QuizApp.Entities.Conception_Entities;
using AutoMapper;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.SignalR.Client;

namespace Authentication.web.Dialogs
{
    public partial class BindEntityToQuizDlg

    {
        [Inject]
        private  IMapper _mapper { get; set; }
        public string dialogTitle { get; set; } = "Bind user to quiz";

        private IEnumerable<User> Users = new List<User>();

        private IEnumerable<ListQuestionDTO> Questions = new List<ListQuestionDTO>();
        
        [Inject]
        private IAdministrationService adminService { get; set; }
   
        [Inject]
        private IQuizService QuizService { get; set; }

        [Inject]
        private IQuestionService QuestionService { get; set; }
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        public MudDataGrid<User> UsersGrid { get; set; }

        public MudDataGrid<ListQuestionDTO>  QuestionGrid { get; set; }
        [Parameter]
        public Entity bindto { get; set; }

        [Parameter]
        public string QuizId { get; set; } = "";
        private HubConnection hubConnection { get; set; }
        protected override async Task OnInitializedAsync()
        {
            hubConnection = new HubConnectionBuilder()
                   .WithUrl("https://localhost:7284/notificationshub")
                   .Build();
            await hubConnection.StartAsync();

            if (bindto == Entity.USER) { 
            var users = await adminService.GetUsers();
            Users = users.Where(usr => usr.role.Contains("User") && usr.role.Count == 1);
            }
            else
            {   var BindedQuestions = await QuestionService.GetQuestionsByQuizId(QuizId);
                var FullQuestions = await QuestionService.GetQuestions(); 
                Questions = FullQuestions.Except(BindedQuestions, new QuestionComparer());
            
            }
        }

        public class QuestionComparer : IEqualityComparer<ListQuestionDTO>
        {
            public bool Equals(ListQuestionDTO x, ListQuestionDTO y)
            {
                return x.Id == y.Id;
            }

            public int GetHashCode([DisallowNull] ListQuestionDTO obj)
            {
                return obj.Id.GetHashCode();
            }
        }
            private async void Addclicked()
        {
            Response response;
            if (bindto == Entity.USER){

                if (UsersGrid.SelectedItems != null)
                {
                    var selectedItems = UsersGrid.SelectedItems.ToList();
                    foreach (var Item in selectedItems)
                    {
                        response = await QuizService.BindQuizToUser(new QuizUserDTO { UserId = Item.id, QuizId = QuizId });
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
            else
            {
                var selectedItems = QuestionGrid.SelectedItems.ToList();
                foreach(var Item in selectedItems)
                {                    
                    response = await QuizService.BindQuizToQuestion(QuizId, Item.Id);
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
            }

        }
        private void removeclicked()
        {
           
        }

        
    

    }
}
