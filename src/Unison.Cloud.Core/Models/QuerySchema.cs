using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Cloud.Core.Models
{
    public class QuerySchema
    {
        public QuerySchema()
        {
            Fields = new List<string>();
            Conditions = new List<QueryParam>();
            Records = new List<QueryRecord>();
            Operation = QueryOperation.Read;
        }

        public string Entity { get; set; }
        public string PrimaryKey { get; set; }
        public IEnumerable<string> Fields { get; set; }
        public IEnumerable<QueryParam> Conditions { get; set; }
        public IEnumerable<QueryRecord> Records { get; set; }
        public QueryOperation Operation { get; set; }
    }

    public class QueryParam
    {
        public QueryParam() { }

        public QueryParam(string name, Type type, object value)
        {
            Name = name;
            Type = type;
            Value = value;
        }

        public string Name { get; set; }
        public string Param { get; set; }
        public Type Type { get; set; }
        public object Value { get; set; }
    }

    public class QueryRecord
    {
        public QueryRecord()
        {
            Fields = new List<QueryParam>();
        }

        public IEnumerable<QueryParam> Fields { get; set; }
    }

    public enum QueryOperation
    {
        Read,
        Insert,
        Update,
        Delete
    }
}
