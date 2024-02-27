using Paganism.Interpreter.Data;
using Paganism.Interpreter.Data.Instances;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class StructureDeclarateExpression : EvaluableExpression, IStatement, IDeclaratable
    {
        public StructureDeclarateExpression(BlockStatementExpression parent, int line, int position, string filepath, string name, StructureMemberExpression[] members) : base(parent, line, position, filepath)
        {
            Name = name;
            Members = members;
        }

        public string Name { get; }

        public StructureMemberExpression[] Members { get; }

        public void Declarate()
        {
            Interpreter.Data.Structures.Instance.Value.Set(Parent, Name, new StructureInstance(this));
        }

        public void Remove()
        {
            Interpreter.Data.Structures.Instance.Value.Remove(Parent, Name);
        }

        public override Value Eval(params Argument[] arguments)
        {
            return new StructureValue(Parent, new StructureInstance(this));
        }
    }
}
