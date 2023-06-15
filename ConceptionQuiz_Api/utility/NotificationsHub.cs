using Microsoft.AspNetCore.SignalR;

namespace ConceptionQuiz_Api.utility
{
    public class NotificationsHub:Hub
    {
        public async Task SendMessage(string user,string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
