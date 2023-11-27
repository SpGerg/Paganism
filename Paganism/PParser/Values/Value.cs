using Paganism.Exceptions;
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

        public abstract TypesType Type { get; }

        public static Value Create(Expression expression)
        {
            switch (expression)
            {
                case StringExpression stringExpression:
                    return new StringValue(stringExpression.Value);
                case NumberExpression numberExpression:
                    return new NumberValue(numberExpression.Value);
                case BooleanExpression booleanExpression:
                    return new BooleanValue(booleanExpression.Value);
                case TypeExpression typeExpression:
                    return new TypeValue(typeExpression.Value);
            }

            return new NoneValue();
        }

        public static Value Create(object value)
        {
            if (value == null) return new NoneValue();

            if (value.GetType() == typeof(string))
            {
                return new StringValue((string)value);
            }
            else if (value.GetType() == typeof(ConsoleKeyInfo))
            {
                return new CharValue(((ConsoleKeyInfo)value).KeyChar);
            }
            else if (value.GetType() == typeof(char))
            {
                return new CharValue((char)value);
            }
            else if (value.GetType() == typeof(int))
            {
                return new NumberValue((int)value);
            }
            else if (value.GetType() == typeof(bool))
            {
                return new BooleanValue((bool)value);
            }

            return new NoneValue();
        }

        public static Value Create(StructureValue structure, VariableExpression variable)
        {
            if (!structure.Values.ContainsKey(variable.Name))
            {
                throw new InterpreterException($"Unknown structure member with {variable.Name} name, in structure with {structure.Structure.Name} name");
            }

            return structure.Values[variable.Name];
        }

        public static Value Create(Value copy)
        {
            switch (copy.Type)
            {
                case TypesType.String:
                    return new StringValue((copy as StringValue).Value);
                case TypesType.Number:
                    return new NumberValue((copy as NumberValue).Value);
                case TypesType.Boolean:
                    return new BooleanValue((copy as BooleanValue).Value);
                case TypesType.Array:
                    var array = copy as ArrayValue;
                    return new ArrayValue(array.Elements);
                case TypesType.None:
                    return new NoneValue();
                case TypesType.Structure:
                    var structure = copy as StructureValue;
                    return new StructureValue(structure.Structure);
                case TypesType.Char:
                    return new CharValue((copy as CharValue).Value);
                case TypesType.Type:
                    return new TypeValue((copy as TypeValue).Value);
            }

            return null;
        }

        public virtual void Set(object value) { }

        public virtual double AsNumber()
        {
            throw new InterpreterException($"You cant cast {Name} to Number");
        }

        public virtual bool AsBoolean()
        {
            throw new InterpreterException($"You cant cast {Name} to Boolean");
        }

        public virtual string AsString()
        {
            throw new InterpreterException($"You cant cast {Name} to String");
        }

        public virtual string AsFunction()
        {
            throw new InterpreterException($"You cant cast {Name} to Function");
        }
    }
}
