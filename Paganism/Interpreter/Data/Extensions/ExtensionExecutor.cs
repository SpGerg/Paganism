using Paganism.PParser;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Hosting;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.Interpreter.Data.Extensions
{
    internal class ExtensionExecutor
    {
        public ExtensionExecutor(TypesType type, string name, Argument[] arguments, Func<VariableExpression, Argument[], Value> action)
        {
            Type = type;
            Name = name;
            Arguments = arguments;
            Action = action;
        }

        public TypesType Type { get; }
        
        public string Name { get; }

        public Argument[] Arguments { get; }

        public Func<VariableExpression, Argument[], Value> Action { get; }

        public static Dictionary<string, Argument> FixArguments(Argument[] ArgumentList)
        {
            Dictionary<string, Argument> Data = new();

            foreach (Argument Argument in ArgumentList)
            {
                if (Data.ContainsKey(Argument.Name)) {
                    Data[Argument.Name] = Argument;
                    continue;
                }
                Data.Add(Argument.Name, Argument);
            }

            return Data;
        }
    }
}
