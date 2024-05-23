using Paganism.Lexer.Enums;
using Paganism.PParser.AST.Enums;
using System.Collections.Generic;

namespace Paganism.Lexer
{
    public static class Tokens
    {
        public static IReadOnlyDictionary<string, TokenType> KeywordsType { get; } = new Dictionary<string, TokenType>()
        {
            { "function", TokenType.Function },
            { "end", TokenType.End },
            { "string", TokenType.StringType },
            { "str", TokenType.StringType },
            { "number", TokenType.NumberType },
            { "nbr", TokenType.NumberType },
            { "nmr", TokenType.NumberType },
            { "boolean", TokenType.BooleanType },
            { "bool", TokenType.BooleanType },
            { "any", TokenType.AnyType },
            { "return", TokenType.Return },
            { "none", TokenType.NoneType },
            { "true", TokenType.True },
            { "yes", TokenType.True },
            { "false", TokenType.False },
            { "no", TokenType.False },
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
            { "struct", TokenType.Structure },
            { "show", TokenType.Show },
            { "hide", TokenType.Hide },
            { "public", TokenType.Show },
            { "prvate", TokenType.Hide },
            { "castable", TokenType.Castable },
            { "object", TokenType.ObjectType },
            { "async", TokenType.Async },
            { "await", TokenType.Await },
            { "char", TokenType.CharType },
            { "required", TokenType.Required },
            { "rqr", TokenType.Required },
            { "structure_type", TokenType.StructureType },
            { "as", TokenType.As },
            { "delegate", TokenType.Delegate },
            { "try", TokenType.Try },
            { "catch", TokenType.Catch },
            { "enum", TokenType.Enum },
            { "new", TokenType.New },
            { "array", TokenType.ArrayType },
            { "enum_type", TokenType.EnumType },
            { "readonly", TokenType.Readonly },
            { "extension", TokenType.Extension },
            { "function_type", TokenType.FunctionType },
            { "fnt", TokenType.FunctionType }
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
            { ">", TokenType.Greater },
            { ".", TokenType.Point },
            { "#", TokenType.Sharp }
        };

        public static IReadOnlyDictionary<TokenType, TypesType> TokenTypeToValueType { get; } = new Dictionary<TokenType, TypesType>()
        {
            { TokenType.NoneType, TypesType.None },
            { TokenType.StringType, TypesType.String },
            { TokenType.NumberType, TypesType.Number },
            { TokenType.BooleanType, TypesType.Boolean },
            { TokenType.AnyType, TypesType.Any },
            { TokenType.String, TypesType.String },
            { TokenType.Char, TypesType.Char },
            { TokenType.CharType, TypesType.Char },
            { TokenType.Number, TypesType.Number },
            { TokenType.True, TypesType.Boolean },
            { TokenType.False, TypesType.Boolean },
            { TokenType.Structure, TypesType.Structure },
            { TokenType.StructureType, TypesType.Structure },
            { TokenType.EnumType, TypesType.Enum },
            { TokenType.Enum, TypesType.Enum },
            { TokenType.Word, TypesType.Structure },
            { TokenType.ObjectType, TypesType.Object },
            { TokenType.FunctionType, TypesType.Function },
            { TokenType.ArrayType, TypesType.Array }
        };
    }
}
