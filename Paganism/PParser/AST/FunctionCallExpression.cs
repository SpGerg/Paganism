using Paganism.Exceptions;
using Paganism.Interpreter.Data;
using Paganism.Interpreter.Data.Instances;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class FunctionCallExpression : EvaluableExpression, IStatement, IExecutable
    {
        public FunctionCallExpression(ExpressionInfo info, string functionName, bool isAwait, Argument[] arguments) : base(info)
        {
            FunctionName = functionName;
            IsAwait = isAwait;
            Arguments = arguments;
        }

        public string FunctionName { get; }

        public bool IsAwait { get; set; }

        public Argument[] Arguments { get; }

        public FunctionInstance GetFunction() => Functions.Instance.Value.Get(ExpressionInfo.Parent, FunctionName, ExpressionInfo);

        public override Value Evaluate(params Argument[] arguments)
        {
            var function = GetFunction();

            if (!function.IsAsync && IsAwait)
            {
                throw new InterpreterException("You cant use await for not async functions", ExpressionInfo);
            }

            try
            {
                if (IsAwait && function.IsAsync)
                {
                    return function.ExecuteAndReturn(Arguments);
                }

                var result = function.ExecuteAndReturn(Arguments);

                foreach (var argument in arguments)
                {
                    Variables.Instance.Value.Remove(function.Statements, argument.Name);
                }

                return result;
            }
            catch (PaganismException exception)
            {
                throw new InterpreterException(exception.OriginalMessage, ExpressionInfo);
            }
        }

        public void Execute(params Argument[] arguments)
        {
            Evaluate();
        }
    }
}
