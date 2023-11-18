using Paganism.PParser.AST.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.Values
{
    public class BooleanValue : Value
    {
        public BooleanValue(bool value)
        {
            Value = value;
        }

        public override string Name => "Boolean";

        public override StandartValueType Type => StandartValueType.Boolean;

        public bool Value { get; set; }

        public override double AsNumber()
        {
            return Value ? 1 : 0;
        }

        public override string AsString()
        {
            return Value.ToString();
        }

        public override bool AsBoolean()
        {
            return Value;
        }
    }
}
