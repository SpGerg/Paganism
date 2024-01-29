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
        public FunctionCallExpression(BlockStatementExpression parent, int line, int position, string filepath, string functionName, bool isAwait, Argument[] arguments) : base(parent, line, position, filepath)
        {
            FunctionName = functionName;
            IsAwait = isAwait;
            Arguments = arguments;
        }

        public string FunctionName { get; }

        public bool IsAwait { get; set; }

        public Argument[] Arguments { get; }

        public FunctionInstance GetFunction() => Functions.Instance.Value.Get(Parent, FunctionName);

        public override Value Eval(params Argument[] arguments)
        {
            var function = GetFunction();

            if (!function.IsAsync && IsAwait)
            {
                throw new InterpreterException("You cant use await for not async functions", Line, Position);
            }

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

        public void Execute(params Argument[] arguments)
        {
            Eval();
        }
    }
}
