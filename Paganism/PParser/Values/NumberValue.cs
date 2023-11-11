using Paganism.PParser.AST.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.Values
{
    public class NumberValue : Value
    {
        public NumberValue(double value)
        {
            Value = value;
        }

        public override string Name => "Number";

        public override StandartValueType Type => StandartValueType.Number;

        public double Value { get; set; }

        public override double AsNumber()
        {
            return Value;
        }
    }
}
