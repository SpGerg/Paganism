using Paganism.Exceptions;
using Paganism.Interpreter.Data.Extensions;
using Paganism.Interpreter.Data.Instances;
using Paganism.Lexer;
using Paganism.Lexer.Enums;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
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

        public string ExtensionFunction { get; internal set; } = string.Empty;

        private BlockStatementExpression _parent;

        public BlockStatementExpression Run()
        {
            var expressions = new List<IStatement>();

            while (Position < Tokens.Length)
            {
                expressions.Add(ParseStatement());
            }

            return new BlockStatementExpression(ExpressionInfo.EmptyInfo, expressions.ToArray());
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

            if (Match(TokenType.Try))
            {
                return ParseTryCatch();
            }

            if (Match(TokenType.Enum))
            {
                return ParseEnum();
            }

            if (Match(TokenType.Show))
            {
                return ParseFunctionOrVariable(true);
            }

            if (Match(TokenType.Hide))
            {
                return ParseFunctionOrVariable();
            }

            if (Match(TokenType.Sharp))
            {
                return ParseDirective();
            }

            if (Current.Type is TokenType.Word)
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

            return IsType(0)
                ? ParseFunctionOrVariable()
                : Match(TokenType.Await)
                ? ParseAwait()
                : throw new ParserException($"Unknown expression {Current.Value}.", Current.Line, Current.Position);
        }

        private Token NextToken(int Relative)
        {
            if (Position + Relative < Tokens.Length)
            {
                return Tokens[Position + Relative];
            }
            return null;
        }

        private Expression ParseNew()
        {
            Match(TokenType.New);

            var name = Current.Value;

            if (!Match(TokenType.Word))
            {
                throw new ParserException("Except structure name", Current.Line, Current.Position);
            }

            return new NewExpression(new ExpressionInfo(_parent, Current.Position, Current.Line, Filepath), name);
        }

        private IStatement ParseDirective()
        {
            if (Match(TokenType.Extension))
            {
                if (Match(TokenType.Word))
                {
                    ExtensionFunction = Tokens[Position-1].Value;
                }
            }
            return new DirectiveExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath));
        }

        private IStatement ParseEnum()
        {
            Match(TokenType.Enum);

            var name = Current.Value;

            if (!Match(TokenType.Word))
            {
                throw new ParserException("Except enum member name", Current.Line, Current.Position);
            }

            var members = new List<EnumMemberExpression>();

            while (!Match(TokenType.End))
            {
                if (Match(TokenType.Semicolon))
                {
                    continue;
                }

                members.Add(ParseEnumMember(name));
            }

            return new EnumDeclarateExpression(new ExpressionInfo(_parent, Current.Position, Current.Line, Filepath), name, members.ToArray());
        }

        private EnumMemberExpression ParseEnumMember(string parent)
        {
            var name = Current.Value;

            if (!Match(TokenType.Word))
            {
                throw new ParserException("Except enum member name", Current.Line, Current.Position);
            }

            if (!Match(TokenType.Assign))
            {
                throw new ParserException("Except assign operator", Current.Line, Current.Position);
            }

            var value = ParsePrimary();

            if (value is not NumberValue numberValue)
            {
                throw new ParserException("Except number value", Current.Line, Current.Position);
            }

            return new EnumMemberExpression(new ExpressionInfo(_parent, Current.Position, Current.Line, Filepath), name, numberValue, parent);
        }

        private IStatement ParseTryCatch()
        {
            Match(TokenType.Try);

            var tryBlock = new BlockStatementExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), new IStatement[0]);
            var catchBlock = new BlockStatementExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), new IStatement[0]);

            ParseExpressions(tryBlock, TokenType.Catch);

            ParseExpressions(catchBlock, TokenType.End);

            return new TryCatchExpression(new ExpressionInfo(_parent, Current.Position, Current.Line, Filepath), tryBlock, catchBlock);
        }

        private IStatement ParseAwait()
        {
            var expression = ParsePrimary();

            if (expression is FunctionCallExpression function)
            {
                function.IsAwait = true;
            }

            return new AwaitExpression(new ExpressionInfo(_parent, expression.ExpressionInfo.Line, expression.ExpressionInfo.Position, Filepath), expression);
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

            return new StructureDeclarateExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), name, statements.ToArray());
        }

        private TypeValue ParseType()
        {
            var type = new TypeValue(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), TypesType.Any, string.Empty);

            if (IsType(0))
            {
                if (Require(-1, TokenType.StructureType))
                {
                    type = new TypeValue(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), TypesType.Structure, Current.Value);
                    Position++;

                    return type;
                }
                else if (Require(-1, TokenType.EnumType))
                {
                    type = new TypeValue(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), TypesType.Enum, Current.Value);
                    Position++;

                    return type;
                }
                else
                {
                    type = new TypeValue(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), Lexer.Tokens.TokenTypeToValueType[Current.Type], string.Empty);
                }

                Position++;
            }

            return type;
        }

        private StructureMemberExpression ParseStructureMember(string structureName)
        {
            var isShow = Match(TokenType.Show) ? true : !Match(TokenType.Hide);
            var isReadOnly = Match(TokenType.Readonly);
            var isCastable = Match(TokenType.Castable);

            var current = Current.Type;

            if (Require(0, TokenType.Delegate))
            {
                var member2 = ParseDelegate(structureName, isShow, isReadOnly, isCastable);

                return member2;
            }

            var type = ParseType();

            var memberName = Current.Value;

            if (!Match(TokenType.Word))
            {
                throw new ParserException("Except structure member name.", Current.Line, Current.Position);
            }

            var member = new StructureMemberExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), structureName, new TypeValue(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath),
                Lexer.Tokens.TokenTypeToValueType[current], type.TypeName), memberName, isShow, isReadOnly, isCastable);

            return !Match(TokenType.Semicolon) ? throw new ParserException("Except ';'.", Current.Line, Current.Position) : member;
        }

        private StructureMemberExpression ParseDelegate(string structureName, bool isShow, bool isReadOnly, bool isCastable)
        {
            Match(TokenType.Delegate);

            var current = Current.Type;

            var isAsync = Match(TokenType.Async);

            var type = ParseType();

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

            return new StructureMemberExpression(new ExpressionInfo( _parent, Current.Line, Current.Position, Filepath), structureName,
                new TypeValue(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), current is TokenType.Function ? TypesType.None : Lexer.Tokens.TokenTypeToValueType[current], type.TypeName), memberName,
                isShow, isReadOnly, isAsync, true, arguments, isCastable);
        }

        private IStatement ParseFor()
        {
            Match(TokenType.For);

            if (!Match(TokenType.LeftPar))
            {
                throw new ParserException("Except '('.", Current.Line, Current.Position);
            }

            var statement = new BlockStatementExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), new IStatement[0]);
            _parent = statement;

            IStatement variable = null;
            EvaluableExpression expression = null;
            IStatement action = null;

            if (!Require(0, TokenType.Semicolon))
            {
                variable = ParseVariable(false, ParseType());
            }

            if (!Match(TokenType.Semicolon))
            {
                throw new ParserException("Except ';'.", Current.Line, Current.Position);
            }

            if (!Require(0, TokenType.Semicolon))
            {
                expression = ParseBinary();
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

            return new ForExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath),
                new BlockStatementExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), statements.ToArray(), true), expression,
                new BlockStatementExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), new IStatement[] { action }), variable);
        }

        private IStatement ParseBreak()
        {
            return new BreakExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath));
        }

        private IStatement[] ParseExpressions(BlockStatementExpression statement = null, TokenType endToken = TokenType.End)
        {
            List<IStatement> statements = new();

            var oldParent = _parent;

            _parent = statement is null
                ? new BlockStatementExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), new IStatement[0])
                : statement;

            if (Match(endToken))
            {
                _parent = oldParent;

                return statements.ToArray();
            }

            while (!Match(endToken))
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

            var expression = ParseBinary();

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

            return new IfExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), expression,
                new BlockStatementExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), statements.ToArray(), InLoop),
                new BlockStatementExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), null));
        }

        private IStatement ParseFunctionOrVariable(bool isShow = false)
        {
            var type = ParseType();
            
            bool isAsync = Match(TokenType.Async);

            if (Match(TokenType.Function))
            {
                return ParseFunction(isAsync, isShow, type);
            }
            else
            {
                return ParseVariable(isShow, type);
            }
        }

        private IStatement ParseVariable(bool isShow = false, TypeValue type = null)
        {
            if (type is null)
            {
                type = new TypeValue(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), TypesType.Any, string.Empty);
            }

            var current = Current;

            var isArray = false;

            EvaluableExpression left;
            EvaluableExpression right;

            left = ParseBinary();

            if (left is BinaryOperatorExpression binaryOperatorExpression && binaryOperatorExpression.Right is FunctionCallExpression function)
            {
                return binaryOperatorExpression;
            }

            Match(TokenType.Assign);

            if (isArray)
            {
                Match(TokenType.LeftBracket);

                right = ParseArray();
            }
            else
            {
                right = ParseBinary();
            }

            var valueType = type;

            if (left is VariableExpression variable)
            {
                left = new VariableExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), variable.Name, type);
            }

            if (right is ArrayExpression array)
            {
                right = new ArrayExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), array.Elements, array.Length);
            }

            return new AssignExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), left, right, isShow);
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
                if (Match(TokenType.RightPar))
                {
                    return new FunctionCallExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), name, isAwait, arguments.ToArray());
                }

                while (!Match(TokenType.RightPar))
                {
                    if (Match(TokenType.Comma))
                    {
                        continue;
                    }

                    if (IsType(0))
                    {
                        arguments.Add(new Argument(string.Empty, Lexer.Tokens.TokenTypeToValueType[Current.Type], ParseBinary()));
                    }
                    else
                    {
                        var argumentName = string.Empty;

                        if (Current.Type is TokenType.Word)
                        {
                            argumentName = Current.Value;
                        }

                        arguments.Add(new Argument(argumentName, TypesType.Any, ParseBinary()));
                    }
                }
            }

            return new FunctionCallExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), name, isAwait, arguments.ToArray());
        }

        private ReturnExpression ParseReturn()
        {
            return new ReturnExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), ParseBinary());
        }

        private FunctionDeclarateExpression ParseFunction(bool isAsync = false, bool isShow = false, TypeValue returnType = null)
        {
            var name = string.Empty;
            var current = Current;

            name = Match(TokenType.Word) ? current.Value : throw new ParserException("Except function name.", Current.Line, Current.Position);

            var arguments = ParseFunctionArguments();

            var statement = new BlockStatementExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath),
                new IStatement[0], InLoop);
            ParseExpressions(statement);

            if (ExtensionFunction == string.Empty)
            {
                return new FunctionDeclarateExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath),
                    name, statement, arguments, isAsync, isShow, returnType);
            }

            if (!Extension.AllowedExtensions.Contains(ExtensionFunction))
            {
                throw new ParserException($"The Extension type {ExtensionFunction} does not exist!");
            }

            var original_name = $"..stringFunc_{name}";
            var DeclarationExpression = new FunctionDeclarateExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath),
                original_name, statement, arguments, isAsync, isShow, returnType);

            switch (ExtensionFunction)
            {
                case "StringExtension":
                    if (!Extension.StringExtension.ContainsKey(name))
                    {
                        Extension.StringExtension.Add(name, new FunctionInstance(DeclarationExpression));
                    }
                    break;
                default:
                    throw new ParserException($"The Extension type {ExtensionFunction} does not exist!");
            }

            return DeclarationExpression;
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

                    if (IsType(0))
                    {
                        type = previousCurrent;

                        if (type.Type is TokenType.StructureType)
                        {
                            structureName = Current.Value;
                        }

                        Position++;
                    }

                    previousCurrent = Current;

                    if (!Match(TokenType.Word))
                    {
                        throw new ParserException("Except argument name.", Current.Line, Current.Position);
                    }

                    if (Match(TokenType.LeftBracket))
                    {
                        isArray = true;

                        if (!Match(TokenType.RightBracket))
                        {
                            throw new ParserException("Except ']'.", Current.Line, Current.Position);
                        }
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

                var index = ParseBinary();

                if (!Match(TokenType.RightBracket))
                {
                    throw new ParserException("Except ']'.", Current.Line, Current.Position);
                }

                result = result == null
                    ? new ArrayElementExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), name, index)
                    : new ArrayElementExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), name, index, result);

                if (!Require(0, TokenType.LeftBracket))
                {
                    break;
                }
            }

            return result;
        }

        private EvaluableExpression ParseArray()
        {
            List<Expression> elements = new();

            while (!Match(TokenType.RightBracket))
            {
                if (Match(TokenType.Comma))
                {
                    continue;
                }

                var element = ParseBinary();

                elements.Add(element);
            }

            return new ArrayExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), elements.ToArray(), elements.Count);
        }

        private EvaluableExpression ParseBinary(bool isIgnoreOrAndAnd = false)
        {
            var result = ParseMultiplicative();

            while (Position < Tokens.Length)
            {
                if (Match(TokenType.Plus))
                {
                    result = new BinaryOperatorExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath),
                        BinaryOperatorType.Plus, result as EvaluableExpression, ParseBinary(true));
                    continue;
                }

                if (Match(TokenType.Minus))
                {
                    result = new BinaryOperatorExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath),
                            BinaryOperatorType.Minus, result as EvaluableExpression, ParseBinary(true));
                    continue;
                }

                if (Match(TokenType.Is))
                {
                    result = new BinaryOperatorExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath),
                        BinaryOperatorType.Is, result as EvaluableExpression, ParseBinary());
                    continue;
                }

                if (!isIgnoreOrAndAnd && Match(TokenType.And))
                {
                    result = new BinaryOperatorExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath),
                        BinaryOperatorType.And, result as EvaluableExpression, ParseBinary());
                    continue;
                }

                if (!isIgnoreOrAndAnd && Match(TokenType.Or))
                {
                    result = new BinaryOperatorExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath),
                        BinaryOperatorType.Or, result as EvaluableExpression, ParseBinary());
                    continue;
                }

                if (Match(TokenType.Less))
                {
                    result = new BinaryOperatorExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath),
                        BinaryOperatorType.Less, result as EvaluableExpression, ParseBinary());
                    continue;
                }

                if (Match(TokenType.More))
                {
                    result = new BinaryOperatorExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath),
                        BinaryOperatorType.More, result as EvaluableExpression, ParseBinary(true));
                    continue;
                }

                if (Match(TokenType.Point))
                {
                    result = new BinaryOperatorExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath),
                        BinaryOperatorType.Point, result as EvaluableExpression, ParsePrimary() as EvaluableExpression);
                    continue;
                }

                if (Match(TokenType.As))
                {
                    result = new BinaryOperatorExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath),
                        BinaryOperatorType.As, result as EvaluableExpression, ParsePrimary() as EvaluableExpression);
                    continue;
                }

                break;
            }

            return (EvaluableExpression)result;
        }

        private Expression ParseMultiplicative()
        {
            var result = ParseUnary();

            while (Position < Tokens.Length)
            {
                if (Match(TokenType.Star))
                {
                    result = new BinaryOperatorExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), BinaryOperatorType.Multiplicative, result as EvaluableExpression, ParseMultiplicative() as EvaluableExpression);
                    continue;
                }

                if (Match(TokenType.Slash))
                {
                    result = new BinaryOperatorExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), BinaryOperatorType.Division, result as EvaluableExpression, ParseMultiplicative() as EvaluableExpression);
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
                ? new UnaryExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), (EvaluableExpression)ParsePrimary(), BinaryOperatorType.Minus)
                : ParsePrimary();
        }

        private Expression ParseValues()
        {
            var current = Current;

            if (Match(TokenType.Number))
            {
                return new NumberValue(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), double.Parse(current.Value.Replace(".", ",")));
            }
            else if (Match(TokenType.String))
            {
                return new StringValue(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), current.Value);
            }
            else if (Match(TokenType.Char))
            {
                return new CharValue(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), current.Value[0]);
            }
            else if (Match(TokenType.True, TokenType.False))
            {
                return new BooleanValue(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), bool.TryParse(current.Value, out var result) ? result :
                    (current.Value == "yes"));
            }
            else if (Match(TokenType.NoneType))
            {
                return new NoneValue(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath));
            }
            else if (Match(TokenType.Show))
            {
                return ParseFunctionOrVariable(true) as Expression;
            }
            else if (Match(TokenType.Hide))
            {
                return ParseFunctionOrVariable() as Expression;
            }
            else if (IsType(0))
            {
                if (Require(1, TokenType.Function))
                {
                    return ParseFunctionOrVariable() as Expression;
                }

                var type = ParseType();

                return new TypeValue(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), type.Value, type.TypeName);
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
                var type = ParseType();

                return new VariableExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), current.Value, type);
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
            else if (Match(TokenType.Not))
            {
                return new NotExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), ParseBinary() as EvaluableExpression);
            }
            else if (Match(TokenType.New))
            {
                return ParseNew();
            }
            else if (Match(TokenType.Point))
            {
                return ParseNew();
            }
            else if (Match(TokenType.Function))
            {
                return ParseFunction();
            }

            try
            {
                var result = ParseStatement() as Expression;

                return result;
            }
            catch
            {
                return isWithException ? throw new ParserException($"Unknown expression {Current.Value}.", Current.Line, Current.Position) : null;
            }
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

            if (position < 0)
            {
                return false;
            }

            return position <= Tokens.Length - 1 && type.Contains(Tokens[position].Type);
        }

        private bool IsType(int relativePosition)
        {
            var result = Require(relativePosition, TokenType.AnyType, TokenType.NumberType, TokenType.StringType, TokenType.BooleanType, TokenType.CharType, TokenType.ObjectType);

            return !result ? CheckTypeWithName() : true;
        }

        private bool CheckTypeWithName()
        {
            if (Require(-1, TokenType.StructureType) && Require(0, TokenType.Word))
            {
                return true;
            }

            if (Require(-1, TokenType.EnumType) && Require(0, TokenType.Word))
            {
                return true;
            }


            if (Require(0, TokenType.StructureType) && Require(1, TokenType.Word))
            {
                Position++;
                return true;
            }

            if (Require(0, TokenType.EnumType) && Require(1, TokenType.Word))
            {
                Position++;
                return true;
            }

            return false;
        }
    }
}
