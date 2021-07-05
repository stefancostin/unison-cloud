using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Cloud.Core.Utilities
{
    public static class Logging
    {
        private const string TimeStampFormat = "[HH:mm:ss] ";

        public static void AddCustomLogging(this IServiceCollection services)
        {
            services.AddLogging(options =>
            {
                options.AddSimpleConsole(c =>
                {
                    c.TimestampFormat = TimeStampFormat;
                });
            });
        }
    }
}
