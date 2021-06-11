using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unison.Cloud.Core.Interfaces.Amqp;
using Unison.Cloud.Core.Interfaces.Configuration;

namespace Unison.Cloud.Core.Services
{
    public class SubscriberManager : IHostedService
    {
        private readonly IAmqpConfiguration _amqpConfig;
        private readonly IServiceProvider _services;
        private readonly ILogger<SubscriberManager> _logger;
        private IEnumerable<IAmqpSubscriber> _subscribers;

        public SubscriberManager(IServiceProvider services, IAmqpConfiguration amqpConfig, ILogger<SubscriberManager> logger)
        {
            _amqpConfig = amqpConfig;
            _logger = logger;
            _services = services;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            InitializeAmqpInfrastructure();

            foreach (var subscriber in _subscribers)
            {
               subscriber.Subscribe();
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach(var subscriber in _subscribers)
            {
                subscriber.Unsubscribe();
            }

            return Task.CompletedTask;
        }

        private void InitializeAmqpInfrastructure()
        {
            using (var scope = _services.CreateScope())
            {
                var amqpInfrastructureInitializer = scope.ServiceProvider.GetRequiredService<IAmqpInfrastructureInitializer>();
                var exchangeQueueMap = amqpInfrastructureInitializer.Initialize();

                var subscriberFactory = scope.ServiceProvider.GetRequiredService<IAmqpSubscriberFactory>();
                _subscribers = subscriberFactory.CreateSubscribers(exchangeQueueMap);
            }
        }
    }
}
