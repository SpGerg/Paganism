using Paganism.PParser;
using Paganism.PParser.AST;
using Paganism.PParser.Values;
using System;

#nullable enable
namespace Paganism.Interpreter.Data.Instances
{
    public class FunctionInstance : Instance
    {
        public FunctionInstance(InstanceInfo instanceInfo, FunctionDeclarateExpression functionDeclarate, Func<Argument[], Value>? FunctionAction = null) : base(instanceInfo)
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

        public FunctionInstance(InstanceInfo instanceInfo, FunctionValue functionValue) : base(instanceInfo)
        {
            Name = functionValue.Name;
            Statements = functionValue.Value.Statement;
            Arguments = functionValue.Value.RequiredArguments;
            ReturnType = functionValue.Value.ReturnType;
            IsAsync = functionValue.Value.IsAsync;
            FunctionDeclarateExpression = functionValue.Value;
            Filepath = functionValue.ExpressionInfo.Filepath;
            Action = null;
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
            return FunctionDeclarateExpression.Evaluate(arguments);
        }
    }
}
