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
    public class SyncResultWorker : ISubscriptionWorker<AmqpSyncResponse>
    {
        private readonly IAmqpConfiguration _amqpConfig;
        private readonly IAmqpPublisher _publisher;
        private readonly ISQLRepository _repository;
        private readonly ILogger<SyncRequestWorker> _logger;

        public SyncResultWorker(IAmqpConfiguration amqpConfig, IAmqpPublisher publisher, ISQLRepository repository, ILogger<SyncRequestWorker> logger)
        {
            _amqpConfig = amqpConfig;
            _publisher = publisher;
            _repository = repository;
            _logger = logger;
        }

        public void ProcessMessage(AmqpSyncResponse message)
        {
            _logger.LogInformation(
                $"Got message from agent: {message.Agent.AgentId}." +
                $" Added: {message.State.Added.Records.Count}," +
                $" Updated: {message.State.Updated.Records.Count}," +
                $" Deleted: {message.State.Deleted.Records.Count}.");

            // TODO: Get the agent id from an agents table based on the input received from the request

            QuerySchema insertSchema = message.State.Added.ToQuerySchema(1, QueryOperation.Insert);
            QuerySchema updateSchema = message.State.Updated.ToQuerySchema(1, QueryOperation.Update);
            QuerySchema deleteSchema = message.State.Deleted.ToQuerySchema(1, QueryOperation.Delete);

            int updatedRecords = _repository.Execute(updateSchema);

            Console.WriteLine("Converted to schema");
        }
    }
}
