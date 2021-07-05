using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Interfaces.Configuration;

namespace Unison.Cloud.Core.Models
{
    public class TimerConfiguration : ITimerConfiguration
    {
        public int SyncTimer { get; set; } = (int)TimeSpan.FromHours(1).TotalSeconds;
        public int DisconnectTimer { get; set; } = (int)TimeSpan.FromMinutes(5).TotalSeconds;
        public int HeartbeatTimer { get; set; } = (int)TimeSpan.FromSeconds(10).TotalSeconds;
    }
}
