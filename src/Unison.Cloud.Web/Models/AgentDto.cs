using System;

namespace Unison.Cloud.Web.Models
{
    [Serializable]
    public class AgentDto
    {
        public int Id { get; set; }
        public string InstanceId { get; set; }
        public NodeDto Node { get; set; }
    }
}
