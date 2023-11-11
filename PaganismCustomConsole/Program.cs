using PaganismCustomConsole.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaganismCustomConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var customConsole = new CustomConsole(false);

            customConsole.Run();
        }
    }
}
