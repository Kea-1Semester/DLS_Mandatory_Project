using ChatClassLibrary;

namespace ChatService
{
    public interface IChatProducer
    {
        Task SendToQueue(LobbyMessage lobbyMessage);
    }
}