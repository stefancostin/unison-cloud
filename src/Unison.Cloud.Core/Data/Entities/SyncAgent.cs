using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Cloud.Core.Data.Entities
{
    public class SyncAgent
    {
        public int Id { get; set; }
        public string InstanceId { get; set; }
        public int NodeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public SyncNode Node { get; set; }
    }
}
