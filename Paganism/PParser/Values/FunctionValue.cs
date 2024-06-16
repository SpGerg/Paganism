using Paganism.Interpreter.Data.Instances;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.Values.Interfaces;
using System;

namespace Paganism.PParser.Values
{
    public class FunctionValue : Value, ISettable
    {
        public FunctionValue(ExpressionInfo info, FunctionDeclarateExpression value, Func<Argument[], Value> func = null) : base(info)
        {
            Value = value;
            Func = func;

            _functionTypeValue = new FunctionTypeValue(ExpressionInfo, Value, Value.IsAsync);
        }

        public FunctionValue(ExpressionInfo info, string name, Argument[] arguments, TypeValue returnType, Func<Argument[], Value> func)
            : this(info,
                  new FunctionDeclarateExpression(
                      ExpressionInfo.EmptyInfo, name, null, arguments, false,
                      new InstanceInfo(true, false, info.Filepath),
                      returnType),
                  func)
        {
        }

        public override string Name => "Function";

        public override TypesType Type => TypesType.Function;

        public override TypesType[] CanCastTypes { get; } = new[]
        {
            TypesType.String
        };

        public FunctionDeclarateExpression Value { get; private set; }

        public Func<Argument[], Value> Func { get; private set; }

        private FunctionTypeValue _functionTypeValue;

        public bool CheckArguments(Argument[] arguments)
        {
            for (var i = 0; i < arguments.Length; i++)
            {
                var argument = Value.Arguments[i];

                if (i > Value.Arguments.Length - 1)
                {
                    return false;
                }

                if (!argument.Is(arguments[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override string AsString()
        {
            if (Value is null)
            {
                return Name + " (Function)";
            }

            var result = $"{Name} (Function): ";
            result += "[";

            foreach (var argument in Value.Arguments)
            {
                result += $"[{argument.Name}: {argument.Type}], ";
            }

            result += "]";

            return result;
        }

        public override bool Is(TypeValue typeValue)
        {
            if (typeValue is FunctionTypeValue functionType)
            {
                return Value.ReturnType.Is(typeValue) && CheckArguments(functionType.Arguments);
            }

            return Value.ReturnType.Is(typeValue);
        }

        public override bool Is(Value value)
        {
            if (value is FunctionValue functionValue)
            {
                return Is(functionValue._functionTypeValue);
            }

            if (value is FunctionTypeValue functionType)
            {
                return functionType.Is(_functionTypeValue);
            }

            return false;
        }

        public void Set(Value value)
        {
            if (value is not FunctionValue functionValue)
            {
                return;
            }

            Value = functionValue.Value;
            Func = functionValue.Func;
        }
    }
}
