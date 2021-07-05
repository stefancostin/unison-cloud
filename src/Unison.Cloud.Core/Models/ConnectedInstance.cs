using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Unison.Cloud.Core.Models
{
    public class ConnectedInstance : IDisposable
    {
        public int AgentId { get; set; }
        public string InstanceId { get; set; }
        public int NodeId { get; set; }
        public DateTime LastSeen { get; set; }
        public Timer RemoveInstanceTimer { get; set; }

        public void Dispose()
        {
            RemoveInstanceTimer?.Dispose();
        }

        ~ConnectedInstance()
        {
            Dispose();
        }
    }
}
