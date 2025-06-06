using Microsoft.AspNetCore.SignalR;
using ChatClassLibrary;

namespace ChatService
{
    public class ChatHub: Hub
    {
        private readonly IChatProducer _producer;

        public ChatHub(IChatProducer producer)
        {
            _producer = producer;
        }

        public async Task SendBroadcastMessage(LobbyMessage lobbyMessage)
        {
            await _producer.SendToQueue(lobbyMessage);
            await Clients.All.SendAsync("ReceiveBroadcastMessage", lobbyMessage);
        }

        //public async Task SendPrivateMessage(ChatMessage chatMessage)
        //{

        //    await Clients.User(chatMessage.ReceiverId).SendAsync("ReceivePrivateMessage", chatMessage);
        //}
    }
}
