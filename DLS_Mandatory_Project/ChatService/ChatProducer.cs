using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChatClassLibrary;
using MassTransit;
using MassTransit.Transports;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ChatService
{
    public class ChatProducer : IChatProducer
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public ChatProducer(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task SendToQueue(LobbyMessage lobbyMessage)
        {
            await _publishEndpoint.Publish(lobbyMessage);
        }
    }
}
