using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Models;

namespace Unison.Cloud.Infrastructure.Data.Adapters
{
    public class DbCommandAdapter
    {
        private const string FieldSeparator = ", ";
        private const string ParameterIdentifier = "@";

        public DbCommandAdapter(DbConnection connection)
        {
            Connection = connection;
        }

        public DbConnection Connection { get; set; }

        public DbCommand ConvertToDbCommand(QuerySchema schema)
        {
            SanitizeIdentifiers(schema);

            var sql = GenerateSql(schema);
            var command = CreateDbCommand(schema, sql);

            return command;
        }

        private DbCommand CreateDbCommand(QuerySchema schema, string sql)
        {
            var command = new SqlCommand(sql, (SqlConnection)Connection);
            command.CommandType = CommandType.Text;

            if (schema.Params.Any())
                SanitizeAndAddParams(schema, command);

            return command;
        }

        private string CreateParameter(string field) => ParameterIdentifier + field;

        private string GenerateSql(QuerySchema schema)
        {
            string selectStatement = GenerateSelectStatement(schema);

            if (!schema.Params.Any())
                return selectStatement;

            string whereSatement = GenerateWhereStatement(schema);

            return $"{selectStatement} {whereSatement}";
        }

        private static string GenerateSelectStatement(QuerySchema schema)
        {
            var table = schema.Entity;
            var fields = string.Join(FieldSeparator, schema.Fields);

            return $"{Sql.Select} {fields} {Sql.From} {table}";
        }

        private string GenerateWhereStatement(QuerySchema schema)
        {
            var conditionsCount = schema.Params.Count();
            var conditionFields = schema.Params.Select(p => p.Field).ToList();
            var conditionParams = schema.Params.Select(p => CreateParameter(p.Field)).ToList();

            var conditions = new StringBuilder($"{Sql.Where} ");
            for (int i = 0; i < conditionsCount; i++)
            {
                conditions.Append($"{conditionFields[i]} = {conditionParams[i]}");

                if (i < conditionsCount - 1)
                    conditions.Append($" {Sql.And} ");
            }

            return $"{Sql.Where} {conditions}";
        }

        private void SanitizeAndAddParams(QuerySchema schema, SqlCommand command)
        {
            foreach (var param in schema.Params)
            {
                command.Parameters.AddWithValue(CreateParameter(param.Field), param.Value);
            }
        }

        private void SanitizeIdentifiers(QuerySchema schema)
        {
            var commandBuilder = new SqlCommandBuilder();
            schema.Entity = commandBuilder.QuoteIdentifier(schema.Entity);
            schema.Fields = schema.Fields.Select(field => commandBuilder.QuoteIdentifier(field));
            schema.Params = schema.Params.Select(param => new QueryParam()
            {
                Field = commandBuilder.QuoteIdentifier(param.Field),
                Value = param.Value,
            });
        }
    }

    internal static class Sql
    {
        public const string Select = "SELECT";
        public const string Distinct = "DISTINCT";
        public const string From = "FROM";
        public const string Where = "WHERE";
        public const string OrderBy = "ORDER BY";
        public const string GroupBy = "GROUP BY";
        public const string Having = "HAVING";
        public const string And = "AND";
        public const string Or = "OR";
    }
}
