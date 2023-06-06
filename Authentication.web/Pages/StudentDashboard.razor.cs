using Authentication.web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using QuizApp.Entities.Conception_Entities;
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
        protected override async Task OnInitializedAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var Idclaim = authState.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
           
            if(Idclaim != null)
            {
               var user =  _administrationService.GetUserById(Idclaim.Value);
            }

            var x = await _quizService.ListeQuizByUser(Idclaim.Value);
           
            
            

            // i need to get the list of quiizs that are affected to this User
            //Console.WriteLine(authState.User.Claims.ToList()[i]);
        }


    }
}
