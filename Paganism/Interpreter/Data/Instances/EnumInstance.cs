using Paganism.PParser.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.Interpreter.Data.Instances
{
    public class EnumInstance : Instance
    {
        public EnumInstance(InstanceInfo instanceInfo, EnumDeclarateExpression enumDeclarateExpression) : base(instanceInfo)
        {
            Name = enumDeclarateExpression.Name;

            Members = new Dictionary<string, EnumMemberExpression>(enumDeclarateExpression.Members.Length);

            foreach (var member in enumDeclarateExpression.Members)
            {
                Members.Add(member.Name, member);
            }
        }

        public override string InstanceName => "Enum";

        public string Name { get; }

        public Dictionary<string, EnumMemberExpression> Members { get; }
    }
}
