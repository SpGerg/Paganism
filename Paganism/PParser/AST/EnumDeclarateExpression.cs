using Paganism.Interpreter.Data.Instances;
using Paganism.PParser.AST.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class EnumDeclarateExpression : Expression, IStatement, IDeclaratable
    {
        public EnumDeclarateExpression(ExpressionInfo info, string name, EnumMemberExpression[] members, bool isShow = false) : base(info)
        {
            Name = name;
            Members = members;
            IsShow = isShow;
        }

        public string Name { get; }

        public bool IsShow { get; }

        public EnumMemberExpression[] Members { get; }

        public void Declarate()
        {
            Interpreter.Data.Enums.Instance.Value.Set(ExpressionInfo.Parent, Name, new EnumInstance(this));
        }

        public void Remove()
        {
            Interpreter.Data.Enums.Instance.Value.Remove(ExpressionInfo.Parent, Name);
        }
    }
}
