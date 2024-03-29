﻿using Microsoft.Data.SqlClient;
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
        private const string Separator = ", ";
        private const string ParameterIdentifier = "@";
        private readonly char[] ParameterBrackets = { '[', ']' };

        public DbCommandAdapter(DbConnection connection)
        {
            Connection = connection;
        }

        public DbConnection Connection { get; set; }

        public DbCommand ConvertToDbCommand(QuerySchema schema)
        {
            var sanitizedSchema = SanitizeIdentifiers(schema);

            var sql = GenerateSql(sanitizedSchema);
            var command = CreateDbCommand(sanitizedSchema, sql);

            return command;
        }

        private DbCommand CreateDbCommand(QuerySchema schema, string sql)
        {
            var command = new SqlCommand(sql, (SqlConnection)Connection);
            command.CommandType = CommandType.Text;

            if (schema.Records.Any())
                SanitizeAndAddValues(schema, command);

            if (schema.Conditions.Any())
                SanitizeAndAddConditions(schema, command);

            return command;
        }

        private string CreateParameter(string field, int index = 0)
        {
            field = field.Trim(ParameterBrackets);

            if (index == 0)
                return ParameterIdentifier + field;

            return ParameterIdentifier + field + index;
        }

        private string GenerateSql(QuerySchema schema)
        {
            return schema.Operation switch
            {
                QueryOperation.Read => GenerateReadSql(schema),
                QueryOperation.Insert => GenerateInsertSql(schema),
                QueryOperation.Update => GenerateUpdateSql(schema),
                QueryOperation.Delete => GenerateDeleteSql(schema),
                _ => GenerateReadSql(schema),
            };
        }

        private string GenerateInsertSql(QuerySchema schema)
        {
            var table = schema.Entity;
            var fields = string.Join(Separator, schema.Fields);

            var sql = new StringBuilder($"{Sql.Insert} {table} ({fields}) {Sql.Values} ");

            var recordIdx = 1;
            foreach (QueryRecord record in schema.Records)
            {
                record.Fields = record.Fields.Select(f =>
                {
                    f.Param = CreateParameter(f.Name, recordIdx);
                    return f;
                })
                .ToList();

                var parameters = record.Fields.Select(f => f.Param);

                sql.Append("(")
                   .Append(string.Join(Separator, parameters))
                   .Append(")")
                   .Append(Separator);

                recordIdx++;
            }

            sql.Remove(sql.Length - Separator.Length, Separator.Length);

            return sql.ToString();
        }

        private string GenerateUpdateSql(QuerySchema schema)
        {
            var table = schema.Entity;
            var fields = string.Join(Separator, schema.Fields);

            var sql = new StringBuilder($"{Sql.Begin} ");

            var updateStatement = $"{Sql.Update} {table} {Sql.Set}";

            var recordIdx = 1;
            foreach (QueryRecord record in schema.Records)
            {
                record.Fields = record.Fields.Select(f =>
                {
                    f.Param = CreateParameter(f.Name, recordIdx);
                    return f;
                })
                .ToList();

                var values = record.Fields
                    .Where(f => f.Name != schema.PrimaryKey)
                    .Select(f => $"{f.Name} = {f.Param}");

                var updateValues = string.Join(Separator, values);

                var pkField = record.Fields.First(f => f.Name == schema.PrimaryKey);
                var updateCondition = $"{Sql.Where} {pkField.Name} = {pkField.Param}";

                updateCondition += ReduceSqlConditions(schema);

                sql.Append($"{updateStatement} {updateValues} {updateCondition}; ");

                recordIdx++;
            }

            sql.Append($" {Sql.End};");

            return sql.ToString();
        }

        private string GenerateDeleteSql(QuerySchema schema)
        {
            var table = schema.Entity;
            var fields = string.Join(Separator, schema.Fields);

            var sql = new StringBuilder($"{Sql.Begin} ");

            var deleteStatement = $"{Sql.Delete} {Sql.From} {table} {Sql.Where}";

            var recordIdx = 1;
            foreach (QueryRecord record in schema.Records)
            {
                var pkField = record.Fields.First(f => f.Name == schema.PrimaryKey);

                pkField.Param = CreateParameter(pkField.Name, recordIdx);

                var deleteCondition = $"{pkField.Name} = {pkField.Param}";

                deleteCondition += ReduceSqlConditions(schema);

                sql.Append($"{deleteStatement} {deleteCondition}; ");

                recordIdx++;
            }

            sql.Append($" {Sql.End};");

            return sql.ToString();
        }

        private string GenerateReadSql(QuerySchema schema)
        {
            string selectStatement = GenerateSelectStatement(schema);

            if (!schema.Conditions.Any())
                return selectStatement;

            string whereSatement = GenerateWhereStatement(schema);

            return $"{selectStatement} {whereSatement}";
        }

        private string GenerateSelectStatement(QuerySchema schema)
        {
            var table = schema.Entity;
            var fields = string.Join(Separator, schema.Fields);

            return $"{Sql.Select} {fields} {Sql.From} {table}";
        }

        private string GenerateWhereStatement(QuerySchema schema)
        {
            var conditionsCount = schema.Conditions.Count();
            var conditionFields = schema.Conditions.Select(p => p.Name).ToList();
            var conditionParams = schema.Conditions.Select(p => CreateParameter(p.Name)).ToList();

            if (conditionsCount == 0)
                return string.Empty;

            var whereStatement = new StringBuilder($"{Sql.Where} ");
            for (int i = 0; i < conditionsCount; i++)
            {
                whereStatement.Append($"{conditionFields[i]} = {conditionParams[i]}");

                if (i < conditionsCount - 1)
                    whereStatement.Append($" {Sql.And} ");
            }

            return whereStatement.ToString();
        }

        private string ReduceSqlConditions(QuerySchema schema)
        {
            schema.Conditions = schema.Conditions.Select(c =>
            {
                c.Param = CreateParameter(c.Name);
                return c;
            })
            .ToList();

            return schema.Conditions.Aggregate("", (acc, c) => acc += $" {Sql.And} {c.Name} = {c.Param} ");
        }

        private void SanitizeAndAddValues(QuerySchema schema, SqlCommand command)
        {
            foreach (var record in schema.Records)
            {
                foreach (var field in record.Fields)
                {
                    if (field.Param != null)
                        command.Parameters.AddWithValue(field.Param, field.Value);
                }
            }
        }

        private void SanitizeAndAddConditions(QuerySchema schema, SqlCommand command)
        {
            foreach (var param in schema.Conditions)
            {
                command.Parameters.AddWithValue(CreateParameter(param.Name), param.Value);
            }
        }

        private QuerySchema SanitizeIdentifiers(QuerySchema schema)
        {
            var commandBuilder = new SqlCommandBuilder();
            var sanitizedSchema = new QuerySchema();
            sanitizedSchema.Operation = schema.Operation;
            sanitizedSchema.Entity = commandBuilder.QuoteIdentifier(schema.Entity);
            sanitizedSchema.PrimaryKey = commandBuilder.QuoteIdentifier(schema.PrimaryKey);
            sanitizedSchema.Fields = schema.Fields.Select(field => commandBuilder.QuoteIdentifier(field)).ToList();
            sanitizedSchema.Records = schema.Records.Select(record =>
            {
                var fields = record.Fields.Select(field => new QueryParam()
                {
                    Name = commandBuilder.QuoteIdentifier(field.Name),
                    Type = field.Type,
                    Value = field.Value
                })
                .ToList();
                return new QueryRecord()
                {
                    Fields = fields
                };
            })
            .ToList();
            sanitizedSchema.Conditions = schema.Conditions.Select(param => new QueryParam()
            {
                Name = commandBuilder.QuoteIdentifier(param.Name),
                Value = param.Value,
            })
            .ToList();
            return sanitizedSchema;
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
        public const string Insert = "INSERT INTO";
        public const string Values = "VALUES";
        public const string Update = "UPDATE";
        public const string Set = "SET";
        public const string Delete = "DELETE";
        public const string Begin = "BEGIN";
        public const string End = "END";
        public const string And = "AND";
        public const string Or = "OR";
    }
}
