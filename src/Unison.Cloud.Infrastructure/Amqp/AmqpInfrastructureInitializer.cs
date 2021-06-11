using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Interfaces.Amqp;
using Unison.Cloud.Core.Interfaces.Configuration;
using Unison.Cloud.Core.Models;
using Unison.Cloud.Infrastructure.Amqp.Interfaces;
using Unison.Cloud.Infrastructure.Amqp.Models;

namespace Unison.Cloud.Infrastructure.Amqp
{
    public class AmqpInfrastructureInitializer : IAmqpInfrastructureInitializer
    {
        private readonly IAmqpConfiguration _amqpConfig;
        private readonly IAmqpChannelFactory _channelFactory;
        private readonly Dictionary<string, string> _consumerExchangeQueueMap;

        public AmqpInfrastructureInitializer(IAmqpConfiguration amqpConfig, IAmqpChannelFactory channelFactory)
        {
            _amqpConfig = amqpConfig;
            _channelFactory = channelFactory;
            _consumerExchangeQueueMap = new Dictionary<string, string>();
        }

        public Dictionary<string, string> Initialize()
        {
            using (var channel = _channelFactory.CreateUnmanagedChannel())
            {
                BindToResponsesExchange(channel);
            }
            return _consumerExchangeQueueMap;
        }

        private void BindToResponsesExchange(IModel channel)
        {
            var exchange = _amqpConfig.Exchanges.Response;
            var queue = exchange;
            var routingKey = exchange;

            channel.QueueDeclare(queue: queue,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.QueueBind(queue: queue,
                 exchange: exchange,
                 routingKey: routingKey,
                 arguments: null);

            _consumerExchangeQueueMap.Add(AmqpExchangeNames.Responses, queue);
        }
    }
}
