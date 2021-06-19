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
using Unison.Cloud.Core.Interfaces.Configuration;
using Unison.Common.Amqp.DTO;
using Unison.Common.Amqp.Interfaces;

namespace Unison.Cloud.Core.Services
{
    public class AmqpServiceManager : IHostedService
    {
        private readonly IAmqpConfiguration _amqpConfig;
        private readonly IServiceProvider _services;
        private readonly ILogger<AmqpServiceManager> _logger;
        private IEnumerable<IAmqpSubscriber> _subscribers;

        public AmqpServiceManager(IServiceProvider services, IAmqpConfiguration amqpConfig, ILogger<AmqpServiceManager> logger)
        {
            _amqpConfig = amqpConfig;
            _logger = logger;
            _services = services;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            InitializeAmqpInfrastructure();

            Parallel.ForEach(_subscribers, subscriber => subscriber.Subscribe());

            SendInitialReconnectCommand();

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
                amqpInfrastructureInitializer.Initialize();

                var subscriberInitializer = scope.ServiceProvider.GetRequiredService<IAmqpSubscriberInitializer>();
                _subscribers = subscriberInitializer.Initialize();
            }
        }

        private void SendInitialReconnectCommand()
        {
            using (var scope = _services.CreateScope())
            {
                var amqpPublisher = scope.ServiceProvider.GetRequiredService<IAmqpPublisher>();

                var exchange = _amqpConfig.Exchanges.Commands;
                var routingKey = $"{_amqpConfig.Exchanges.Commands}.{_amqpConfig.Commands.Reconnect}";

                amqpPublisher.PublishMessage(new AmqpReconnect(), exchange, routingKey);
            }
        }
    }
}
