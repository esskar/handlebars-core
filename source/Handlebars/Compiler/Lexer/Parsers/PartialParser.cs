using System.IO;
using Handlebars.Core.Compiler.Lexer.Tokens;

namespace Handlebars.Core.Compiler.Lexer.Parsers
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

