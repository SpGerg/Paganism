using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;

namespace Paganism.PParser.Values
{
    public class ArrayValue : Value
    {
        public ArrayValue(Value[] elements)
        {
            Elements = elements;
        }

        public ArrayValue(Expression[] elements)
        {
            Value[] expressions = new Value[elements.Length];

            for (int i = 0; i < expressions.Length; i++)
            {
                expressions[i] = (elements[i] as EvaluableExpression).Eval();
            }

            Elements = expressions;
        }

        public override string Name => "Array";

        public override TypesType Type => TypesType.Array;

        public Value[] Elements { get; set; }

        public override TypesType[] CanCastTypes { get; } = new TypesType[0];

        public override void Set(object value)
        {
            if (value is ArrayValue arrayValue)
            {
                Elements = arrayValue.Elements;
            }
        }

        public void Set(int index, object value)
        {
            if (value is Value objectValue)
            {
                Elements[index] = objectValue;
                return;
            }

            Elements[index] = value as Value;
        }

        public override string AsString()
        {
            var result = $"{Name}: ";
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
