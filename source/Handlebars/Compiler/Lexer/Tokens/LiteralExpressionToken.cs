namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class LiteralExpressionToken : ExpressionToken
    {
        public LiteralExpressionToken(string value, string delimiter = null)
        {
            Value = value;
            Delimiter = delimiter;
        }

        public bool IsDelimitedLiteral => Delimiter == null;

        public string Delimiter { get; }

        public override TokenType Type => TokenType.Literal;

        public override string Value { get; }
    }
}

