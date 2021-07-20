using System;

namespace Unison.Cloud.Web.Models
{
    [Serializable]
    public class ConnectionDto
    {
        public string InstanceId { get; set; }
        public NodeDto Node { get; set; }
        public DateTime LastSeen { get; set; }
    }
}
