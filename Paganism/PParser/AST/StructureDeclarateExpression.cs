using Paganism.Interpreter.Data;
using Paganism.Interpreter.Data.Instances;
using Paganism.PParser.AST.Interfaces;

namespace Paganism.PParser.AST
{
    public class StructureDeclarateExpression : Expression, IStatement
    {
        public StructureDeclarateExpression(BlockStatementExpression parent, int line, int position, string filepath, string name, StructureMemberExpression[] members) : base(parent, line, position, filepath)
        {
            Name = name;
            Members = members;
        }

        public string Name { get; }

        public StructureMemberExpression[] Members { get; }

        public void Create()
        {
            Structures.Instance.Value.Add(Parent, Name, new StructureInstance(this));
        }

        public void Remove()
        {
            Structures.Instance.Value.Remove(Parent, Name);
        }
    }
}
