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

        public override StandartValueType Type => StandartValueType.String;

        public string Value { get; }

        public override string AsString()
        {
            return Value;
        }
    }
}
