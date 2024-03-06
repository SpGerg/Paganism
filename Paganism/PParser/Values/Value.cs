using Paganism.Exceptions;
using Paganism.Interpreter.Data.Instances;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Paganism.PParser.Values
{
    public abstract class Value : EvaluableExpression
    {
        public Value(ExpressionInfo info) : base(info)
        {
        }

        public Value() : base(new ExpressionInfo()) { }

        public abstract string Name { get; }

        public abstract TypesType Type { get; }

        public virtual TypesType[] CanCastTypes { get; } = new TypesType[0];

        public static Value Create(Expression expression)
        {
            return expression switch
            {
                VariableExpression variableExpression => Create(variableExpression),
                StructureDeclarateExpression structureDeclarateExpression => new StructureValue(structureDeclarateExpression.ExpressionInfo, new StructureInstance(structureDeclarateExpression)),
                FunctionDeclarateExpression functionDeclarateExpression => new FunctionValue(functionDeclarateExpression.ExpressionInfo, functionDeclarateExpression),
                _ => new NoneValue(expression.ExpressionInfo),
            };
        }

        public static Value Create(object value)
        {
            if (value is null)
            {
                return new NoneValue(new ExpressionInfo());
            }

            var @string = Convert.ToString(value);

            if (@string != string.Empty)
            {
                return new StringValue(new ExpressionInfo(), @string);
            }

            var @char = Convert.ToChar(value);

            if (@char is '\0')
            {
                return new CharValue(new ExpressionInfo(), @char);
            }

            var @double = Convert.ToDouble(value);

            if (@double is 0.0)
            {
                return new NumberValue(new ExpressionInfo(), @double);
            }

            var @bool = Convert.ToBoolean(value);

            return new BooleanValue(new ExpressionInfo(), @bool);
        }

        public static object CreateFromCSharp(Type type)
        {
            if (type.IsEnum)
            {
                return CreateEnum(type);
            }

            if (type.IsValueType)
            {
                if (IsStructure(type))
                {
                    return CreateClassOrStructure(type);
                }
                else
                {
                    return Create(type);
                }
            }

            return CreateClassOrStructure(type);
        }

        private static Instance CreateEnum(Type type)
        {
            var values = type.GetEnumValues();

            var members = new EnumMemberExpression[values.Length];

            System.Collections.IList list = values;
            for (int i = 0; i < list.Count; i++)
            {
                var value = list[i];

                members[i] = new EnumMemberExpression(new ExpressionInfo(), type.GetEnumName(value), new NumberValue(new ExpressionInfo(),
                    (Create(value) as NumberValue).Value), type.Name);
            }

            return new EnumInstance(new EnumDeclarateExpression(new ExpressionInfo(), type.Name, members, true));
        }

        private static Value CreateClassOrStructure(Type type)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            var members = new Dictionary<string, StructureMemberExpression>(fields.Length);

            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                var typeFromCSharp = CreateFromCSharp(field.FieldType) as Value;

                members.Add(field.Name, new StructureMemberExpression(new ExpressionInfo(),
                    type.Name, new TypeValue(new ExpressionInfo(), typeFromCSharp.Type, typeFromCSharp.GetTypeName()), field.Name));
            }

            return new StructureValue(new ExpressionInfo(), members);
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
                    return new StringValue(copy.ExpressionInfo, stringValue.Value);
                case NumberValue numberValue:
                    return new NumberValue(copy.ExpressionInfo, numberValue.Value);
                case BooleanValue booleanValue:
                    return new BooleanValue(copy.ExpressionInfo, booleanValue.Value);
                case FunctionValue functionValue:
                    return new FunctionValue(copy.ExpressionInfo, functionValue.Value);
                case ArrayValue arrayValue:
                    return new ArrayValue(copy.ExpressionInfo, arrayValue.Elements);
                case NoneValue noneValue:
                    return new NoneValue(noneValue.ExpressionInfo);
                case EnumValue enumValue:
                    return new EnumValue(copy.ExpressionInfo, enumValue.Member);
                case StructureValue structureValue:
                    return new StructureValue(copy.ExpressionInfo, structureValue.Structure);
                case CharValue charValue:
                    return new CharValue(copy.ExpressionInfo, charValue.Value);
                case TypeValue typeValue:
                    return new TypeValue(copy.ExpressionInfo, typeValue.Value, typeValue.TypeName);
            }

            return new VoidValue(new ExpressionInfo());
        }

        private static bool IsStructure(Type source)
        {
            return source.IsValueType && !source.IsPrimitive && !source.IsEnum;
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

        public string GetTypeName()
        {
            if (this is StructureValue structureValue)
            {
                return structureValue.Structure.Name;
            }

            if (this is EnumValue enumValue)
            {
                return enumValue.Member.Enum;
            }

            return string.Empty;
        }

        public override Value Eval(params Argument[] arguments)
        {
            return this;
        }

        public bool IsCanCast(Value value)
        {
            return value.CanCastTypes.Contains(Type);
        }

        public T Cast<T>(TypesType type) where T : Value, new()
        {
            switch (type)
            {
                case TypesType.Number:
                    return new NumberValue(ExpressionInfo, AsNumber()) as T;
                case TypesType.Boolean:
                    return new BooleanValue(ExpressionInfo, AsBoolean()) as T;
                case TypesType.String:
                    return new StringValue(ExpressionInfo, AsString()) as T;
            }

            return this as T;
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
    }
}
