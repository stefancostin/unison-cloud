using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Unison.Cloud.Core.Models
{
    public class ServiceTimers : IDisposable
    {
        public Timer InitializationTimer { get; set; }
        public Timer SyncTimer { get; set; }

        public void Dispose()
        {
            InitializationTimer?.Dispose();
            SyncTimer?.Dispose();
        }
    }
}
