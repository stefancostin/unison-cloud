using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Interfaces.Workers;
using Unison.Cloud.Core.Models;
using Unison.Common.Amqp.DTO;

namespace Unison.Cloud.Core.Workers
{
    public class SyncResultWorker : ISubscriptionWorker<AmqpResponse>
    {
        private readonly ILogger<SyncRequestWorker> _logger;

        public SyncResultWorker(ILogger<SyncRequestWorker> logger)
        {
            _logger = logger;
        }

        public void ProcessMessage(AmqpResponse message)
        {
            _logger.LogInformation($"Got message from agent: {message.QueryResult}");
        }
    }
}
