﻿using Innofactor.EfCoreJsonValueConverter;
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
        public SyncNode Node { get; set; }
        public string Entity { get; set; }
        public string PrimaryKey { get; set; }

        [JsonField]
        public IEnumerable<string> Fields { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public IEnumerable<SyncVersion> Versions { get; set; }
    }
}
