using Paganism.Lexer;
using Paganism.Lexer.Enums;
using Paganism.PParser;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.Interpreter.Data.Instances
{
    public class FunctionInstance : Instance
    {
        public FunctionInstance(FunctionDeclarateExpression functionDeclarate)
        {
            Name = functionDeclarate.Name;
            Statements = functionDeclarate.Statement;
            Arguments = functionDeclarate.RequiredArguments;
            ReturnTypes = functionDeclarate.ReturnTypes;
            IsAsync = functionDeclarate.IsAsync;
            FunctionDeclarateExpression = functionDeclarate;
            Filepath = functionDeclarate.Filepath;
        }

        public override string InstanceName => "Function";

        public string Name { get; }

        public BlockStatementExpression Statements { get; }

        public Argument[] Arguments { get; }

        public Return[] ReturnTypes { get; }

        public bool IsAsync { get; }

        public string Filepath { get; }

        public FunctionDeclarateExpression FunctionDeclarateExpression { get; }

        public Value ExecuteAndReturn(params Argument[] arguments)
        {
            return FunctionDeclarateExpression.Eval(arguments);
        }
    }
}
