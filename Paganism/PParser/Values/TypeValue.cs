using Paganism.PParser.AST.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.Values
{
    public class TypeValue : Value
    {
        public TypeValue(TypesType value)
        {
            Value = value;
        }

        public override string Name => "Type";

        public override TypesType Type => TypesType.Type;

        public TypesType Value { get; }

        public override string AsString()
        {
            return Value.ToString();
        }
    }
}
