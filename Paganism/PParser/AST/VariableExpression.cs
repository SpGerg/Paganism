using Paganism.Interpreter.Data;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class VariableExpression : EvaluableExpression, IStatement
    {
        public VariableExpression(ExpressionInfo info, string name, TypeValue type) : base(info)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }

        public TypeValue Type { get; }

        public override Value Eval(params Argument[] arguments)
        {
            var variable = Variables.Instance.Value.Get(ExpressionInfo.Parent, Name);

            if (variable is null)
            {
                return new VoidValue(new ExpressionInfo());
            }

            if (variable is NoneValue)
            {
                return variable;
            }

            if (variable is not StructureValue)
            {
                return Value.Create(variable);
            }

            return variable;
        }
    }
}
