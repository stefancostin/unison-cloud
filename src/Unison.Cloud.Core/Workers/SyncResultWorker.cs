using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
    public class SyncResultWorker : ISubscriptionWorker<AmqpSyncResponse>
    {
        private readonly IAmqpConfiguration _amqpConfig;
        private readonly ConnectionsManager _connectionsManager;
        private readonly ILogger<SyncRequestWorker> _logger;
        private readonly IAmqpPublisher _publisher;
        private readonly ISQLRepository _repository;
        private readonly IVersioningService _versioningService;
        private readonly ServicesContext _servicesContext;
        private readonly object _syncLock;

        public SyncResultWorker(
            IAmqpConfiguration amqpConfig,
            ConnectionsManager connectionsManager,
            ILogger<SyncRequestWorker> logger,
            IAmqpPublisher publisher,
            ISQLRepository repository,
            IVersioningService versioningService,
            ServicesContext servicesContext)
        {
            _amqpConfig = amqpConfig;
            _publisher = publisher;
            _connectionsManager = connectionsManager;
            _logger = logger;
            _repository = repository;
            _servicesContext = servicesContext;
            _versioningService = versioningService;
            _syncLock = new object();
        }

        public void ProcessMessage(AmqpSyncResponse message)
        {
            ValidateMessage(message);

            string correlationId = message.CorrelationId;

            LogMessage(message, correlationId);

            ConnectedInstance connectedInstance = _connectionsManager.ProcessInstance(message.Agent.InstanceId, correlationId);

            if (message.State.IsEmpty())
            {
                LogSyncStatus(correlationId);
                return;
            }

            QuerySchemaBuilder qb = new QuerySchemaBuilder();

            QuerySchema insertSchema = qb
                .From(message.State.Added)
                .ToInsertSchema()
                .MapFieldToRecords(Agent.IdKey, connectedInstance.AgentId)
                .Build();

            QuerySchema updateSchema = qb
                .From(message.State.Updated)
                .ToUpdateSchema()
                .AddWhereCondition(Agent.IdKey, connectedInstance.AgentId)
                .Build();

            QuerySchema deleteSchema = qb
                .From(message.State.Deleted)
                .ToDeleteSchema()
                .AddWhereCondition(Agent.IdKey, connectedInstance.AgentId)
                .Build();

            Dictionary<int, int> affectedRowsMap = new Dictionary<int, int>();
            SyncVersion currentVersion;

            lock (_syncLock)
            {
                currentVersion = _versioningService.GetVersion(message.State.Entity, connectedInstance.AgentId);

                if (currentVersion.Version != message.State.Version)
                    return;

                affectedRowsMap = _repository.ExecuteInTransaction(insertSchema, updateSchema, deleteSchema);

                _versioningService.IncrementVersion(currentVersion);
            }

            _logger.LogDebug($"CorrelationId: {correlationId}. Database synchronization complete.");

            int insertedRecords = affectedRowsMap.GetValueOrDefault(insertSchema.GetHashCode());
            int updatedRecords = affectedRowsMap.GetValueOrDefault(updateSchema.GetHashCode());
            int deletedRecords = affectedRowsMap.GetValueOrDefault(deleteSchema.GetHashCode());

            LogSyncStatus(correlationId, insertedRecords, updatedRecords, deletedRecords);

            SendSynchronizeCacheCommand(message.Agent.InstanceId, message.State.Entity, currentVersion.Version, correlationId);
        }

        private void LogMessage(AmqpSyncResponse message, string correlationId)
        {
            _logger.LogInformation($"CorrelationId: {correlationId}. " +
                $"Received message from agent: {message.Agent.InstanceId}. " +
                $"Added: {message.State.Added.Records.Count}, " +
                $"Updated: {message.State.Updated.Records.Count}, " +
                $"Deleted: {message.State.Deleted.Records.Count}.");
        }

        private void LogSyncStatus(string correlationId, int addedRecords = 0, int updatedRecords = 0, int deletedRecords = 0)
        {
            using (var scope = _servicesContext.Services.CreateScope())
            {
                var syncLogRepository = scope.ServiceProvider.GetRequiredService<ISyncLogRepository>();
                SyncLog syncLog = syncLogRepository.FindByCorrelationId(correlationId);

                if (syncLog == null)
                {
                    _logger.LogWarning($"CorrelationId: {correlationId}. Sync log initialized during sync request not found.");
                    return;
                }

                syncLog.AddedRecords = addedRecords;
                syncLog.UpdatedRecords = updatedRecords;
                syncLog.DeletedRecords = deletedRecords;
                syncLog.UpdatedAt = DateTime.Now;
                syncLog.Completed = true;

                syncLogRepository.Save();
            }
        }

        private void ValidateMessage(AmqpSyncResponse message)
        {
            if (message == null || message.Agent == null || message.State == null)
                throw new InvalidRequestException("A synchronization response cannot be empty");

            if (string.IsNullOrWhiteSpace(message.State.Entity))
                throw new InvalidRequestException("An entity name must be provided");
        }

        private void SendSynchronizeCacheCommand(string instanceId, string entity, long version, string correlationId)
        {
            AmqpApplyVersion message = new AmqpApplyVersion()
            {
                CorrelationId = correlationId,
                Entity = entity,
                Version = version
            };

            string exchange = _amqpConfig.Exchanges.Commands;
            string command = _amqpConfig.Commands.ApplyVersion;
            string routingKey = $"{exchange}.{command}.{instanceId}";

            _publisher.PublishMessage(message, exchange, routingKey);
        }
    }
}
