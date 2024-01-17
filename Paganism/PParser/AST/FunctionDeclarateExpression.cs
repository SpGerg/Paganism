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
            Task task = Task.Run(() =>
            {
                _ = Statement.ExecuteAndReturn(arguments);
            });

            _ = task.ContinueWith(_ =>
            {
                Tasks.Remove(task);
            });

            Tasks.Add(task);

            return task;
        }

        public void Execute(params Argument[] arguments)
        {
            _ = Eval(arguments);
        }

        public override Value Eval(params Argument[] arguments)
        {
            if (Name == "pgm_call")
            {

                if (!Types.TryGetValue(arguments[0].Value.Eval().AsString(), out Type findedClass))
                {
                    string name = arguments[0].Value.Eval().AsString();

                    findedClass = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).FirstOrDefault(type => type.FullName == name);

                    if (findedClass == default)
                    {
                        throw new InterpreterException($"Method with {name} name not found");
                    }
                }

                if (arguments[2].Value.Eval() is NoneValue)
                {
                    MethodInfo method = findedClass.GetMethod(arguments[1].Value.Eval().AsString(), new Type[] { });

                    return Value.Create(method.Invoke(null, new object[] { }));
                }
                else
                {
                    Value paramater = arguments[2].Value.Eval();

                    MethodInfo method = paramater is StringValue
                        ? findedClass.GetMethod(arguments[1].Value.Eval().AsString(), new Type[] { typeof(string) })
                        : paramater is CharValue
                            ? findedClass.GetMethod(arguments[1].Value.Eval().AsString(), new Type[] { typeof(char) })
                            : paramater is BooleanValue
                                                    ? findedClass.GetMethod(arguments[1].Value.Eval().AsString(), new Type[] { typeof(bool) })
                                                    : paramater is NumberValue
                                                                            ? findedClass.GetMethod(arguments[1].Value.Eval().AsString(), new Type[] { typeof(int) })
                                                                            : findedClass.GetMethod(arguments[1].Value.Eval().AsString(), new Type[] { typeof(object) });
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
                        Value g = arguments[2].Value.Eval();
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
                ArrayValue array = arguments[0].Value.Eval() as ArrayValue;

                Value[] newElements = new Value[(int)arguments[1].Value.Eval().AsNumber()];

                for (int i = 0; i < newElements.Length; i++)
                {
                    if (i > array.Elements.Length - 1)
                    {
                        newElements[i] = new NoneValue();
                        continue;
                    }

                    newElements[i] = array.Elements[i];
                }

                ArrayValue newArray = new(newElements);

                return newArray;
            }
            else if (Name == "pgm_import")
            {
                string name = arguments[0].Value.Eval().AsString();
                string[] files = Directory.GetFileSystemEntries(Directory.GetCurrentDirectory(), name);
                string[] result = File.ReadAllLines(files[0]);

                Lexer.Lexer lexer = new(result);
                Parser parser = new(lexer.Run(), files[0]);
                Interpreter.Interpreter interpreter = new(parser.Run());
                interpreter.Run(false);
            }

            if (Statement == null)
            {
                return new NoneValue();
            }

            if (IsAsync)
            {
                Task task = ExecuteAsync(arguments);

                StructureDeclarateExpression structureExpression = new(Parent, Line, Position, Filepath, "task", new StructureMemberExpression[1]);
                structureExpression.Members[0] = new StructureMemberExpression(
                    structureExpression.Parent, structureExpression.Line, structureExpression.Position, Filepath, structureExpression.Name, string.Empty, TypesType.Number, "id", true);

                StructureValue structure = Value.Create(structureExpression) as StructureValue;
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
