using Paganism.Interpreter.Data.Instances;
using Paganism.PParser;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using System;
using System.Collections.Generic;

namespace Paganism.Interpreter.Data
{
    public class Functions : DataStorage<FunctionInstance>
    {
        public static Lazy<Functions> Instance { get; } = new();

        protected override IReadOnlyDictionary<string, FunctionInstance> Language { get; } = new Dictionary<string, FunctionInstance>()
        {
            { "pgm_call", new FunctionInstance(
                new FunctionDeclarateExpression(null, -1, -1, string.Empty, "pgm_call", new BlockStatementExpression(null, 0, 0, string.Empty, null), new Argument[]
                {
                    new Argument("namespace", TypesType.String),
                    new Argument("method", TypesType.String),
                    new Argument("arguments", TypesType.Any)
                },
                    false,
                    true)
                )
            },
            { "pgm_import", new FunctionInstance(
                new FunctionDeclarateExpression(null, -1, -1, string.Empty, "pgm_import", new BlockStatementExpression(null, 0, 0, string.Empty, null), new Argument[]
                {
                    new Argument("file", TypesType.String)
                },
                    false,
                    true)
                )
            },
            { "pgm_resize", new FunctionInstance(
                new FunctionDeclarateExpression(null, -1, -1, string.Empty, "pgm_resize", new BlockStatementExpression(null, 0, 0, string.Empty, null), new Argument[]
                {
                    new Argument("array", TypesType.Array),
                    new Argument("size", TypesType.Number)
                },
                    false,
                    true)
                )
            },
            { "pgm_size", new FunctionInstance(
                new FunctionDeclarateExpression(null, -1, -1, string.Empty, "pgm_size", new BlockStatementExpression(null, 0, 0, string.Empty, null), new Argument[]
                {
                    new Argument("array", TypesType.Array)
                },
                    false,
                    true)
                )
            },
        };

        public override string Name => "Function";
    }
}
