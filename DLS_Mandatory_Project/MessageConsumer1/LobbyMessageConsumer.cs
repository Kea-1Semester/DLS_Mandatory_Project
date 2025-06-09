using ChatClassLibrary;
using MassTransit;
using System.Text.Json;

public class LobbyMessageConsumer : IConsumer<LobbyMessage>
{
    //private readonly MessageDbContext _dbContext;

    //public LobbyMessageConsumer(MessageDbContext dbContext)
    //{
    //    _dbContext = dbContext;
    //}
    public async Task Consume(ConsumeContext<LobbyMessage> context)
    {
        Console.WriteLine("Message received.");
        var jsonMessage = JsonSerializer.Serialize(context.Message);
        Console.WriteLine($"Received message: {jsonMessage}");

        //await _dbContext.AddAsync(context.Message);
        //await _dbContext.SaveChangesAsync();
    }
}