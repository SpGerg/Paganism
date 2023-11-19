using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.Values
{
    public abstract class Value
    {
        public abstract string Name { get; }

        public abstract StandartValueType Type { get; }

        public static Value Create(Expression expression)
        {
            if (expression is StringExpression stringExpression)
            {
                return new StringValue(stringExpression.Value);
            }
            else if (expression is NumberExpression numberExpression)
            {
                return new NumberValue(numberExpression.Value);
            }

            return null;
        }

        public static Value Create(Value copy)
        {
            switch (copy.Type)
            {
                case StandartValueType.String:
                    return new StringValue((copy as StringValue).Value);
                case StandartValueType.Number:
                    return new NumberValue((copy as NumberValue).Value);
                case StandartValueType.Boolean:
                    return new BooleanValue((copy as BooleanValue).Value);
                case StandartValueType.Array:
                    var array = copy as ArrayValue;
                    return new ArrayValue(array.Elements, array.ElementsType);
                case StandartValueType.Structure:
                    var structure = copy as StructureValue;
                    return new StructureValue(structure.StructureDeclarate);
            }

            return null;
        }

        public virtual double AsNumber()
        {
            throw new Exception($"You cant cast {Name} to Number");
        }

        public virtual bool AsBoolean()
        {
            throw new Exception($"You cant cast {Name} to Boolean");
        }

        public virtual string AsString()
        {
            throw new Exception($"You cant cast {Name} to String");
        }

        public virtual string AsFunction()
        {
            throw new Exception($"You cant cast {Name} to Function");
        }
    }
}
