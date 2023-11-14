using Paganism.Lexer.Enums;
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
        public VariableExpression(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public Value Eval()
        {
            return Variables.Get(Name);
        }
    }
}
