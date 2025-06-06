using ChatClassLibrary;
using MassTransit;
using System.Text.Json;

public class LobbyMessageConsumer : IConsumer<LobbyMessage>
{
    public Task Consume(ConsumeContext<LobbyMessage> context)
    {
        // Handle the received LobbyMessage here
        var jsonMessage = JsonSerializer.Serialize(context.Message);
        Console.WriteLine($"Received message: {jsonMessage}");
        return Task.CompletedTask;
    }
}