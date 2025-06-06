using ChatClassLibrary;
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
                .WithUrl("http://localhost/ChatHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                    options.SkipNegotiation = true;
                })
                .WithKeepAliveInterval(TimeSpan.FromMinutes(5))
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<LobbyMessage>("ReceiveBroadcastMessage", (message) =>
            {
                var encodedMsg = $"{message.SenderId}: {message.Message}";
                OnMessageReceived?.Invoke(encodedMsg);
            });

            _hubConnection.Closed += async (error) =>
            {
                if (error is not null)
                {
                    Console.WriteLine(error.Message);
                }
                await Task.Run(() => OnStateChanged?.Invoke(_hubConnection.State));
            };

            _hubConnection.Reconnecting += async (error) =>
            {
                if (error is not null)
                {
                    Console.WriteLine(error.Message);
                }
                await Task.Run(() => OnStateChanged?.Invoke(_hubConnection.State));
            };

            _hubConnection.Reconnected += async (connectionId) =>
            {
                if (connectionId is not null)
                {
                    Console.WriteLine($"Reconnected to server with connection ID: {connectionId}");
                }
                await Task.Run(() => OnStateChanged?.Invoke(_hubConnection.State));
            };
        }

        public async Task StartAsync()
        {
            try
            {
                await _hubConnection.StartAsync();
                OnStateChanged?.Invoke(_hubConnection.State);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                OnStateChanged?.Invoke(_hubConnection.State);
            }
        }

        public async Task SendBroadcastMessage(LobbyMessage lobbyMessage)
        {
            try
            {
                await _hubConnection.SendAsync("SendBroadcastMessage", lobbyMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                OnStateChanged?.Invoke(_hubConnection.State);
            }
        }
    }
}
