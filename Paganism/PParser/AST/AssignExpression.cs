using Paganism.PParser.AST.Enums;
using Paganism.PParser.AST.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class AssignExpression : BinaryOperatorExpression, IStatement
    {
        public AssignExpression(BinaryOperatorType type, IEvaluable left, IEvaluable right) : base(type, left, right)
        {
        }
    }
}
