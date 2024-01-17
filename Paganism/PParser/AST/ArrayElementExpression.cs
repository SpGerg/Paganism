using Paganism.Exceptions;
using Paganism.Interpreter.Data;
using Paganism.PParser.Values;
using System.Collections.Generic;

namespace Paganism.PParser.AST
{
    public class ArrayElementExpression : EvaluableExpression
    {
        public ArrayElementExpression(BlockStatementExpression parent, int line, int position, string filepath, string name, EvaluableExpression index, ArrayElementExpression left = null) : base(parent, line, position, filepath)
        {
            Name = name;
            Index = index;
            Left = left;
        }

        public string Name { get; }

        public ArrayElementExpression Left { get; }

        public EvaluableExpression Index { get; }

        public override Value Eval(params Argument[] arguments)
        {
            return EvalWithKey().Value;
        }

        public KeyValuePair<int, Value> EvalWithKey()
        {
            Value variable = Variables.Instance.Value.Get(Parent, Name);

            if (variable is ArrayValue arrayValue)
            {
                double value = Index.Eval().AsNumber();

                if (value < 0)
                {
                    throw new InterpreterException($"Index must be a non-negative, in array variable with {Name} name");
                }

                if (arrayValue.Elements.Length - 1 < value && Left.Eval() is ArrayValue)
                {
                    throw new InterpreterException($"Index out of range, in array variable with {Name} name");
                }

                //Breaking bad...
                if (Left is not null)
                {
                    Value left = Left.Eval();

                    return left is not ArrayValue arrayValue1
                        ? left is StringValue stringValue
                            ? (int)value > stringValue.Value.Length - 1
                                ? throw new InterpreterException($"Index out of range, in array variable with {Name} name")
                                : new KeyValuePair<int, Value>((int)value, new CharValue(stringValue.Value[(int)value]))
                            : new KeyValuePair<int, Value>((int)value, left)
                        : new KeyValuePair<int, Value>((int)value, arrayValue1.Elements[(int)value]);
                }

                return new KeyValuePair<int, Value>((int)value, arrayValue.Elements[(int)value]);
            }

            throw new InterpreterException($"Variable must be array, in array variable with {Name} name");
        }
    }
}
