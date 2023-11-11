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
    public class UnaryExpression : Expression, IEvaluable
    {
        public UnaryExpression(IEvaluable expression, BinaryOperatorType @operator)
        {
            Expression = expression;
            Operator = @operator;
        }

        public IEvaluable Expression { get; }

        public BinaryOperatorType Operator { get; }

        public Value Eval()
        {
            switch (Operator)
            {
                case BinaryOperatorType.Plus:
                    return Expression.Eval();
                case BinaryOperatorType.Minus:
                    return new NumberValue(-Expression.Eval().AsNumber());
            }

            return Expression.Eval();
        }
    }
}
