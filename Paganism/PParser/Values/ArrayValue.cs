using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.Values.Interfaces;

namespace Paganism.PParser.Values
{
    public class ArrayValue : Value, ISettable
    {
        public ArrayValue(ExpressionInfo info, Value[] elements) : base(info)
        {
            Value = elements;
        }

        public ArrayValue(ExpressionInfo info, Expression[] elements) : base(info)
        {
            Value[] expressions = new Value[elements.Length];

            for (int i = 0; i < expressions.Length; i++)
            {
                expressions[i] = (elements[i] as EvaluableExpression).Evaluate();
            }

            Value = expressions;
        }

        public override string Name => "Array";

        public override TypesType Type => TypesType.Array;

        public Value[] Value { get; private set; }

        public override TypesType[] CanCastTypes { get; } = new TypesType[0];

        public override string AsString()
        {
            var result = $"{Name}: ";
            result += "[";

            foreach (var item in Value)
            {
                result += $"{item.AsString()}, ";
            }

            result += "]";

            return result;
        }

        public override bool Is(TypeValue typeValue)
        {
            return Type == typeValue.Value;
        }

        public override bool Is(Value value)
        {
            if (value is not ArrayValue arrayValue)
            {
                return false;
            }

            if (arrayValue.Value.Length != Value.Length)
            {
                return false;
            }

            for (var i = 0; i < Value.Length; i++)
            {
                var element = Value[i];

                if (!element.Is(arrayValue.Value[i]))
                {
                    return false;
                }
            }

            return true;
        }


        public void Set(int index, object value)
        {
            if (value is Value objectValue)
            {
                Value[index] = objectValue;
                return;
            }

            Value[index] = value as Value;
        }

        public void Set(Value value)
        {
            if (value is not ArrayValue arrayValue)
            {
                return;
            }

            Value = arrayValue.Value;
        }
    }
}
