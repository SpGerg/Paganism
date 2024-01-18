using Paganism.Exceptions;
using Paganism.Lexer;
using Paganism.Lexer.Enums;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System.Collections.Generic;
using System.Linq;

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

        private BlockStatementExpression _parent;

        public BlockStatementExpression Run()
        {
            var expressions = new List<IStatement>();

            while (Position < Tokens.Length)
            {
                expressions.Add(ParseStatement());
            }

            return new BlockStatementExpression(null, 0, 0, Filepath, expressions.ToArray());
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

            if (Match(TokenType.Async))
            {
                return !Match(TokenType.Function)
                    ? throw new ParserException("Except function keyword.", Current.Line, Current.Position)
                    : (IStatement)ParseFunction(true);
            }

            if (Match(TokenType.Await))
            {
                return ParseFunctionCall(true);
            }

            if (Match(TokenType.Function))
            {
                return ParseFunction();
            }

            if (Require(0, TokenType.Structure))
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

            return IsType(0, true, true)
                ? ParseFunctionOrVariable()
                : Match(TokenType.Await)
                ? ParseAwait()
                : throw new ParserException($"Unknown expression {Current.Value}.", Current.Line, Current.Position);
        }

        private IStatement ParseAwait()
        {
            var expression = ParsePrimary();

            if (expression is FunctionCallExpression function)
            {
                function.IsAwait = true;
            }

            return new AwaitExpression(_parent, expression.Line, expression.Position, Filepath, expression);
        }

        private IStatement ParseStructure()
        {
            Match(TokenType.Structure);
            var name = Current.Value;

            if (!Match(TokenType.Word))
            {
                throw new ParserException("Except structure name.", Current.Line, Current.Position);
            }

            List<StructureMemberExpression> statements = new();

            while (!Match(TokenType.End))
            {
                var member = ParseStructureMember(name);

                statements.Add(member);
            }

            return new StructureDeclarateExpression(_parent, Current.Line, Current.Position, Filepath, name, statements.ToArray());
        }

        private StructureMemberExpression ParseStructureMember(string structureName)
        {
            var isShow = Match(TokenType.Show);
            var isCastable = Match(TokenType.Castable);

            var current = Current.Type;

            var structureTypeName = string.Empty;

            if (Require(0, TokenType.Delegate))
            {
                var memher = ParseDelegate(structureName, isShow, isCastable);

                return memher;
            }

            if (!IsType(0, true))
            {
                structureTypeName = Match(TokenType.StructureType)
                    ? Current.Value
                    : throw new ParserException("Except structure member type", Current.Line, Current.Position);
            }
            
            Position++;

            var memberName = Current.Value;

            if (!Match(TokenType.Word))
            {
                throw new ParserException("Except structure member name.", Current.Line, Current.Position);
            }

            var member = new StructureMemberExpression(_parent, Current.Line, Current.Position, Filepath, structureName, structureTypeName, Lexer.Tokens.TokenTypeToValueType[current], memberName, isShow, isCastable);

            return !Match(TokenType.Semicolon) ? throw new ParserException("Except ';'.", Current.Line, Current.Position) : member;
        }

        private StructureMemberExpression ParseDelegate(string structureName, bool isShow, bool isCastable)
        {
            Match(TokenType.Delegate);

            var structureTypeName = string.Empty;
            var current = Current.Type;

            if (!IsType(0, true))
            {
                if (Match(TokenType.StructureType))
                {
                    structureTypeName = Current.Value;
                }
            }

            if (!Match(TokenType.Function))
            {
                throw new ParserException("Except delegate name.", Current.Line, Current.Position);
            }

            var memberName = Current.Value;

            if (!Match(TokenType.Word))
            {
                throw new ParserException("Except structure member name.", Current.Line, Current.Position);
            }

            var arguments = ParseFunctionArguments();

            Match(TokenType.Semicolon);

            return new StructureMemberExpression(_parent, Current.Line, Current.Position, Filepath, structureName, structureTypeName, current is TokenType.Function ? TypesType.None : Lexer.Tokens.TokenTypeToValueType[current], memberName, isShow, true, arguments, isCastable);
        }

        private IStatement ParseFor()
        {
            if (!Match(TokenType.LeftPar))
            {
                throw new ParserException("Except '('.", Current.Line, Current.Position);
            }

            var statement = new BlockStatementExpression(_parent, Current.Line, Current.Position, Filepath, new IStatement[0]);
            _parent = statement;

            IStatement variable = null;
            EvaluableExpression expression = null;
            IStatement action = null;

            if (!Require(0, TokenType.Semicolon))
            {
                variable = ParseVariable();
            }

            if (!Match(TokenType.Semicolon))
            {
                throw new ParserException("Except ';'.", Current.Line, Current.Position);
            }

            if (!Require(0, TokenType.Semicolon))
            {
                expression = ParseBinary() as EvaluableExpression;
            }

            if (!Match(TokenType.Semicolon))
            {
                throw new ParserException("Except ';'.", Current.Line, Current.Position);
            }

            if (!Require(0, TokenType.Semicolon, TokenType.RightPar))
            {
                action = ParseStatement();
            }

            if (!Match(TokenType.RightPar))
            {
                throw new ParserException("Except ')'.", Current.Line, Current.Position);
            }

            InLoop = true;

            var statements = ParseExpressions(statement);

            InLoop = false;

            return new ForExpression(_parent, Current.Line, Current.Position, Filepath,
                new BlockStatementExpression(_parent, Current.Line, Current.Position, Filepath, statements.ToArray(), true), expression,
                new BlockStatementExpression(_parent, Current.Line, Current.Position, Filepath, new IStatement[] { action }), variable);
        }

        private IStatement ParseBreak()
        {
            return new BreakExpression(_parent, Current.Line, Current.Position, Filepath);
        }

        private IStatement[] ParseExpressions(BlockStatementExpression statement = null, bool isToEnd = true)
        {
            List<IStatement> statements = new();

            var oldParent = _parent;

            _parent = statement is null
                ? new BlockStatementExpression(_parent, Current.Line, Current.Position, Filepath, new IStatement[0])
                : statement;

            if (Match(TokenType.End))
            {
                _parent = oldParent;

                return statements.ToArray();
            }

            while (!Match(TokenType.End))
            {
                statements.Add(ParseStatement());
            }

            _parent.Statements = statements.ToArray();

            _parent = oldParent;

            return statements.ToArray();
        }

        private IStatement ParseIf()
        {
            if (!Match(TokenType.LeftPar))
            {
                throw new ParserException("Except (", Current.Line, Current.Position);
            }

            var expression = ParseBinary() as EvaluableExpression;

            if (!Match(TokenType.RightPar))
            {
                throw new ParserException("Except ')'.", Current.Line, Current.Position);
            }

            if (!Match(TokenType.Then))
            {
                throw new ParserException("Except 'then'.", Current.Line, Current.Position);
            }

            List<IStatement> statements = new();

            while (!Match(TokenType.End))
            {
                statements.Add(ParseStatement());
            }

            return new IfExpression(_parent, Current.Line, Current.Position, Filepath, expression,
                new BlockStatementExpression(_parent, Current.Line, Current.Position, Filepath, statements.ToArray(), InLoop),
                new BlockStatementExpression(_parent, Current.Line, Current.Position, Filepath, null));
        }

        private IStatement ParseFunctionOrVariable()
        {
            List<Return> types = new();

            while (IsType(0, true, true))
            {
                if (Require(-1, TokenType.StructureType))
                {
                    types.Add(new Return(TypesType.Structure, Current.Value));
                }
                else
                {
                    types.Add(new Return(Lexer.Tokens.TokenTypeToValueType[Current.Type], string.Empty));
                }

                Position++;
            }

            bool isAsync = Match(TokenType.Async);

            if (Match(TokenType.Function))
            {
                return ParseFunction(isAsync, types.ToArray());
            }
            else
            {
                Position--;
                return ParseVariable();
            }
        }

        private IStatement ParseVariable()
        {
            TypeValue type = null;
            var current = Current;

            if (IsType(0, true, true))
            {
                type = Require(-1, TokenType.StructureType)
                    ? new TypeValue(TypesType.Structure, Current.Value)
                    : new TypeValue(Lexer.Tokens.TokenTypeToValueType[Current.Type], string.Empty);

                Position++; //Skip type
            }

            var isArray = false;

            EvaluableExpression left;
            EvaluableExpression right;

            left = ParseBinary() as EvaluableExpression;

            if (Match(TokenType.LeftBracket))
            {
                isArray = true;

                if (!Match(TokenType.RightBracket))
                {
                    throw new ParserException("Except ].", Current.Line, Current.Position);
                }
            }

            Match(TokenType.Assign);

            if (isArray)
            {
                Match(TokenType.LeftBracket);

                right = ParseArray() as EvaluableExpression;
            }
            else
            {
                right = ParseBinary() as EvaluableExpression;
            }

            var valueType = type;

            if (left is VariableExpression variable)
            {
                left = new VariableExpression(_parent, Current.Line, Current.Position, Filepath, variable.Name, type);
            }

            if (right is ArrayExpression array)
            {
                right = new ArrayExpression(_parent, Current.Line, Current.Position, Filepath, array.Elements, array.Length);
            }

            return new AssignExpression(_parent, Current.Line, Current.Position, Filepath, left, right);
        }

        private FunctionCallExpression ParseFunctionCall(bool isAwait = false)
        {
            isAwait = Match(TokenType.Await);

            var name = Current.Value;

            if (!Match(TokenType.Word))
            {
                throw new ParserException("Except function name to call.", Current.Line, Current.Position);
            }

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
                        arguments.Add(new Argument(string.Empty, Lexer.Tokens.TokenTypeToValueType[Current.Type], ParseBinary() as EvaluableExpression));
                    }
                    else
                    {
                        var argumentName = string.Empty;

                        if (Current.Type == TokenType.Word)
                        {
                            argumentName = Current.Value;
                        }

                        arguments.Add(new Argument(argumentName, TypesType.Any, ParseBinary() as EvaluableExpression));
                    }
                }
            }

            return new FunctionCallExpression(_parent, Current.Line, Current.Position, Filepath, name, isAwait, arguments.ToArray());
        }

        private ReturnExpression ParseReturn()
        {
            List<Expression> expressions = new();

            while (true)
            {
                if (Match(TokenType.Comma))
                {
                    continue;
                }

                var position = Position;
                var binary = ParseBinary();

                if (binary is not EvaluableExpression)
                {
                    Position = position;
                    break;
                }

                expressions.Add(binary);

                break;
            }

            return new ReturnExpression(_parent, Current.Line, Current.Position, Filepath, expressions.ToArray());
        }

        private FunctionDeclarateExpression ParseFunction(bool isAsync = false, params Return[] returnTypes)
        {
            var name = string.Empty;
            var current = Current;

            name = Match(TokenType.Word) ? current.Value : throw new ParserException("Except function name.", Current.Line, Current.Position);

            var arguments = ParseFunctionArguments();

            var statement = new BlockStatementExpression(_parent, Current.Line, Current.Position, Filepath, new IStatement[0], InLoop);
            ParseExpressions(statement);

            return new FunctionDeclarateExpression(_parent, Current.Line, Current.Position, Filepath, name, statement, arguments, isAsync, returnTypes);
        }

        private Argument[] ParseFunctionArguments()
        {
            var arguments = new List<Argument>();

            if (Match(TokenType.LeftPar))
            {
                while (!Match(TokenType.RightPar))
                {
                    if (Match(TokenType.Comma))
                    {
                        continue;
                    }

                    var isRequired = Match(TokenType.Required);

                    Token type = Current;
                    var structureName = string.Empty;
                    var previousCurrent = Current;
                    var isArray = false;

                    if (IsType(0, true, true))
                    {
                        type = previousCurrent;

                        if (type.Type is TokenType.StructureType)
                        {
                            structureName = Current.Value;
                        }

                        Position++;
                    }

                    if (Match(TokenType.LeftBracket))
                    {
                        isArray = true;

                        if (!Match(TokenType.RightBracket))
                        {
                            throw new ParserException("Except ']'.", Current.Line, Current.Position);
                        }
                    }

                    previousCurrent = Current;

                    if (!Match(TokenType.Word))
                    {
                        throw new ParserException("Except argument name.", Current.Line, Current.Position);
                    }

                    if (previousCurrent == type)
                    {
                        arguments.Add(new Argument(previousCurrent.Value, TypesType.Any, null, isRequired, isArray, structureName));
                        continue;
                    }

                    arguments.Add(new Argument(previousCurrent.Value, Lexer.Tokens.TokenTypeToValueType[type.Type], null, isRequired, isArray, structureName));
                }
            }

            return arguments.ToArray();
        }

        private Expression ParseElementFromArray()
        {
            var name = Current.Value;

            Match(TokenType.Word);

            ArrayElementExpression result = null;

            while (true)
            {
                Match(TokenType.LeftBracket);

                var index = ParseBinary() as EvaluableExpression;

                if (!Match(TokenType.RightBracket))
                {
                    throw new ParserException("Except ']'.", Current.Line, Current.Position);
                }

                result = result == null
                    ? new ArrayElementExpression(_parent, Current.Line, Current.Position, Filepath, name, index)
                    : new ArrayElementExpression(_parent, Current.Line, Current.Position, Filepath, name, index, result);

                if (!Require(0, TokenType.LeftBracket))
                {
                    break;
                }
            }

            return result;
        }

        private Expression ParseArray()
        {
            List<Expression> elements = new();

            while (!Match(TokenType.RightBracket))
            {
                if (Match(TokenType.Comma))
                {
                    continue;
                }

                var element = ParseBinary() as EvaluableExpression;

                elements.Add(element);
            }

            return new ArrayExpression(_parent, Current.Line, Current.Position, Filepath, elements.ToArray(), elements.Count);
        }

        private Expression ParseBinary()
        {
            var result = ParseMultiplicative();

            while (Position < Tokens.Length)
            {
                if (Match(TokenType.Plus))
                {
                    result = new BinaryOperatorExpression(_parent, Current.Line, Current.Position, Filepath,
                        BinaryOperatorType.Plus, result as EvaluableExpression, ParseBinary() as EvaluableExpression);
                    continue;
                }

                if (Match(TokenType.Minus))
                {
                    result = new BinaryOperatorExpression(_parent, Current.Line, Current.Position, Filepath,
                            BinaryOperatorType.Minus, result as EvaluableExpression, ParseMultiplicative() as EvaluableExpression);
                    continue;
                }

                if (Match(TokenType.Is))
                {
                    result = new BinaryOperatorExpression(_parent, Current.Line, Current.Position, Filepath,
                        BinaryOperatorType.Is, result as EvaluableExpression, ParseMultiplicative() as EvaluableExpression);
                    continue;
                }

                if (Match(TokenType.And))
                {
                    result = new BinaryOperatorExpression(_parent, Current.Line, Current.Position, Filepath,
                        BinaryOperatorType.And, result as EvaluableExpression, ParseBinary() as EvaluableExpression);
                    continue;
                }

                if (Match(TokenType.Or))
                {
                    result = new BinaryOperatorExpression(_parent, Current.Line, Current.Position, Filepath,
                        BinaryOperatorType.Or, result as EvaluableExpression, ParseBinary() as EvaluableExpression);
                    continue;
                }

                if (Match(TokenType.Less))
                {
                    result = new BinaryOperatorExpression(_parent, Current.Line, Current.Position, Filepath,
                        BinaryOperatorType.Less, result as EvaluableExpression, ParseBinary() as EvaluableExpression);
                    continue;
                }

                if (Match(TokenType.More))
                {
                    result = new BinaryOperatorExpression(_parent, Current.Line, Current.Position, Filepath,
                        BinaryOperatorType.More, result as EvaluableExpression, ParseBinary() as EvaluableExpression);
                    continue;
                }

                if (Match(TokenType.Point))
                {
                    result = new BinaryOperatorExpression(_parent, Current.Line, Current.Position, Filepath,
                        BinaryOperatorType.Point, result as EvaluableExpression, ParsePrimary() as EvaluableExpression);
                    continue;
                }

                if (Match(TokenType.As))
                {
                    result = new BinaryOperatorExpression(_parent, Current.Line, Current.Position, Filepath,
                        BinaryOperatorType.As, result as EvaluableExpression, ParsePrimary() as EvaluableExpression);
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
                    result = new BinaryOperatorExpression(_parent, Current.Line, Current.Position, Filepath, BinaryOperatorType.Multiplicative, result as EvaluableExpression, ParseMultiplicative() as EvaluableExpression);
                    continue;
                }

                if (Match(TokenType.Slash))
                {
                    result = new BinaryOperatorExpression(_parent, Current.Line, Current.Position, Filepath, BinaryOperatorType.Division, result as EvaluableExpression, ParseMultiplicative() as EvaluableExpression);
                    continue;
                }

                break;
            }

            return result;
        }

        private Expression ParseUnary()
        {
            return Match(TokenType.Plus)
                ? ParsePrimary()
                : Match(TokenType.Minus)
                ? new UnaryExpression(_parent, Current.Line, Current.Position, Filepath, (EvaluableExpression)ParsePrimary(), BinaryOperatorType.Minus)
                : ParsePrimary();
        }

        private Expression ParseValues()
        {
            var current = Current;

            if (Match(TokenType.Number))
            {
                return new NumberExpression(_parent, Current.Line, Current.Position, Filepath, double.Parse(current.Value.Replace(".", ",")));
            }
            else if (Match(TokenType.String))
            {
                return new StringExpression(_parent, Current.Line, Current.Position, Filepath, current.Value);
            }
            else if (Match(TokenType.Char))
            {
                return new CharExpression(_parent, Current.Line, Current.Position, Filepath, current.Value[0]);
            }
            else if (Match(TokenType.True, TokenType.False))
            {
                return new BooleanExpression(_parent, Current.Line, Current.Position, Filepath, bool.TryParse(current.Value, out var result) ? result :
                    (current.Value == "yes"));
            }
            else if (Match(TokenType.NoneType))
            {
                return new NoneExpression(_parent, Current.Line, Current.Position, Filepath);
            }
            else if (IsType(0, true, true))
            {
                Position++;

                if (Require(0, TokenType.Function))
                {
                    Position--;

                    return ParseFunctionOrVariable() as Expression;
                }

                return Require(-1, TokenType.StructureType)
                    ? new TypeExpression(_parent, Current.Line, Current.Position, Filepath, Lexer.Tokens.TokenTypeToValueType[current.Type], string.Empty)
                    : (Expression)new TypeExpression(_parent, Current.Line, Current.Position, Filepath, Lexer.Tokens.TokenTypeToValueType[current.Type], Tokens[Position - 1].Value);
            }

            return null;
        }

        private Expression ParsePrimary(bool isWithException = true)
        {
            var current = Current;

            var values = ParseValues();

            if (values is not null)
            {
                return values;
            }
            else if (Require(0, TokenType.Word) && Require(1, TokenType.LeftBracket))
            {
                return ParseElementFromArray();
            }
            else if (Require(0, TokenType.Word) && Require(1, TokenType.LeftPar))
            {
                return ParseFunctionCall();
            }
            else if (Match(TokenType.Word))
            {
                TypeValue type = null;

                if (IsType(0, true, true))
                {
                    type = Require(-1, TokenType.StructureType)
                        ? new TypeValue(TypesType.Structure, Current.Value)
                        : new TypeValue(Lexer.Tokens.TokenTypeToValueType[Current.Type], string.Empty);

                    Position++; //Skip type
                }

                return new VariableExpression(_parent, Current.Line, Current.Position, Filepath, current.Value, type);
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
                return new NotExpression(_parent, Current.Line, Current.Position, Filepath, ParseBinary() as EvaluableExpression);
            }
            else if (Match(TokenType.Function))
            {
                return ParseFunction();
            }
            else if (Match(TokenType.Async))
            {
                return ParseFunction(true);
            }

            return isWithException ? throw new ParserException($"Unknown expression {Current.Value}.", Current.Line, Current.Position) : null;
        }

        private bool Match(params TokenType[] type)
        {
            if (Position > Tokens.Length - 1)
            {
                return false;
            }

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

            return position <= Tokens.Length - 1 && type.Contains(Tokens[position].Type);
        }

        private bool IsType(int relativePosition, bool isWithAnyType = false, bool isWithStructureType = false)
        {
            if (isWithAnyType)
            {
                var result = Require(relativePosition, TokenType.AnyType, TokenType.NumberType, TokenType.StringType, TokenType.BooleanType, TokenType.CharType, TokenType.ObjectType);

                return !result && isWithStructureType ? CheckStructureType() : result;
            }

            var result2 = Require(relativePosition, TokenType.NumberType, TokenType.StringType, TokenType.BooleanType, TokenType.CharType, TokenType.ObjectType);

            return !result2 && isWithStructureType ? CheckStructureType() : result2;
        }

        private bool CheckStructureType()
        {
            if (Require(-1, TokenType.StructureType) && Require(0, TokenType.Word))
            {
                return true;
            }

            if (Require(0, TokenType.StructureType) && Require(1, TokenType.Word))
            {
                Position++;
                return true;
            }

            return false;
        }
    }
}
