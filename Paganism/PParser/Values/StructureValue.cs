using Paganism.Exceptions;
using Paganism.Interpreter.Data;
using Paganism.Interpreter.Data.Instances;
using Paganism.Lexer.Enums;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.Values
{
    public class StructureValue : Value
    {
        public StructureValue(BlockStatementExpression expression, StructureInstance structureInstance, string parent = null)
        {
            Structure = structureInstance;
            Parent = parent;
            var values = new Dictionary<string, Value> ();

            foreach (var member in structureInstance.Members)
            {
                values.Add(member.Key, new NoneValue());
            }

            Values = values;
            BlockStatement = expression;
        }

        public StructureValue(BlockStatementExpression expression, string name, string parent = null)
        {
            Structure = Structures.Instance.Value.Get(expression, name);
            Parent = parent;
            var values = new Dictionary<string, Value>();

            foreach (var member in Structure.Members)
            {
                values.Add(member.Key, new NoneValue());
            }

            Values = values;
            BlockStatement = expression;
        }

        public StructureValue(BlockStatementExpression expression, string name, Dictionary<string, Value> values, string parent = null)
        {
            Structure = Structures.Instance.Value.Get(expression, name);
            Parent = parent;
            Values = new Dictionary<string, Value>();

            foreach (var member in values)
            {
                Values.Add(member.Key, member.Value);
            }

            BlockStatement = expression;
        }

        public override string Name => "Structure";

        public override TypesType Type => TypesType.Structure;

        public string Parent { get; }

        public Dictionary<string, Value> Values { get; }

        public StructureInstance Structure { get; }

        public BlockStatementExpression BlockStatement { get; }

        public void Set(string key, Value value)
        {
            if (!Values.ContainsKey(key))
            {
                throw new InterpreterException($"Didnt found member with '{key}' name.");
            }

            var member = Structure.Members[key];

            if (member.Type != value.Type && value is TypeValue typeValue && typeValue.Value is TypesType.None)
            {
                throw new InterpreterException($"Except {member.Type} type");
            }

            if (member.Structure is not null && member.Structure != string.Empty && value is StructureValue structureValue1 && structureValue1.Structure.Name != member.StructureTypeName)
            {
                throw new InterpreterException($"Except structure '{member.StructureTypeName}' type");
            }

            if (value is StructureValue structureValue)
            {
                Values[key] = new StructureValue(BlockStatement, structureValue.Structure.Name, structureValue.Values, Structure.Name);
            }

            if (Values.TryGetValue(key, out Value result))
            {
                if (result is NoneValue)
                {
                    Values.Remove(key);

                    Values[key] = value;
                    return;
                }

                result.Set(value);
            }

            Values[key] = result;
        }

        public override string AsString()
        {
            var result = string.Empty;

            result += $"{Structure.Name} ({Name}): {{ ";

            foreach (var item in Structure.Members)
            {
                try
                {
                    result += $"{item.Key} = {Values[item.Key].AsString()}, ";
                }
                catch
                {
                    result += $"{item.Key} = {Values[item.Key]}, ";
                }
            }

            result += "}";

            return result;
        }
    }
}
