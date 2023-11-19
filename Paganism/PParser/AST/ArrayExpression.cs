using Paganism.Lexer.Enums;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class ArrayExpression : Expression, IEvaluable
    {
        public ArrayExpression(Expression[] elements, int length, StandartValueType elementsType = StandartValueType.Any)
        {
            Elements = elements;
            Length = length;        }

        public Expression[] Elements { get; }

        public StandartValueType ElementsType { get; }

        public bool IsArray => true;

        public int Length { get; }

        public Value Eval()
        {
            return new ArrayValue(Elements, ElementsType);
        }
    }
}
