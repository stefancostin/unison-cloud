using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data;

namespace Unison.Cloud.Infrastructure.Data.Adapters
{
    public class DbDataReaderAdapter
    {
        private readonly DbDataReader _reader;
        private IEnumerable<ColumnMetadata> _columns;

        public DbDataReaderAdapter(DbDataReader reader)
        {
            _reader = reader;
        }

        public Record Read()
        {
            Record record = new Record();
            IEnumerable<ColumnMetadata> columns = GetColumns();

            foreach (var column in columns)
            {
                record.AddField(column.ColumnName, column.DataType, _reader[column.ColumnName]);
            }

            return record;
        }

        private IEnumerable<ColumnMetadata> ExtractColumnMetadata()
        {
            IList<ColumnMetadata> columns = new List<ColumnMetadata>();

            DataTable schemaTable = _reader.GetSchemaTable();

            foreach (DataRow row in schemaTable.Rows)
            {
                var columnMetadata = new ColumnMetadata();

                foreach (DataColumn column in schemaTable.Columns)
                {
                    if (column.ColumnName == ColumnMetadata.ColumnNameDbKey)
                        columnMetadata.ColumnName = (string)row[column];

                    else if (column.ColumnName == ColumnMetadata.DataTypeDbKey)
                        columnMetadata.DataType = (Type)row[column];
                }

                columns.Add(columnMetadata);
            }

            return columns;
        }


        private IEnumerable<ColumnMetadata> GetColumns()
        {
            if (_columns == null)
                _columns = ExtractColumnMetadata();

            return _columns;
        }
    }

    internal class ColumnMetadata
    {
        public const string ColumnNameDbKey = "ColumnName";
        public const string DataTypeDbKey = "DataType";

        public string ColumnName { get; set; }
        public Type DataType { get; set; }
    }
}
