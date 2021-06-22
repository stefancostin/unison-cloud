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
            Params = new List<QueryParam>();
        }

        public string Entity { get; set; }
        public string PrimaryKey { get; set; }
        public IEnumerable<string> Fields { get; set; }
        public IEnumerable<QueryParam> Params { get; set; }
    }

    public class QueryParam
    {
        public string Field { get; set; }
        public string Value { get; set; }
    }
}
