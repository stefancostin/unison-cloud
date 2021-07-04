using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Interfaces.Configuration;
using Unison.Cloud.Core.Interfaces.Workers;
using Unison.Cloud.Core.Models;
using Unison.Cloud.Core.Services;
using Unison.Cloud.Core.Workers;
using Unison.Common.Amqp.DTO;

namespace Unison.Cloud.Infrastructure
{
    public static class StartupSetup
    {
        public static void AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IAmqpConfiguration>((serviceProvider) =>
                configuration.GetSection("Amqp").Get<AmqpConfiguration>());
        }

        public static void AddCoreServices(this IServiceCollection services)
        {
            services.AddSingleton<ServicesContext>();
            services.AddSingleton<ConnectionsManager>();

            services.AddScoped<ISubscriptionWorker<AmqpSyncResponse>, SyncResultWorker>();
            services.AddScoped<ISubscriptionWorker<AmqpConnected>, CacheWorker>();

            services.AddTransient<ITimedWorker, SyncRequestWorker>();

            services.AddHostedService<TimedServiceManager>();
            services.AddHostedService<AmqpServiceManager>();
        }
    }
}
