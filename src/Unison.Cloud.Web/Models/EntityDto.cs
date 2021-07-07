using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unison.Cloud.Web.Models
{
    public class EntityDto
    {
        public int Id { get; set; }
        public int NodeId { get; set; }
        public string Entity { get; set; }
        public string PrimaryKey { get; set; }
        public IEnumerable<string> Fields { get; set; }
    }
}
