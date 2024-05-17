using Paganism.Interpreter.Data;
using Paganism.Interpreter.Data.Instances;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class VariableExpression : EvaluableExpression, IStatement, IAccessible
    {
        public VariableExpression(InstanceInfo instanceInfo, ExpressionInfo info, string name, TypeValue type) : base(info)
        {
            Name = name;
            Type = type;
            Info = instanceInfo;
        }

        public string Name { get; }

        public TypeValue Type { get; }

        public InstanceInfo Info { get; }

        public override Value Evaluate(params Argument[] arguments)
        {
            if (!Variables.Instance.TryGet(ExpressionInfo.Parent, Name, ExpressionInfo, out var variableInstance))
            {
                return new VoidValue(ExpressionInfo.EmptyInfo);
            }

            var variable = variableInstance.Value;

            if (variable is NoneValue)
            {
                return variable;
            }

            if (variable is not StructureValue or FunctionValue or EnumValue)
            {
                return Value.Create(variable);
            }

            return variable;
        }

        public void Set(ExpressionInfo expressionInfo, Value value)
        {
            Variables.Instance.Set(expressionInfo, ExpressionInfo.Parent, Name, new VariableInstance(Info, value));
        }

        public void Remove()
        {
            Variables.Instance.Remove(ExpressionInfo.Parent, Name);
        }
    }
}
