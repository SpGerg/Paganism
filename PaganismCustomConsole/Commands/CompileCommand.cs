using Paganism;
using Paganism.Compiler;
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

            if (CustomConsole.IsDebug)
            {
                lexer = new Lexer(new string[] { });
            }
            else
            {
                var path = Path.Combine(CustomConsole.CurrentDirectory, arguments["filename"]);

                if (!File.Exists(path))
                {
                    response = "File is not exists";
                    return false;
                }

                var result = File.ReadAllLines(path);

                for (int i = 0;i < result.Length;i++)
                {
                    result[i] += '\n';
                }

                lexer = new Lexer(result);
            }
            
            Token[] tokens;
            BlockStatementExpression expressions;

            try
            {
                tokens = lexer.Run();
            }
            catch (Exception ex)
            {
                response = "Error: " + ex;
                return true;
            }

            /*
            foreach (var token in tokens)
            {
                Console.WriteLine($"{token.Type}: {token.Value}");
            }
            */
            
            parser = new Parser(tokens);

            try
            {
                expressions = parser.Run();
            }
            catch (Exception ex)
            {
                response = "Error: " + ex;
                return true;
            }

            var compiler = new Compiler(expressions);

            try
            {
                compiler.Run();
            }
            catch (Exception ex)
            {
                response = "Error: " + ex;
                return true;
            }

            response = "Script has been compilated";
            return true;
        }
    }
}
