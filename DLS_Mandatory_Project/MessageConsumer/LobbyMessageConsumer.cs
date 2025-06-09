using ChatClassLibrary;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MessageConsumer
{
    public class LobbyMessageConsumer : IConsumer<LobbyMessage>
    {
        public async Task Consume(ConsumeContext<LobbyMessage> context)
        {
            // Process the LobbyMessage here
            Console.WriteLine($"Received LobbyMessage: {JsonSerializer.Serialize(context.Message)}");
            // Add any additional processing logic as needed
        }
    }    
}
