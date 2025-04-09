using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace DLS_Mandatory_Project.Client.Clients
{
    public class ChatClient : IChatClient
    {
        private readonly HubConnection _hubConnection;
        public event Action<string>? OnMessageReceived;
        public event Action<HubConnectionState>? OnStateChanged;       
        public HubConnectionState State => _hubConnection.State;

        public ChatClient()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:30001/ChatHub", HttpTransportType.WebSockets | HttpTransportType.ServerSentEvents)
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<string?, string?>("ReceiveBroadcastMessage", (user, message) =>
            {
                var encodedMsg = $"{user}: {message}";
                OnMessageReceived?.Invoke(encodedMsg);
            });
        }

        public async Task StartAsync()
        {
            try
            {
                if (_hubConnection.State != HubConnectionState.Connected)
                {
                    await _hubConnection.StartAsync();
                    OnStateChanged?.Invoke(_hubConnection.State);
                }
                else
                {
                    OnStateChanged?.Invoke(_hubConnection.State);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task SendBroadcastMessage(string? user, string? message)
        {
            try
            {
                if (_hubConnection.State != HubConnectionState.Connected)
                {
                    await _hubConnection.StartAsync();
                    OnStateChanged?.Invoke(_hubConnection.State);                    
                }
                else
                {
                    OnStateChanged?.Invoke(_hubConnection.State);
                    await _hubConnection.SendAsync("SendBroadcastMessage", user, message);
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
