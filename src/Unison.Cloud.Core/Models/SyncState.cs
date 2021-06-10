using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Unison.Cloud.Core.Models
{
    public class SyncState
    {
        public string SyncId { get; set; }
        // The local agent id
        public string AgentId { get; set; }
        // The client id / node id
        public string NodeId { get; set; }
        public Timer Timer { get; set; }
    }
}
