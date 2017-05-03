using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class Tokenizer
    {
        private static readonly Parser WordParser = new WordParser();
        private static readonly Parser LiteralParser = new LiteralParser();
        private static readonly Parser CommentParser = new CommentParser();
        private static readonly Parser PartialParser = new PartialParser();
        private static readonly Parser BlockWordParser = new BlockWordParser();
        //TODO: structure parser

        public IEnumerable<Token> Tokenize(TextReader source)
        {
            try
            {
                return Parse(source);
            }
            catch (Exception ex)
            {
                throw new HandlebarsParserException("An unhandled exception occurred while trying to compile the template", ex);
            }
        }

        private static IEnumerable<Token> Parse(TextReader source)
        {
            var inExpression = false;
            var trimWhitespace = false;
            var buffer = new StringBuilder();
            var node = source.Read();
            while (true)
            {
                if (node == -1)
                {
                    if (buffer.Length > 0)
                    {
                        if (inExpression)
                        {
                            throw new InvalidOperationException("Reached end of template before expression was closed");
                        }
                        yield return Token.Static(buffer.ToString());
                    }
                    break;
                }
                if (inExpression)
                {
                    if ((char)node == '(')
                    {
                        yield return Token.StartSubExpression();
                    }

                    var token = WordParser.Parse(source);
                    token = token ?? LiteralParser.Parse(source);
                    token = token ?? CommentParser.Parse(source);
                    token = token ?? PartialParser.Parse(source);
                    token = token ?? BlockWordParser.Parse(source);

                    if (token != null)
                    {
                        yield return token;
                    }
                    if ((char)node == '}' && (char)source.Read() == '}')
                    {
                        var escaped = true;
                        if ((char)source.Peek() == '}')
                        {
                            source.Read();
                            escaped = false;
                        }
                        node = source.Read();
                        yield return Token.EndExpression(escaped, trimWhitespace);
                        inExpression = false;
                    }
                    else if ((char)node == ')')
                    {
                        node = source.Read();
                        yield return Token.EndSubExpression();
                    }
                    else if (char.IsWhiteSpace((char)node) || char.IsWhiteSpace((char)source.Peek()))
                    {
                        node = source.Read();
                    }
                    else if ((char)node == '~')
                    {
                        node = source.Read();
                        trimWhitespace = true;
                    }
                    else
                    {
                        if (token == null)
                        {

                            throw new HandlebarsParserException("Reached unparseable token in expression: " + source.ReadLine());
                        }
                        node = source.Read();
                    }
                }
                else
                {
                    if ((char)node == '\\' && (char)source.Peek() == '{')
                    {
                        source.Read();
                        if ((char)source.Peek() == '{')
                        {
                            source.Read();
                            buffer.Append('{', 2);
                        }
                        else
                        {
                            buffer.Append("\\{");
                        }
                        node = source.Read();
                    }
                    else if ((char)node == '{' && (char)source.Peek() == '{')
                    {
                        var escaped = true;
                        trimWhitespace = false;
                        node = source.Read();
                        if ((char)source.Peek() == '{')
                        {
                            node = source.Read();
                            escaped = false;
                        }
                        if ((char)source.Peek() == '~')
                        {
                            source.Read();
                            node = source.Peek();
                            trimWhitespace = true;
                        }
                        yield return Token.Static(buffer.ToString());
                        yield return Token.StartExpression(escaped, trimWhitespace);
                        trimWhitespace = false;
                        buffer = new StringBuilder();
                        inExpression = true;
                    }
                    else
                    {
                        buffer.Append((char)node);
                        node = source.Read();
                    }
                }
            }
        }
    }
}

