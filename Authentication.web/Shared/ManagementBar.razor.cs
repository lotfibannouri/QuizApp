using Authentication.web.Model;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Authentication.web.Shared
{
    public partial class ManagementBar
    {
        [Parameter]
        public Type TypeBar { get; set; }
        [Parameter]
        public EventCallback OnAddEventClicked  { get; set; }
        [Parameter]
        public EventCallback OnDeleteEventClicked { get; set; }
        [Parameter]
        public EventCallback OnEditEventClicked { get; set; }

        [Parameter]
        public EventCallback OnAssignRoleToUserEventClicked { get; set; }

        [Parameter]
        public EventCallback OnBindUserToQuiz { get; set; }

        [Parameter]
        public EventCallback OnBindQuestionToQuiz { get; set; }

        private async Task InvokAddUser()=> await OnAddEventClicked.InvokeAsync();

        private async Task InvokDeleteUser()=> await OnDeleteEventClicked.InvokeAsync();

        private async Task InvokEditUser() => await OnEditEventClicked.InvokeAsync();

        private async Task InvokAssignRoletoUser() => await OnAssignRoleToUserEventClicked.InvokeAsync();

        private async Task InvokeBindUsertoQuiz() => await OnBindUserToQuiz.InvokeAsync();
       
        private async Task InvokAddQuiz() => await OnAddEventClicked.InvokeAsync();
        private async Task InvokDeleteQuiz() => await OnDeleteEventClicked.InvokeAsync();

        private async Task InvokQuestionQuiz() => await OnBindQuestionToQuiz.InvokeAsync();

        public enum Type
        {
            USER,
            QUIZ,
            ROLE
        }
    }
}
