namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class EndExpressionToken : ExpressionScopeToken
    {
        public EndExpressionToken(bool isEscaped, bool trimWhitespace)
        {
            IsEscaped = isEscaped;
            TrimTrailingWhitespace = trimWhitespace;
        }

        public bool IsEscaped { get; }

        public bool TrimTrailingWhitespace { get; }

        public override string Value => IsEscaped ? "}}" : "}}}";

        public override TokenType Type { get; } = TokenType.EndExpression;

        public override string ToString()
        {
            return Value;
        }
    }
}

