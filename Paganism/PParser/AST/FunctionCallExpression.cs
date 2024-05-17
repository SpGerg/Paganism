using Paganism.Exceptions;
using Paganism.Interpreter.Data;
using Paganism.Interpreter.Data.Instances;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class FunctionCallExpression : EvaluableExpression, IStatement, IExecutable
    {
        public FunctionCallExpression(ExpressionInfo info, string functionName, Argument[] arguments) : base(info)
        {
            FunctionName = functionName;
            Arguments = arguments;
        }

        public string FunctionName { get; }

        public Argument[] Arguments { get; }

        public FunctionInstance GetFunction() => Functions.Instance.Get(ExpressionInfo.Parent, FunctionName, ExpressionInfo);

        public override Value Evaluate(params Argument[] arguments)
        {
            var function = GetFunction();

            try
            {
                var result = function.ExecuteAndReturn(Arguments);

                foreach (var argument in arguments)
                {
                    Variables.Instance.Remove(function.Statements, argument.Name);
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
