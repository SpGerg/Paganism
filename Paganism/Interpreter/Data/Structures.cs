using Paganism.Interpreter.Data.Instances;
using Paganism.PParser;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;

namespace Paganism.Interpreter.Data
{
    public class Structures : DataStorage<StructureInstance>
    {
        public override string Name => "Structure";

        public static Structures Instance => Lazy.Value;

        private static Lazy<Structures> Lazy { get; } = new();

        private static readonly InstanceInfo _languageInfo = new();

        protected override IReadOnlyDictionary<string, StructureInstance> Language { get; } = new Dictionary<string, StructureInstance>()
        {
            { "task", new StructureInstance(new StructureDeclarateExpression(ExpressionInfo.EmptyInfo, "task", new StructureMemberExpression[]
                {
                    new StructureMemberExpression(ExpressionInfo.EmptyInfo, "task", new TypeValue(ExpressionInfo.EmptyInfo, TypesType.Number, string.Empty), "id", true)
                }, _languageInfo)
            )
            },
            { "exception", new StructureInstance(new StructureDeclarateExpression(ExpressionInfo.EmptyInfo, "exception", new StructureMemberExpression[]
                {
                    new StructureMemberExpression(ExpressionInfo.EmptyInfo, "exception", new TypeValue(ExpressionInfo.EmptyInfo, TypesType.String, string.Empty), "name", true),
                    new StructureMemberExpression(ExpressionInfo.EmptyInfo, "exception", new TypeValue(ExpressionInfo.EmptyInfo, TypesType.String, string.Empty), "description", true)
                }, _languageInfo)
            )
            }
        };
    }
}
