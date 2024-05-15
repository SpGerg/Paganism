using Paganism.Exceptions;
using Paganism.Interpreter.Data;
using Paganism.Interpreter.Data.Instances;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class FunctionDeclarateExpression : EvaluableExpression, IStatement, IExecutable, IDeclaratable
    {
        public FunctionDeclarateExpression(ExpressionInfo info, string name, BlockStatementExpression statement, Argument[] requiredArguments, bool isAsync, bool isShow = false, TypeValue returnType = null) : base(info)
        {
            Name = name;
            Statement = statement;
            RequiredArguments = requiredArguments;
            RequiredArguments ??= new Argument[0];
            IsAsync = isAsync;
            IsShow = isShow;
            ReturnType = returnType;

            ReturnType ??= new TypeValue(ExpressionInfo.EmptyInfo, TypesType.None, string.Empty);

            if (name.StartsWith("__"))
            {
                throw new InterpreterException($"Function cant start with '__'",
                    ExpressionInfo);
            }

            if (Statement is null || Statement.Statements is null)
            {
                return;
            }

            if (!Functions.Instance.IsLanguage(Name) && ReturnType.Value is not TypesType.None && Statement.Statements.FirstOrDefault(statementInBlock => statementInBlock is ReturnExpression) == default)
            {
                throw new InterpreterException($"Function with {Name} name must return value",
                    ExpressionInfo);
            }

            if (ReturnType.Type is TypesType.None && Statement.Statements.FirstOrDefault(statementInBlock => statementInBlock is ReturnExpression) != default)
            {
                throw new InterpreterException($"Except return value type in function with {Name} name",
                    ExpressionInfo);
            }
        }

        public static readonly Dictionary<string, Type> Types = new()
        {
            { "System.Console", typeof(Console) }
        };

        public string Name { get; }

        public BlockStatementExpression Statement { get; }

        public bool IsAsync { get; }

        public bool IsShow { get; }

        public Argument[] RequiredArguments { get; }

        public TypeValue ReturnType { get; }

        public void Declarate()
        {
            Functions.Instance.Set(ExpressionInfo.Parent, Name, new FunctionInstance(this));
        }

        public void Remove()
        {
            Functions.Instance.Remove(ExpressionInfo.Parent, Name);
        }

        public Task ExecuteAsync(params Argument[] arguments)
        {
            var task = Task.Run(() =>
            {
                Statement.Evaluate(arguments);
            });

            task.ContinueWith(_ =>
            {
                Tasks.Remove(task);
            });

            Tasks.Add(task);

            return task;
        }

        public void Execute(params Argument[] arguments)
        {
            Evaluate(arguments);
        }

        public void CreateArguments(params Argument[] arguments)
        {
            Argument[] totalArguments = new Argument[RequiredArguments.Length];

            for (int i = 0; i < RequiredArguments.Length; i++)
            {
                var functionArgument = RequiredArguments[i];

                if (i > arguments.Length - 1)
                {
                    if (functionArgument.IsRequired)
                    {
                        throw new InterpreterException($"Argument in {Name} function is required.",
                            ExpressionInfo);
                    }

                    var noneArgument = new Argument(functionArgument.Name, functionArgument.Type, new NoneValue(ExpressionInfo));

                    totalArguments[i] = noneArgument;
                    Variables.Instance.Set(Statement, functionArgument.Name, noneArgument.Value.Evaluate());
                    continue;
                }

                var argument = arguments[i];

                if (!functionArgument.Equals(argument) && functionArgument.Type.IsCanCast(argument.Type))
                {
                    throw new InterpreterException($"Except {functionArgument.Type}", ExpressionInfo);
                }

                var initArgument = new Argument(functionArgument.Name, functionArgument.Type, argument.Value, functionArgument.IsRequired, functionArgument.IsArray);

                totalArguments[i] = initArgument;

                if (initArgument.Value is FunctionCallExpression function)
                {
                    initArgument.Value = function.Evaluate(function.Arguments);
                }
                else if (initArgument.Value is BinaryOperatorExpression operatorExpression)
                {
                    initArgument.Value = operatorExpression.Evaluate();
                }
            }

            foreach (var argument in totalArguments)
            {
                if (argument.Value is FunctionDeclarateExpression functionDeclarateExpression)
                {
                    Variables.Instance.Set(Statement, argument.Name, new FunctionValue(ExpressionInfo, functionDeclarateExpression));
                    Functions.Instance.Set(Statement, argument.Name, new FunctionInstance(functionDeclarateExpression));
                }
                else
                {
                    Variables.Instance.Set(Statement, argument.Name, argument.Value.Evaluate());
                }
            }
        }

        public override Value Evaluate(params Argument[] arguments)
        {
            CreateArguments(arguments);

            if (Functions.Instance.IsLanguage(Name))
            {
                var nativeFunction = Functions.Instance.Get(Statement, Name, ExpressionInfo);

                if (nativeFunction.Action is not null)
                {
                    return nativeFunction.Action(arguments);
                }
            }

            if (Statement is null)
            {
                return new VoidValue(ExpressionInfo.EmptyInfo);
            }

            if (IsAsync)
            {
                var task = ExecuteAsync(arguments);

                var structureExpression = new StructureDeclarateExpression(ExpressionInfo, "task", new StructureMemberExpression[1]);
                structureExpression.Members[0] = new StructureMemberExpression(
                    structureExpression.ExpressionInfo, structureExpression.Name, new TypeValue(ExpressionInfo, TypesType.Number, string.Empty), "id", true);

                var structure = Value.Create(structureExpression) as StructureValue;
                structure.Set("id", new NumberValue(ExpressionInfo, task.Id), ExpressionInfo.Filepath);

                return structure;
            }
            else
            {
                var result = Statement.Evaluate(arguments);

                if (result is null)
                {
                    return new NoneValue(ExpressionInfo);
                }

                if (!result.Is(ReturnType.Value, ReturnType.TypeName))
                {
                    throw new InterpreterException($"Except {ReturnType.AsString()} type", ExpressionInfo);
                }

                return result;
            }
        }
    }
}
