using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;
using Unison.Cloud.Core.Interfaces.Amqp;
using Unison.Cloud.Core.Models;
using Unison.Cloud.Infrastructure.Amqp.Interfaces;

namespace Unison.Cloud.Infrastructure.Amqp.Client
{
    public class AmqpPublisher : IAmqpPublisher
    {
        private readonly IAmqpChannelFactory _channelFactory;
        private readonly IAmqpManagedChannel _channel;

        private const string exchange = "unison.commands";
        private const string queue = "unison.commands.sync";

        public AmqpPublisher(IAmqpChannelFactory channelFactory)
        {
            _channelFactory = channelFactory;
            _channel = channelFactory.CreateManagedChannel();
        }

        public void Publish(AmqpMessage message)
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            _channel.GetChannel().BasicPublish(exchange: exchange,
                                               routingKey: queue,
                                               basicProperties: null,
                                               body: body);
        }

        // I can have another method for TaskParallel operations that each create a channel
        // Basically, this meants having a method be used for parallel that begins with:
        //   using var channel = _channelFactory.CreateChannel();

    }
}
