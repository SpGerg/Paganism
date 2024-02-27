using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS0659
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

        public override TypesType[] CanCastTypes { get; } = new[]
        {
            TypesType.Number,
            TypesType.String
        };

        public EnumMemberExpression Member { get; }

        public override string AsString()
        {
            return $"{Member.Name}: {Member.Value.Value}";
        }

        public override double AsNumber()
        {
            return Member.Value.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is not EnumValue enumValue)
            {
                return false;
            }

            if (Member.Enum != enumValue.Member.Enum)
            {
                return false;
            }

            return true;
        }
    }
}
