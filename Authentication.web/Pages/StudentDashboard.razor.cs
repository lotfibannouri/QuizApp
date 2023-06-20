using Authentication.web.Services;
using Authentication.web.Shared;
using Authentication.web.utility;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;
using System.Security.Claims;
using static MudBlazor.CategoryTypes;

namespace Authentication.web.Pages
{
    public partial class StudentDashboard
    {
        [Inject]
        public AuthenticationStateProvider _authenticationStateProvider { get; set; }
        [Inject]
        public IAdministrationService _administrationService { get; set; }
        [Inject]
        public IQuizService _quizService { get; set; }
        [Inject]
        public NavigationManager _navigationManager { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        [Inject]
        public IJSRuntime jsruntime { get; set; }

        private HubConnection _hubConnection { get; set; }

        string state = "Message box hasn't been opened yet";

        private readonly string notificationSoundPath = "/beep.mp3";

        public List<ListQuizDTO> _quizList { get; set; }

        public List<string> listLogos = new List<string> { Icons.Material.Filled.Quiz, Icons.Material.Filled.Code, Icons.Material.Filled.School };

        protected override async Task OnInitializedAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7284/notificationshub")
                .Build();

            _hubConnection.On<string, string>("ReceiveMessage", async (user, message) =>
            {
                await jsruntime.InvokeAsync<string>("PlayAudio");

                var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true, FullWidth = true };
                var parameters = new DialogParameters();
                parameters.Add("AlertMessage", "A Quiz have been assigned to you!!!");
                await DialogService.ShowAsync<AlertBox>("Updates", parameters, options);
                await LoadData();
                this.StateHasChanged();
            });
            await LoadData();

            await _hubConnection.StartAsync();
        }

        private async Task LoadData()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var Idclaim = authState.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (Idclaim != null)
            {
                var user = _administrationService.GetUserById(Idclaim.Value);
                _quizList = await _quizService.ListeQuizByUser(Idclaim.Value);
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
