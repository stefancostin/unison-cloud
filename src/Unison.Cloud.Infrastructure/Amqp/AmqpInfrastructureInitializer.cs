using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Interfaces.Configuration;
using Unison.Common.Amqp.Constants;
using Unison.Common.Amqp.Interfaces;

namespace Unison.Cloud.Infrastructure.Amqp
{
    public class AmqpInfrastructureInitializer : IAmqpInfrastructureInitializer
    {
        private readonly IAmqpConfiguration _amqpConfig;
        private readonly IAmqpChannelFactory _channelFactory;
        private readonly IAmqpInitializationState _initializationState;

        public AmqpInfrastructureInitializer(IAmqpConfiguration amqpConfig, IAmqpChannelFactory channelFactory, IAmqpInitializationState initializationState)
        {
            _amqpConfig = amqpConfig;
            _channelFactory = channelFactory;
            _initializationState = initializationState;
        }

        public void Initialize()
        {
            using (var channel = _channelFactory.CreateUnmanagedChannel())
            {
                BindToResponsesExchange(channel);
            }
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

            _initializationState.ConsumerExchangeQueueMap.Add(AmqpExchangeNames.Responses, queue);
        }
    }
}
