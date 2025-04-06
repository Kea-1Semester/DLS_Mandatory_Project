using Microsoft.AspNetCore.SignalR;

namespace ChatService
{
    public class ChatHub: Hub
    {
        public async Task SendBroadcastMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveBroadcastMessage", user, message);
        }

        public async Task SendUserMessage(string userId, string user, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveUserMessage", user, message);
        }
    }
}
