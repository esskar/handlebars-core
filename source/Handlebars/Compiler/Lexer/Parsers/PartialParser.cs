using System.IO;
using Handlebars.Compiler.Lexer.Tokens;

namespace Handlebars.Compiler.Lexer.Parsers
{
    internal class PartialParser : Parser
    {
        public override Token Parse(TextReader reader)
        {
            PartialToken token = null;
            if ((char)reader.Peek() == '>')
            {
                token = Token.Partial();
            }
            return token;
        }
    }
}

