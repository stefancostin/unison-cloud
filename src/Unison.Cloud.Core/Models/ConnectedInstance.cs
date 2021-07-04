using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Cloud.Core.Models
{
    public class ConnectedInstance
    {
        public int AgentId { get; set; }
        public string InstanceId { get; set; }
        public int NodeId { get; set; }
        public DateTime LastSeen { get; set; }
    }
}
