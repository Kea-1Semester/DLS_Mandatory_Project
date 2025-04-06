
namespace DLS_Mandatory_Project.Client.Services
{
    public interface IChatClient
    {
        IReadOnlyList<string> Messages { get; }

        Task SendBroadcastMessage(string userInput, string messageInput);
        
        bool IsConnected();
    }
}