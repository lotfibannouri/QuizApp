using Microsoft.AspNetCore.Components;

namespace Authentication.web.Shared.Questions
{
    public partial class MultiCheckQ
    {
        public List<int> QuestionsList = new List<int>() {1,2,3,4};
        public bool checkedItem { get; set; } = false;
        public string TextValue { get; set; } = "";
       
        [Parameter]
        public int index { get; set; }
        
        [Parameter]
        public EventCallback<int> OnClick { get; set; }

        private async Task InvokDeleteItem() 
        {
            OnClick.InvokeAsync();
        }
    }
}
