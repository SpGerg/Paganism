using Paganism.API.Attributes;
using Paganism.PParser;
using Paganism.PParser.Values;
using System;

namespace Paganism.API.Example
{
    public class TypicalCSharpCodeLoL
    {
        public TypicalCSharpCodeLoL(string someCoolName, LoL loL)
        {
            this.someCoolName = someCoolName;
            this.loL = loL;
        }

        public TypicalCSharpCodeLoL() { }

        [PaganismSerializable]
        public string someCoolName;

        [PaganismSerializable]
        public LoL loL;

        [PaganismSerializable]
        public Value LoL(Argument[] arguments)
        {
            Console.WriteLine(arguments[0].Value.Evaluate().AsString());

            return new VoidValue(ExpressionInfo.EmptyInfo);
        }
    }

    public enum LoL
    {
        None = 0,
        First = 1,
        Second = 2,
        Third = 3
    }
}
