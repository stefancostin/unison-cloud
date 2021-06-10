using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Services;

namespace Unison.Cloud.Infrastructure
{
    public static class StartupSetup
    {
        public static void AddCoreServices(this IServiceCollection services)
        {
            // We'll probably have to add these jobs from a database.
            services.AddTransient<ISync, Sync>();

            services.AddHostedService<TimedServiceManager>();
        }
    }
}
