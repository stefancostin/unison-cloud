using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Cloud.Infrastructure.Data.Adapters
{
    public class DbDataReaderAdapter
    {
        private readonly DbDataReader _reader;
        private List<string> _columns;

        public DbDataReaderAdapter(DbDataReader reader)
        {
            _reader = reader;
        }

        public Dictionary<string, object> Read()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            List<string> columns = GetColumns();

            foreach (var column in columns)
            {
                result.Add(column, _reader[column]);
            }

            return result;
        }

        private List<string> ExtractColumns()
        {
            List<string> columns = new List<string>();

            DataTable schemaTable = _reader.GetSchemaTable();

            foreach (DataRow row in schemaTable.Rows)
            {
                foreach (DataColumn column in schemaTable.Columns)
                {
                    columns.Add((string)row[column]);
                    break;
                }
            }

            return columns;
        }


        private List<string> GetColumns()
        {
            if (_columns == null)
                _columns = ExtractColumns();

            return _columns;
        }
    }
}
