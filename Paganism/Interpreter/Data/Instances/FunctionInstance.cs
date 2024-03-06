using Paganism.PParser;
using Paganism.PParser.AST;
using Paganism.PParser.Values;
using System;

#nullable enable
namespace Paganism.Interpreter.Data.Instances
{
    public class FunctionInstance : Instance
    {
        public FunctionInstance(FunctionDeclarateExpression functionDeclarate, Func<Argument[], Value>? FunctionAction = null)
        {
            Name = functionDeclarate.Name;
            Statements = functionDeclarate.Statement;
            Arguments = functionDeclarate.RequiredArguments;
            ReturnType = functionDeclarate.ReturnType;
            IsAsync = functionDeclarate.IsAsync;
            FunctionDeclarateExpression = functionDeclarate;
            Filepath = functionDeclarate.ExpressionInfo.Filepath;
            Action = FunctionAction;
        }

        public override string InstanceName => "Function";

        public string Name { get; }

        public BlockStatementExpression Statements { get; }

        public Argument[] Arguments { get; }

        public TypeValue ReturnType { get; }

        public bool IsAsync { get; }

        public string Filepath { get; }

        public Func<Argument[], Value>? Action { get; } 

        public FunctionDeclarateExpression FunctionDeclarateExpression { get; }

        public Value ExecuteAndReturn(params Argument[] arguments)
        {
            return FunctionDeclarateExpression.Eval(arguments);
        }
    }
}
