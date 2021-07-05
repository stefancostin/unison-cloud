using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Exceptions;
using Unison.Cloud.Core.Interfaces.Workers;
using Unison.Cloud.Core.Services;
using Unison.Common.Amqp.DTO;

namespace Unison.Cloud.Core.Workers
{
    public class HeartbeatWorker : ISubscriptionWorker<AmqpHeartbeat>
    {
        private readonly ConnectionsManager _connectionsManager;
        private readonly ILogger<HeartbeatWorker> _logger;

        public HeartbeatWorker(ConnectionsManager connectionsManager, ILogger<HeartbeatWorker> logger)
        {
            _connectionsManager = connectionsManager;
            _logger = logger;
        }

        public void ProcessMessage(AmqpHeartbeat message)
        {
            ValidateMessage(message);
            _logger.LogDebug($"Received heartbeat from {message.Agent.InstanceId}.");
            _connectionsManager.ProcessInstance(message.Agent.InstanceId, message.CorrelationId);
        }

        private void ValidateMessage(AmqpHeartbeat message)
        {
            if (message == null || message.Agent == null)
                throw new InvalidRequestException("A heartbeat's origin must be provided");

            if (string.IsNullOrWhiteSpace(message.Agent.InstanceId))
                throw new InvalidRequestException("An agent instance id must be provided");
        }
    }
}
