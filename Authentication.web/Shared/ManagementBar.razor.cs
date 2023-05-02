using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Authentication.web.Shared
{
    public partial class ManagementBar
    {
        [Parameter]
        public EventCallback OnAddEventClicked  { get; set; }


       private async Task InvokAddUser()=> await OnAddEventClicked.InvokeAsync();

    }
}
