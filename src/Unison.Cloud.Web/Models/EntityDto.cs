using System;
using System.Collections.Generic;

namespace Unison.Cloud.Web.Models
{
    [Serializable]
    public class EntityDto
    {
        public int Id { get; set; }
        public NodeDto Node { get; set; }
        public string Entity { get; set; }
        public string PrimaryKey { get; set; }
        public IEnumerable<string> Fields { get; set; }
    }
}
