using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data.Entities;
using Unison.Cloud.Core.Exceptions;
using Unison.Cloud.Core.Interfaces.Data;
using Unison.Cloud.Core.Models;

namespace Unison.Cloud.Core.Services
{
    public class ConnectionsManager
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<ConnectionsManager> _logger;

        public ConnectionsManager(IServiceProvider services, ILogger<ConnectionsManager> logger)
        {
            _services = services;
            _logger = logger;

            ConnectedInstances = new ConcurrentDictionary<string, ConnectedInstance>();
        }

        public ConcurrentDictionary<string, ConnectedInstance> ConnectedInstances { get; set; }

        public ConnectedInstance ProcessInstance(string instanceId, string correlationId)
        {
            if (ConnectedInstances.ContainsKey(instanceId))
            {
                _logger.LogInformation($"CorrelationId: {correlationId}. Instance {instanceId} just reconnected.");
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
    }
}
