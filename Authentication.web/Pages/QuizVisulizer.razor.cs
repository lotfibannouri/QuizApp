using Microsoft.AspNetCore.Components;
using QuizApp.Entities.Conception_Entities;

namespace Authentication.web.Pages
{
    public partial class QuizVisulizer : ComponentBase
    {
        [Parameter]
        public int QuizId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Console.WriteLine(QuizId);
        }
    }
}
