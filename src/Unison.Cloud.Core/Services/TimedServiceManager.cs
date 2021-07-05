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
using Unison.Cloud.Core.Interfaces.Workers;
using Unison.Cloud.Core.Models;

namespace Unison.Cloud.Core.Services
{
    public class TimedServiceManager : BackgroundService
    {
        private readonly TimeSpan _initTimeout;
        private readonly IServiceProvider _services;
        private readonly ITimerConfiguration _timerConfig;
        private readonly ILogger<TimedServiceManager> _logger;
        private readonly ServiceTimers _timers;

        public TimedServiceManager(IServiceProvider services, ServiceTimers timers, ITimerConfiguration timerConfig, ILogger<TimedServiceManager> logger)
        {
            _initTimeout = TimeSpan.FromSeconds(5);
            _services = services;
            _timers = timers;
            _timerConfig = timerConfig;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timers.InitializationTimer = new Timer(
                callback: ExecuteServices, 
                state: null, 
                dueTime:_initTimeout, 
                period: Timeout.InfiniteTimeSpan);
            return Task.CompletedTask;
        }

        protected void ExecuteServices(object state)
        {
            _logger.LogInformation("Starting timeed services...");

            using (var scope = _services.CreateScope())
            {
                var syncRequestWorker = scope.ServiceProvider.GetRequiredService<ITimedWorker>();
                _timers.SyncTimer = new Timer(
                    callback: syncRequestWorker.Start, 
                    state: null, 
                    dueTime: TimeSpan.Zero,
                    period: TimeSpan.FromSeconds(_timerConfig.SyncTimer));
            }
        }

        ~TimedServiceManager()
        {
            _timers?.Dispose();
        }
    }
}
