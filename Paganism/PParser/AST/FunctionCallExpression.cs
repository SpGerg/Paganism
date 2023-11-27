using Paganism.Exceptions;
using Paganism.Interpreter.Data;
using Paganism.Lexer;
using Paganism.Lexer.Enums;
using Paganism.PParser.AST.Enums;
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

            Argument[] totalArguments = new Argument[function.Arguments.Length];

            for (int i = 0; i < function.Arguments.Length; i++)
            {
                var functionArgument = function.Arguments[i];

                if (i > Arguments.Length - 1)
                {
                    var noneArgument = new Argument(functionArgument.Name, functionArgument.Type, functionArgument.IsRequired, new NoneExpression());

                    totalArguments[i] = noneArgument;
                    Variables.Add(functionArgument.Name, noneArgument.Value.Eval());
                    continue;
                }

                var argument = Arguments[i];

                if (functionArgument.Type == TypesType.Structure)
                {
                    var variable = Variables.Get(argument.Name);

                    if (variable is not NoneValue)
                    {
                        if (variable is not StructureValue structure)
                        {
                            throw new InterpreterException($"Except variable with structure {functionArgument.Name} type");
                        }

                        if (functionArgument.StructureName != structure.Structure.Name)
                        {
                            throw new InterpreterException($"Except structure {functionArgument.Name} type");
                        }
                    }            
                }

                if ((functionArgument.Type != TypesType.Any && argument.Type != TypesType.Any) && functionArgument.Type != argument.Type)
                {
                    throw new InterpreterException($"Except {functionArgument.Type}");
                }

                var initArgument = new Argument(functionArgument.Name, functionArgument.Type, functionArgument.IsRequired, argument.Value);

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

                return result;
            }

            function.ExecuteAndReturn(totalArguments);

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
