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
    public class ArrayExpression : EvaluableExpression
    {
        public ArrayExpression(BlockStatementExpression parent, int line, int position, string filepath, Expression[] elements, int length) : base(parent, line, position, filepath)
        {
            Elements = elements;
            Length = length;
        }

        public Expression[] Elements { get; }

        public int Length { get; }

        public override Value Eval(params Argument[] arguments)
        {
            return new ArrayValue(Elements);
        }
    }
}
