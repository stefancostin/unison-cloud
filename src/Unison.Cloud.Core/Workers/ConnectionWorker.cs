using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Interfaces.Workers;
using Unison.Common.Amqp.DTO;

namespace Unison.Cloud.Core.Workers
{
    public class ConnectionWorker : ISubscriptionWorker<AmqpConnected>
    {
        private readonly ILogger<ConnectionWorker> _logger;

        public ConnectionWorker(ILogger<ConnectionWorker> logger)
        {
            _logger = logger;
        }

        public void ProcessMessage(AmqpConnected message)
        {
            _logger.LogInformation($"Connections-Worker: Got message from agent with id: {message.Agent.AgentId}");
        }
    }
}
