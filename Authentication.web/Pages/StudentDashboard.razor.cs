using Authentication.web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;
using System.Security.Claims;

namespace Authentication.web.Pages
{
    public partial class StudentDashboard : ComponentBase
    {
        [Inject]
        public AuthenticationStateProvider _authenticationStateProvider { get; set; }
        [Inject]
        public IAdministrationService _administrationService { get; set; }
        [Inject]
        public IQuizService _quizService { get; set; }
        [Inject]
        public NavigationManager _navigationManager { get; set; }
        public List<ListQuizDTO> _quizList { get; set; } = new List<ListQuizDTO>();
        protected override async Task OnInitializedAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var Idclaim = authState.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
           
            if(Idclaim != null)
            {
               var user =  _administrationService.GetUserById(Idclaim.Value);
               _quizList = await _quizService.ListeQuizByUser(Idclaim.Value);
            }
            foreach (var item in _quizList)
            {
                Console.WriteLine(item.titre);
            }

        }
        private void ExecuteQuiz(ListQuizDTO employee)
        {
            _navigationManager.NavigateTo($"/QuizVisulizer/{employee.Id}");
            // Handle the button click for the specific employee
            // You can access the employee object and perform any logic as needed
        }


    }

}
