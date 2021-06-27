using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data;
using Unison.Cloud.Core.Models;
using Unison.Common.Amqp.DTO;

namespace Unison.Cloud.Core.Utilities
{
    public static class Mapper
    {
        #region Data.Fields
        public static AmqpField ToAmqpFieldModel(this Field field)
        {
            if (field == null)
                return null;

            return new AmqpField(name: field.Name, type: field.Type, value: field.Value);
        }

        public static Field ToFieldModel(this AmqpField amqpField)
        {
            if (amqpField == null)
                return null;

            return new Field(name: amqpField.Name, type: amqpField.Type, value: amqpField.Value);
        }

        public static Field ToFieldModel(this QueryParam schemaField)
        {
            if (schemaField == null)
                return null;

            return new Field(name: schemaField.Name, type: schemaField.Type, value: schemaField.Value);
        }

        public static QueryParam ToQueryParamModel(this Field field)
        {
            if (field == null)
                return null;

            return new QueryParam(name: field.Name, type: field.Type, value: field.Value);
        }

        public static QueryParam ToQueryParamModel(this AmqpField amqpField)
        {
            if (amqpField == null)
                return null;

            return new QueryParam(name: amqpField.Name, type: amqpField.Type, value: amqpField.Value);
        }
        #endregion

        #region Data.Records
        public static AmqpRecord ToAmqpRecordModel(this Record record)
        {
            if (record == null)
                return null;

            AmqpRecord amqpRecord = new AmqpRecord();

            if (record.Fields == null)
                return amqpRecord;

            amqpRecord.Fields = record.Fields.ToDictionary(f => f.Key, f => f.Value?.ToAmqpFieldModel());

            return amqpRecord;
        }

        public static Record ToRecordModel(this AmqpRecord amqpRecord)
        {
            if (amqpRecord == null)
                return null;

            Record record = new Record();

            if (amqpRecord.Fields == null)
                return record;

            record.Fields = amqpRecord.Fields.ToDictionary(f => f.Key, f => f.Value?.ToFieldModel());

            return record;
        }

        public static QueryRecord ToQueryRecordModel(this Record record)
        {
            if (record == null)
                return null;

            QueryRecord queryRecord = new QueryRecord();

            if (record.Fields == null)
                return queryRecord;

            queryRecord.Fields = record.Fields.Select(f => f.Value?.ToQueryParamModel()).ToList();

            return queryRecord;
        }

        public static QueryRecord ToQueryRecordModel(this AmqpRecord amqpRecord)
        {
            if (amqpRecord == null)
                return null;

            QueryRecord queryRecord = new QueryRecord();

            if (amqpRecord.Fields == null)
                return queryRecord;

            queryRecord.Fields = amqpRecord.Fields.Select(f => f.Value?.ToQueryParamModel()).ToList();

            return queryRecord;
        }
        #endregion

        #region Data.DataSets
        public static AmqpDataSet ToAmqpDataSetModel(this DataSet dataSet)
        {
            if (dataSet == null)
                return null;

            AmqpDataSet amqpDataSet = new AmqpDataSet(entity: dataSet.Entity, primaryKey: dataSet.PrimaryKey);

            if (dataSet.Records == null)
                return amqpDataSet;

            amqpDataSet.Records = dataSet.Records.ToDictionary(r => r.Key, r => r.Value?.ToAmqpRecordModel());

            return amqpDataSet;
        }

        public static DataSet ToDataSetModel(this AmqpDataSet amqpDataSet)
        {
            if (amqpDataSet == null)
                return null;

            DataSet dataSet = new DataSet(entity: amqpDataSet.Entity, primaryKey: amqpDataSet.PrimaryKey);

            if (amqpDataSet.Records == null)
                return dataSet;

            dataSet.Records = amqpDataSet.Records.ToDictionary(r => r.Key, r => r.Value?.ToRecordModel());

            return dataSet;
        }

        public static QuerySchema ToQuerySchema(this DataSet dataSet)
        {
            if (dataSet == null)
                return null;

            QuerySchema schema = new QuerySchema()
            {
                Entity = dataSet.Entity,
                PrimaryKey = dataSet.PrimaryKey,
            };

            if (dataSet.Records == null || !dataSet.Records.Any())
                return schema;

            schema.Fields = dataSet.Records.First().Value.Fields
                .Select(f => f.Value.Name)
                .ToList();

            schema.Records = dataSet.Records
                .Select(r => r.Value?.ToQueryRecordModel())
                .ToList();

            return schema;
        }

        public static QuerySchema ToQuerySchema(this AmqpDataSet amqpDataSet)
        {
            if (amqpDataSet == null)
                return null;

            QuerySchema schema = new QuerySchema() {
                Entity = amqpDataSet.Entity,
                // TODO: Move this to something else
                //PrimaryKey = Agent.RecordIdKey,
                PrimaryKey = amqpDataSet.PrimaryKey,
                // TODO: Move this to something else
                //Operation = operation
            };

            if (amqpDataSet.Records == null || !amqpDataSet.Records.Any())
                return schema;

            schema.Fields = amqpDataSet.Records.First().Value.Fields
                //.Select(f => f.Value.Name == amqpDataSet.PrimaryKey ? Agent.RecordIdKey : f.Value.Name)
                .Select(f => f.Value.Name)
                .ToList();
            schema.Records = amqpDataSet.Records
                .Select(r => r.Value?.ToQueryRecordModel())
                // TODO: Move this to something else
                //.Select(r =>
                //{
                //    r.Fields = r.Fields.Select(f => 
                //    {
                //        QueryParam field = new QueryParam(f);
                //        if (f.Name == amqpDataSet.PrimaryKey)
                //            field.Name = Agent.RecordIdKey;
                //        return field;
                //    })
                //    .ToList();
                //    return r;
                //})
                //.Select(r => 
                //{
                //    if (operation == QueryOperation.Insert)
                //        r.Fields.Add(new QueryParam(name: Agent.IdKey, type: typeof(Int32), value: agentId));
                //    return r;
                //})
                .ToList();

            // TODO: Move this to something else
            //if (operation == QueryOperation.Insert)
            //    schema.Fields.Add(Agent.IdKey);

            // TODO: Move this to something else
            //QueryParam agentCondition = new QueryParam(name: Agent.IdKey, type: typeof(Int32), value: agentId);
            //schema.Conditions = new List<QueryParam>() { agentCondition };

            return schema;
        }
        #endregion
    }
}
