using Paganism.Exceptions;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class UnaryExpression : EvaluableExpression, IStatement
    {
        public UnaryExpression(ExpressionInfo info, EvaluableExpression expression, OperatorType operatorType) : base(info)
        {
            Expression = expression;
            Operator = operatorType;
        }

        public EvaluableExpression Expression { get; }

        public OperatorType Operator { get; }

        public override Value Evaluate(params Argument[] arguments)
        {
            return Operator switch
            {
                OperatorType.Plus => Expression.Evaluate(),
                OperatorType.Minus => new NumberValue(ExpressionInfo, -Expression.Evaluate().AsNumber()),
                OperatorType.IncrementPrefix => Increment(true),
                OperatorType.IncrementPostfix => Increment(),
                OperatorType.DicrementPrefix => Dicrement(true),
                OperatorType.DicrementPostfix => Dicrement(),
                _ => Expression.Evaluate(),
            };
        }

        private Value Increment(bool isPrefix = false)
        {
            if (Expression is not VariableExpression variableExpression)
            {
                throw new InterpreterException("Must be a variable", ExpressionInfo);
            }

            var value = variableExpression.Evaluate();

            if (value is not NumberValue)
            {
                throw new InterpreterException("The variable value must be of type Number", ExpressionInfo);
            }

            variableExpression.Set(ExpressionInfo, new NumberValue(ExpressionInfo, value.AsNumber() + 1));

            if (isPrefix)
            {
                return new NumberValue(ExpressionInfo, value.AsNumber());
            }
            else
            {
                return new NumberValue(ExpressionInfo, value.AsNumber() + 1);
            }
        }

        private Value Dicrement(bool isPrefix = false)
        {
            if (Expression is not VariableExpression variableExpression)
            {
                throw new InterpreterException("Must be a variable", ExpressionInfo);
            }

            var value = variableExpression.Evaluate();

            if (value is not NumberValue)
            {
                throw new InterpreterException("The variable value must be of type Number", ExpressionInfo);
            }

            variableExpression.Set(ExpressionInfo, new NumberValue(ExpressionInfo, value.AsNumber() - 1));

            if (isPrefix)
            {
                return new NumberValue(ExpressionInfo, value.AsNumber());
            }
            else
            {
                return new NumberValue(ExpressionInfo, value.AsNumber() - 1);
            }
        }
    }
}
