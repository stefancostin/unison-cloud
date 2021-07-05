using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Interfaces.Configuration;
using Unison.Cloud.Core.Interfaces.Workers;
using Unison.Common.Amqp.Constants;
using Unison.Common.Amqp.DTO;
using Unison.Common.Amqp.Interfaces;

namespace Unison.Cloud.Infrastructure.Amqp
{
    public class AmqpSubscriberInitializer : IAmqpSubscriberInitializer
    {
        private readonly IAmqpConfiguration _amqpConfig;
        private readonly IServiceProvider _services;
        private readonly IAmqpSubscriberFactory _subscriberFactory;

        public AmqpSubscriberInitializer(IServiceProvider services, IAmqpConfiguration amqpConfig, IAmqpSubscriberFactory subscriberFactory)
        {
            _amqpConfig = amqpConfig;
            _services = services;
            _subscriberFactory = subscriberFactory;
        }

        public IEnumerable<IAmqpSubscriber> Initialize()
        {
            using (var scope = _services.CreateScope())
            {
                var connectionsSubscriber = InitializeConnectionsSubscriber(scope);
                var heartbeatSubscriber = InitializeHeartbeatSubscriber(scope);
                var syncResultSubscriber = InitializeSyncResultsSubscriber(scope);

                var subscribers = new List<IAmqpSubscriber>();

                subscribers.Add(connectionsSubscriber);
                subscribers.Add(heartbeatSubscriber);
                subscribers.Add(syncResultSubscriber);

                return subscribers;
            }
        }

        private IAmqpSubscriber InitializeConnectionsSubscriber(IServiceScope scope)
        {
            var connectionsWorker = scope.ServiceProvider.GetRequiredService<ISubscriptionWorker<AmqpConnected>>();
            var queue = _amqpConfig.Queues.Connections;
            return _subscriberFactory.CreateSubscriber(queue, connectionsWorker);
        }

        private IAmqpSubscriber InitializeHeartbeatSubscriber(IServiceScope scope)
        {
            var heartbeatWorker = scope.ServiceProvider.GetRequiredService<ISubscriptionWorker<AmqpHeartbeat>>();
            var queue = _amqpConfig.Queues.Heartbeat;
            return _subscriberFactory.CreateSubscriber(queue, heartbeatWorker);
        }

        private IAmqpSubscriber InitializeSyncResultsSubscriber(IServiceScope scope)
        {
            var syncResultWorker = scope.ServiceProvider.GetRequiredService<ISubscriptionWorker<AmqpSyncResponse>>();
            var queue = _amqpConfig.Queues.Response;
            return _subscriberFactory.CreateSubscriber(queue, syncResultWorker);
        }
    }
}
