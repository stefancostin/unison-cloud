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
        public AmqpCommands Commands { get; set; }
        public AmqpQueues Queues { get; set; }
    }

    public class AmqpCredentials
    {
        public string Hostname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class AmqpCommands
    {
        public string ApplyVersion { get; set; }
        public string Cache { get; set; }
        public string Configure { get; set; }
        public string Reconnect { get; set; }
        public string Sync { get; set; }
    }

    public class AmqpExchanges
    {
        public string Commands { get; set; }
        public string Connections { get; set; }
        public string Errors { get; set; }
        public string Heartbeat { get; set; }
        public string Response { get; set; }
    }

    public class AmqpQueues
    {
        public string CommandApplyVersion { get; set; }
        public string CommandCache { get; set; }
        public string CommandConfigure { get; set; }
        public string CommandReconnect { get; set; }
        public string CommandSync { get; set; }
        public string Connections { get; set; }
        public string Errors { get; set; }
        public string Heartbeat { get; set; }
        public string Response { get; set; }
    }
}
