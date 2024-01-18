using Paganism.Interpreter.Data;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class VariableExpression : EvaluableExpression, IStatement
    {
        public VariableExpression(BlockStatementExpression parent, int line, int position, string filepath, string name, TypeValue type) : base(parent, line, position, filepath)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }

        public TypeValue Type { get; }

        public override Value Eval(params Argument[] arguments)
        {
            var variable = Variables.Instance.Value.Get(Parent, Name);

            if (variable is null)
            {
                return new NoneValue();
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
