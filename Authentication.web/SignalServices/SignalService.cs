using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;

namespace Authentication.web.SignalServices
{
    public class SignalService
    {
       private HubConnection _hubConnection;
        [Inject]
        public IJSRuntime jsruntime { get; set; }
        [Inject]
        public NavigationManager navigationManager { get; set; }
        [Inject]
        public IDialogService DialogService { get; set; }
        string state = "Message box hasn't been opened yet";
        public SignalService()
        {
           
        }

        public async Task StartConnectionAsync()
        {
            _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7284/notificationshub")
            .Build();
            _hubConnection.On<string, string>("ReceiveMessage", async (user, message) =>
            {
                await jsruntime.InvokeAsync<string>("PlayAudio");
                bool? result = await DialogService.ShowMessageBox("Updates", "A new Quiz have been assigned To you!", yesText: "OK!");
                state = result == null ? "Canceled" : "Deleted!";

            });

            await _hubConnection.StartAsync();
        }

        public async Task StopConnectionAsync()
        {
            await _hubConnection.StopAsync();
        }

    }
}
