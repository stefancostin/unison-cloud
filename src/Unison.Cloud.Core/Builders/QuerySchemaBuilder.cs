using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data;
using Unison.Cloud.Core.Models;
using Unison.Cloud.Core.Utilities;
using Unison.Common.Amqp.DTO;

namespace Unison.Cloud.Core.Builders
{
    public class QuerySchemaBuilder
    {
        public QuerySchemaBuilder() 
        {
            Schema = new QuerySchema();
        }

        public QuerySchema Schema { get; set; }

        public QuerySchema Build()
        {
            QuerySchema finalizedSchema = Schema;
            Schema = new QuerySchema();
            return finalizedSchema;
        }

        public QuerySchemaBuilder From(string entity)
        {
            Schema = new QuerySchema() { Entity = entity };
            return this;
        }

        public QuerySchemaBuilder From(DataSet dataSet)
        {
            Schema = dataSet.ToQuerySchema();
            return this;
        }

        public QuerySchemaBuilder From(AmqpDataSet amqpDataSet)
        {
            Schema = amqpDataSet.ToQuerySchema();
            return this;
        }

        public QuerySchemaBuilder ToReadSchema()
        {
            Schema.Operation = QueryOperation.Read;
            return this;
        }

        public QuerySchemaBuilder ToInsertSchema()
        {
            Schema.Operation = QueryOperation.Insert;
            MapAgentPkToCloudPk();
            return this;
        }

        public QuerySchemaBuilder ToUpdateSchema()
        {
            Schema.Operation = QueryOperation.Update;
            MapAgentPkToCloudPk();
            return this;
        }

        public QuerySchemaBuilder ToDeleteSchema()
        {
            Schema.Operation = QueryOperation.Delete;
            MapAgentPkToCloudPk();
            return this;
        }

        public QuerySchemaBuilder AddSelectFields(params string[] fields)
        {
            foreach (string field in fields)
            {
                Schema.Fields.Add(field);
            }
            return this;
        }

        public QuerySchemaBuilder AddRecord(params QueryParam[] fields)
        {
            QueryRecord record = new QueryRecord();
            foreach (QueryParam field in fields)
            {
                record.Fields.Add(field);
            }
            Schema.Records.Add(record);
            return this;
        }

        public QuerySchemaBuilder AddWhereCondition(string fieldName, object fieldValue)
        {
            QueryParam condition = new QueryParam { Name = fieldName, Value = fieldValue };
            Schema.Conditions.Add(condition);
            return this;
        }

        public QuerySchemaBuilder MapFieldToRecords(string fieldName, object fieldValue)
        {
            Schema.Fields.Add(fieldName);
            Schema.Records = Schema.Records
                .Select(r =>
                {
                    r.Fields.Add(new QueryParam { Name = fieldName, Value = fieldValue });
                    return r;
                })
                .ToList();
            return this;
        }

        public QuerySchemaBuilder MapAgentPkToCloudPk()
        {
            Schema.Fields = Schema.Fields
                .Select(f => f == Schema.PrimaryKey ? Agent.RecordIdKey : f)
                .ToList();

            Schema.Records = Schema.Records
                .Select(r =>
                {
                    r.Fields = r.Fields.Select(f =>
                    {
                        QueryParam field = new QueryParam(f);
                        if (f.Name == Schema.PrimaryKey)
                            field.Name = Agent.RecordIdKey;
                        return field;
                    })
                    .ToList();
                    return r;
                })
                .ToList();

            Schema.PrimaryKey = Agent.RecordIdKey;

            return this;
        }

        public QuerySchemaBuilder SetPrimaryKey(string primaryKey)
        {
            Schema.PrimaryKey = primaryKey;
            return this;
        }
    }
}
