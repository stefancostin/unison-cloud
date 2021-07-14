using System;

namespace Unison.Cloud.Web.Models
{
    [Serializable]
    public class NodeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
