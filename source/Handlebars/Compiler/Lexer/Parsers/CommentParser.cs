using System;
using System.IO;
using System.Text;
using Handlebars.Compiler.Lexer.Tokens;

namespace Handlebars.Compiler.Lexer.Parsers
{
    internal class CommentParser : Parser
    {
        public override Token Parse(TextReader reader)
        {
            Token token = null;
            if (IsComment(reader))
            {
                var buffer = AccumulateComment(reader).Trim();
                if (buffer.StartsWith("<")) //syntax for layout is {{<! layoutname }} - i.e. its inside a comment block
                    token = Token.Layout(buffer.Substring(1).Trim());

                token = token ?? Token.Comment(buffer);
            }
            return token;
        }

        private static bool IsComment(TextReader reader)
        {
            var peek = (char)reader.Peek();
            return peek == '!';
        }

        private static string AccumulateComment(TextReader reader)
        {
            reader.Read();
            bool? escaped = null;
            var buffer = new StringBuilder();
            while (true)
            {
                if (escaped == null)
                {
                    escaped = CheckIfEscaped(reader, buffer);
                }
                if (IsClosed(reader, buffer, escaped.Value))
                {
                    break;
                }
                var node = reader.Read();
                if (node == -1)
                    throw new InvalidOperationException("Reached end of template in the middle of a comment");
                buffer.Append((char)node);
            }
            return buffer.ToString();
        }

        private static bool IsClosed(TextReader reader, StringBuilder buffer, bool isEscaped)
        {
            return isEscaped && CheckIfEscaped(reader, buffer) && CheckIfStatementClosed(reader) 
                || !isEscaped && CheckIfStatementClosed(reader);
        }

        private static bool CheckIfStatementClosed(TextReader reader)
        {
            var isClosed = (char)reader.Peek() == '}';
            return isClosed;
        }

        private static bool CheckIfEscaped(TextReader reader, StringBuilder buffer)
        {
            var escaped = false;
            if ((char)reader.Peek() == '-')
            {
                var first = reader.Read();
                if ((char)reader.Peek() == '-')
                {
                    reader.Read();
                    escaped = true;
                }
                else
                {
                    buffer.Append(first);
                }
            }
            return escaped;
        }
    }
}

