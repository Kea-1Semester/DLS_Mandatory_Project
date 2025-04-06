using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace DLS_Mandatory_Project.Client.Services
{
    public class ChatClient : IChatClient
    {
        private readonly HubConnection _hubConnection;
        private readonly List<string> _messages = [];
        public IReadOnlyList<string> Messages => _messages.AsReadOnly();

        public ChatClient()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:30001/chathub")
                .WithStatefulReconnect()
                .Build();

            _hubConnection.On<string, string>("ReceiveBroadcastMessage", (user, message) =>
            {
                var encodedMsg = $"{user}: {message}";
                _messages.Add(encodedMsg);
            });

            Task.Run(async () => await _hubConnection.StartAsync());
        }

        public async Task SendBroadcastMessage(string userInput, string messageInput)
        {
            if (_hubConnection is not null)
            {
                await _hubConnection.SendAsync("SendBroadcastMessage", userInput, messageInput);
            }
        }

        public bool IsConnected()
        {
            if (_hubConnection is not null)
            {
                return _hubConnection.State == HubConnectionState.Connected;
            }

            return false;
        }
    }
}
