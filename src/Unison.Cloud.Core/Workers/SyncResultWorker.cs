using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Interfaces.Workers;
using Unison.Cloud.Core.Models;
using Unison.Cloud.Core.Utilities;
using Unison.Common.Amqp.DTO;

namespace Unison.Cloud.Core.Workers
{
    public class SyncResultWorker : ISubscriptionWorker<AmqpSyncResponse>
    {
        private readonly ILogger<SyncRequestWorker> _logger;

        public SyncResultWorker(ILogger<SyncRequestWorker> logger)
        {
            _logger = logger;
        }

        public void ProcessMessage(AmqpSyncResponse message)
        {
            _logger.LogInformation(
                $"Got message from agent: {message.Agent.AgentId}." +
                $" Added: {message.State.Added.Records.Count}," +
                $" Updated: {message.State.Updated.Records.Count}," +
                $" Deleted: {message.State.Deleted.Records.Count}.");

            QuerySchema insertSchema = message.State.Added.ToQuerySchema(QueryOperation.Insert);
            QuerySchema updateSchema = message.State.Updated.ToQuerySchema(QueryOperation.Update);
            QuerySchema deleteSchema = message.State.Deleted.ToQuerySchema(QueryOperation.Delete);

            Console.WriteLine("Converted to schema");
        }
    }
}
