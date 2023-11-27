using Paganism.Interpreter.Data;
using Paganism.PParser.AST.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class StructureDeclarateExpression : Expression, IStatement
    {
        public StructureDeclarateExpression(string name, StructureMemberExpression[] members)
        {
            Name = name;
            Members = members;
        }

        public string Name { get; }

        public StructureMemberExpression[] Members { get; }

        public void Create()
        {
            Structures.Add(this);
        }

        public void Remove()
        {
            Structures.Remove(Name);
        }
    }
}
