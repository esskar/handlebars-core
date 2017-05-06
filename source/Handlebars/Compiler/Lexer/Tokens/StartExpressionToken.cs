namespace Handlebars.Core.Compiler.Lexer.Tokens
{
    internal class StartExpressionToken : ExpressionScopeToken
    {
        public StartExpressionToken(bool isEscaped, bool trimWhitespace)
        {
            IsEscaped = isEscaped;
            TrimPreceedingWhitespace = trimWhitespace;
        }

        public bool IsEscaped { get; }

        public bool TrimPreceedingWhitespace { get; }

        public override string Value => IsEscaped ? "{{" : "{{{";

        public override TokenType Type => TokenType.StartExpression;

        public override string ToString()
        {
            return Value;
        }
    }
}