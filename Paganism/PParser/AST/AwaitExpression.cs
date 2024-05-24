using Paganism.Exceptions;
using Paganism.Interpreter.Data;
using Paganism.Interpreter.Data.Instances;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class AwaitExpression : Expression, IStatement, IExecutable
    {
        public AwaitExpression(ExpressionInfo info, Expression expression) : base(info)
        {
            Expression = expression;
        }

        public Expression Expression { get; }

        private FunctionDeclarateExpression _functionDeclarateExpression;

        public void Execute(params Argument[] arguments)
        {
            if (_functionDeclarateExpression is null && Expression is FunctionCallExpression functionCallExpression)
            {
                _functionDeclarateExpression = functionCallExpression.GetFunction().FunctionDeclarateExpression;
            }
            else if (_functionDeclarateExpression is null && Expression is BinaryOperatorExpression binaryOperatorExpression)
            {
                if (binaryOperatorExpression.Evaluate() is not StructureValue taskStructure ||
                    !taskStructure.Values.TryGetValue("id", out var value) ||
                    value is not NumberValue numberValue ||
                    !Tasks.TryGet((int)numberValue.Value, out var result))
                {
                    throw new InterpreterException("Must be an async function", ExpressionInfo);
                }

                result.Wait();
                return;
            }

            if (!_functionDeclarateExpression.IsAsync)
            {
                throw new InterpreterException("Must be an async function", ExpressionInfo);
            }

            var id = (int)((NumberValue)(_functionDeclarateExpression.Evaluate() as StructureValue).Values["id"]).Value;

            var task = Tasks.Get(id);

            task.Wait();
        }
    }
}
