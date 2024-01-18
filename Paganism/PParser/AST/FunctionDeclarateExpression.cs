﻿using Paganism.Exceptions;
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
    public class FunctionDeclarateExpression : EvaluableExpression, IStatement, IExecutable
    {
        public FunctionDeclarateExpression(BlockStatementExpression parent, int line, int position, string filepath, string name, BlockStatementExpression statement, Argument[] requiredArguments, bool isAsync, params Return[] returnTypes) : base(parent, line, position, filepath)
        {
            Name = name;
            Statement = statement;
            RequiredArguments = requiredArguments;
            IsAsync = isAsync;
            ReturnTypes = returnTypes;

            if (Statement is null || Statement.Statements is null)
            {
                return;
            }

            if (!Functions.Instance.Value.IsLanguage(Name) && ReturnTypes.Length > 0 && Statement.Statements.FirstOrDefault(statementInBlock => statementInBlock is ReturnExpression) == default)
            {
                throw new InterpreterException($"Function with {Name} name must return value");
            }

            if (ReturnTypes.Length == 0 && Statement.Statements.FirstOrDefault(statementInBlock => statementInBlock is ReturnExpression) != default)
            {
                throw new InterpreterException($"Except return value type in function with {Name} name");
            }
        }

        public string Name { get; }

        public BlockStatementExpression Statement { get; }

        public bool IsAsync { get; }

        public Argument[] RequiredArguments { get; }

        public Return[] ReturnTypes { get; }

        private static readonly Dictionary<string, Type> Types = new()
        {
            { "System.Console", typeof(Console) }
        };

        public void Create()
        {
            Functions.Instance.Value.Add(Parent, Name, new FunctionInstance(this));
        }

        public void Remove()
        {
            Functions.Instance.Value.Remove(Parent, Name);
        }

        public Task ExecuteAsync(params Argument[] arguments)
        {
            var task = Task.Run(() =>
            {
                Statement.ExecuteAndReturn(arguments);
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
            Eval(arguments);
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
                        throw new InterpreterException($"Argument in {Name} function is required.", Line, Position);
                    }

                    var noneArgument = new Argument(functionArgument.Name, functionArgument.Type, new NoneExpression(Parent, Line, Position, Filepath));

                    totalArguments[i] = noneArgument;
                    Variables.Instance.Value.Add(Statement, functionArgument.Name, noneArgument.Value.Eval());
                    continue;
                }

                var argument = arguments[i];

                if (functionArgument.Type is TypesType.Structure)
                {
                    Value value = null;

                    try
                    {
                        value = Variables.Instance.Value.Get(Statement, argument.Name);
                    }
                    catch
                    {
                        value = argument.Value.Eval();
                    }

                    if (value is not NoneValue)
                    {
                        if (value is not StructureValue structure)
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

                var initArgument = new Argument(functionArgument.Name, functionArgument.Type, argument.Value, functionArgument.IsRequired, functionArgument.IsArray, functionArgument.StructureName);

                totalArguments[i] = initArgument;

                Variables.Instance.Value.Add(Statement, initArgument.Name, initArgument.Value.Eval());
            }
        }

        public override Value Eval(params Argument[] arguments)
        {
            CreateArguments(arguments);

            if (Name == "pgm_call")
            {
                if (!Types.TryGetValue(arguments[0].Value.Eval().AsString(), out Type findedClass))
                {
                    var name = arguments[0].Value.Eval().AsString();

                    findedClass = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).FirstOrDefault(type => type.FullName == name);

                    if (findedClass == default)
                    {
                        throw new InterpreterException($"Method with {name} name not found");
                    }
                }

                if (arguments[2].Value.Eval() is NoneValue)
                {
                    var method = findedClass.GetMethod(arguments[1].Value.Eval().AsString(), new Type[] { });

                    return Value.Create(method.Invoke(null, new object[] { }));
                }
                else
                {
                    var paramater = arguments[2].Value.Eval();

                    MethodInfo method = null;

                    if (paramater is StringValue)
                    {
                        method = findedClass.GetMethod(arguments[1].Value.Eval().AsString(), new Type[] { typeof(string) });
                    }
                    else if (paramater is CharValue)
                    {
                        method = findedClass.GetMethod(arguments[1].Value.Eval().AsString(), new Type[] { typeof(char) });
                    }
                    else
                    {
                        method = paramater is BooleanValue
                            ? findedClass.GetMethod(arguments[1].Value.Eval().AsString(), new Type[] { typeof(bool) })
                            : paramater is NumberValue
                                                    ? findedClass.GetMethod(arguments[1].Value.Eval().AsString(), new Type[] { typeof(int) })
                                                    : findedClass.GetMethod(arguments[1].Value.Eval().AsString(), new Type[] { typeof(object) });
                    }

                    if (method.GetParameters()[0].ParameterType == typeof(string) || method.GetParameters()[0].ParameterType == typeof(object))
                    {
                        return Value.Create(method.Invoke(null, new object[] { arguments[2].Value.Eval().AsString() }));
                    }
                    else if (method.GetParameters()[0].ParameterType == typeof(bool))
                    {
                        return Value.Create(method.Invoke(null, new object[] { arguments[2].Value.Eval().AsBoolean() }));
                    }
                    else if (method.GetParameters()[0].ParameterType == typeof(int))
                    {
                        return Value.Create(method.Invoke(null, new object[] { (int)arguments[2].Value.Eval().AsNumber() }));
                    }
                    else if (method.GetParameters()[0].ParameterType == typeof(char))
                    {
                        return Value.Create(method.Invoke(null, new object[] { arguments[2].Value.Eval().AsString()[0] }));
                    }
                    else
                    {
                        var g = arguments[2].Value.Eval();
                        return Value.Create(method.Invoke(null, new object[] { arguments[2].Value.Eval().AsString() }));
                    }
                }
            }
            else if (Name == "pgm_create")
            {
                return new StructureValue(Parent, arguments[0].Value.Eval().AsString());
            }
            else if (Name == "pgm_size")
            {
                return new NumberValue((arguments[0].Value.Eval() as ArrayValue).Elements.Length);
            }
            else if (Name == "pgm_resize")
            {
                var array = arguments[0].Value.Eval() as ArrayValue;

                var newElements = new Value[(int)arguments[1].Value.Eval().AsNumber()];

                for (int i = 0; i < newElements.Length; i++)
                {
                    if (i > array.Elements.Length - 1)
                    {
                        newElements[i] = new NoneValue();
                        continue;
                    }

                    newElements[i] = array.Elements[i];
                }

                var newArray = new ArrayValue(newElements);

                return newArray;
            }
            else if (Name == "pgm_import")
            {
                var name = arguments[0].Value.Eval().AsString();
                var files = Directory.GetFileSystemEntries(Directory.GetCurrentDirectory(), name);
                var result = File.ReadAllLines(files[0]);

                var lexer = new Lexer.Lexer(result);
                var parser = new Parser(lexer.Run(), files[0]);
                var interpreter = new Interpreter.Interpreter(parser.Run());
                interpreter.Run(false);
            }

            if (Statement == null)
            {
                return new NoneValue();
            }

            if (IsAsync)
            {
                var task = ExecuteAsync(arguments);

                var structureExpression = new StructureDeclarateExpression(Parent, Line, Position, Filepath, "task", new StructureMemberExpression[1]);
                structureExpression.Members[0] = new StructureMemberExpression(
                    structureExpression.Parent, structureExpression.Line, structureExpression.Position, Filepath, structureExpression.Name, string.Empty, TypesType.Number, "id", true);

                var structure = Value.Create(structureExpression) as StructureValue;
                structure.Set("id", new NumberValue(task.Id));

                return structure;
            }
            else
            {
                return Statement.ExecuteAndReturn(arguments);
            }
        }
    }
}
