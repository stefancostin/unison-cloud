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
        private readonly ServicesContext _servicesContext;
        private readonly object _syncLock;

        public SyncResultWorker(
            IAmqpConfiguration amqpConfig,
            ConnectionsManager connectionsManager,
            ILogger<SyncRequestWorker> logger,
            IAmqpPublisher publisher,
            ISQLRepository repository,
            ServicesContext servicesContext)
        {
            _amqpConfig = amqpConfig;
            _publisher = publisher;
            _connectionsManager = connectionsManager;
            _logger = logger;
            _repository = repository;
            _servicesContext = servicesContext;
            _syncLock = new object();
        }

        public void ProcessMessage(AmqpSyncResponse message)
        {
            ValidateMessage(message);

            string correlationId = message.CorrelationId;

            _logger.LogInformation($"CorrelationId: {correlationId}. " +
                $"Received message from agent: {message.Agent.InstanceId}. " +
                $"Added: {message.State.Added.Records.Count}, " +
                $"Updated: {message.State.Updated.Records.Count}, " +
                $"Deleted: {message.State.Deleted.Records.Count}.");

            ConnectedInstance connectedInstance = _connectionsManager.ProcessInstance(message.Agent.InstanceId, correlationId);

            if (message.State.IsEmpty())
            {
                LogSyncStatus(correlationId);
                return;
            }

            QuerySchemaBuilder qb = new QuerySchemaBuilder();

            QuerySchema readVersionSchema = qb
                .From(GetSyncEntityTableName())
                .ToReadSchema()
                .AddSelectFields(nameof(SyncEntity.Id), nameof(SyncEntity.Version))
                .SetPrimaryKey(nameof(SyncEntity.Id))
                .AddWhereCondition(nameof(SyncEntity.Entity), message.State.Entity)
                .Build();

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
            long currentVersion;

            lock (_syncLock)
            {
                DataSet entityMetadata = _repository.Read(readVersionSchema);
                currentVersion = (long)GetCurrentEntityVersion(entityMetadata, nameof(SyncEntity.Version));
                int entityId = (int)GetCurrentEntityVersion(entityMetadata, nameof(SyncEntity.Id));

                if (currentVersion != message.State.Version)
                    return;

                long newVersion = currentVersion;
                Interlocked.Increment(ref newVersion);

                QuerySchema incrementVersionSchema = qb
                    .From(GetSyncEntityTableName())
                    .ToUpdateSchema()
                    .SetPrimaryKey(nameof(SyncEntity.Id))
                    .AddRecord(new QueryParam { Name = nameof(SyncEntity.Id), Value = entityId },
                               new QueryParam { Name = nameof(SyncEntity.Version), Value = newVersion })
                    .AddWhereCondition(nameof(SyncEntity.Entity), message.State.Entity)
                    .Build();

                affectedRowsMap = _repository.ExecuteInTransaction(
                    insertSchema, updateSchema, deleteSchema, incrementVersionSchema);
            }

            _logger.LogDebug($"CorrelationId: {correlationId}. Database synchronization complete.");

            int insertedRecords = affectedRowsMap.GetValueOrDefault(insertSchema.GetHashCode());
            int updatedRecords = affectedRowsMap.GetValueOrDefault(updateSchema.GetHashCode());
            int deletedRecords = affectedRowsMap.GetValueOrDefault(deleteSchema.GetHashCode());

            LogSyncStatus(correlationId, insertedRecords, updatedRecords, deletedRecords);

            SendSynchronizeCacheCommand(message.Agent.InstanceId, message.State.Entity, currentVersion);
        }

        private string GetSyncEntityTableName()
        {
            TableAttribute tableAttribute = (TableAttribute)Attribute.GetCustomAttribute(typeof(SyncEntity), typeof(TableAttribute));
            return tableAttribute.Name;
        }

        private object GetCurrentEntityVersion(DataSet entityMetadata, string fieldName)
        {
            return entityMetadata.Records.First().Value.Fields.Values.First(f => f.Name == fieldName).Value;
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

                syncLogRepository.SaveChanges();
            }
        }

        private void ValidateMessage(AmqpSyncResponse message)
        {
            if (message == null || message.Agent == null || message.State == null)
                throw new InvalidRequestException("A synchronization response cannot be empty");

            if (string.IsNullOrWhiteSpace(message.State.Entity))
                throw new InvalidRequestException("An entity name must be provided");
        }

        private void SendSynchronizeCacheCommand(string instanceId, string entity, long version)
        {
            AmqpApplyVersion message = new AmqpApplyVersion()
            {
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
