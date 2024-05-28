using Paganism.Exceptions;
using Paganism.Interpreter.Data.Extensions;
using Paganism.Interpreter.Data.Instances;
using Paganism.Lexer;
using Paganism.Lexer.Enums;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using Paganism.Structures;
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
            if (Require(0, TokenType.Plus, TokenType.Minus) && Require(1, TokenType.Plus, TokenType.Minus))
            {
                return ParseUnary() as IStatement;
            }

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
                return ParseFunction(true, returnType: ParseType());
            }

            if (Match(TokenType.Await))
            {
                return ParseAwait();
            }

            if (Match(TokenType.Function))
            {
                return ParseFunction();
            }

            if (Match(TokenType.Structure))
            {
                return ParseStructure(false);
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
                return ParseEnum(false);
            }

            if (Match(TokenType.Show))
            {
                return ParseFunctionOrVariableOrEnumOrStructure(true);
            }

            if (Match(TokenType.Hide))
            {
                return ParseFunctionOrVariableOrEnumOrStructure();
            }

            if (Match(TokenType.Sharp))
            {
                return ParseDirective();
            }

            if (Require(0, TokenType.Word))
            {
                if (Require(1, TokenType.Plus, TokenType.Minus) && Require(2, TokenType.Plus, TokenType.Minus))
                {
                    return ParseBinary() as IStatement;
                }
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

            if (IsType(0))
            {
                return ParseFunctionOrVariableOrEnumOrStructure();
            }

            throw new ParserException($"Unknown expression {Current.Value}.", Current.Line, Current.Position, Filepath);
        }

        private Expression ParseNew()
        {
            Match(TokenType.New);

            var name = Current.Value;

            if (!Match(TokenType.Word))
            {
                throw new ParserException("Except structure name", Current.Line, Current.Position, Filepath);
            }

            return new NewExpression(CreateExpressionInfo(), name);
        }

        private IStatement ParseDirective()
        {
            if (Match(TokenType.Extension))
            {
                if (Match(TokenType.Word))
                {
                    ExtensionFunction = Tokens[Position - 1].Value;
                }
            }
            return new DirectiveExpression(CreateExpressionInfo());
        }

        private IStatement ParseEnum(bool isShow)
        {
            Match(TokenType.Enum);

            var name = Current.Value;

            if (!Match(TokenType.Word))
            {
                throw new ParserException("Except enum member name", Current.Line, Current.Position, Filepath);
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

            return new EnumDeclarateExpression(CreateExpressionInfo(), name, members.ToArray(), new InstanceInfo(isShow, false, Filepath));
        }

        private EnumMemberExpression ParseEnumMember(string parent)
        {
            var name = Current.Value;

            if (!Match(TokenType.Word))
            {
                throw new ParserException("Except enum member name", Current.Line, Current.Position, Filepath);
            }

            if (!Match(TokenType.Assign))
            {
                throw new ParserException("Except assign operator", Current.Line, Current.Position, Filepath);
            }

            var value = ParsePrimary();

            if (value is not NumberValue numberValue)
            {
                throw new ParserException("Except number value", Current.Line, Current.Position, Filepath);
            }

            return new EnumMemberExpression(CreateExpressionInfo(), name, numberValue, parent);
        }

        private IStatement ParseTryCatch()
        {
            Match(TokenType.Try);

            var tryBlock = new BlockStatementExpression(CreateExpressionInfo(), new IStatement[0]);
            var catchBlock = new BlockStatementExpression(CreateExpressionInfo(), new IStatement[0]);

            ParseExpressions(tryBlock, TokenType.Catch);

            ParseExpressions(catchBlock, TokenType.End);

            return new TryCatchExpression(CreateExpressionInfo(), tryBlock, catchBlock);
        }

        private IStatement ParseAwait()
        {
            var expression = ParseBinary();

            if (expression is not BinaryOperatorExpression && expression is not FunctionCallExpression)
            {
                throw new ParserException("Must be async function", CreateExpressionInfo());
            }

            if (expression is BinaryOperatorExpression binaryOperatorExpression && binaryOperatorExpression.Right is not FunctionCallExpression)
            {
                throw new ParserException("Must be async function", CreateExpressionInfo());
            }

            return new AwaitExpression(new ExpressionInfo(_parent, expression.ExpressionInfo.Line, expression.ExpressionInfo.Position, Filepath), expression);
        }

        private IStatement ParseStructure(bool isShow)
        {
            Match(TokenType.Structure);
            var name = Current.Value;

            if (!Match(TokenType.Word))
            {
                throw new ParserException("Except structure name.", CreateExpressionInfo());
            }

            List<StructureMemberExpression> statements = new();

            while (!Match(TokenType.End))
            {
                var member = ParseStructureMember(name);

                statements.Add(member);
            }

            return new StructureDeclarateExpression(CreateExpressionInfo(), name, statements.ToArray(), new InstanceInfo(isShow, false, Filepath));
        }

        private FunctionTypeValue ParseFunctionType()
        {
            var isAsync = false;
            Match(TokenType.FunctionType);

            var expressionInfo = CreateExpressionInfo();
            TypeValue returnType;

            if (!Match(TokenType.Less))
            {
                returnType = new TypeValue(expressionInfo, TypesType.Void, string.Empty);
                isAsync = false;
            }
            else
            {
                isAsync = Match(TokenType.Async);

                if (isAsync && !Match(TokenType.Comma))
                {
                    throw new ParserException("Except comma", CreateExpressionInfo());
                }

                returnType = ParseType();

                Match(TokenType.Greater);
            }

            var arguments = ParseFunctionArguments();

            return new FunctionTypeValue(expressionInfo, returnType, arguments, isAsync);
        }

        private TypeValue ParseType(bool isReturnNull = false)
        {
            TypeValue type = null;

            if (isReturnNull)
            {
                type = null;
            }
            else
            {
                type = new TypeValue(CreateExpressionInfo(), TypesType.Any, string.Empty);
            }

            if (IsType(0))
            {
                if (Require(-1, TokenType.StructureType))
                {
                    type = new TypeValue(CreateExpressionInfo(), TypesType.Structure, Current.Value);
                    Position++;

                    return type;
                }
                else if (Require(-1, TokenType.EnumType))
                {
                    type = new TypeValue(CreateExpressionInfo(), TypesType.Enum, Current.Value);
                    Position++;

                    return type;
                }
                else if (Require(0, TokenType.FunctionType))
                {
                    return ParseFunctionType();
                }
                else
                {
                    type = new TypeValue(CreateExpressionInfo(), Lexer.Tokens.TokenTypeToValueType[Current.Type], string.Empty);
                }

                Position++;
            }

            return type;
        }

        private StructureMemberExpression ParseStructureMember(string structureName)
        {
            var isShow = Match(TokenType.Show) || !Match(TokenType.Hide);
            var isReadOnly = Match(TokenType.Readonly);
            var isCastable = Match(TokenType.Castable);

            var type = ParseType();

            var memberName = Current.Value;

            if (!Match(TokenType.Word))
            {
                throw new ParserException("Except structure member name.", Current.Line, Current.Position, Filepath);
            }

            var member = new StructureMemberExpression(CreateExpressionInfo(), structureName, type, memberName, new StructureMemberInfo(isReadOnly, isShow, isCastable));

            return !Match(TokenType.Semicolon) ? throw new ParserException("Except ';'.", Current.Line, Current.Position, Filepath) : member;
        }

        private IStatement ParseFor()
        {
            Match(TokenType.For);

            if (!Match(TokenType.LeftPar))
            {
                throw new ParserException("Except '('.", Current.Line, Current.Position, Filepath);
            }

            var statement = new BlockStatementExpression(CreateExpressionInfo(), new IStatement[0]);
            _parent = statement;

            IStatement variable = null;
            EvaluableExpression expression = null;
            IStatement action = null;

            if (!Require(0, TokenType.Semicolon))
            {
                variable = ParseVariable(false, ParseType(true));
            }

            if (!Match(TokenType.Semicolon))
            {
                throw new ParserException("Except ';'.", Current.Line, Current.Position, Filepath);
            }

            if (!Require(0, TokenType.Semicolon))
            {
                expression = ParseBinary();
            }

            if (!Match(TokenType.Semicolon))
            {
                throw new ParserException("Except ';'.", Current.Line, Current.Position, Filepath);
            }

            if (!Require(0, TokenType.Semicolon, TokenType.RightPar))
            {
                action = ParseStatement();
            }

            if (!Match(TokenType.RightPar))
            {
                throw new ParserException("Except ')'.", Current.Line, Current.Position, Filepath);
            }

            InLoop = true;

            var statements = ParseExpressions(statement);

            InLoop = false;

            return new ForExpression(CreateExpressionInfo(),
                new BlockStatementExpression(CreateExpressionInfo(), statements.ToArray(), true), expression,
                new BlockStatementExpression(CreateExpressionInfo(), new IStatement[] { action }), variable);
        }

        private IStatement ParseBreak()
        {
            return new BreakExpression(CreateExpressionInfo());
        }

        private IStatement[] ParseExpressions(BlockStatementExpression statement = null, TokenType endToken = TokenType.End)
        {
            List<IStatement> statements = new();

            var oldParent = _parent;

            _parent = statement is null
                ? new BlockStatementExpression(CreateExpressionInfo(), new IStatement[0])
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
                throw new ParserException("Except (", Current.Line, Current.Position, Filepath);
            }

            var expression = ParseBinary();

            if (!Match(TokenType.RightPar))
            {
                throw new ParserException("Except ')'.", Current.Line, Current.Position, Filepath);
            }

            if (!Match(TokenType.Then))
            {
                throw new ParserException("Except 'then'.", Current.Line, Current.Position, Filepath);
            }

            var ifStatements = new List<IStatement>();
            IStatement[] elseStatements = null;

            while (!Match(TokenType.End, TokenType.Else))
            {
                ifStatements.Add(ParseStatement());
            }

            if (Require(-1, TokenType.Else))
            {
                elseStatements = ParseExpressions();
            }

            return new IfExpression(CreateExpressionInfo(), expression,
                new BlockStatementExpression(CreateExpressionInfo(), ifStatements.ToArray(), InLoop),
                new BlockStatementExpression(CreateExpressionInfo(), elseStatements));
        }

        private IStatement ParseFunctionOrVariableOrEnumOrStructure(bool isShow = false)
        {
            var type = ParseType();

            bool isAsync = Match(TokenType.Async);

            if (Match(TokenType.Function))
            {
                return ParseFunction(isAsync, isShow, type);
            }

            if (Match(TokenType.Structure))
            {
                return ParseStructure(isShow);
            }

            if (Match(TokenType.Enum))
            {
                return ParseEnum(isShow);
            }

            return ParseVariable(isShow, type);
        }

        private IStatement ParseVariable(bool isShow = false, TypeValue type = null)
        {
            var isReadOnly = Match(TokenType.Readonly);

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

            if (left is VariableExpression variable)
            {
                left = new VariableExpression(new InstanceInfo(isShow, isReadOnly, Filepath), CreateExpressionInfo(), variable.Name, type);
            }

            if (right is ArrayExpression array)
            {
                right = new ArrayExpression(CreateExpressionInfo(), array.Elements, array.Length);
            }

            return new AssignExpression(CreateExpressionInfo(), left, right, isShow, isReadOnly);
        }

        private FunctionCallExpression ParseFunctionCall()
        {
            var name = Current.Value;

            if (!Match(TokenType.Word))
            {
                throw new ParserException("Except function name to call.", Current.Line, Current.Position, Filepath);
            }

            var arguments = new List<Argument>();

            if (Match(TokenType.LeftPar))
            {
                if (Match(TokenType.RightPar))
                {
                    return new FunctionCallExpression(CreateExpressionInfo(), name, arguments.ToArray());
                }

                while (!Match(TokenType.RightPar))
                {
                    if (Match(TokenType.Comma))
                    {
                        continue;
                    }

                    var argumentName = string.Empty;

                    if (Require(0, TokenType.Word))
                    {
                        argumentName = Current.Value;
                    }

                    arguments.Add(new Argument(argumentName, TypesType.Any, ParseBinary()));
                }
            }

            return new FunctionCallExpression(CreateExpressionInfo(), name, arguments.ToArray());
        }

        private ReturnExpression ParseReturn()
        {
            return new ReturnExpression(CreateExpressionInfo(), ParseBinary());
        }

        private FunctionDeclarateExpression ParseFunction(bool isAsync = false, bool isShow = false, TypeValue returnType = null)
        {
            Match(TokenType.Function);

            var name = string.Empty;
            var current = Current;

            name = Match(TokenType.Word) ? current.Value : throw new ParserException("Except function name.", Current.Line, Current.Position, Filepath);

            var arguments = ParseFunctionArguments();

            var statement = new BlockStatementExpression(CreateExpressionInfo(),
                new IStatement[0], InLoop);
            ParseExpressions(statement);

            if (ExtensionFunction == string.Empty)
            {
                return new FunctionDeclarateExpression(CreateExpressionInfo(),
                    name, statement, arguments, isAsync, new InstanceInfo(isShow, false, Filepath), returnType);
            }

            if (!Extension.AllowedExtensions.Contains(ExtensionFunction))
            {
                throw new ParserException($"The Extension type {ExtensionFunction} does not exist!");
            }

            var original_name = $"..stringFunc_{name}";
            var DeclarationExpression = new FunctionDeclarateExpression(CreateExpressionInfo(),
                original_name, statement, arguments, isAsync, new InstanceInfo(isShow, false, Filepath), returnType);

            switch (ExtensionFunction)
            {
                case "StringExtension":
                    if (!Extension.StringExtension.ContainsKey(name))
                    {
                        Extension.StringExtension.Add(name, new FunctionInstance(new InstanceInfo(true, false, Filepath), DeclarationExpression));
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

            Match(TokenType.LeftPar);

            while (!Match(TokenType.RightPar))
            {
                if (Match(TokenType.Comma))
                {
                    continue;
                }

                var name = Current;

                var isRequired = Match(TokenType.Required);

                TypeValue type = new(CreateExpressionInfo(), TypesType.Any, string.Empty);

                if (IsType(0))
                {
                    type = ParseType();

                    name = Current;
                }

                Match(TokenType.RightPar);

                if (!Match(TokenType.Word))
                {
                    throw new ParserException("Except argument name.", Current.Line, Current.Position, Filepath);
                }

                arguments.Add(new Argument(name.Value, type, null, isRequired, type.Value is TypesType.Array));
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
                    throw new ParserException("Except ']'.", Current.Line, Current.Position, Filepath);
                }

                result = result == null
                    ? new ArrayElementExpression(CreateExpressionInfo(), name, index)
                    : new ArrayElementExpression(CreateExpressionInfo(), name, index, result);

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
            Expression result = null;

            if (Require(0, TokenType.Word) && Require(1, TokenType.Plus, TokenType.Minus))
            {
                result = ParseUnaryPostfix();
            }
            else
            {
                result = ParseMultiplicative();
            }

            if (Require(0, TokenType.Plus, TokenType.Minus) && Require(1, TokenType.Plus, TokenType.Minus) && Require(2, TokenType.Word))
            {
                return result as EvaluableExpression;
            }

            while (Position < Tokens.Length)
            {
                if (Match(TokenType.Plus))
                {
                    result = new BinaryOperatorExpression(CreateExpressionInfo(),
                        OperatorType.Plus, result as EvaluableExpression, ParseBinary(true));
                    continue;
                }

                if (Match(TokenType.Minus))
                {
                    result = new BinaryOperatorExpression(CreateExpressionInfo(),
                            OperatorType.Minus, result as EvaluableExpression, ParseBinary(true));
                    continue;
                }

                if (Match(TokenType.Is))
                {
                    result = new BinaryOperatorExpression(CreateExpressionInfo(),
                        OperatorType.Is, result as EvaluableExpression, ParseBinary());
                    continue;
                }

                if (!isIgnoreOrAndAnd && Match(TokenType.And))
                {
                    result = new BinaryOperatorExpression(CreateExpressionInfo(),
                        OperatorType.And, result as EvaluableExpression, ParseBinary());
                    continue;
                }

                if (!isIgnoreOrAndAnd && Match(TokenType.Or))
                {
                    result = new BinaryOperatorExpression(CreateExpressionInfo(),
                        OperatorType.Or, result as EvaluableExpression, ParseBinary());
                    continue;
                }

                if (Match(TokenType.Less))
                {
                    result = new BinaryOperatorExpression(CreateExpressionInfo(),
                        OperatorType.Less, result as EvaluableExpression, ParseBinary());
                    continue;
                }

                if (Match(TokenType.Greater))
                {
                    result = new BinaryOperatorExpression(CreateExpressionInfo(),
                        OperatorType.More, result as EvaluableExpression, ParseBinary(true));
                    continue;
                }

                if (Match(TokenType.Point))
                {
                    result = new BinaryOperatorExpression(CreateExpressionInfo(),
                        OperatorType.Point, result as EvaluableExpression, ParsePrimary() as EvaluableExpression);
                    continue;
                }

                if (Match(TokenType.As))
                {
                    result = new BinaryOperatorExpression(CreateExpressionInfo(),
                        OperatorType.As, result as EvaluableExpression, ParsePrimary() as EvaluableExpression);
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
                    result = new BinaryOperatorExpression(CreateExpressionInfo(), OperatorType.Multiplicative, result as EvaluableExpression, ParseMultiplicative() as EvaluableExpression);
                    continue;
                }

                if (Match(TokenType.Slash))
                {
                    result = new BinaryOperatorExpression(CreateExpressionInfo(), OperatorType.Division, result as EvaluableExpression, ParseMultiplicative() as EvaluableExpression);
                    continue;
                }

                break;
            }

            return result;
        }

        private UnaryExpression ParseUnaryPostfix()
        {
            var variable = Current.Value;

            var expressionInfo = new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath);

            Match(TokenType.Word);

            if (Match(TokenType.Plus) && Match(TokenType.Plus))
            {
                return new UnaryExpression(CreateExpressionInfo(), new VariableExpression(new InstanceInfo(false, false, Filepath),
                    expressionInfo, variable, new TypeValue(expressionInfo, TypesType.Number, string.Empty)), OperatorType.IncrementPostfix);
            }
            else
            {
                if (Match(TokenType.Minus) && Match(TokenType.Minus))
                {
                    return new UnaryExpression(CreateExpressionInfo(), new VariableExpression(new InstanceInfo(false, false, Filepath),
                    expressionInfo, variable, new TypeValue(expressionInfo, TypesType.Number, string.Empty)), OperatorType.IncrementPostfix);
                }
            }

            throw new ParserException("Unary operator must have 2 pluses or minuses", Current.Line, Current.Position, Filepath);
        }

        private Expression ParseUnary()
        {
            if (Match(TokenType.Plus))
            {
                if (Match(TokenType.Plus))
                {
                    return new UnaryExpression(CreateExpressionInfo(), (EvaluableExpression)ParsePrimary(), OperatorType.IncrementPrefix);
                }

                return ParsePrimary();
            }
            else if (Match(TokenType.Minus))
            {
                if (Match(TokenType.Minus))
                {
                    return new UnaryExpression(CreateExpressionInfo(), (EvaluableExpression)ParsePrimary(), OperatorType.DicrementPrefix);
                }

                return ParsePrimary();
            }

            return ParsePrimary();
        }

        private Expression ParseValues()
        {
            var current = Current;

            if (Match(TokenType.Number))
            {
                return new NumberValue(CreateExpressionInfo(), double.Parse(current.Value.Replace(".", ",")));
            }
            else if (Match(TokenType.String))
            {
                return new StringValue(CreateExpressionInfo(), current.Value);
            }
            else if (Match(TokenType.Char))
            {
                return new CharValue(CreateExpressionInfo(), current.Value[0]);
            }
            else if (Match(TokenType.True, TokenType.False))
            {
                return new BooleanValue(CreateExpressionInfo(), bool.TryParse(current.Value, out var result) ? result :
                    (current.Value == "yes"));
            }
            else if (Match(TokenType.NoneType))
            {
                return new NoneValue(CreateExpressionInfo());
            }
            else if (Match(TokenType.Show))
            {
                return ParseFunctionOrVariableOrEnumOrStructure(true) as Expression;
            }
            else if (Match(TokenType.Hide))
            {
                return ParseFunctionOrVariableOrEnumOrStructure() as Expression;
            }
            else if (IsType(0))
            {
                if (Require(1, TokenType.Function))
                {
                    return ParseFunctionOrVariableOrEnumOrStructure() as Expression;
                }

                var type = ParseType();

                return new TypeValue(CreateExpressionInfo(), type.Value, type.TypeName);
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

                return new VariableExpression(new InstanceInfo(false, false, Filepath), new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), current.Value, type);
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
                return new NotExpression(new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath), ParseBinary());
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
                return isWithException ? throw new ParserException($"Unknown expression {Current.Value}.", Current.Line, Current.Position, Filepath) : null;
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
            var result = Require(relativePosition,
                TokenType.AnyType,
                TokenType.NumberType,
                TokenType.StringType,
                TokenType.BooleanType,
                TokenType.CharType,
                TokenType.ObjectType,
                TokenType.FunctionType,
                TokenType.ArrayType);

            return result || CheckTypeWithName();
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

        private ExpressionInfo CreateExpressionInfo() => new ExpressionInfo(_parent, Current.Line, Current.Position, Filepath);
    }
}
