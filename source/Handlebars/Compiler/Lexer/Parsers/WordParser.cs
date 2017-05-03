using System;
using System.IO;
using System.Linq;
using System.Text;

namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class WordParser : Parser
    {
        private const string ValidWordStartCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_$.@[]";

        public override Token Parse(TextReader reader)
        {
            if (IsWord(reader))
            {
                var buffer = AccumulateWord(reader);

                if (buffer.Contains("="))
                {
                    return Token.HashParameter(buffer);
                }
                return Token.Word(buffer);
            }
            return null;
        }

        private static bool IsWord(TextReader reader)
        {
            var peek = (char)reader.Peek();
            return ValidWordStartCharacters.Contains(peek.ToString());
        }

        private static string AccumulateWord(TextReader reader)
        {
            var buffer = new StringBuilder();

            var inString = false;

            while (true)
            {
                if (!inString)
                {
                    var peek = (char)reader.Peek();

                    if (peek == '}' || peek == '~' || peek == ')' || (char.IsWhiteSpace(peek) && CanBreakAtSpace(buffer.ToString())))
                    {
                        break;
                    }
                }

                var node = reader.Read();
                if (node == -1)
                {
                    throw new InvalidOperationException("Reached end of template before the expression was closed.");
                }

                if (node == '\'' || node == '"')
                {
                    inString = !inString;
                }

                buffer.Append((char)node);
            }

            if (buffer.ToString().Contains("[") && !buffer.ToString().Contains("]"))
            {
                throw new HandlebarsCompilerException("Expression is missing a closing ].");
            }

            return buffer.ToString().Trim();
        }

        private static bool CanBreakAtSpace(string buffer)
        {
            if (!buffer.Contains("["))
                return true;

            var chars = buffer.ToCharArray();
            return chars.Count(x => x == '[') == chars.Count(x => x == ']');
        }

    }
}

