using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Cloud.Core.Data.Entities
{
    [Table("SyncVersions")]
    public class SyncVersion
    {
        public int Id { get; set; }
        public int EntityId { get; set; }
        public SyncEntity Entity { get; set; }
        public int AgentId { get; set; }
        public SyncAgent Agent { get; set; }
        public long Version { get; set; }
    }
}
