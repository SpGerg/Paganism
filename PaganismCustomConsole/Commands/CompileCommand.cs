using Paganism;
using Paganism.Exceptions;
using Paganism.Interpreter;
using Paganism.Interpreter.Data;
using Paganism.Lexer;
using Paganism.PParser;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Interfaces;
using PaganismCustomConsole.API.Features;
using PaganismCustomConsole.API.Features.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PaganismCustomConsole.Commands
{
    public class CompileCommand : CommandBase
    {
        public CompileCommand(CustomConsole customConsole) : base(customConsole)
        {
        }

        public override string Command => "compile";

        public override string Description => "Compile a paganism script";

        public override string[] Aliases { get; } = new string[] { "cmp" };

        public override CommandParameter[] Parameters { get; } = new CommandParameter[]
        {
            new CommandParameter("filename", "filename to compile", true)
        };

        public override bool Execute(Dictionary<string, string> arguments, out string response)
        {
            Lexer lexer;
            Parser parser;

            var path = Path.Combine(CustomConsole.CurrentDirectory, arguments["filename"]);

            if (!File.Exists(path))
            {
                response = "File is not exists";
                return false;
            }

            lexer = new Lexer(File.ReadAllLines(path));
            Token[] tokens;
            BlockStatementExpression expressions;

            try
            {
                tokens = lexer.Run();
            }
            catch (Exception ex)
            {
                if (ex is not LexerException)
                {
                    response = "Lexer error";
                    return true;
                }

                response = "Lexer error: " + ex.Message;
                return true;
            }

            /*
            foreach (var token in tokens)
            {
                Console.WriteLine($"{token.Type}: {token.Value}");
            }
            */
            
            parser = new Parser(tokens, path);

            try
            {
                expressions = parser.Run();
            }
            catch (Exception ex)
            {
                if (ex is not ParserException)
                {
                    response = "Parser error";
                    return true;
                }

                response = "Parser error: " + ex.Message;
                return true;
            }

            var compiler = new Interpreter(expressions);

            try
            {
                compiler.Run();
            }
            catch (Exception ex)
            {
                Variables.Clear();
                Functions.Clear();
                Structures.Clear();

                if (ex is not InterpreterException)
                {
                    response = "Interpreter error";
                    return true;
                }

                response = "Interpreter error: " + ex.Message;
                return true;
            }

            Variables.Clear();
            Functions.Clear();
            Structures.Clear();

            response = "Script has been compilated";
            return true;
        }
    }
}
