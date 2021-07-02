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
using Unison.Cloud.Core.Utilities;
using Unison.Common.Amqp.DTO;
using Unison.Common.Amqp.Interfaces;

namespace Unison.Cloud.Core.Workers
{
    public class SyncResultWorker : ISubscriptionWorker<AmqpSyncResponse>
    {
        private readonly IAmqpConfiguration _amqpConfig;
        private readonly IAmqpPublisher _publisher;
        private readonly ISQLRepository _repository;
        private readonly ILogger<SyncRequestWorker> _logger;
        private readonly object _syncLock;

        public SyncResultWorker(IAmqpConfiguration amqpConfig, IAmqpPublisher publisher, ISQLRepository repository, ILogger<SyncRequestWorker> logger)
        {
            _amqpConfig = amqpConfig;
            _publisher = publisher;
            _repository = repository;
            _logger = logger;
            _syncLock = new object();
        }

        public void ProcessMessage(AmqpSyncResponse message)
        {
            _logger.LogInformation(
                $"Received message from agent: {message.Agent.InstanceId}." +
                $" Added: {message.State.Added.Records.Count}," +
                $" Updated: {message.State.Updated.Records.Count}," +
                $" Deleted: {message.State.Deleted.Records.Count}.");

            ValidateMessage(message);

            // TODO: Start the sync log
            int syncLogId = 1;

            // TODO: Get the agent id from an agents table based on the input received from the request
            int agentId = 1;

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
                .MapFieldToRecords(Agent.IdKey, agentId)
                .Build();

            QuerySchema updateSchema = qb
                .From(message.State.Updated)
                .ToUpdateSchema()
                .AddWhereCondition(Agent.IdKey, agentId)
                .Build();

            QuerySchema deleteSchema = qb
                .From(message.State.Deleted)
                .ToDeleteSchema()
                .AddWhereCondition(Agent.IdKey, agentId)
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

            _logger.LogInformation("Database synchronization complete");

            int insertedRecords = affectedRowsMap.GetValueOrDefault(insertSchema.GetHashCode());
            int updatedRecords = affectedRowsMap.GetValueOrDefault(updateSchema.GetHashCode());
            int deletedRecords = affectedRowsMap.GetValueOrDefault(deleteSchema.GetHashCode());

            LogSyncStatus(syncLogId, insertedRecords, updatedRecords, deletedRecords);

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

        private void LogSyncStatus(int syncLogId, int insertedRecords, int updatedRecords, int deletedRecords)
        {
            // Update the sync log
        }

        private void ValidateMessage(AmqpSyncResponse message)
        {
            if (message == null || message.Agent == null || message.State == null)
                throw new InvalidRequestException("A synchronization response cannot be empty");

            if (string.IsNullOrWhiteSpace(message.State.Entity))
                throw new InvalidRequestException("An entity name must be provided");

            //if (message.State.IsEmpty())
            //    throw new InvalidRequestException("The synchronization state cannot be empty");
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
