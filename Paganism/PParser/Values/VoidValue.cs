using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.Values
{
    public class VoidValue : Value
    {
        public VoidValue(ExpressionInfo info) : base(info)
        {
        }

        public override string Name => "Void";

        public override TypesType Type => TypesType.Void;
    }
}
