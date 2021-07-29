using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Cloud.Core.Data.Entities
{
    [Table("SyncLog")]
    public class SyncLog
    {
        public int Id { get; set; }
        public string CorrelationId { get; set; }
        public int AgentId { get; set; }
        public SyncAgent Agent { get; set; }
        public string Entity { get; set; }
        public int AddedRecords { get; set; }
        public int UpdatedRecords { get; set; }
        public int DeletedRecords { get; set; }
        public bool Completed { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
