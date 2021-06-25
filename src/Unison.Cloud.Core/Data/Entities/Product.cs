using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Cloud.Core.Data.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public int AgentRecordId { get; set; }

        public int AgentId { get; set; }

        public int NodeId { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }
    }
}
