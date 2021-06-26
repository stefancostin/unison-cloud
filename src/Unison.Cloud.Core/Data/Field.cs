using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Cloud.Core.Data
{
    public class Field : IEquatable<Field>
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public object Value { get; set; }

        public Field(string name, Type type, object value)
        {
            Name = name;
            Type = type;
            Value = value;
        }

        public bool Equals(Field other)
        {
            if (this.Name != other.Name)
                return false;

            if (this.Type != other.Type)
                return false;

            return Convert.ChangeType(Value, Type).Equals(Convert.ChangeType(other.Value, other.Type));
        }
    }
}
 