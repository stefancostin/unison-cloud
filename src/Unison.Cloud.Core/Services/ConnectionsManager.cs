using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data.Entities;
using Unison.Cloud.Core.Exceptions;
using Unison.Cloud.Core.Interfaces.Configuration;
using Unison.Cloud.Core.Interfaces.Data;
using Unison.Cloud.Core.Models;

namespace Unison.Cloud.Core.Services
{
    public class ConnectionsManager
    {
        private readonly IServiceProvider _services;
        private readonly TimeSpan _disconnectTimer;
        private readonly ILogger<ConnectionsManager> _logger;

        public ConnectionsManager(IServiceProvider services, ITimerConfiguration timersConfig, ILogger<ConnectionsManager> logger)
        {
            _services = services;
            _logger = logger;

            _disconnectTimer = TimeSpan.FromSeconds(timersConfig.DisconnectTimer);
            ConnectedInstances = new ConcurrentDictionary<string, ConnectedInstance>();
        }

        public ConcurrentDictionary<string, ConnectedInstance> ConnectedInstances { get; set; }

        public ConnectedInstance ProcessInstance(string instanceId, string correlationId)
        {
            if (ConnectedInstances.ContainsKey(instanceId))
            {
                SetDisconnectTimer(ConnectedInstances[instanceId]);
                return ConnectedInstances[instanceId];
            }

            SyncAgent agent = GetAgentInformation(instanceId);

            if (agent == null)
            {
                _logger.LogError($"CorrelationId: {correlationId}. Suspicious request: Unknown agent instance id received.");
                throw new AgentNotFoundException("An agent with the provided instance id does not exist");
            }

            ConnectedInstance connectedInstance = new ConnectedInstance()
            {
                AgentId = agent.Id,
                InstanceId = agent.InstanceId,
                NodeId = agent.NodeId,
                LastSeen = DateTime.Now
            };
            ConnectedInstances.TryAdd(instanceId, connectedInstance);

            _logger.LogInformation($"CorrelationId: {correlationId}. Instance {instanceId} added to the list of known connections.");

            SetDisconnectTimer(connectedInstance);

            return connectedInstance;
        }

        private SyncAgent GetAgentInformation(string instanceId)
        {
            using (var scope = _services.CreateScope())
            {
                var agentRepository = scope.ServiceProvider.GetRequiredService<ISyncAgentRepository>();
                return agentRepository.FindByInstanceId(instanceId);
            }
        }

        private void RemoveDisconnectedInstance(object state)
        {
            string instanceId = state.ToString();
            ConnectedInstance removedInstance = null;

            ConnectedInstances.Remove(instanceId, out removedInstance);
            _logger.LogInformation($"Instance {instanceId} last seen at {removedInstance?.LastSeen} has disconnected.");
        }

        private void SetDisconnectTimer(ConnectedInstance connectedInstance)
        {
            connectedInstance.LastSeen = DateTime.Now;

            if (connectedInstance.RemoveInstanceTimer is null)
            {
                connectedInstance.RemoveInstanceTimer = new Timer(
                    callback: RemoveDisconnectedInstance,
                    state: connectedInstance.InstanceId,
                    dueTime: _disconnectTimer,
                    period: Timeout.InfiniteTimeSpan);
            }
            else
            {
                connectedInstance.RemoveInstanceTimer.Change(
                    dueTime: _disconnectTimer,
                    period: Timeout.InfiniteTimeSpan);
            }
        }
    }
}
