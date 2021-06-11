using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Interfaces.Amqp;
using Unison.Cloud.Infrastructure.Amqp;
using Unison.Cloud.Infrastructure.Amqp.Client;
using Unison.Cloud.Infrastructure.Amqp.Factories;
using Unison.Cloud.Infrastructure.Amqp.Interfaces;
using Unison.Cloud.Infrastructure.Data;
using Unison.Cloud.Infrastructure.Data.Repositories;

namespace Unison.Cloud.Infrastructure
{
    public static class StartupSetup
    {
        // public static void AddDbContext(this IServiceCollection services, string connectionString) =>
        //     services.AddDbContext<AppDbContext>(options =>
        //         options.UseSqlServer(connectionString));

        public static void AddDbContext(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>();
            services.AddScoped<IProductRepository, ProductRepository>();
        }

        public static void AddAmqpContext(this IServiceCollection services)
        {
            services.AddSingleton<IAmqpChannelFactory, AmqpChannelFactory>();

            services.AddScoped<IAmqpInfrastructureInitializer, AmqpInfrastructureInitializer>();
            services.AddScoped<IAmqpSubscriberFactory, AmqpSubscriberFactory>();
            services.AddScoped<IAmqpPublisher, AmqpPublisher>();
        }
    }
}
