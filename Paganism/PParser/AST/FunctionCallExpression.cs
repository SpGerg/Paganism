using Paganism.Lexer;
using Paganism.Lexer.Enums;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class FunctionCallExpression : Expression, IStatement, IExecutable, IEvaluable
    {
        public FunctionCallExpression(string functionName, Argument[] arguments)
        {
            FunctionName = functionName;
            Arguments = arguments;
        }

        public string FunctionName { get; }

        public Argument[] Arguments { get; }

        public Value Eval()
        {
            var function = Functions.Get(FunctionName);

            Argument[] totalArguments = new Argument[function.RequiredArguments.Length];

            for (int i = 0; i < function.RequiredArguments.Length; i++)
            {
                if (i > Arguments.Length - 1)
                {
                    totalArguments[i] = null;
                    continue;
                }

                var argument = function.RequiredArguments[i];

                if (argument.Type != TokenType.AnyType && (argument.Type != Arguments[i].Type && argument.Type != Tokens.TypeToVariableType[Arguments[i].Type]))
                {
                    throw new Exception($"Except {argument.Type}");
                }

                var initArgument = new Argument(argument.Name, argument.Type, argument.IsRequired, Arguments[i].Value);

                totalArguments[i] = initArgument;
                Variables.Add(initArgument.Name, initArgument.Value.Eval());
            }

            if (function.ReturnTypes.Length > 0)
            {
                var result = function.ExecuteAndReturn(totalArguments);

                foreach (var argument in totalArguments)
                {
                    Variables.Remove(argument.Name);
                }

                return Value.Create(result[0]);
            }

            function.Execute(totalArguments);

            foreach (var argument in totalArguments)
            {
                Variables.Remove(argument.Name);
            }

            return null;
        }

        public void Execute(params Argument[] arguments)
        {
            Eval();
        }
    }
}
