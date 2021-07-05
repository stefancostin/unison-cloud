using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data.Entities;
using Unison.Cloud.Core.Interfaces.Configuration;
using Unison.Cloud.Core.Interfaces.Data;
using Unison.Cloud.Core.Interfaces.Workers;
using Unison.Cloud.Core.Models;
using Unison.Cloud.Core.Services;
using Unison.Common.Amqp.DTO;
using Unison.Common.Amqp.Interfaces;

namespace Unison.Cloud.Core.Workers
{
    public class SyncRequestWorker : ITimedWorker
    {
        private readonly IAmqpConfiguration _amqpConfig;
        private readonly ConnectionsManager _connectionsManager;
        private readonly IAmqpPublisher _publisher;
        private readonly ServicesContext _servicesContext;
        private readonly ILogger<SyncRequestWorker> _logger;

        public SyncRequestWorker(
            IAmqpConfiguration amqpConfig,
            ConnectionsManager connectionsManager,
            IAmqpPublisher publisher,
            ServicesContext servicesContext,
            ILogger<SyncRequestWorker> logger)
        {
            _amqpConfig = amqpConfig;
            _connectionsManager = connectionsManager;
            _publisher = publisher;
            _servicesContext = servicesContext;
            _logger = logger;
        }

        public void Start(object state)
        {
            if (_connectionsManager.ConnectedInstances.IsEmpty)
            {
                _logger.LogWarning("There are no connected instances to synchronize with.");
                return;
            }

            List<SyncLog> syncLog = new List<SyncLog>();

            foreach (ConnectedInstance connectedInstance in _connectionsManager.ConnectedInstances.Values)
            {
                List<SyncEntity> entities = GetEntitiesMetadata(connectedInstance.NodeId);

                foreach (SyncEntity entity in entities)
                {
                    string correlationId = Guid.NewGuid().ToString();
                    _logger.LogInformation($"CorrelationId: {correlationId}. " +
                        $"Sending synchronization request to {connectedInstance.InstanceId} for table {entity.Entity}.");

                    var syncRequest = new AmqpSyncRequest 
                    { 
                        Entity = "Products", 
                        Fields = new List<string>() { "Id", "Name", "Price" }, 
                        PrimaryKey = "Id",
                        CorrelationId = correlationId
                    };

                    SyncLog initialSyncRequestLog = new SyncLog()
                    {
                        AgentId = connectedInstance.AgentId,
                        Completed = false,
                        CorrelationId = correlationId,
                        Entity = entity.Entity,
                    };
                    syncLog.Add(initialSyncRequestLog);

                    SendSyncRequest(syncRequest, connectedInstance.InstanceId);
                }
            }

            InitializeSyncLog(syncLog);
        }

        private List<SyncEntity> GetEntitiesMetadata(int nodeId)
        {
            using (var scope = _servicesContext.Services.CreateScope())
            {
                var entityRepository = scope.ServiceProvider.GetRequiredService<ISyncEntityRepository>();
                return entityRepository.FindByNodeId(nodeId).ToList();
            }
        }

        private void InitializeSyncLog(List<SyncLog> syncLog)
        {
            using (var scope = _servicesContext.Services.CreateScope())
            {
                var syncLogRepository = scope.ServiceProvider.GetRequiredService<ISyncLogRepository>();
                syncLogRepository.Add(syncLog);
                syncLogRepository.SaveChanges();
            }
        }

        private void SendSyncRequest(AmqpSyncRequest message, string instanceId)
        {
            var exchange = _amqpConfig.Exchanges.Commands;
            var command = _amqpConfig.Commands.Sync;

            var routingKey = $"{exchange}.{command}.{instanceId}";

            _publisher.PublishMessage(message, exchange, routingKey);
        }
    }
}
