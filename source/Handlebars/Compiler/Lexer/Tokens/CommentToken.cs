namespace Handlebars.Compiler.Lexer.Tokens
{
    internal class CommentToken : Token
    {
        public CommentToken(string comment)
        {
            Value = comment.Trim('-', ' ');
        }

        public override TokenType Type => TokenType.Comment;

        public override string Value { get; }
    }
}

