using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Interfaces.Amqp;
using Unison.Cloud.Core.Interfaces.Configuration;
using Unison.Cloud.Core.Interfaces.Workers;
using Unison.Cloud.Infrastructure.Amqp.Client;
using Unison.Cloud.Infrastructure.Amqp.Interfaces;
using Unison.Cloud.Infrastructure.Amqp.Models;

namespace Unison.Cloud.Infrastructure.Amqp.Factories
{
    public class AmqpSubscriberFactory : IAmqpSubscriberFactory
    {
        private readonly IServiceProvider _services;
        private readonly IAmqpChannelFactory _channelFactory;
        private readonly ILoggerFactory _loggerFactory;

        public AmqpSubscriberFactory(IAmqpChannelFactory channelFactory, ILoggerFactory loggerFactory, IServiceProvider services)
        {
            _services = services;
            _channelFactory = channelFactory;
            _loggerFactory = loggerFactory;
        }

        public IEnumerable<IAmqpSubscriber> CreateSubscribers(Dictionary<string, string> consumerExchangeQueueMap)
        {
            using (var scope = _services.CreateScope())
            {
                //var connectionsSubscriber = CreateConnectionsSubscriber(scope, exchangeQueueMap);
                var syncResultSubscriber = CreateSyncResultSubscriber(scope, consumerExchangeQueueMap);

                var subscribers = new List<IAmqpSubscriber>();
                //subscribers.Add(connectionsSubscriber);
                subscribers.Add(syncResultSubscriber);

                return subscribers;
            }
        }

        private IAmqpSubscriber CreateConnectionsSubscriber(IServiceScope scope, Dictionary<string, string> consumerExchangeQueueMap)
        {
            var connectionsWorker = scope.ServiceProvider.GetRequiredService<ISubscriptionWorker>();
            var queue = consumerExchangeQueueMap[AmqpExchangeNames.Connections];
            return CreateSubscriber(queue, connectionsWorker);
        }

        private IAmqpSubscriber CreateSyncResultSubscriber(IServiceScope scope, Dictionary<string, string> consumerExchangeQueueMap)
        {
            var syncResultWorker = scope.ServiceProvider.GetRequiredService<ISubscriptionWorker>();
            var queue = consumerExchangeQueueMap[AmqpExchangeNames.Responses];
            return CreateSubscriber(queue, syncResultWorker);
        }


        private IAmqpSubscriber CreateSubscriber(string queue, ISubscriptionWorker worker)
        {
            return new AmqpSubscriber(queue, worker, _channelFactory.CreateManagedChannel(), _loggerFactory.CreateLogger(CreateLoggerName(queue)));
        }

        private string CreateLoggerName(string queue)
        {
            return $"{nameof(IAmqpSubscriber)}-{queue}";
        }
    }
}
