using Paganism.Exceptions;
using Paganism.Interpreter.Data;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class AwaitExpression : Expression, IStatement, IExecutable
    {
        public AwaitExpression(ExpressionInfo info, Expression expression) : base(info)
        {
            Expression = expression;
        }

        public Expression Expression { get; }

        public void Execute(params Argument[] arguments)
        {
            var function = Expression as FunctionCallExpression;

            if (!function.IsAwait)
            {
                throw new InterpreterException("You need async function to use await.", ExpressionInfo);
            }

            var id = (int)((NumberValue)(function.Evaluate() as StructureValue).Values["id"]).Value;

            var task = Tasks.Get(id);

            task.Wait();
        }
    }
}
