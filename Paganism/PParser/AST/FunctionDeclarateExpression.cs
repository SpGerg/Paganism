using Paganism.Exceptions;
using Paganism.Interpreter;
using Paganism.Interpreter.Data;
using Paganism.Lexer.Enums;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class FunctionDeclarateExpression : Expression, IStatement, IExecutable
    {
        public FunctionDeclarateExpression(string name, BlockStatementExpression statement, Argument[] requiredArguments, params TokenType[] returnTypes)
        {
            Name = name;
            Statement = statement;
            RequiredArguments = requiredArguments;
            ReturnTypes = returnTypes;

            if (Statement == null || Statement.Statements == null) return;

            if (ReturnTypes.Length > 0 && Statement.Statements.FirstOrDefault(statementInBlock => statementInBlock is ReturnExpression) == default)
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

        public Argument[] RequiredArguments { get; }

        public TokenType[] ReturnTypes { get; }

        public string DeclarateFilePath { get; }

        private static Dictionary<string, Type> Types = new Dictionary<string, Type>()
        {
            { "System.Console", typeof(Console) }
        };

        public void Create()
        {
            Functions.Add(this);
        }

        public void Remove()
        {
            Functions.Remove(Name);
        }

        public Value ExecuteAndReturn(params Argument[] arguments)
        {
            if (Name == "pgm_call")
            {
                Type findedClass = null;

                if (!Types.TryGetValue(arguments[0].Value.Eval().AsString(), out findedClass))
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

                    Variables.DeclaratedVariables.Count();

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
                    else if (paramater is BooleanValue)
                    {
                        method = findedClass.GetMethod(arguments[1].Value.Eval().AsString(), new Type[] { typeof(bool) });
                    }
                    else if (paramater is NumberValue)
                    {
                        method = findedClass.GetMethod(arguments[1].Value.Eval().AsString(), new Type[] { typeof(int) });
                    }

                    if (method.GetParameters()[0].ParameterType == typeof(string) || method.GetParameters()[0].ParameterType == typeof(object))
                    {
                        return Value.Create(method.Invoke(null, new object[] { arguments[2].Value.Eval().AsString() }));
                    }
                    else if (method.GetParameters()[0].ParameterType == typeof(bool))
                    {
                        return Value.Create(method.Invoke(null, new object[] { arguments[2].Value.Eval().AsBoolean() }));
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
                return new StructureValue(arguments[0].Value.Eval().AsString());
            }
            else if (Name == "pgm_size")
            {
                return new NumberValue((arguments[0].Value.Eval() as ArrayValue).Elements.Length);
            }
            else if (Name == "pgm_resize")
            {
                var array = arguments[0].Value.Eval() as ArrayValue;

                var newElements = new Value[(int)arguments[1].Value.Eval().AsNumber()];

                for (int i = 0;i < newElements.Length;i++)
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

            if (Statement == null) return new NoneValue();

            return Statement.ExecuteAndReturn(arguments);
        }

        public void Execute(params Argument[] arguments)
        {
            ExecuteAndReturn(arguments);
        }
    }
}
