using Microsoft.AspNetCore.Components;

namespace Authentication.web.Shared.Questions
{
    public partial class MultiCheckQ 
    {
        public bool checkedItem { get; set; } = false;
        public string TextValue { get; set; } = "";
       
        [Parameter]
        public int index { get; set; }
        
        [Parameter]
        public EventCallback<int> OnDeleteEventClicked { get; set; }

        private async Task InvokDeleteItem() 
        { 
            await OnDeleteEventClicked.InvokeAsync(index); 
        }
    }
}
