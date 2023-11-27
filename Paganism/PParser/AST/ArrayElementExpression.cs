using Paganism.Exceptions;
using Paganism.Interpreter.Data;
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
        public ArrayElementExpression(string name, IEvaluable index, ArrayElementExpression left = null)
        {
            Name = name;
            Index = index;
            Left = left;
        }

        public string Name { get; }

        public ArrayElementExpression Left { get; }

        public IEvaluable Index { get; }

        public Value Eval()
        {
            return EvalWithKey().Value;
        }

        public KeyValuePair<int, Value> EvalWithKey()
        {
            var variable = Variables.Get(Name);

            if (variable is ArrayValue arrayValue)
            {
                var value = Index.Eval().AsNumber();

                if (value < 0)
                {
                    throw new InterpreterException($"Index must be a non-negative, in array variable with {Name} name");
                }

                if (arrayValue.Elements.Length < value)
                {
                    throw new InterpreterException($"Index out of range, in array variable with {Name} name");
                }

                //Breaking bad...
                if (Left != null)
                {
                    var left = Left.Eval();

                    if (left is not ArrayValue arrayValue1)
                    {
                        if (left is StringValue stringValue)
                        {
                            if ((int)value > stringValue.Value.Length - 1)
                            {
                                throw new InterpreterException($"Index out of range, in array variable with {Name} name");
                            }

                            return new KeyValuePair<int, Value>((int)value, new CharValue(stringValue.Value[(int)value]));
                        }

                        return new KeyValuePair<int, Value>((int)value, left);
                    }

                    return new KeyValuePair<int, Value>((int)value, arrayValue1.Elements[(int)value]);
                }

                return new KeyValuePair<int, Value>((int)value, arrayValue.Elements[(int)value]);
            }

            throw new InterpreterException($"Variable must be array, in array variable with {Name} name");
        }
    }
}
