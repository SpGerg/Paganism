using Paganism.PParser.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.Interpreter.Data.Instances
{
    public class StructureInstance : Instance
    {
        public StructureInstance(StructureDeclarateExpression structureDeclarate)
        {
            Name = structureDeclarate.Name;
            Members = new StructureMemberInstance[structureDeclarate.Members.Length];

            for (int i = 0;i < structureDeclarate.Members.Length;i++)
            {
                Members[i] = new StructureMemberInstance(structureDeclarate.Members[i]);
            }
        }

        public override string InstanceName => "Structure";

        public string Name { get; }

        public StructureMemberInstance[] Members { get; }
    }
}
