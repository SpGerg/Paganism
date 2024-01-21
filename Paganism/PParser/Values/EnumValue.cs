using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.Values
{
    public class EnumValue : Value
    {
        public EnumValue(EnumMemberExpression member)
        {
            Member = member;
        }

        public override string Name => "Enum";

        public override TypesType Type => TypesType.Enum;

        public EnumMemberExpression Member { get; }

        public override string AsString()
        {
            return $"{Member.Name}: {Member.Value.Value}";
        }

        public override double AsNumber()
        {
            return Member.Value.Value;
        }
    }
}
