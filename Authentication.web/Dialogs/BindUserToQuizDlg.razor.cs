using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;
using Authentication.web.Model;
using Authentication.web.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Reflection;
using System.Linq;
namespace Authentication.web.Dialogs
{
    public partial class BindUserToQuizDlg
    {

        public string dialogTitle { get; set; } = "Bind user to quiz";

        private IEnumerable<User> Users = new List<User>();
        [Inject]
        private IAdministrationService adminService { get; set; }
        [Inject]
        private IQuizService QuizService { get; set; }
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        public MudDataGrid<User> UsersGrid { get; set; }

        [Parameter]
        public string QuizId { get; set; } = "";
        protected override async Task OnInitializedAsync()
        {
            var users = await adminService.GetUsers();
            Users = users.Where(usr => usr.role.Contains("User") && usr.role.Count==1);
        }

        private void Addclicked()
        {

            if (UsersGrid.SelectedItems != null)
            {
                var selectedItems = UsersGrid.SelectedItems.ToList();
                foreach(var Item in selectedItems)
                {
                    QuizService.BindQuiz(new QuizUserDTO { UserId = Item.id,QuizId = QuizId });
                }

            }
        }
        private void removeclicked()
        {

        }

    }
}
