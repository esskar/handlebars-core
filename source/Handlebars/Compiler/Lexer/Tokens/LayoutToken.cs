namespace Handlebars.Core.Compiler.Lexer.Tokens
{
    internal class LayoutToken : Token
    {
        public LayoutToken(string layout)
        {
            Value = layout.Trim('-', ' ');
        }

        public override TokenType Type => TokenType.Layout;

        public override string Value { get; }
    }
}