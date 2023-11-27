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
            ReturnTypes = new TypesType[functionDeclarate.ReturnTypes.Length];
            FunctionDeclarateExpression = functionDeclarate;
            DeclarateFilePath = functionDeclarate.DeclarateFilePath;
            
            for (int i = 0;i < ReturnTypes.Length;i++)
            {
                var type = functionDeclarate.ReturnTypes[i];

                ReturnTypes[i] = Tokens.TokenTypeToValueType[type];
            }
        }

        public override string InstanceName => "Variable";

        public string Name { get; }

        public BlockStatementExpression Statements { get; }

        public Argument[] Arguments { get; }

        public TypesType[] ReturnTypes { get; }

        public string DeclarateFilePath { get; }

        private FunctionDeclarateExpression FunctionDeclarateExpression { get; }

        public Value ExecuteAndReturn(params Argument[] arguments)
        {
            return FunctionDeclarateExpression.ExecuteAndReturn(arguments);
        }
    }
}
