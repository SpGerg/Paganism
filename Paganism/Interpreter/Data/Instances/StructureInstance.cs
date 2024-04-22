using Paganism.PParser;
using Paganism.PParser.AST;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;

namespace Paganism.Interpreter.Data.Instances
{
    public class StructureInstance : Instance
    {
        public StructureInstance(StructureDeclarateExpression structureDeclarate, Dictionary<string, Func<Argument[], Value>> functions = null)
        {
            Name = structureDeclarate.Name;
            StructureDeclarateExpression = structureDeclarate;
            Functions = functions;
            Members = new Dictionary<string, StructureMemberExpression>(structureDeclarate.Members.Length);

            for (int i = 0; i < structureDeclarate.Members.Length; i++)
            {
                Members.Add(structureDeclarate.Members[i].Name, structureDeclarate.Members[i]);
            }
        }

        public override string InstanceName => "Structure";

        public string Name { get; }

        public StructureDeclarateExpression StructureDeclarateExpression { get; }

        public Dictionary<string, StructureMemberExpression> Members { get; }

        internal Dictionary<string, Func<Argument[], Value>> Functions { get; }
    }
}
