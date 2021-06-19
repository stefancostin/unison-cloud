using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Interfaces.Configuration;
using Unison.Cloud.Core.Interfaces.Data;
using Unison.Cloud.Core.Interfaces.Workers;
using Unison.Common.Amqp.DTO;
using Unison.Common.Amqp.Interfaces;

namespace Unison.Cloud.Core.Workers
{
    public class CacheWorker : ISubscriptionWorker<AmqpConnected>
    {
        private readonly ISQLRepository _repository;
        private readonly IAmqpConfiguration _amqpConfig;
        private readonly IAmqpPublisher _publisher;
        private readonly ILogger<CacheWorker> _logger;

        public CacheWorker(ISQLRepository repository, IAmqpConfiguration amqpConfig, IAmqpPublisher publisher, ILogger<CacheWorker> logger)
        {
            _repository = repository;
            _amqpConfig = amqpConfig;
            _publisher = publisher;
            _logger = logger;
        }

        public void ProcessMessage(AmqpConnected message)
        {
            _logger.LogInformation($"Connections-Worker: Got message from agent with id: {message.Agent.AgentId}");

            var products = _repository.Read("SELECT Id, Name, Price FROM Products");

            var productsCache = new AmqpCachedEntity() { Entity = "Products", Data = products };

            var cache = new AmqpCache() {
                Entities = new List<AmqpCachedEntity> { productsCache }
            };

            PublishMessage(cache);
        }

        private void PublishMessage(AmqpCache message)
        {
            var exchange = _amqpConfig.Exchanges.Commands;
            var command = _amqpConfig.Commands.Cache;
            var routingKey = $"{exchange}.{command}";

            _publisher.PublishMessage(message, exchange, routingKey);
        }
    }
}
