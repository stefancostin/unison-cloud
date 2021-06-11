using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Interfaces.Amqp;
using Unison.Cloud.Core.Interfaces.Workers;
using Unison.Cloud.Core.Models;

namespace Unison.Cloud.Core.Workers
{
    public class SyncResultWorker : ISubscriptionWorker
    {
        private readonly ILogger<SyncRequestWorker> _logger;

        public SyncResultWorker(ILogger<SyncRequestWorker> logger)
        {
            _logger = logger;
        }

        public void ProcessRequest(string message)
        {
            _logger.LogInformation($"Got message from agent: {message}");
        }
    }
}
