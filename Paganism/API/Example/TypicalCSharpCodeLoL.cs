using Paganism.API.Attributes;
using Paganism.PParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.API.Example
{
    public class TypicalCSharpCodeLoL
    {
        public TypicalCSharpCodeLoL(string someCoolName, LoL loL)
        {
            this.someCoolName = someCoolName;
            this.loL = loL;
        }

        [PaganismSerializable]
        public string someCoolName;

        [PaganismSerializable]
        public LoL loL;

        [PaganismSerializable]
        public void LoL(Argument[] arguments)
        {
            Console.WriteLine("Hello, world");
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
