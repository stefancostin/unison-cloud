using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data;
using Unison.Cloud.Core.Interfaces.Data;
using Unison.Cloud.Core.Models;
using Unison.Cloud.Infrastructure.Data.Adapters;

namespace Unison.Cloud.Infrastructure.Data.Repositories
{
    public class SQLRepository : ISQLRepository
    {
        private readonly IDbContext _context;
        private readonly ILogger<SQLRepository> _logger;

        public SQLRepository(IDbContext context, ILogger<SQLRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public DataSet Read(QuerySchema schema)
        {
            DataSet result = new DataSet(schema.Entity, schema.PrimaryKey);

            using var connection = _context.GetConnection();
            try
            {
                connection.Open();

                var commandAdapter = new DbCommandAdapter(connection);
                using var command = commandAdapter.ConvertToDbCommand(schema);

                using var reader = command.ExecuteReader();
                var readerAdapter = new DbDataReaderAdapter(reader);

                while (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result.AddRecord(readerAdapter.Read());
                    }

                    reader.NextResult();
                }

                reader.Close();

                return result;
            }
            finally
            {
                connection.Close();
            }
        }

        public int Execute(QuerySchema schema)
        {
            if (!schema.Records.Any())
                return 0;

            using var connection = _context.GetConnection();
            try
            {
                connection.Open();

                var commandAdapter = new DbCommandAdapter(connection);
                using var command = commandAdapter.ConvertToDbCommand(schema);

                var recordsAffected = command.ExecuteNonQuery();

                return recordsAffected;
            }
            finally
            {
                connection.Close();
            }
        }

        public Dictionary<int, int> ExecuteInTransaction(params QuerySchema[] schemas)
        {
            if (schemas.Length == 0)
                return new Dictionary<int, int>();

            IList<QuerySchema> schemaList = new List<QuerySchema>(schemas)
                .Where(s => s.Operation != QueryOperation.Read)
                .Where(s => s.Records.Any())
                .ToList();

            var affectedRowsMap = new Dictionary<int, int>();

            using var connection = _context.GetConnection();
            try
            {
                connection.Open();

                using var transaction = connection.BeginTransaction();
                var commandAdapter = new DbCommandAdapter(connection);

                try
                {
                    foreach (QuerySchema schema in schemaList)
                    {
                        var command = commandAdapter.ConvertToDbCommand(schema);
                        command.Connection = connection;
                        command.Transaction = transaction;

                        var affectedRows = command.ExecuteNonQuery();
                        affectedRowsMap.Add(schema.GetHashCode(), affectedRows);
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }

                return affectedRowsMap;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
