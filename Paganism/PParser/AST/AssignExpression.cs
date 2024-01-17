﻿using Paganism.PParser.AST.Enums;
using Paganism.PParser.AST.Interfaces;

namespace Paganism.PParser.AST
{
    public class AssignExpression : BinaryOperatorExpression, IStatement
    {
        public AssignExpression(BlockStatementExpression parent, int line, int position, string filepath, EvaluableExpression left, EvaluableExpression right) : base(parent, line, position, filepath, BinaryOperatorType.Assign, left, right)
        {
        }
    }
}