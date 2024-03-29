﻿using Microsoft.Extensions.DependencyInjection;
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
using Unison.Cloud.Core.Interfaces.Services;
using Unison.Cloud.Core.Interfaces.Workers;
using Unison.Cloud.Core.Models;
using Unison.Cloud.Core.Services;
using Unison.Cloud.Core.Utilities;
using Unison.Common.Amqp.DTO;
using Unison.Common.Amqp.Interfaces;

namespace Unison.Cloud.Core.Workers
{
    public class CacheWorker : ISubscriptionWorker<AmqpConnected>
    {
        private readonly IAmqpConfiguration _amqpConfig;
        private readonly ConnectionsManager _connectionsManager;
        private readonly IAmqpPublisher _publisher;
        private readonly ISQLRepository _repository;
        private readonly IVersioningService _versioningService;
        private readonly ServicesContext _servicesContext;
        private readonly ITimerConfiguration _timerConfig;
        private readonly ILogger<CacheWorker> _logger;

        public CacheWorker(
            IAmqpConfiguration amqpConfig,
            ConnectionsManager connectionsManager,
            IAmqpPublisher publisher,
            ISQLRepository repository,
            IVersioningService versioningService,
            ServicesContext servicesContext,
            ITimerConfiguration timerConfig,
            ILogger<CacheWorker> logger)
        {
            _amqpConfig = amqpConfig;
            _connectionsManager = connectionsManager;
            _publisher = publisher;
            _repository = repository;
            _versioningService = versioningService;
            _servicesContext = servicesContext;
            _timerConfig = timerConfig;
            _logger = logger;
        }

        public void ProcessMessage(AmqpConnected message)
        {
            ValidateMessage(message);

            string correlationId = Guid.NewGuid().ToString();

            _logger.LogInformation($"CorrelationId: {correlationId}. " +
                $"Received connection from agent: {message.Agent.InstanceId}.");

            ConnectedInstance connectedInstance = _connectionsManager.ProcessInstance(message.Agent.InstanceId, correlationId);

            SendConfiguration(message.Agent.InstanceId, correlationId);

            List<SyncEntity> entities = GetEntitiesMetadata(connectedInstance.NodeId);

            if (!entities.Any())
            {
                _logger.LogWarning($"CorrelationId: {correlationId}. " +
                    $"No synchronizable entities have been defined for agent: {message.Agent.InstanceId}.");
                return;
            }

            List<DataSet> entitiesCache = RetrieveEntitiesAndPrepareCache(entities, connectedInstance.AgentId);

            _logger.LogInformation($"CorrelationId: {correlationId}. " +
                $"{entitiesCache.Count} entities are sent to be cached by agent: {message.Agent.InstanceId}.");

            AmqpCache cacheMessage = MapEntitiesCacheToResponseModel(entitiesCache, correlationId);
            SendCache(cacheMessage, connectedInstance.InstanceId);
        }

        private long ExtractVersion(string entity, int agentId)
        {
            return _versioningService.GetVersion(entity, agentId)?.Version ?? Versioning.NewVersion;
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

        private AmqpCache MapEntitiesCacheToResponseModel(List<DataSet> entitiesCache, string correlationId)
        {
            return new AmqpCache()
            {
                CorrelationId = correlationId,
                Entities = entitiesCache.Select(e => e.ToAmqpDataSetModel()).ToList()
            };
        }

        private List<DataSet> RetrieveEntitiesAndPrepareCache(List<SyncEntity> entities, int agentId)
        {
            List<DataSet> entitiesCache = new List<DataSet>();
            QuerySchemaBuilder qb = new QuerySchemaBuilder();

            foreach (SyncEntity entity in entities)
            {
                try
                {
                    var schema = qb
                        .From(entity.Entity)
                        .ToReadSchema()
                        .SetPrimaryKey(Agent.RecordIdKey)
                        .AddSelectFields(Agent.RecordIdKey)
                        .AddSelectFields(entity.Fields.ToArray())
                        .AddWhereCondition(Agent.IdKey, agentId)
                        .Build();

                    DataSet cache = _repository.Read(schema);
                    cache.Version = ExtractVersion(entity.Entity, agentId);
                    cache.Records = MapAgentPrimaryKey(cache, entity.PrimaryKey);

                    entitiesCache.Add(cache);
                }
                catch (Exception ex)
                {
                    LogError(ex, entity, agentId);
                }
            }

            return entitiesCache;
        }

        private void LogError(Exception ex, SyncEntity entity, int agentId)
        {
            using (var scope = _servicesContext.Services.CreateScope())
            {
                var syncLogRepository = scope.ServiceProvider.GetRequiredService<ISyncLogRepository>();

                SyncLog syncError = new SyncLog()
                {
                    AgentId = agentId,
                    Completed = false,
                    CorrelationId = Guid.NewGuid().ToString(),
                    Entity = entity.Entity,
                    ErrorMessage = "Origin: Cloud Server Cache Worker. " + ex.Message,
                };

                syncLogRepository.Add(syncError);
                syncLogRepository.Save();
            }
        }

        private void SendCache(AmqpCache message, string instanceId)
        {
            var exchange = _amqpConfig.Exchanges.Commands;
            var command = _amqpConfig.Commands.Cache;

            var routingKey = $"{exchange}.{command}.{instanceId}";

            _publisher.PublishMessage(message, exchange, routingKey);
        }

        private void SendConfiguration(string instanceId, string correlationId)
        {
            _logger.LogInformation($"CorrelationId: {correlationId}. " +
                $"Sending heartbeat configuration to agent {instanceId}.");

            var message = new AmqpAgentConfiguration()
            {
                CorrelationId = correlationId,
                HeartbeatTimer = _timerConfig.HeartbeatTimer
            };

            var exchange = _amqpConfig.Exchanges.Commands;
            var command = _amqpConfig.Commands.Configure;

            var routingKey = $"{exchange}.{command}.{instanceId}";

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
