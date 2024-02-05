using Paganism.Exceptions;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using System;

namespace Paganism.PParser.Values
{
    public abstract class Value : EvaluableExpression
    {
        protected Value() : base(null, 0, 0, string.Empty)
        {
        }

        public abstract string Name { get; }

        public abstract TypesType Type { get; }

        public static Value Create(Expression expression)
        {
            return expression switch
            {
                StringExpression stringExpression => new StringValue(stringExpression.Value),
                NumberExpression numberExpression => new NumberValue(numberExpression.Value),
                BooleanExpression booleanExpression => new BooleanValue(booleanExpression.Value),
                CharExpression charExpression => new CharValue(charExpression.Value),
                VariableExpression variableExpression => Create(variableExpression),
                StructureDeclarateExpression structureDeclarateExpression => new StructureValue(structureDeclarateExpression.Parent, structureDeclarateExpression.Name),
                FunctionDeclarateExpression functionDeclarateExpression => new FunctionValue(functionDeclarateExpression),
                TypeExpression typeExpression => new TypeValue(typeExpression.Value, typeExpression.StructureName),
                _ => new NoneValue(),
            };
        }

        public static Value Create(object value)
        {
            if (value == null)
            {
                return new NoneValue();
            }

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
            return !structure.Values.ContainsKey(variable.Name)
                ? throw new InterpreterException($"Unknown structure member with {variable.Name} name, in structure with {structure.Structure.Name} name")
                : structure.Values[variable.Name];
        }

        public static Value Create(Value copy)
        {
            switch (copy)
            {
                case StringValue stringValue:
                    return new StringValue(stringValue.Value);
                case NumberValue numberValue:
                    return new NumberValue(numberValue.Value);
                case BooleanValue booleanValue:
                    return new BooleanValue(booleanValue.Value);
                case FunctionValue functionValue:
                    return new FunctionValue(functionValue.Value);
                case ArrayValue arrayValue:
                    return new ArrayValue(arrayValue.Elements);
                case NoneValue:
                    return new NoneValue();
                case EnumValue enumValue:
                    return new EnumValue(enumValue.Member);
                case StructureValue structureValue:
                    return new StructureValue(structureValue.BlockStatement, structureValue.Structure);
                case CharValue charValue:
                    return new CharValue(charValue.Value);
                case TypeValue typeValue:
                    return new TypeValue(typeValue.Value, typeValue.TypeName);
            }

            return null;
        }

        public bool IsType(Value value)
        {
            if (value.Type is TypesType.Any)
            {
                return true;
            }

            if (value.Type != Type && value.Type != TypesType.Type)
            {
                return false;
            }

            if (value is StructureValue structureValue)
            {
                return structureValue.Equals(this);
            }

            if (value is EnumValue enumValue)
            {
                return enumValue.Equals(this);
            }

            if (value is TypeValue typeValue)
            {
                return typeValue.Equals(this);
            }

            if (value is FunctionValue functionValue)
            {
                return functionValue.Equals(this);
            }

            return true;
        }

        public bool Is(TypesType type, string typeName)
        {
            if (this is NoneValue)
            {
                return true;
            }

            if (this is FunctionValue functionValue)
            {
                return (functionValue.Value.ReturnType is null && type is TypesType.None)
                    ||
                    functionValue.Value.ReturnType.Value == type && functionValue.Value.ReturnType.TypeName == typeName;
            }

            if (this is StructureValue structureValue)
            {
                return structureValue.Structure.Name == typeName;
            }

            if (this is EnumValue enumValue)
            {
                return enumValue.Member.Enum == typeName;
            }

            return Type == type;
        }

        public override Value Eval(params Argument[] arguments)
        {
            return this;
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
