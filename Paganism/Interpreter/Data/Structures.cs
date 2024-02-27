using Paganism.Interpreter.Data.Instances;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;

namespace Paganism.Interpreter.Data
{
    public class Structures : DataStorage<StructureInstance>
    {
        public static Lazy<Structures> Instance { get; } = new();

        public override string Name => "Structure";

        protected override IReadOnlyDictionary<string, StructureInstance> Language { get; } = new Dictionary<string, StructureInstance>()
        {
            { "task", new StructureInstance(new StructureDeclarateExpression(null, -1, -1, string.Empty, "task", new StructureMemberExpression[]
                {
                    new StructureMemberExpression(null, -1, -1, string.Empty, "task", new TypeValue(TypesType.Number, string.Empty), "id", true)
                }
            ))
            },
            { "exception", new StructureInstance(new StructureDeclarateExpression(null, -1, -1, string.Empty, "exception", new StructureMemberExpression[]
                {
                    new StructureMemberExpression(null, -1, -1, string.Empty, "exception", new TypeValue(TypesType.String, string.Empty), "name", true),
                    new StructureMemberExpression(null, -1, -1, string.Empty, "exception", new TypeValue(TypesType.String, string.Empty), "description", true)
                }
            ))
            }
        };
    }
}
