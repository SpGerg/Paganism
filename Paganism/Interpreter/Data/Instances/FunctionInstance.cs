using Paganism.PParser;
using Paganism.PParser.AST;
using Paganism.PParser.Values;

namespace Paganism.Interpreter.Data.Instances
{
    public class FunctionInstance : Instance
    {
        public FunctionInstance(FunctionDeclarateExpression functionDeclarate)
        {
            Name = functionDeclarate.Name;
            Statements = functionDeclarate.Statement;
            Arguments = functionDeclarate.RequiredArguments;
            ReturnType = functionDeclarate.ReturnType;
            IsAsync = functionDeclarate.IsAsync;
            FunctionDeclarateExpression = functionDeclarate;
            Filepath = functionDeclarate.Filepath;
        }

        public override string InstanceName => "Function";

        public string Name { get; }

        public BlockStatementExpression Statements { get; }

        public Argument[] Arguments { get; }

        public TypeValue ReturnType { get; }

        public bool IsAsync { get; }

        public string Filepath { get; }

        public FunctionDeclarateExpression FunctionDeclarateExpression { get; }

        public Value ExecuteAndReturn(params Argument[] arguments)
        {
            return FunctionDeclarateExpression.Eval(arguments);
        }
    }
}
