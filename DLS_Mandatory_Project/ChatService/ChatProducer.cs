using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChatClassLibrary;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ChatService
{
    public class ChatProducer : IChatProducer
    {
        private readonly IChannel _channel;

        public ChatProducer()
        {
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
            var connection = factory.CreateConnectionAsync().Result;
            var channel = connection.CreateChannelAsync().Result;

            channel.QueueDeclareAsync(
                queue: "chat_queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            _channel = channel;
        }

        public async Task SendToQueue(LobbyMessage lobbyMessage)
        {
            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: "chat_queue",
                mandatory: false,
                basicProperties: new BasicProperties(),
                body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(lobbyMessage))
            );
        }
    }
}
