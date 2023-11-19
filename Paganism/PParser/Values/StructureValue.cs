using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.Values
{
    public class StructureValue : Value
    {
        public StructureValue(StructureDeclarateExpression structureDeclarate)
        {
            StructureDeclarate = structureDeclarate;
        }

        public override string Name => "Structure";

        public override StandartValueType Type => StandartValueType.Structure;

        public StructureDeclarateExpression StructureDeclarate { get; }

        public override string AsString()
        {
            var result = string.Empty;

            result += $"{Name}: {{";

            foreach (var item in StructureDeclarate.Members)
            {
                result += $"{item.Name}, ";
            }

            result += "}";

            return result;
        }
    }
}
