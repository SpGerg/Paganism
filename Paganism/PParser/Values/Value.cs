using Paganism.API;
using Paganism.Exceptions;
using Paganism.Interpreter.Data.Instances;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using System;
using System.Linq;

namespace Paganism.PParser.Values
{
    public abstract class Value : EvaluableExpression
    {
        public Value(ExpressionInfo info) : base(info)
        {
        }

        public Value() : base(ExpressionInfo.EmptyInfo) { }

        public abstract string Name { get; }

        public abstract TypesType Type { get; }

        public virtual TypesType[] CanCastTypes { get; } = new TypesType[0];

        public abstract bool Is(TypeValue typeValue);

        public abstract bool Is(Value value);

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

        public static Value Create(object value, ExpressionInfo? expressionInfo = null)
        {
            var expressionInfo1 = expressionInfo is null ? ExpressionInfo.EmptyInfo : (ExpressionInfo) expressionInfo;

            if (value is string)
            {
                return new StringValue(expressionInfo1, Convert.ToString(value));
            }

            if (value is double or int or float)
            {
                return new NumberValue(expressionInfo1, Convert.ToDouble(value));
            }

            if (value is bool)
            {
                return new BooleanValue(expressionInfo1, Convert.ToBoolean(value));
            }

            if (value is not Type type || (!type.IsValueType && type != typeof(string)) || (type.IsClass && type != typeof(string)) || type.IsEnum || PaganismFromCSharp.IsStructure(type))
            {
                return new NoneValue(expressionInfo1);
            }

            if (Interpreter.Data.Structures.Instance.TryGet(null, type.Name, expressionInfo1, out var structure))
            {
                return new StructureValue(structure.StructureDeclarateExpression.ExpressionInfo, structure);
            }

            return new NoneValue(expressionInfo1);
        }

        public static Value Create(StructureValue structure, VariableExpression variable)
        {
            return !structure.Values.ContainsKey(variable.Name)
                ? throw new InterpreterException($"Unknown structure member with name: {variable.Name}, in structure with name: {structure.Structure.Name}", variable.ExpressionInfo)
                : structure.Values[variable.Name];
        }

        public static Value Create(Value copy)
        {
            return copy switch
            {
                StringValue stringValue => new StringValue(copy.ExpressionInfo, stringValue.Value),
                NumberValue numberValue => new NumberValue(copy.ExpressionInfo, numberValue.Value),
                BooleanValue booleanValue => new BooleanValue(copy.ExpressionInfo, booleanValue.Value),
                FunctionValue functionValue => new FunctionValue(copy.ExpressionInfo, functionValue.Value, functionValue.Func),
                ArrayValue arrayValue => new ArrayValue(copy.ExpressionInfo, arrayValue.Value),
                NoneValue noneValue => new NoneValue(noneValue.ExpressionInfo),
                EnumValue enumValue => new EnumValue(copy.ExpressionInfo, enumValue.Value),
                StructureValue structureValue => new StructureValue(copy.ExpressionInfo, structureValue.Structure),
                CharValue charValue => new CharValue(copy.ExpressionInfo, charValue.Value),
                FunctionTypeValue functionTypeValue => new FunctionTypeValue(copy.ExpressionInfo, functionTypeValue.Value, functionTypeValue.TypeName, functionTypeValue.Arguments, functionTypeValue.IsAsync),
                TypeValue typeValue => new TypeValue(copy.ExpressionInfo, typeValue.Value, typeValue.TypeName),
                ObjectValue objectValue => new ObjectValue(copy.ExpressionInfo, objectValue.Value),
                _ => new VoidValue(copy.ExpressionInfo),
            };
        }

        public string GetTypeName()
        {
            if (this is StructureValue structureValue)
            {
                return structureValue.Structure.Name;
            }

            if (this is EnumValue enumValue)
            {
                return enumValue.Value.Enum;
            }

            return string.Empty;
        }

        public override Value Evaluate(params Argument[] arguments)
        {
            return this;
        }

        public bool IsCanCast(Value value)
        {
            if (value is TypeValue typeValue)
            {
                return typeValue.Value is TypesType.Any;
            }

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

        public virtual double AsNumber()
        {
            throw new InterpreterException($"You cant cast a {Name} to a Number", ExpressionInfo);
        }

        public virtual bool AsBoolean()
        {
            throw new InterpreterException($"You cant cast a {Name} to a Boolean", ExpressionInfo);
        }

        public virtual string AsString()
        {
            throw new InterpreterException($"You cant cast a {Name} to a String", ExpressionInfo);
        }
    }
}
