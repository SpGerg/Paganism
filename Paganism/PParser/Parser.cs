using Paganism.Exceptions;
using Paganism.Lexer;
using Paganism.Lexer.Enums;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser
{
    public class Parser
    {
        public Parser(Token[] tokens, string filePath)
        {
            Tokens = tokens;
            Filepath = filePath;
        }

        public int Position { get; private set; }

        public Token[] Tokens { get; }

        public Token Current => Position > Tokens.Length - 1 ? Tokens[Tokens.Length - 1] : Tokens[Position];

        public bool InLoop { get; private set; }

        public string Filepath { get; private set; }

        public BlockStatementExpression Run()
        {
            var expressions = new List<IStatement>();

            while (Position < Tokens.Length)
            {
                expressions.Add(ParseStatement());
            }

            return new BlockStatementExpression(expressions.ToArray());
        }

        private IStatement ParseStatement()
        {
            if (Match(TokenType.If))
            {
                return ParseIf();
            }

            if (Match(TokenType.For))
            {
                return ParseFor();
            }

            if (Match(TokenType.Function))
            {
                return ParseFunction();
            }

            if (Match(TokenType.Structure))
            {
                return ParseStructure();
            }

            if (Match(TokenType.Return))
            {
                return ParseReturn();
            }

            if (Match(TokenType.Break))
            {
                return ParseBreak();
            }

            if (Current.Type == TokenType.Word)
            {
                if (Require(1, TokenType.LeftPar))
                {
                    return ParseFunctionCall();
                }
                else if (Require(1, TokenType.Assign) || Require(1, TokenType.LeftBracket))
                {
                    return ParseVariable();
                }
                else if (Require(1, TokenType.Point))
                {
                    return ParseVariable();
                }
            }

            if (IsType(0, true))
            {
                return ParseFunctionOrVariable();
            }

            throw new ParserException($"Unknown expression {Current.Value}.", Current.Line, Current.Position);
        }

        private IStatement ParseStructure()
        {
            var name = Current.Value;

            if (!Match(TokenType.Word)) throw new ParserException("Except structure name.", Current.Line, Current.Position);

            List<StructureMemberExpression> statements = new List<StructureMemberExpression>();

            while (!Match(TokenType.End))
            {
                var isCastable = false;
                var isShow = false;

                if (Match(TokenType.Show))
                {
                    isShow = true;
                }

                if (Match(TokenType.Castable))
                {
                    isCastable = true;
                }

                var current = Current.Type;

                if (!IsType(0, true)) throw new ParserException("Except structure member type.", Current.Line, Current.Position);

                Position++;

                var memberName = Current.Value;

                if (!Match(TokenType.Word)) throw new ParserException("Except structure member name.", Current.Line, Current.Position);

                statements.Add(new StructureMemberExpression(name, Lexer.Tokens.TokenTypeToValueType[current], memberName, isShow, isCastable));

                if (!Match(TokenType.Semicolon)) throw new ParserException("Except ';'.", Current.Line, Current.Position);
            }

            return new StructureDeclarateExpression(name, statements.ToArray());
        }

        private IStatement ParseFor()
        {
            if (!Match(TokenType.LeftPar)) throw new ParserException("Except '('.", Current.Line, Current.Position);

            IStatement variable = null;
            IEvaluable expression = null;
            IStatement action = null;

            if (!Require(0, TokenType.Semicolon))
            {
                variable = ParseVariable();
            }

            if (!Match(TokenType.Semicolon)) throw new ParserException("Except ';'.", Current.Line, Current.Position);

            if (!Require(0, TokenType.Semicolon))
            {
                expression = ParseBinary() as IEvaluable;
            }  

            if (!Match(TokenType.Semicolon)) throw new ParserException("Except ';'.", Current.Line, Current.Position);

            if (!Require(0, TokenType.Semicolon))
            {
                action = ParseStatement();
            }

            if (!Match(TokenType.RightPar)) throw new ParserException("Except ')'.", Current.Line, Current.Position);

            InLoop = true;

            List<IStatement> statements = new List<IStatement>();

            while (!Match(TokenType.End))
            {
                statements.Add(ParseStatement());
            }

            InLoop = false;

            return new ForExpression(new BlockStatementExpression(statements.ToArray(), true), expression, new BlockStatementExpression(new IStatement[] { action }), variable);
        }

        private IStatement ParseBreak()
        {
            return new BreakExpression();
        }

        private IStatement ParseIf()
        {
            if (!Match(TokenType.LeftPar))
            {
                throw new ParserException("Except (", Current.Line, Current.Position);
            }

            var expression = ParseBinary() as IEvaluable;

            if (!Match(TokenType.RightPar)) throw new ParserException("Except ')'.", Current.Line, Current.Position);

            if (!Match(TokenType.Then)) throw new ParserException("Except 'then'.", Current.Line, Current.Position);

            List<IStatement> statements = new List<IStatement>();

            while (!Match(TokenType.End))
            {
                statements.Add(ParseStatement());
            }

            return new IfExpression(expression, new BlockStatementExpression(statements.ToArray(), InLoop), new BlockStatementExpression(null));
        }

        private IStatement ParseFunctionOrVariable()
        {
            List<TokenType> types = new List<TokenType>();

            while (IsType(0, true))
            {
                types.Add(Current.Type);
                Position++;
            }

            if (Match(TokenType.Function))
            {
                return ParseFunction(types.ToArray());
            }
            else
            {
                Position--;
                return ParseVariable();
            }
        }

        private IStatement ParseVariable()
        {
            var type = TokenType.AnyType;

            if (IsType(0, true))
            {
                type = Current.Type;
                Position++; //Skip type
            }

            var isArray = false;

            IEvaluable left;
            IEvaluable right;

            left = ParseBinary() as IEvaluable;

            if (Match(TokenType.LeftBracket))
            {
                isArray = true;

                if (!Match(TokenType.RightBracket)) throw new ParserException("Except ].", Current.Line, Current.Position);
            }

            Match(TokenType.Assign);

            if (isArray)
            {
                Match(TokenType.LeftBracket);

                right = ParseArray() as IEvaluable;
            }
            else
            {
                right = ParseBinary() as IEvaluable;
            }

            var valueType = type;

            if (left is VariableExpression variable)
            {
                left = new VariableExpression(variable.Name, Lexer.Tokens.TokenTypeToValueType[valueType]);
            }

            if (right is ArrayExpression array)
            {
                right = new ArrayExpression(array.Elements, array.Length);
            }

            return new AssignExpression(left, right);
        }

        private FunctionCallExpression ParseFunctionCall()
        {
            var name = Current.Value;

            Match(TokenType.Word);

            var arguments = new List<Argument>();

            if (Match(TokenType.LeftPar))
            {
                while (!Match(TokenType.RightPar))
                {
                    if (Match(TokenType.Comma))
                    {
                        continue;
                    }

                    if (IsType(0))
                    {
                        arguments.Add(new Argument(string.Empty, Lexer.Tokens.TokenTypeToValueType[Current.Type], true, ParseBinary() as IEvaluable));
                    }
                    else
                    {
                        var argumentName = string.Empty;

                        if (Current.Type == TokenType.Word)
                        {
                            argumentName = Current.Value;
                        }

                        arguments.Add(new Argument(argumentName, TypesType.Any, true, ParseBinary() as IEvaluable));
                    }
                }
            }

            return new FunctionCallExpression(name, arguments.ToArray());
        }

        private ReturnExpression ParseReturn()
        {
            List<Expression> expressions = new List<Expression>();

            while (true)
            {
                if (Match(TokenType.Comma))
                {
                    continue;
                }

                var position = Position;
                var binary = ParseBinary();

                if (binary is not IEvaluable)
                {
                    Position = position;
                    break;
                }

                expressions.Add(binary);

                break;
            }

            return new ReturnExpression(expressions.ToArray());
        }

        private FunctionDeclarateExpression ParseFunction(params TokenType[] returnTypes)
        {
            var name = string.Empty;
            var arguments = new List<Argument>();

            var current = Current;

            if (Match(TokenType.Word))
            {
                name = current.Value;
            }

            if (Match(TokenType.LeftPar))
            {
                while (!Match(TokenType.RightPar))
                {
                    if (Match(TokenType.Comma))
                    {
                        continue;
                    }

                    Token type = Current;
                    var previousCurrent = Current;
                    var isArray = false;
                    var isRequired = false;
                    Value defaultValue = null;

                    if (IsType(0, true))
                    {
                        type = previousCurrent;
                        Position++;
                    }

                    if (Match(TokenType.LeftBracket))
                    {
                        isArray = true;

                        if (!Match(TokenType.RightBracket)) throw new ParserException("Except ']'.", Current.Line, Current.Position);
                    }

                    previousCurrent = Current;

                    if (!Match(TokenType.Word))
                    {
                        throw new ParserException("Except argument name.", Current.Line, Current.Position);
                    }

                    if (Match(TokenType.Assign))
                    {
                        isRequired = true;

                        if (!Match(TokenType.NoneType)) throw new ParserException("Is not required argument need default value.", Current.Line, Current.Position);
                    }

                    if (previousCurrent == type)
                    {
                        arguments.Add(new Argument(previousCurrent.Value, TypesType.Any, isRequired, null, isArray, defaultValue));
                        continue;
                    }

                    arguments.Add(new Argument(previousCurrent.Value, Lexer.Tokens.TokenTypeToValueType[type.Type], isRequired, null, isArray, defaultValue, type.Value));
                }
            }

            List<IStatement> statements = new List<IStatement>();

            while (!Match(TokenType.End))
            {
                statements.Add(ParseStatement());
            }

            return new FunctionDeclarateExpression(name, new BlockStatementExpression(statements.ToArray(), InLoop), arguments.ToArray(), returnTypes);
        }

        private Expression ParseElementFromArray()
        {
            var name = Current.Value;

            Match(TokenType.Word);

            ArrayElementExpression result = null;

            while (true)
            {
                Match(TokenType.LeftBracket);

                var index = ParseBinary() as IEvaluable;

                if (!Match(TokenType.RightBracket))
                {
                    throw new ParserException("Except ']'.", Current.Line, Current.Position);
                }

                if (result == null)
                {
                    result = new ArrayElementExpression(name, index);
                }
                else
                {
                    result = new ArrayElementExpression(name, index, result);
                }

                if (!Require(0, TokenType.LeftBracket))
                {
                    break;
                }
            }

            return result;
        }

        private Expression ParseArray()
        {
            List<Expression> elements = new List<Expression>();

            while (!Match(TokenType.RightBracket))
            {
                if (Match(TokenType.Comma))
                {
                    continue;
                }

                var element = ParseBinary() as IEvaluable;

                elements.Add(element as Expression);
            }

            return new ArrayExpression(elements.ToArray(), elements.Count);
        }

        private Expression ParseBinary()
        {
            var result = ParseMultiplicative();

            while (Position < Tokens.Length)
            {
                if (Match(TokenType.Plus))
                {
                    result = new BinaryOperatorExpression(BinaryOperatorType.Plus, result as IEvaluable, ParseMultiplicative() as IEvaluable);
                    continue;
                }

                if (Match(TokenType.Minus))
                {
                    result = new BinaryOperatorExpression(BinaryOperatorType.Minus, result as IEvaluable, ParseMultiplicative() as IEvaluable);
                    continue;
                }

                if (Match(TokenType.Is))
                {
                    result = new BinaryOperatorExpression(BinaryOperatorType.Is, result as IEvaluable, ParseMultiplicative() as IEvaluable);
                    continue;
                }

                if (Match(TokenType.And))
                {
                    result = new BinaryOperatorExpression(BinaryOperatorType.And, result as IEvaluable, ParseBinary() as IEvaluable);
                    continue;
                }

                if (Match(TokenType.Or))
                {
                    result = new BinaryOperatorExpression(BinaryOperatorType.Or, result as IEvaluable, ParseBinary() as IEvaluable);
                    continue;
                }

                if (Match(TokenType.Less))
                {
                    result = new BinaryOperatorExpression(BinaryOperatorType.Less, result as IEvaluable, ParseBinary() as IEvaluable);
                    continue;
                }

                if (Match(TokenType.More))
                {
                    result = new BinaryOperatorExpression(BinaryOperatorType.More, result as IEvaluable, ParseBinary() as IEvaluable);
                    continue;
                }

                if (Match(TokenType.Point))
                {
                    result = new BinaryOperatorExpression(BinaryOperatorType.Point, result as IEvaluable, ParsePrimary() as IEvaluable);
                    continue;
                }

                break;
            }

            return result;
        }

        private Expression ParseMultiplicative()
        {
            var result = ParseUnary();

            while (Position < Tokens.Length)
            {
                if (Match(TokenType.Star))
                {
                    result = new BinaryOperatorExpression(BinaryOperatorType.Multiplicative, result as IEvaluable, ParseMultiplicative() as IEvaluable);
                    continue;
                }

                if (Match(TokenType.Slash))
                {
                    result = new BinaryOperatorExpression(BinaryOperatorType.Division, result as IEvaluable, ParseMultiplicative() as IEvaluable);
                    continue;
                }

                break;
            }

            return result;
        }

        private Expression ParseUnary()
        {
            if (Match(TokenType.Plus))
            {
                return ParsePrimary();
            }
            if (Match(TokenType.Minus))
            {
                return new UnaryExpression((IEvaluable)ParsePrimary(), BinaryOperatorType.Minus);
            }

            return ParsePrimary();
        }

        private Expression ParsePrimary()
        {
            var current = Current;

            if (Match(TokenType.Number))
            {
                return new NumberExpression(double.Parse(current.Value.Replace(".", ",")));
            }
            else if (Match(TokenType.String))
            {
                return new StringExpression(current.Value);
            }
            else if (Match(TokenType.Char))
            {
                return new CharExpression(current.Value[0]);
            }
            else if (Match(TokenType.True, TokenType.False))
            {
                return new BooleanExpression(bool.Parse(current.Value));
            }
            else if (Match(TokenType.NoneType))
            {
                return new NoneExpression();
            }
            else if (IsType(0, true))
            {
                Position++;
                return new TypeExpression(Lexer.Tokens.TokenTypeToValueType[current.Type]);
            }
            else if (Require(0, TokenType.Word) && Require(1, TokenType.LeftBracket))
            {
                return ParseElementFromArray();
            }
            else if(Require(0, TokenType.Word) && Require(1, TokenType.LeftPar))
            {
                return ParseFunctionCall();
            }
            else if (Match(TokenType.Word))
            {
                return new VariableExpression(current.Value, TypesType.Any);
            }
            else if (Match(TokenType.LeftPar))
            {
                var result = ParseBinary();
                Match(TokenType.RightPar);
                return result;
            }
            else if (Match(TokenType.LeftBracket))
            {
                return ParseArray();
            }
            else if (Match(TokenType.Return))
            {
                return ParseReturn();
            }
            else if (Match(TokenType.Not))
            {
                return new NotExpression(ParseBinary() as IEvaluable);
            }

            throw new ParserException($"Unknown expression {Current.Value}.", Current.Line, Current.Position);
        }

        private bool Match(params TokenType[] type)
        {
            if (Position > Tokens.Length - 1) return false;

            if (type.Contains(Current.Type))
            {
                Position++;

                return true;
            }

            return false;
        }

        private bool Require(int relativePosition, params TokenType[] type)
        {
            var position = Position + relativePosition;

            if (position > Tokens.Length - 1) return false;

            return type.Contains(Tokens[position].Type);
        }

        private bool IsType(int relativePosition, bool isWithAnyType = false)
        {
            if (isWithAnyType)
            {
                var result = Require(relativePosition, TokenType.AnyType, TokenType.NumberType, TokenType.StringType, TokenType.BooleanType, TokenType.ObjectType);

                if (!result)
                {
                    return CheckStructureType();
                }

                return result;
            }
  
            var result2 = Require(relativePosition, TokenType.NumberType, TokenType.StringType, TokenType.BooleanType, TokenType.ObjectType);

            if (!result2)
            {
                return CheckStructureType();
            }

            return result2;
        }

        private bool CheckStructureType()
        {
            if (Require(0, TokenType.Word) && (Require(1, TokenType.Word) || Require(1, TokenType.Function)))
            {
                return true;
            }

            return false;
        }
    }
}
