using System;
using System.Collections.Generic;
using System.Text;

namespace PaganismCustomConsole.API.Features.Commands
{
    public abstract class CommandBase
    {
        public abstract string Command { get; }

        public abstract string Description { get; }

        public abstract string[] Aliases { get; }

        public virtual CommandParameter[] Parameters { get; } = new CommandParameter[0];

        public CustomConsole CustomConsole { get; }

        protected CommandBase(CustomConsole customConsole)
        {
            CustomConsole = customConsole;
        }

        public abstract bool Execute(Dictionary<string, string> arguments, out string response);

        public bool Execute(ArraySegment<string> arguments, out string response)
        {
            var result = new Dictionary<string, string>();

            for (int i = 0; i < Parameters.Length; i++)
            {
                var parameter = Parameters[i];
                //arguments.Count - 2, cuz first argument it is command name
                if (i > arguments.Count - 2)
                {
                    if (parameter.IsRequired)
                    {
                        throw new ArgumentException($"Except {Parameters[i].Name} argument");
                    }
                    else
                    {
                        continue;
                    }
                }

                var argument = arguments.Array[i + 1];

                if (!TryParse(parameter.Type, argument))
                {
                    throw new ArgumentException($"Except {parameter.Type} type in {parameter.Name} argument");
                }

                result.Add(parameter.Name, argument);
            }

            return Execute(result, out response);
        }

        private bool TryParse(Type type, string value)
        {
            if (type is null)
            {
                return true;
            }

            if (type == typeof(int))
            {
                return int.TryParse(value, out _);
            }

            if (type == typeof(bool))
            {
                return bool.TryParse(value, out _);
            }

            if (type == typeof(char))
            {
                return char.TryParse(value, out _);
            }

            return false;
        }
    }
}
