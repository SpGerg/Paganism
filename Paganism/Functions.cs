using Paganism.PParser.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism
{
    public static class Functions
    {
        private static Dictionary<string, FunctionDeclarateExpression> DeclaratedFunctions { get; } = new Dictionary<string, FunctionDeclarateExpression>();

        static Functions()
        {
            DeclaratedFunctions.Add("call_lang", new FunctionDeclarateExpression("call_lang", new BlockStatementExpression(null), new PParser.Argument[] {
                new PParser.Argument("class_namespace", Lexer.Enums.TokenType.String, true),
                new PParser.Argument("method_name", Lexer.Enums.TokenType.String, true),
                new PParser.Argument("arguments", Lexer.Enums.TokenType.AnyType, true) }));
            DeclaratedFunctions.Add("create", new FunctionDeclarateExpression("create", new BlockStatementExpression(null), new PParser.Argument[] {
                new PParser.Argument("name", Lexer.Enums.TokenType.String, true) }
            ));
        }

        public static void Add(FunctionDeclarateExpression functionDeclarate)
        {
            DeclaratedFunctions.Add(functionDeclarate.Name, functionDeclarate);
        }

        public static void Remove(string name)
        {
            DeclaratedFunctions.Remove(name);
        }

        public static void Clear()
        {
            DeclaratedFunctions.Clear();
        }

        public static FunctionDeclarateExpression Get(string name)
        {
            if (!DeclaratedFunctions.TryGetValue(name, out FunctionDeclarateExpression result))
            {
                throw new Exception($"Function with {name} name not found");
            }

            return result;
        }
    }
}
