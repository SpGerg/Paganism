using Paganism.PParser.AST.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.Values
{
    public class StringValue : Value
    {
        public StringValue(string value)
        {
            Value = value;
        }

        public override string Name => "String";

        public override TypesType Type => TypesType.String;

        public string Value { get; set; }

        public override void Set(object value)
        {
            if (value is Value objectValue)
            {
                Value = objectValue.AsString();
                return;
            }

            Value = (string) value;
        }

        public override string AsString()
        {
            return Value;
        }
    }
}
