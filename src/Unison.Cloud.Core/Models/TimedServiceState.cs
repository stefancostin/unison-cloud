using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Cloud.Core.Models
{
    public class TimedServiceState : IDisposable
    {
        public IServiceScope Scope { get; set; }

        public void Dispose()
        {
            Scope?.Dispose();
        }
    }
}
