using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Models;

namespace Unison.Cloud.Core.Interfaces.Configuration
{
    public interface ITimerConfiguration
    {
        int SyncTimer { get; set; }
        int DisconnectTimer { get; set; }
        int HeartbeatTimer { get; set; }
    }
}
