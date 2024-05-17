using Paganism.API;
using Paganism.Exceptions;
using Paganism.Interpreter.Data.Instances;
using Paganism.PParser;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Paganism.Interpreter.Data
{
    public class Functions : DataStorage<FunctionInstance>
    {
        public override string Name => "Function";

        public static Functions Instance => Lazy.Value;

        private static Lazy<Functions> Lazy { get; } = new();

        private static InstanceInfo _languageFunctionInfo = new InstanceInfo(true, false, string.Empty);

        protected override IReadOnlyDictionary<string, FunctionInstance> Language { get; } = new Dictionary<string, FunctionInstance>()
        {
            { "cs_call", new FunctionInstance(InstanceInfo.Empty,
                new FunctionDeclarateExpression(ExpressionInfo.EmptyInfo, "cs_call", new BlockStatementExpression(ExpressionInfo.EmptyInfo, null), new Argument[]
                {
                    new("namespace", TypesType.String, null, true),
                    new("method", TypesType.String, null, true),
                    new("arguments", TypesType.Any, null, true, true)
                },
                    false,
                    _languageFunctionInfo), (Argument[] arguments) =>
                    {
                        if (!FunctionDeclarateExpression.Types.TryGetValue(arguments[0].Value.Evaluate().AsString(), out Type findedClass))
                        {
                            var name = arguments[0].Value.Evaluate().AsString();

                            findedClass = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).FirstOrDefault(type => type.FullName == name);

                            if (findedClass == default)
                            {
                                throw new InterpreterException($"Method with {name} name not found", ExpressionInfo.EmptyInfo);
                            }
                        }

                        if (arguments[2].Value.Evaluate() is NoneValue)
                        {
                            var method = findedClass.GetMethod(arguments[1].Value.Evaluate().AsString(), new Type[] { });

                            return Value.Create(method.Invoke(null, new object[] { }));
                        }
                        else
                        {
                            var paramater = arguments[2].Value.Evaluate();

                            MethodInfo method = null;

                            if (paramater is ArrayValue arrayValue && arrayValue.Elements.Length == 0)
                            {
                                method = findedClass.GetMethod(arguments[1].Value.Evaluate().AsString());
                            }
                            if (paramater is StringValue)
                            {
                                method = findedClass.GetMethod(arguments[1].Value.Evaluate().AsString(), new Type[] { typeof(string) });
                            }
                            else if (paramater is CharValue)
                            {
                                method = findedClass.GetMethod(arguments[1].Value.Evaluate().AsString(), new Type[] { typeof(char) });
                            }
                            else
                            {
                                method = paramater is BooleanValue
                                    ? findedClass.GetMethod(arguments[1].Value.Evaluate().AsString(), new Type[] { typeof(bool) })
                                    : paramater is NumberValue
                                                            ? findedClass.GetMethod(arguments[1].Value.Evaluate().AsString(), new Type[] { typeof(int) })
                                                            : findedClass.GetMethod(arguments[1].Value.Evaluate().AsString(), new Type[] { typeof(object) });
                            }

                            var paramaters = method.GetParameters();

                            if (paramaters.Length == 0)
                            {
                                return Value.Create(method.Invoke(null, new object[] { }));
                            }
                            else if (paramaters[0].ParameterType == typeof(string) || paramaters[0].ParameterType == typeof(object))
                            {
                                return Value.Create(method.Invoke(null, new object[] { arguments[2].Value.Evaluate().AsString() }));
                            }
                            else if (paramaters[0].ParameterType == typeof(bool))
                            {
                                return Value.Create(method.Invoke(null, new object[] { arguments[2].Value.Evaluate().AsBoolean() }));
                            }
                            else if (paramaters[0].ParameterType == typeof(int))
                            {
                                return Value.Create(method.Invoke(null, new object[] { (int)arguments[2].Value.Evaluate().AsNumber() }));
                            }
                            else if (paramaters[0].ParameterType == typeof(char))
                            {
                                return Value.Create(method.Invoke(null, new object[] { arguments[2].Value.Evaluate().AsString()[0] }));
                            }
                            else
                            {
                                var g = arguments[2].Value.Evaluate();
                                return Value.Create(method.Invoke(null, new object[] { arguments[2].Value.Evaluate().AsString() }));
                            }
                        }
                    }
                )
            },
            { "import", new FunctionInstance(InstanceInfo.Empty,
                new FunctionDeclarateExpression(ExpressionInfo.EmptyInfo, "import", new BlockStatementExpression(ExpressionInfo.EmptyInfo, null), new Argument[]
                {
                    new("file", TypesType.String, null, true)
                },
                    false,
                    _languageFunctionInfo)
                , (Argument[] arguments) => {
                    var name = arguments[0].Value.Evaluate().AsString();
                    var baseDir = Directory.GetCurrentDirectory();
                    if (ImportManager.SpecificDirectory is not null)
                    {
                        baseDir = API.ImportManager.SpecificDirectory;
                    }

                    if (name.Contains(".cs"))
                    {
                        var type = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).FirstOrDefault(type => type.Name + ".cs" == name);
                        var types = new List<object>();

                        if (type is null)
                        {
                            var info = arguments[0].Value.ExpressionInfo;

                            throw new InterpreterException($"Unknown type with {name} name", info);
                        }

                        var value = PaganismFromCSharp.Create(type, ref types);

                        types.Add(value);

                        foreach (var paganismType in types)
                        {
                            var instance = Instances.Instance.ToInstance(value);

                            if (paganismType is FunctionValue functionValue)
                            {
                                Instance.Set(ExpressionInfo.EmptyInfo, null, functionValue.Value.Name, instance as FunctionInstance);
                            }
                            else if (paganismType is StructureValue structureValue)
                            {
                                Structures.Instance.Set(ExpressionInfo.EmptyInfo, null, structureValue.Structure.Name, instance as StructureInstance);
                            }
                            else if (paganismType is EnumInstance enumInstance)
                            {
                                Enums.Instance.Set(ExpressionInfo.EmptyInfo, null, enumInstance.Name, enumInstance);
                            }
                        }

                        return new VoidValue(arguments[0].Value.ExpressionInfo);
                    }

                    string[] result;
                    string[] files;
                    if (ImportManager.PreLoadedFiles.ContainsKey(name))
                    {
                        files = new string[]
                        {
                            name
                        };
                        result = ImportManager.PreLoadedFiles[name];
                    }
                    else
                    {
                        files = Directory.GetFileSystemEntries(baseDir, name);
                        result = File.ReadAllLines(files[0]);
                    }

                    var lexer = new Lexer.Lexer(result, string.Empty);
                    var parser = new Parser(lexer.Run(), files[0]);
                    var interpreter = new Interpreter(parser.Run());
                    interpreter.Run(false);

                    return new VoidValue(ExpressionInfo.EmptyInfo);
                })
            },
            { "pgm_resize", new FunctionInstance(InstanceInfo.Empty,
                new FunctionDeclarateExpression(ExpressionInfo.EmptyInfo, "pgm_resize", new BlockStatementExpression(ExpressionInfo.EmptyInfo, null), new Argument[]
                {
                    new("array", TypesType.Array),
                    new("size", TypesType.Number)
                },
                    false,
                    _languageFunctionInfo), (Argument[] arguments) =>
                    {
                        var array = arguments[0].Value.Evaluate() as ArrayValue;

                        var newElements = new Value[(int)arguments[1].Value.Evaluate().AsNumber()];

                        for (int i = 0; i < newElements.Length; i++)
                        {
                            if (i > array.Elements.Length - 1)
                            {
                                newElements[i] = new NoneValue(arguments[0].Value.ExpressionInfo);
                                continue;
                            }

                            newElements[i] = array.Elements[i];
                        }

                        var argument = arguments[0].Value;

                        var newArray = new ArrayValue(argument.ExpressionInfo, newElements);

                        return newArray;
                    }
                )
            },
            { "pgm_size", new FunctionInstance(InstanceInfo.Empty,
                new FunctionDeclarateExpression(ExpressionInfo.EmptyInfo, "pgm_size", new BlockStatementExpression(ExpressionInfo.EmptyInfo, null), new Argument[]
                {
                    new("array", TypesType.Array)
                },
                    false,
                    _languageFunctionInfo), (Argument[] arguments) =>
                    {
                        var argument = arguments[0].Value;

                        return new NumberValue(argument.ExpressionInfo, (arguments[0].Value.Evaluate() as ArrayValue).Elements.Length);
                    }
                )
            },
            { "print", new FunctionInstance(InstanceInfo.Empty,
                new FunctionDeclarateExpression(ExpressionInfo.EmptyInfo, "print", new BlockStatementExpression(ExpressionInfo.EmptyInfo, null), new Argument[]
                {
                    new("content", TypesType.String, null, true)
                }, false, _languageFunctionInfo), (Argument[] arguments) =>
                    {
                        Console.Write(arguments[0].Value.Evaluate().AsString());
                        return new VoidValue(arguments[0].Value.ExpressionInfo);
                    }
                )
            },
            { "println", new FunctionInstance(InstanceInfo.Empty,
                new FunctionDeclarateExpression(ExpressionInfo.EmptyInfo, "println", new BlockStatementExpression(ExpressionInfo.EmptyInfo, null), new Argument[]
                {
                    new("content", TypesType.String)
                }, false, _languageFunctionInfo), (Argument[] arguments) =>
                    {
                        Console.WriteLine(arguments[0].Value.Evaluate().AsString());
                        return new VoidValue(arguments[0].Value.ExpressionInfo);
                    }
                )
            },
            { "read", new FunctionInstance(InstanceInfo.Empty,
                new FunctionDeclarateExpression(ExpressionInfo.EmptyInfo, "read", new BlockStatementExpression(ExpressionInfo.EmptyInfo, null), new Argument[]
                {
                    new("content", TypesType.String)
                }, false, _languageFunctionInfo), (Argument[] arguments) =>
                    {
                        return Value.Create(Console.ReadLine());
                    }
                )
            },
            { "millitime", new FunctionInstance(InstanceInfo.Empty,
                new FunctionDeclarateExpression(ExpressionInfo.EmptyInfo, "millitime", new BlockStatementExpression(ExpressionInfo.EmptyInfo, null), new Argument[]{},
                    false, _languageFunctionInfo), (Argument[] arguments) =>
                {
                        return Value.Create(DateTimeOffset.Now.ToUnixTimeMilliseconds());
                }
                )
            }
        };
    }
}
