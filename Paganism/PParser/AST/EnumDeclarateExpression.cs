using Paganism.Interpreter.Data.Instances;
using Paganism.PParser.AST.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class EnumDeclarateExpression : Expression, IStatement, IDeclaratable, IAccessible
    {
        public EnumDeclarateExpression(ExpressionInfo info, string name, EnumMemberExpression[] members, InstanceInfo instanceInfo) : base(info)
        {
            Name = name;
            Members = members;
            Info = instanceInfo;
        }

        public string Name { get; }

        public EnumMemberExpression[] Members { get; }

        public InstanceInfo Info { get; }

        public void Declarate()
        {
            Interpreter.Data.Enums.Instance.Set(ExpressionInfo, ExpressionInfo.Parent, Name, new EnumInstance(Info, this));
        }

        public void Remove()
        {
            Interpreter.Data.Enums.Instance.Remove(ExpressionInfo.Parent, Name);
        }
    }
}
