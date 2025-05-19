using Microsoft.AspNetCore.SignalR;

namespace ChatService
{
    public class ChatHub: Hub
    {
        public async Task SendBroadcastMessage(string? user, string? message)
        {
            await Clients.All.SendAsync("ReceiveBroadcastMessage", user, message);
        }
    }
}
