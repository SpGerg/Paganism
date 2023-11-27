using Paganism.Lexer.Enums;
using Paganism.PParser.AST.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.Lexer
{
    public static class Tokens
    {
        public static IReadOnlyDictionary<string, TokenType> KeywordsType { get; } = new Dictionary<string, TokenType>()
        {
            { "function", TokenType.Function },
            { "end", TokenType.End },
            { "string", TokenType.StringType },
            { "number", TokenType.NumberType },
            { "boolean", TokenType.BooleanType },
            { "any", TokenType.AnyType },
            { "return", TokenType.Return },
            { "none", TokenType.NoneType },
            { "true", TokenType.True },
            { "false", TokenType.False },
            { "if", TokenType.If },
            { "elif", TokenType.Elif },
            { "else", TokenType.Else },
            { "is", TokenType.Is },
            { "and", TokenType.And },
            { "or", TokenType.Or },
            { "then", TokenType.Then },
            { "for", TokenType.For },
            { "not", TokenType.Not },
            { "break", TokenType.Break },
            { "structure", TokenType.Structure },
            { "show", TokenType.Show },
            { "hide", TokenType.Hide },
            { "castable", TokenType.Castable },
            { "object", TokenType.ObjectType },
        };
         
        public static IReadOnlyDictionary<string, TokenType> OperatorsType { get; } = new Dictionary<string, TokenType>()
        {
            { "+", TokenType.Plus },
            { "-", TokenType.Minus },
            { "*", TokenType.Star },
            { "/", TokenType.Slash },
            { ";", TokenType.Semicolon },
            { ":", TokenType.Colon },
            { "(", TokenType.LeftPar },
            { ")", TokenType.RightPar },
            { "[", TokenType.LeftBracket },
            { "]", TokenType.RightBracket },
            { ",", TokenType.Comma },
            { "=", TokenType.Assign },
            { "<", TokenType.Less },
            { ">", TokenType.More },
            { ".", TokenType.Point },
        };

        public static IReadOnlyDictionary<TypesType, TokenType> ValueTypeToTokenType { get; } = new Dictionary<TypesType, TokenType>()
        {
            { TypesType.String, TokenType.StringType },
            { TypesType.Number, TokenType.NumberType },
            { TypesType.Boolean, TokenType.BooleanType },
            { TypesType.Any, TokenType.AnyType }
        };

        public static IReadOnlyDictionary<TokenType, TypesType> TokenTypeToValueType { get; } = new Dictionary<TokenType, TypesType>()
        {
            { TokenType.NoneType, TypesType.None },
            { TokenType.StringType, TypesType.String },
            { TokenType.NumberType, TypesType.Number },
            { TokenType.BooleanType, TypesType.Boolean },
            { TokenType.AnyType, TypesType.Any },
            { TokenType.String, TypesType.String },
            { TokenType.Number, TypesType.Number },
            { TokenType.True, TypesType.Boolean },
            { TokenType.False, TypesType.Boolean },
            { TokenType.Structure, TypesType.Structure },
            { TokenType.Word, TypesType.Structure },
        };
    }
}
