using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Interfaces.Configuration;
using Unison.Cloud.Core.Models;
using Unison.Common.Amqp.Interfaces;

namespace Unison.Cloud.Infrastructure.Amqp
{
    public class AmqpInfrastructureInitializer : IAmqpInfrastructureInitializer
    {
        private readonly IAmqpConfiguration _amqpConfig;
        private readonly IAmqpChannelFactory _channelFactory;

        public AmqpInfrastructureInitializer(IAmqpConfiguration amqpConfig, IAmqpChannelFactory channelFactory)
        {
            _amqpConfig = amqpConfig;
            _channelFactory = channelFactory;

            amqpConfig.Queues = new AmqpQueues();
        }

        public void Initialize()
        {
            using (var channel = _channelFactory.CreateUnmanagedChannel())
            {
                BindToConnectionsExchange(channel);
                BindToErrorsExchange(channel);
                BindToHeartbeatExchange(channel);
                BindToResponsesExchange(channel);
            }
        }

        private void BindToConnectionsExchange(IModel channel)
        { 
            var exchange = _amqpConfig.Exchanges.Connections;
            var queue = exchange;
            var routingKey = exchange;

            channel.QueueDeclare(queue: queue,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.QueueBind(queue: queue,
                 exchange: exchange,
                 routingKey: routingKey,
                 arguments: null);

            channel.QueuePurge(queue);

            _amqpConfig.Queues.Connections = queue;
        }

        private void BindToErrorsExchange(IModel channel)
        {
            var exchange = _amqpConfig.Exchanges.Errors;
            var queue = exchange;
            var routingKey = exchange;

            channel.QueueDeclare(queue: queue,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.QueueBind(queue: queue,
                 exchange: exchange,
                 routingKey: routingKey,
                 arguments: null);

            channel.QueuePurge(queue);

            _amqpConfig.Queues.Errors = queue;
        }

        private void BindToHeartbeatExchange(IModel channel)
        {
            var exchange = _amqpConfig.Exchanges.Heartbeat;
            var queue = exchange;
            var routingKey = exchange;

            channel.QueueDeclare(queue: queue,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.QueueBind(queue: queue,
                 exchange: exchange,
                 routingKey: routingKey,
                 arguments: null);

            channel.QueuePurge(queue);

            _amqpConfig.Queues.Heartbeat = queue;
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

            channel.QueuePurge(queue);

            _amqpConfig.Queues.Response = queue;
        }
    }
}
