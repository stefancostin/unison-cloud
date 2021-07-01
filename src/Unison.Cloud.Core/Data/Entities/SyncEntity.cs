using Innofactor.EfCoreJsonValueConverter;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Cloud.Core.Data.Entities
{
    [Table("SyncEntities")]
    public class SyncEntity
    {
        public int Id { get; set; }

        public int NodeId { get; set; }

        public long Version { get; set; }

        public string Entity { get; set; }

        public string PrimaryKey { get; set; }

        [JsonField]
        public string Schema { get; set; }
    }
}
