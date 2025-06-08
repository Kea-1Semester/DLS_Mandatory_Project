using ChatClassLibrary;
using Microsoft.AspNetCore.SignalR.Client;

namespace DLS_Mandatory_Project.Client.Clients
{
    public interface IChatClient
    {
        event Action<LobbyMessage> OnLobbyMessageReceived;
        event Action<HubConnectionState> OnStateChanged;
        HubConnectionState State { get; }

        Task SendBroadcastMessage(LobbyMessage lobbyMessage);
        Task StartAsync();
        Guid GetLobbyId();
    }
}