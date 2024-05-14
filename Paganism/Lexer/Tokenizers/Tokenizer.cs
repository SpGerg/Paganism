namespace Paganism.Lexer.Tokenizers
{
    public abstract class Tokenizer
    {
        public Tokenizer(Lexer lexer)
        {
            Lexer = lexer;

            Position = lexer.Position;
            Line = lexer.Line;
        }

        public Lexer Lexer { get; }

        public char Current => Lexer.Text[Line][Position];

        public int Position { get; protected set; }

        public int Line { get; protected set; }

        public string Filepath => Lexer.Filepath;

        public abstract Token Tokenize();
    }
}
