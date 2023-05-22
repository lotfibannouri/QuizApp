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
        private async Task InvokAddUser()=> await OnAddEventClicked.InvokeAsync();

        private async Task InvokDeleteUser()=> await OnDeleteEventClicked.InvokeAsync();


        private async Task InvokEditUser() => await OnEditEventClicked.InvokeAsync();

        private async Task InvokAssignRoletoUser() => await OnAssignRoleToUserEventClicked.InvokeAsync();
  
        public enum Type
        {
            USER,
            QUIZ,
            ROLE
        }
    }
}
