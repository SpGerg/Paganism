using Paganism.PParser.AST.Enums;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class TypeExpression : Expression, IEvaluable
    {
        public TypeExpression(TypesType type)
        {
            Value = type;
            _value = new TypeValue(type);
        }

        public TypesType Value { get; }

        private readonly TypeValue _value;

        public Value Eval() => _value;
    }
}
