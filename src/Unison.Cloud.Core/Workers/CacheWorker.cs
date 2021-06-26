using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Interfaces.Configuration;
using Unison.Cloud.Core.Interfaces.Data;
using Unison.Cloud.Core.Interfaces.Workers;
using Unison.Cloud.Core.Models;
using Unison.Cloud.Core.Utilities;
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

            // TODO: Construct the query schema from the client's database records
            var schema = new QuerySchema()
            {
                Entity = "Products",
                PrimaryKey = Agent.RecordIdKey,
                Fields = new List<string>() { Agent.RecordIdKey, "Name", "Price" },
                Conditions = new List<QueryParam>() { new QueryParam { Name = Agent.IdKey, Value = 1 } }
            };

            var products = _repository.Read(schema);

            // TODO: Extract this to a different service or query schema mapper + Also add the specific actions from Mapper
            // Map agent primary key to agent record id field
            products.Records = products.Records.Select(r =>
            {
                r.Value.Fields = r.Value.Fields.ToDictionary(f => f.Key == Agent.RecordIdKey ? "Id" : f.Key, f =>
                {
                    if (f.Value.Name == Agent.RecordIdKey)
                        f.Value.Name = "Id";
                    return f.Value;
                });
                return r;
            })
            .ToDictionary(r => r.Key, r => r.Value);

            var productsCache = products.ToAmqpDataSetModel();

            var cache = new AmqpCache()
            {
                Entities = new List<AmqpDataSet> { productsCache }
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
