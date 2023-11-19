using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.AST.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.Values
{
    public class ArrayValue : Value
    {
        public override string Name => "Array";

        public override StandartValueType Type => StandartValueType.Array;

        public StandartValueType ElementsType { get; }

        public Value[] Elements { get; }

        public ArrayValue(Value[] elements, StandartValueType elementsType)
        {
            Elements = elements;
            ElementsType = elementsType;
        }

        public ArrayValue(Expression[] elements, StandartValueType elementsType)
        {
            Value[] expressions = new Value[elements.Length];

            for (int i = 0;i < expressions.Length;i++)
            {
                expressions[i] = (elements[i] as IEvaluable).Eval();
            }

            Elements = expressions;
            ElementsType = elementsType;
        }

        public override string AsString()
        {
            var result = string.Empty;
            result += "[";

            foreach (var item in Elements)
            {
                result += $"{item.AsString()}, ";
            }

            result += "]";

            return result;
        }
    }
}
