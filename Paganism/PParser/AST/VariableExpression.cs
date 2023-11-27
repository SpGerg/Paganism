using Paganism.Exceptions;
using Paganism.Interpreter.Data;
using Paganism.Lexer.Enums;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class VariableExpression : Expression, IStatement, IEvaluable
    {
        public VariableExpression(string name, TypesType type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }

        public TypesType Type { get; }

        public Value Eval()
        {
            var variable = Variables.Get(Name);

            if (variable is NoneValue)
            {
                return variable;
            }

            if (variable is not StructureValue)
            {
                return Value.Create(variable);
            }

            return variable;
        }
    }
}
