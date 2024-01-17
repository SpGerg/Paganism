using PaganismCustomConsole.API.Features;

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
