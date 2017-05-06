namespace Handlebars.Core.Compiler.Lexer.Tokens
{
    internal class StaticToken : Token
    {
        private StaticToken(string value, string original)
        {
            Value = value;
            Original = original;
        }

        internal StaticToken(string value)
            : this(value, value)
        {
        }

        public override TokenType Type => TokenType.Static;

        public override string Value { get; }

        public string Original { get; }

        public StaticToken GetModifiedToken(string value)
        {
            return new StaticToken(value, Original);
        }
    }
}