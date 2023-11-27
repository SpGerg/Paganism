using Paganism.Interpreter.Data;
using Paganism.Interpreter.Data.Instances;
using Paganism.Lexer.Enums;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.Values
{
    public class StructureValue : Value
    {
        public StructureValue(StructureInstance structureInstance, string parent = null)
        {
            Structure = structureInstance;
            Parent = parent;
            var values = new Dictionary<string, Value> ();

            foreach (var member in structureInstance.Members)
            {
                values.Add(member.Name, new NoneValue());
            }

            Values = values;
        }

        public StructureValue(string name, string parent = null)
        {
            Structure = Structures.Get(name);
            Parent = parent;
            var values = new Dictionary<string, Value>();

            foreach (var member in Structure.Members)
            {
                values.Add(member.Name, new NoneValue());
            }

            Values = values;
        }

        public StructureValue(string name, Dictionary<string, Value> values, string parent = null)
        {
            Structure = Structures.Get(name);
            Parent = parent;
            Values = new Dictionary<string, Value>();

            foreach (var member in values)
            {
                Values.Add(member.Key, member.Value);
            }
        }

        public override string Name => "Structure";

        public override TypesType Type => TypesType.Structure;

        public string Parent { get; }

        public Dictionary<string, Value> Values { get; }

        public StructureInstance Structure { get; }

        public void Set(string key, Value value)
        {
            if (value is StructureValue structureValue)
            {
                Values[key] = new StructureValue(structureValue.Structure.Name, structureValue.Values, Structure.Name);
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
                    result += $"{item.Name} = {Values[item.Name].AsString()}, ";
                }
                catch
                {
                    result += $"{item.Name} = {Values[item.Name].Name}, ";
                }
            }

            result += "}";

            return result;
        }
    }
}
