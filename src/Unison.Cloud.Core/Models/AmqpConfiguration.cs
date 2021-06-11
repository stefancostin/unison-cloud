using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Interfaces.Configuration;

namespace Unison.Cloud.Core.Models
{
    public class AmqpConfiguration : IAmqpConfiguration
    {
        public AmqpCredentials Credentials { get; set; }
        public AmqpExchanges Exchanges { get; set; }
    }

    public class AmqpCredentials
    {
        public string Hostname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class AmqpExchanges
    {
        public string Commands { get; set; }
        public string Connections { get; set; }
        public string Heartbeat { get; set; }
        public string Response { get; set; }
    }
}
