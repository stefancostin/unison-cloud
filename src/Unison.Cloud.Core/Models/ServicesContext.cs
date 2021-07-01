using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Cloud.Core.Models
{
    public class ServicesContext
    {
        public ServicesContext(IServiceProvider services)
        {
            Services = services;
        }

        public IServiceProvider Services { get; set; }
    }
}
