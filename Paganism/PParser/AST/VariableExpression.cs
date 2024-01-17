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
    public class VariableExpression : EvaluableExpression, IStatement
    {
        public VariableExpression(BlockStatementExpression parent, int line, int position, string filepath, string name, TypesType type) : base(parent, line, position, filepath)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }

        public TypesType Type { get; }

        public override Value Eval(params Argument[] arguments)
        {
            var variable = Variables.Instance.Value.Get(Parent, Name);

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
