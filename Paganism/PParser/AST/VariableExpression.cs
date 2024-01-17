using Paganism.Interpreter.Data;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class VariableExpression : EvaluableExpression, IStatement
    {
        public VariableExpression(BlockStatementExpression parent, int line, int position, string filepath, string name, TypesType type) : base(parent, line, position, filepath)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }

        public TypesType Type { get; }

        public override Value Eval(params Argument[] arguments)
        {
            Value variable = Variables.Instance.Value.Get(Parent, Name);

            return variable is NoneValue ? variable : variable is not StructureValue ? Value.Create(variable) : variable;
        }
    }
}
