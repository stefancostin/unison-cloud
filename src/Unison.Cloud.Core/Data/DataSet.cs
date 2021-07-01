using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Utilities;

namespace Unison.Cloud.Core.Data
{
    public class DataSet
    {
        public DataSet(string entity, string primaryKey)
        {
            Entity = entity;
            PrimaryKey = primaryKey;
            Records = new Dictionary<string, Record>();
        }

        public DataSet(string entity, string primaryKey, long version) : this(entity, primaryKey)
        {
            Version = version;
        }

        public string Entity { get; }
        public string PrimaryKey { get; }
        public long Version { get; set; }
        public IDictionary<string, Record> Records { get; set; }

        public void AddRecord(Record record)
        {
            string primaryKey = record?.Fields?[PrimaryKey]?.Value?.ToString();
            
            if (primaryKey == null)
                return;

            if (Records.ContainsKey(primaryKey))
                return;

            Records.Add(primaryKey, record);
        }

        public Record GetRecord(string primaryKey)
        {
            return Records[primaryKey];
        }
    }
}
