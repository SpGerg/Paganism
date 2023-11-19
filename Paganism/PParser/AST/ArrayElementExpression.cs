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
    public class ArrayElementExpression : Expression, IEvaluable
    {
        public ArrayElementExpression(string name, IEvaluable index)
        {
            Name = name;
            Index = index;
        }

        public string Name { get; }

        public IEvaluable Index { get; }

        public Value Eval()
        {
            var variable = Variables.Get(Name);

            if (variable is ArrayValue arrayValue)
            {
                var value = Index.Eval().AsNumber();

                if (value < 0)
                {
                    throw new Exception($"Index must be a non-negative, in array variable with {Name} name");
                }

                if (arrayValue.Elements.Length < value)
                {
                    throw new Exception($"Index out of range, in array variable with {Name} name");
                }

                return arrayValue.Elements[(int)value];
            }

            throw new Exception($"Variable must be array, in array variable with {Name} name");
        }
    }
}
