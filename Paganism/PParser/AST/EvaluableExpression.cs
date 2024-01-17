﻿using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public abstract class EvaluableExpression : Expression
    {
        protected EvaluableExpression(BlockStatementExpression parent, int position, int line, string filepath) : base(parent, position, line, filepath)
        {
        }

        public abstract Value Eval(params Argument[] arguments);
    }
}