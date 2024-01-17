using Paganism.Exceptions;
using Paganism.Interpreter.Data;
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

        public override Value Eval(params Argument[] arguments)
        {
            Interpreter.Data.Instances.FunctionInstance function = Functions.Instance.Value.Get(Parent, FunctionName);

            if (!function.IsAsync && IsAwait)
            {
                throw new InterpreterException("You cant use await for not async functions");
            }

            Argument[] totalArguments = new Argument[function.Arguments.Length];

            for (int i = 0; i < function.Arguments.Length; i++)
            {
                Argument functionArgument = function.Arguments[i];

                if (i > Arguments.Length - 1)
                {
                    if (functionArgument.IsRequired)
                    {
                        throw new InterpreterException($"Argument in {function.Name} function is required.", Line, Position);
                    }

                    Argument noneArgument = new(functionArgument.Name, functionArgument.Type, new NoneExpression(Parent, Line, Position, Filepath));

                    totalArguments[i] = noneArgument;
                    Variables.Instance.Value.Add(function.Statements, functionArgument.Name, noneArgument.Value.Eval());
                    continue;
                }

                Argument argument = Arguments[i];

                if (functionArgument.Type == TypesType.Structure)
                {
                    Value variable = Variables.Instance.Value.Get(function.Statements, argument.Name);

                    if (variable is not NoneValue)
                    {
                        if (variable is not StructureValue structure)
                        {
                            throw new InterpreterException($"Except variable with structure {functionArgument.Name} type");
                        }

                        if (functionArgument.StructureName != structure.Structure.Name)
                        {
                            throw new InterpreterException($"Except structure {functionArgument.StructureName} type");
                        }
                    }
                }

                if (functionArgument.Type != TypesType.Any && argument.Type != TypesType.Any && functionArgument.Type != argument.Type)
                {
                    throw new InterpreterException($"Except {functionArgument.Type}", Line, Position);
                }

                Argument initArgument = new(functionArgument.Name, functionArgument.Type, argument.Value, functionArgument.IsRequired, functionArgument.IsArray, functionArgument.StructureName);

                totalArguments[i] = initArgument;

                Variables.Instance.Value.Add(function.Statements, initArgument.Name, initArgument.Value.Eval());
            }

            if (IsAwait && function.IsAsync)
            {
                return function.ExecuteAndReturn(totalArguments);
            }

            if (function.ReturnTypes.Length > 0)
            {
                Value result = function.ExecuteAndReturn(totalArguments);

                foreach (Argument argument in totalArguments)
                {
                    Variables.Instance.Value.Remove(function.Statements, argument.Name);
                }

                return result;
            }

            _ = function.ExecuteAndReturn(totalArguments);

            foreach (Argument argument in totalArguments)
            {
                Variables.Instance.Value.Remove(function.Statements, argument.Name);
            }

            return null;
        }

        public void Execute(params Argument[] arguments)
        {
            _ = Eval();
        }
    }
}
