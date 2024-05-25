using PaganismCustomConsole.API.Features.Commands;
using PaganismCustomConsole.Commands;
using System;
using System.Linq;

namespace PaganismCustomConsole.API.Features
{
    public class CustomConsole
    {
        public CommandBase[] Commands { get; }

        public bool IsOpen { get; set; }

        public string CurrentDirectory { get; set; }

        public bool IsDebug { get; }

        public CustomConsole(bool isDebug)
        {
            Commands = new CommandBase[]
            {
                new CompileCommand(this),
                new ChangeDirectoryCommand(this),
                new CreditsCommand(this),
                new InfoCommand(this),
                new HelpCommand(this)
            };

            IsDebug = isDebug;
            CurrentDirectory = Environment.CurrentDirectory;
        }

        public void Run()
        {
            IsOpen = true;

            while (IsOpen)
            {
                Console.Write($"{CurrentDirectory}> ");
                string line = Console.ReadLine();

                var arguments = line.Split(' ');

                var executeCommand = Commands.FirstOrDefault(command => command.Command == arguments[0] || command.Aliases.Contains(arguments[0]));

                if (executeCommand is null)
                {
                    Console.WriteLine($"Unknown command with name {line}");
                }
                else
                {
                    try
                    {
                        executeCommand.Execute(new ArraySegment<string>(arguments), out var response);

                        Console.WriteLine(response);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }

                Console.WriteLine();
            }
        }
    }
}
