﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Interfaces.Data;
using Unison.Cloud.Infrastructure.Amqp;
using Unison.Cloud.Infrastructure.Data;
using Unison.Cloud.Infrastructure.Data.Repositories;
using Unison.Common.Amqp;
using Unison.Common.Amqp.Interfaces;

namespace Unison.Cloud.Infrastructure
{
    public static class StartupSetup
    {
        public static void AddDbContext(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>();

            services.AddSingleton<IDbContext, RawDbContext>();

            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ISyncAgentRepository, SyncAgentRepository>();
            services.AddScoped<ISyncEntityRepository, SyncEntityRepository>();
            services.AddScoped<ISyncLogRepository, SyncLogRepository>();
            services.AddScoped<ISyncNodeRepository, SyncNodeRepository>();
            services.AddScoped<ISyncVersionRepository, SyncVersionRepository>();
            services.AddScoped<ISQLRepository, SQLRepository>();
        }

        public static void AddAmqpContext(this IServiceCollection services)
        {
            services.AddAmqpInfrastructure();

            services.AddScoped<IAmqpInfrastructureInitializer, AmqpInfrastructureInitializer>();
            services.AddScoped<IAmqpSubscriberInitializer, AmqpSubscriberInitializer>();
        }
    }
}
