namespace Paganism.PParser.AST.Interfaces
{
    public interface IDeclaratable
    {
        void Declarate();

        void Declarate(string name);

        void Remove();
    }
}
