using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.Lexer.Tokenizers
{
    public abstract class Tokenizer
    {
        public abstract Token Tokenize();
    }
}
