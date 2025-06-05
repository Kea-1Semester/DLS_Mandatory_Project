using ChatClassLibrary;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace MessageConsumer
{
    public class ChatConsumer : BackgroundService
    {
        private IConnection _connection;
        private IChannel _channel;

        public ChatConsumer()
        {
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
            _connection = factory.CreateConnectionAsync().Result;
            _channel = _connection.CreateChannelAsync().Result;

            _channel.QueueDeclareAsync(
                queue: "chat_queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Received message: {message}");
                // Process the message (e.g., save to database, send to another service, etc.)
                await Task.Delay(1000); // Simulate some processing delay
            };

            await _channel.BasicConsumeAsync(
                queue: "chat_queue",
                autoAck: true,
                consumer: consumer
            );
        }
    }
}
