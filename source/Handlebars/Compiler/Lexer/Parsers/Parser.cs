using System.IO;
using Handlebars.Compiler.Lexer.Tokens;

namespace Handlebars.Compiler.Lexer.Parsers
{
    internal abstract class Parser
    {
        public abstract Token Parse(TextReader reader);
    }
}

