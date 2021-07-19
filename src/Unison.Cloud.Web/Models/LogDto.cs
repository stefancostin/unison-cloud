using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unison.Cloud.Web.Models
{
    public class LogDto
    {
        public int Id { get; set; }
        public string CorrelationId { get; set; }
        public AgentDto Agent { get; set; }
        public NodeDto Node { get; set; }
        public string Entity { get; set; }
        public int AddedRecords { get; set; }
        public int UpdatedRecords { get; set; }
        public int DeletedRecords { get; set; }
        public bool Completed { get; set; }
        public DateTime Date { get; set; }
    }
}
