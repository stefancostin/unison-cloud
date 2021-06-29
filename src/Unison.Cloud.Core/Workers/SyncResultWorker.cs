using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Builders;
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
        private readonly IServiceProvider _services;
        private readonly ILogger<SyncRequestWorker> _logger;

        public SyncResultWorker(IAmqpConfiguration amqpConfig, IAmqpPublisher publisher, ISQLRepository repository, IServiceProvider services, ILogger<SyncRequestWorker> logger)
        {
            _amqpConfig = amqpConfig;
            _publisher = publisher;
            _repository = repository;
            _services = services;
            _logger = logger;
        }

        public void ProcessMessage(AmqpSyncResponse message)
        {
            _logger.LogInformation(
                $"Got message from agent: {message.Agent.AgentId}." +
                $" Added: {message.State.Added.Records.Count}," +
                $" Updated: {message.State.Updated.Records.Count}," +
                $" Deleted: {message.State.Deleted.Records.Count}.");

            ValidateMessage(message);

            // TODO: Get the agent id from an agents table based on the input received from the request
            QuerySchemaBuilder qb = new QuerySchemaBuilder(agentId: 1); 

            QuerySchema insertSchema = qb.From(message.State.Added).ToInsertSchema().Build();
            QuerySchema updateSchema = qb.From(message.State.Updated).ToUpdateSchema().Build();
            QuerySchema deleteSchema = qb.From(message.State.Deleted).ToDeleteSchema().Build();

            // TODO: Add transaction
            int insertedRecords = _repository.Execute(insertSchema);
            int updatedRecords = _repository.Execute(updateSchema);
            int deletedRecords = _repository.Execute(deleteSchema);

            Console.WriteLine("Database synchronized");

            if (insertedRecords == 0 && updatedRecords == 0 && deletedRecords == 0)
                return;

            SynchronizeAgentCache(message.Agent.AgentId, message.State.Entity, 0);
        }

        private void ValidateMessage(AmqpSyncResponse message)
        {
            if (message == null || message.Agent == null || message.State == null)
                throw new InvalidRequestException("A synchronization response cannot be empty");

            if (string.IsNullOrWhiteSpace(message.State.Entity))
                throw new InvalidRequestException("An entity name must be provided");

            if (message.State.IsEmpty())
                throw new InvalidRequestException("The synchronization state cannot be empty");
        }

        private void SynchronizeAgentCache(string agentId, string entity, long version)
        {
            AmqpApplyVersion message = new AmqpApplyVersion()
            {
                Entity = entity,
                Version = version
            };

            string exchange = _amqpConfig.Exchanges.Commands;
            string command = _amqpConfig.Commands.ApplyVersion;
            string routingKey = $"{exchange}.{command}.{agentId}";

            _publisher.PublishMessage(message, exchange, routingKey);
        }
    }
}
