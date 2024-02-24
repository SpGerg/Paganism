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

        public static Lazy<Functions> Instance { get; } = new();

        protected override IReadOnlyDictionary<string, FunctionInstance> Language { get; } = new Dictionary<string, FunctionInstance>()
        {
            { "cs_call", new FunctionInstance(
                new FunctionDeclarateExpression(null, -1, -1, string.Empty, "cs_call", new BlockStatementExpression(null, 0, 0, string.Empty, null), new Argument[]
                {
                    new("namespace", TypesType.String, null, true),
                    new("method", TypesType.String, null, true),
                    new("arguments", TypesType.Any, null, true, true)
                },
                    false,
                    true), (Argument[] arguments) =>
                    {
                        if (!FunctionDeclarateExpression.Types.TryGetValue(arguments[0].Value.Eval().AsString(), out Type findedClass))
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
                )
            },
            { "import", new FunctionInstance(
                new FunctionDeclarateExpression(null, -1, -1, string.Empty, "import", new BlockStatementExpression(null, 0, 0, string.Empty, null), new Argument[]
                {
                    new("file", TypesType.String)
                },
                    false,
                    true)
                , (Argument[] arguments) => {
                    var name = arguments[0].Value.Eval().AsString();
                    var baseDir = Directory.GetCurrentDirectory();
                    if (API.ImportManager.SpecificDirectory is not null)
                    {
                        baseDir = API.ImportManager.SpecificDirectory;
                    }
                    string[] result;
                    string[] files;
                    if (API.ImportManager.PreLoadedFiles.ContainsKey(name))
                    {
                        files = new string[]
                        {
                            name
                        };
                        result = API.ImportManager.PreLoadedFiles[name];
                    } else
                    {
                        files = Directory.GetFileSystemEntries(baseDir, name);
                        result = File.ReadAllLines(files[0]);
                    }

                    var lexer = new Lexer.Lexer(result);
                    var parser = new Parser(lexer.Run(), files[0]);
                    var interpreter = new Interpreter(parser.Run());
                    interpreter.Run(false); 
                    return new NoneValue();
                })
            },
            { "pgm_resize", new FunctionInstance(
                new FunctionDeclarateExpression(null, -1, -1, string.Empty, "pgm_resize", new BlockStatementExpression(null, 0, 0, string.Empty, null), new Argument[]
                {
                    new("array", TypesType.Array),
                    new("size", TypesType.Number)
                },
                    false,
                    true), (Argument[] arguments) =>
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
                )
            },
            { "pgm_size", new FunctionInstance(
                new FunctionDeclarateExpression(null, -1, -1, string.Empty, "pgm_size", new BlockStatementExpression(null, 0, 0, string.Empty, null), new Argument[]
                {
                    new("array", TypesType.Array)
                },
                    false,
                    true), (Argument[] arguments) =>
                    {
                        return new NumberValue((arguments[0].Value.Eval() as ArrayValue).Elements.Length);
                    }
                )
            },
            { "print", new FunctionInstance(
                new FunctionDeclarateExpression(null, -1, -1, string.Empty, "print", new BlockStatementExpression(null, 0, 0, string.Empty, null), new Argument[]
                {
                    new("content", TypesType.String)
                }, false, true), (Argument[] arguments) =>
                    {
                        Console.WriteLine(arguments[0].Value.Eval().AsString());
                        return new NoneValue();
                    }
                ) 
            },
            { "println", new FunctionInstance(
                new FunctionDeclarateExpression(null, -1, -1, string.Empty, "println", new BlockStatementExpression(null, 0, 0, string.Empty, null), new Argument[]
                {
                    new("content", TypesType.String)
                }, false, true), (Argument[] arguments) =>
                    {
                        Console.Write(arguments[0].Value.Eval().AsString());
                        return new NoneValue();
                    }
                )
            },
            { "read", new FunctionInstance(
                new FunctionDeclarateExpression(null, -1, -1, string.Empty, "read", new BlockStatementExpression(null, 0, 0, string.Empty, null), new Argument[]
                {
                    new("content", TypesType.String)
                }, false, true), (Argument[] arguments) =>
                    {
                        return Value.Create(Console.ReadLine());
                    }
                )
            },
            {
                "millitime", new FunctionInstance(
                new FunctionDeclarateExpression(null, -1, -1, string.Empty, "millitime", new BlockStatementExpression(null, 0, 0, string.Empty, null), new Argument[]{}, false, true), (Argument[] arguments) =>
                    {
                        return Value.Create(DateTimeOffset.Now.ToUnixTimeMilliseconds());
                    }
                )
            }
        };
    }
}
