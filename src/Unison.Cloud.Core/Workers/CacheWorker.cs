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
using Unison.Cloud.Core.Exceptions;
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
            ValidateMessage(message);

            _logger.LogInformation($"Received message from agent: {message.Agent.InstanceId}");

            SyncAgent agent = GetAgentInformation(message.Agent.InstanceId);

            if (agent == null)
                throw new AgentNotFoundException("An agent with the provided instance id does not exist");

            List<SyncEntity> entities = GetEntitiesMetadata(agent.NodeId);

            if (!entities.Any())
            {
                _logger.LogInformation($"No synchronizable entities have been defined for agent: {message.Agent.InstanceId}");
                return;
            }

            List<DataSet> entitiesCache = RetrieveEntitiesAndPrepareCache(entities, agent);

            AmqpCache cacheMessage = MapEntitiesCacheToResponseModel(entitiesCache);
            SendCache(cacheMessage);
        }

        private SyncAgent GetAgentInformation(string instanceId)
        {
            using (var scope = _servicesContext.Services.CreateScope())
            {
                var agentRepository = scope.ServiceProvider.GetRequiredService<ISyncAgentRepository>();
                return agentRepository.FindByInstanceId(instanceId);
            }
        }

        private List<SyncEntity> GetEntitiesMetadata(int nodeId)
        {
            using (var scope = _servicesContext.Services.CreateScope())
            {
                var entityRepository = scope.ServiceProvider.GetRequiredService<ISyncEntityRepository>();
                return entityRepository.FindByNodeId(nodeId).ToList();
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

        private AmqpCache MapEntitiesCacheToResponseModel(List<DataSet> entitiesCache)
        {
            return new AmqpCache()
            {
                Entities = entitiesCache.Select(e => e.ToAmqpDataSetModel()).ToList()
            };
        }

        private List<DataSet> RetrieveEntitiesAndPrepareCache(List<SyncEntity> entities, SyncAgent agent)
        {
            List<DataSet> entitiesCache = new List<DataSet>();
            QuerySchemaBuilder qb = new QuerySchemaBuilder();

            foreach (SyncEntity entity in entities)
            {
                // TODO: Construct the query schema from the client's database records
                var schema = qb
                    .From("Products") // entity.Entity
                    .ToReadSchema()
                    .SetPrimaryKey(Agent.RecordIdKey)
                    .AddSelectFields(Agent.RecordIdKey, "Name", "Price")
                    .AddWhereCondition(Agent.IdKey, agent.Id)
                    .Build();

                DataSet cache = _repository.Read(schema);
                cache.Version = entity.Version;
                cache.Records = MapAgentPrimaryKey(cache, "Id"); // entity.PrimaryKey

                entitiesCache.Add(cache);
            }

            return entitiesCache;
        }

        private void SendCache(AmqpCache message)
        {
            var exchange = _amqpConfig.Exchanges.Commands;
            var command = _amqpConfig.Commands.Cache;
            var routingKey = $"{exchange}.{command}";

            _publisher.PublishMessage(message, exchange, routingKey);
        }

        private void ValidateMessage(AmqpConnected message)
        {
            if (message == null || message.Agent == null)
                throw new InvalidRequestException("A connection/cache request cannot be empty");

            if (string.IsNullOrWhiteSpace(message.Agent.InstanceId))
                throw new InvalidRequestException("An agent instance id must be provided");
        }
    }
}
