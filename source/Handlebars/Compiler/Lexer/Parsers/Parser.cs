using System.IO;
using Handlebars.Core.Compiler.Lexer.Tokens;

namespace Handlebars.Core.Compiler.Lexer.Parsers
{
    internal abstract class Parser
    {
        public abstract Token Parse(TextReader reader);
    }
}

