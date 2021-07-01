using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Builders;
using Unison.Cloud.Core.Data;
using Unison.Cloud.Core.Data.Entities;
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
        private readonly IAmqpConfiguration _amqpConfig;
        private readonly IAmqpPublisher _publisher;
        private readonly ISQLRepository _repository;
        private readonly ServicesContext _servicesContext;
        private readonly ILogger<CacheWorker> _logger;

        public CacheWorker(ISQLRepository repository, IAmqpConfiguration amqpConfig, IAmqpPublisher publisher, ServicesContext servicesContext, ILogger<CacheWorker> logger)
        {
            _repository = repository;
            _amqpConfig = amqpConfig;
            _publisher = publisher;
            _servicesContext = servicesContext;
            _logger = logger;
        }

        public void ProcessMessage(AmqpConnected message)
        {
            _logger.LogInformation($"Connections-Worker: Got message from agent with id: {message.Agent.AgentId}");

            IEnumerable<SyncEntity> entities = GetEntityMetadata();
            SyncEntity productEntity = entities.First();

            // TODO: Construct the query schema from the client's database records
            var agentId = 1;
            var qb = new QuerySchemaBuilder();
            var schema = qb
                .From("Products")
                .ToReadSchema()
                .SetPrimaryKey(Agent.RecordIdKey)
                .AddSelectFields(Agent.RecordIdKey, "Name", "Price")
                .AddWhereCondition(Agent.IdKey, agentId)
                .Build();

            DataSet products = _repository.Read(schema);
            products.Version = productEntity.Version;
            products.Records = MapAgentPrimaryKey(products, "Id");

            AmqpDataSet productsCache = products.ToAmqpDataSetModel();

            var cache = new AmqpCache()
            {
                Entities = new List<AmqpDataSet> { productsCache }
            };

            PublishMessage(cache);
        }

        private IEnumerable<SyncEntity> GetEntityMetadata()
        {
            using (var scope = _servicesContext.Services.CreateScope())
            {
                var entityRepository = scope.ServiceProvider.GetRequiredService<ISyncEntityRepository>();
                return entityRepository.GetAll();
            }
        }

        private Dictionary<string, Record> MapAgentPrimaryKey(DataSet dataSet, string primaryKey)
        {
            return dataSet.Records.Select(r =>
            {
                r.Value.Fields = r.Value.Fields.ToDictionary(f => f.Key == Agent.RecordIdKey ? primaryKey : f.Key, f =>
                {
                    if (f.Value.Name == Agent.RecordIdKey)
                        f.Value.Name = primaryKey;
                    return f.Value;
                });
                return r;
            })
            .ToDictionary(r => r.Key, r => r.Value);
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
