using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Interfaces.Workers;
using Unison.Common.Amqp.Constants;
using Unison.Common.Amqp.Interfaces;

namespace Unison.Cloud.Infrastructure.Amqp
{
    public class AmqpSubscriberInitializer : IAmqpSubscriberInitializer
    {
        private readonly IServiceProvider _services;
        private readonly IAmqpSubscriberFactory _subscriberFactory;
        private readonly Dictionary<string, string> _consumerExchangeQueueMap;

        public AmqpSubscriberInitializer(IServiceProvider services, IAmqpSubscriberFactory subscriberFactory, IAmqpInitializationState initializationState)
        {
            _services = services;
            _subscriberFactory = subscriberFactory;
            _consumerExchangeQueueMap = initializationState.ConsumerExchangeQueueMap;
        }

        public IEnumerable<IAmqpSubscriber> Initialize()
        {
            using (var scope = _services.CreateScope())
            {
                //var connectionsSubscriber = CreateConnectionsSubscriber(scope, exchangeQueueMap);
                var syncResultSubscriber = InitializeSyncResultsSubscriber(scope);

                var subscribers = new List<IAmqpSubscriber>();
                //subscribers.Add(connectionsSubscriber);
                subscribers.Add(syncResultSubscriber);

                return subscribers;
            }
        }


        private IAmqpSubscriber InitializeConnectionsSubscriber(IServiceScope scope)
        {
            var connectionsWorker = scope.ServiceProvider.GetRequiredService<ISubscriptionWorker>();
            var queue = _consumerExchangeQueueMap[AmqpExchangeNames.Connections];
            return _subscriberFactory.CreateSubscriber(queue, connectionsWorker);
        }

        private IAmqpSubscriber InitializeSyncResultsSubscriber(IServiceScope scope)
        {
            var syncResultWorker = scope.ServiceProvider.GetRequiredService<ISubscriptionWorker>();
            var queue = _consumerExchangeQueueMap[AmqpExchangeNames.Responses];
            return _subscriberFactory.CreateSubscriber(queue, syncResultWorker);
        }
    }
}
