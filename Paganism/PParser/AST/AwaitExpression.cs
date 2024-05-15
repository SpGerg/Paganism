using Paganism.Exceptions;
using Paganism.Interpreter.Data;
using Paganism.Interpreter.Data.Instances;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class AwaitExpression : Expression, IStatement, IExecutable
    {
        public AwaitExpression(ExpressionInfo info, FunctionCallExpression expression) : base(info)
        {
            Expression = expression;
        }

        public FunctionCallExpression Expression { get; }

        private FunctionInstance _functionInstance;

        public void Execute(params Argument[] arguments)
        {
            if (_functionInstance is null)
            {
                _functionInstance = Functions.Instance.Value.Get(ExpressionInfo.Parent, Expression.FunctionName, ExpressionInfo);
            }

            if (!_functionInstance.IsAsync)
            {
                throw new InterpreterException("Must be async function", ExpressionInfo);
            }

            var id = (int)((NumberValue)(Expression.Evaluate() as StructureValue).Values["id"]).Value;

            var task = Tasks.Get(id);

            task.Wait();
        }
    }
}
