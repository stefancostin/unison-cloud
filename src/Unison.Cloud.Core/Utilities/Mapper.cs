using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data;
using Unison.Common.Amqp.DTO;

namespace Unison.Cloud.Core.Utilities
{
    public static class Mapper
    {
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
    }
}
