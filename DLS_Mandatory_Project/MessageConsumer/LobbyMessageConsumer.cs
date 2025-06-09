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
        private readonly MessageDbContext _context;

        public LobbyMessageConsumer(MessageDbContext messageDbContext)
        {
            _context = messageDbContext;
        }

        public async Task Consume(ConsumeContext<LobbyMessage> context)
        {            
            Console.WriteLine($"Received LobbyMessage: {JsonSerializer.Serialize(context.Message)}");
            await _context.LobbyMessages.AddAsync(context.Message);
            await _context.SaveChangesAsync();
            Console.WriteLine("LobbyMessage saved...");
        }
    }    
}
