using Paganism.Lexer.Enums;
using Paganism.PParser.AST.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class StructureMemberExpression : Expression, IStatement
    {
        public StructureMemberExpression(TokenType type, string name, bool isShow = false, bool isCastable = false)
        {
            Type = type;
            Name = name;
            IsShow = isShow;
            IsCastable = isCastable;
        }

        public TokenType Type { get; }

        public string Name { get; }

        public bool IsShow { get; }

        public bool IsCastable { get; }
    }
}
