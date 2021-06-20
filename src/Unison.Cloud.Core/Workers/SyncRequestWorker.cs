using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Interfaces.Workers;
using Unison.Common.Amqp.DTO;
using Unison.Common.Amqp.Interfaces;

namespace Unison.Cloud.Core.Workers
{
    public class SyncRequestWorker : ITimedWorker
    {
        private readonly ILogger<SyncRequestWorker> _logger;
        private readonly IAmqpPublisher _amqpPublisher;

        public SyncRequestWorker(ILogger<SyncRequestWorker> logger, IAmqpPublisher amqpPublisher)
        {
            _logger = logger;
            _amqpPublisher = amqpPublisher;
        }

        public void Execute(object state)
        {
            _logger.LogInformation("Sending query...");
            var message = new AmqpSyncRequest { Entity = "Products", Fields = new List<string>() { "Id", "Name", "Price" }, PrimaryKey = "Id" };
            _amqpPublisher.PublishMessage(message, "unison.commands", "unison.commands.sync");
        }
    }
}
