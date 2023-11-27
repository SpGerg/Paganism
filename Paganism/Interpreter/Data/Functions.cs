using Paganism.Exceptions;
using Paganism.Interpreter.Data.Instances;
using Paganism.Lexer.Enums;
using Paganism.PParser;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.Interpreter.Data
{
    public static class Functions
    {
        private static Dictionary<string, FunctionInstance> DeclaratedFunctions { get; } = new Dictionary<string, FunctionInstance>();

        private static Dictionary<string, FunctionInstance> LanguageFunctions { get; } = new Dictionary<string, FunctionInstance>()
        {
            { "pgm_call", new FunctionInstance(
                new FunctionDeclarateExpression("pgm_call", null, new Argument[]
                {
                    new Argument("namespace", TypesType.String, true),
                    new Argument("method", TypesType.String, true),
                    new Argument("arguments", TypesType.Any, true)
                },
                    TokenType.AnyType)
                )
            },
            { "pgm_create", new FunctionInstance(
                new FunctionDeclarateExpression("pgm_create", null, new Argument[]
                {
                    new Argument("name", TypesType.String, true)
                },
                    TokenType.Structure)
                )
            },
            { "pgm_import", new FunctionInstance(
                new FunctionDeclarateExpression("pgm_import", null, new Argument[]
                {
                    new Argument("file", TypesType.String, true)
                },
                    TokenType.NoneType)
                )
            },
            { "pgm_resize", new FunctionInstance(
                new FunctionDeclarateExpression("pgm_resize", null, new Argument[]
                {
                    new Argument("array", TypesType.Array, true),
                    new Argument("size", TypesType.Number, true)
                },
                    TokenType.NoneType)
                )
            },
            { "pgm_size", new FunctionInstance(
                new FunctionDeclarateExpression("pgm_size", null, new Argument[]
                {
                    new Argument("array", TypesType.Array, true)
                },
                    TokenType.NoneType)
                )
            },
        };

        public static void Add(FunctionDeclarateExpression functionDeclarate)
        {
            DeclaratedFunctions.Add(functionDeclarate.Name, new FunctionInstance(functionDeclarate));
        }

        public static void Remove(string name)
        {
            DeclaratedFunctions.Remove(name);
        }

        public static void Clear()
        {
            DeclaratedFunctions.Clear();
        }

        public static FunctionInstance Get(string name)
        {
            if (!DeclaratedFunctions.TryGetValue(name, out FunctionInstance result) && !LanguageFunctions.TryGetValue(name, out result))
            {
                throw new InterpreterException($"Function with {name} name not found");
            }

            return result;
        }
    }
}
