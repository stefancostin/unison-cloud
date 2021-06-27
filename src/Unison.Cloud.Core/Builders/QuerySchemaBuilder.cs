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
    public static class QuerySchemaBuilder
    {
        public static QuerySchema From(DataSet dataSet) => dataSet.ToQuerySchema();

        public static QuerySchema From(AmqpDataSet amqpDataSet) => amqpDataSet.ToQuerySchema();

        public static QuerySchema ToInsertSchema(this QuerySchema schema, int agentId)
        {
            schema.Operation = QueryOperation.Insert;
            schema.MapAgentPkToCloudPk()
                  .AddAgentIdField(agentId);
            return schema;
        }

        public static QuerySchema ToUpdateSchema(this QuerySchema schema, int agentId)
        {
            schema.Operation = QueryOperation.Update;
            schema.MapAgentPkToCloudPk()
                  .AddAgentWhereCondition(agentId);
            return schema;
        }

        public static QuerySchema ToDeleteSchema(this QuerySchema schema, int agentId)
        {
            schema.Operation = QueryOperation.Delete;
            schema.MapAgentPkToCloudPk()
                  .AddAgentWhereCondition(agentId);
            return schema;
        }

        public static QuerySchema AddAgentIdField(this QuerySchema schema, int agentId)
        {
            schema.Fields.Add(Agent.IdKey);
            schema.Records = schema.Records
                .Select(r =>
                {
                    r.Fields.Add(new QueryParam(name: Agent.IdKey, type: typeof(Int32), value: agentId));
                    return r;
                })
                .ToList();
            return schema;
        }

        public static QuerySchema AddAgentWhereCondition(this QuerySchema schema, int agentId)
        {
            QueryParam agentCondition = new QueryParam(name: Agent.IdKey, type: typeof(Int32), value: agentId);
            schema.Conditions = new List<QueryParam>() { agentCondition };
            return schema;
        }

        public static QuerySchema MapAgentPkToCloudPk(this QuerySchema schema)
        {
            schema.Fields = schema.Fields
                .Select(f => f == schema.PrimaryKey ? Agent.RecordIdKey : f)
                .ToList();

            schema.Records = schema.Records
                .Select(r =>
                {
                    r.Fields = r.Fields.Select(f =>
                    {
                        QueryParam field = new QueryParam(f);
                        if (f.Name == schema.PrimaryKey)
                            field.Name = Agent.RecordIdKey;
                        return field;
                    })
                    .ToList();
                    return r;
                })
                .ToList();

            schema.PrimaryKey = Agent.RecordIdKey;

            return schema;
        }
    }
}
