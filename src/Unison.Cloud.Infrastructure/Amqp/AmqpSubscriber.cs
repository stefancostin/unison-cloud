using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Services.Amqp;

namespace Unison.Cloud.Infrastructure.Amqp
{
    public class AmqpSubscriber : IAmqpSubscriber
    {
        private const string queue = "connections";

        private readonly IAmqpChannelFactory _channelFactory;
        private readonly ILogger<AmqpSubscriber> _logger;

        public AmqpSubscriber(IAmqpChannelFactory channelFactory, ILogger<AmqpSubscriber> logger)
        {
            _channelFactory = channelFactory;
            _logger = logger;

            _logger.LogInformation("Hello from Subscriber");

            Subscribe();
        }

        private void Subscribe()
        {
            using var channel = _channelFactory.CreateUnmanagedChannel();

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation(message);
            };

            channel.BasicConsume(queue: queue, autoAck: true, consumer: consumer);
        }
    }
}
