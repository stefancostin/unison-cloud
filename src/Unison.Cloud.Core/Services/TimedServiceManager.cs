using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unison.Cloud.Core.Interfaces.Workers;

namespace Unison.Cloud.Core.Services
{
    public class TimedServiceManager : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<TimedServiceManager> _logger;
        private Timer _timer;

        public TimedServiceManager(IServiceProvider services, ILogger<TimedServiceManager> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _services.CreateScope())
            {
                var syncRequestWorker = scope.ServiceProvider.GetRequiredService<ITimedWorker>();
                _timer = new Timer(syncRequestWorker.Execute, null, TimeSpan.Zero, TimeSpan.FromSeconds(15));
            }
            return Task.CompletedTask;
        }

        protected void ExecuteService(object state) { _logger.LogInformation("Task executed by scheduler: " + Thread.GetCurrentProcessorId() + ", " + Thread.CurrentThread.Name); }
    }
}
