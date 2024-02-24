using Paganism.PParser.AST.Enums;
using System.Collections.Generic;
using Paganism.PParser;
using Paganism.PParser.AST;
using Paganism.PParser.Values;
using Paganism.Exceptions;

#nullable enable
namespace Paganism.Interpreter.Data.Extensions
{
    internal class Extension
    {
        public static List<string> AllowedExtensions { get; set; } = new()
        {
            "StringExtension"
        };
        public static Dictionary<TypesType, string> TypeExtensionAssociation { get; } = new()
        {
            { TypesType.String, "StringExtension" },
            { TypesType.Char, "StringExtension" },
            { TypesType.Number, "NumberExtension" },
            { TypesType.Array, "ArrayExtension" }
        };
        public static Dictionary<string, dynamic> StringExtension { get; set; } = new()
        {
            { "Replace", new ExtensionExecutor(TypesType.String, "Replace", new Argument[]
                {
                    new("original", TypesType.String),
                    new("replace", TypesType.String)
                }, (VariableExpression Original, Argument[] Arguments) =>
                {
                    if (!Variables.Instance.Value.TryGet(Original.Parent, Original.Name, out Value result))
                    {
                        throw new InterpreterException($"Variable {Original.Name} cannot be null while using Variable Extensions!");

                    } 
                    else
                    {
                        if (result is null)
                        {
                            throw new InterpreterException($"Variable {Original.Name} cannot be null while using Variable Extensions!");
                        } 
                        else
                        {
                            return Value.Create(result.AsString().Replace(Arguments[0].Value.Eval().AsString(), Arguments[1].Value.Eval().AsString()));
                        }
                    }
                })
            },
            { "Split", new ExtensionExecutor(TypesType.String, "Split", new Argument[]
                {
                    new("char", TypesType.Char)
                }, (VariableExpression Original, Argument[] Arguments) =>
                {
                    if (!Variables.Instance.Value.TryGet(Original.Parent, Original.Name, out Value result))
                    {
                        throw new InterpreterException($"Variable {Original.Name} cannot be null while using Variable Extensions!");

                    } 
                    else
                    {
                        if (result is null)
                        {
                            throw new InterpreterException($"Variable {Original.Name} cannot be null while using Variable Extensions!");
                        } 
                        else
                        {
                            return Value.Create(result.AsString().Split(char.Parse(Arguments[0].Value.Eval().AsString())));
                        }
                    }
                })
            }
        };

        public static Dictionary<string, dynamic> ArrayExtension { get; set; } = new();

        public static dynamic? Get(Dictionary<string, dynamic> Base, string Name)
        {
            return Base[Name] ?? null;
        }

        public static bool TryGet(Dictionary<string, dynamic> Base, string Name, out dynamic? executor)
        {
            executor = Get(Base, Name);

            if (executor is null)
            {
                executor = null;
                return false;
            }

            return true;
        }
    }
}
