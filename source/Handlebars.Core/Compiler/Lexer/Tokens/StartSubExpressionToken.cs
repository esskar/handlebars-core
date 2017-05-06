namespace Handlebars.Core.Compiler.Lexer.Tokens
{
    internal class StartSubExpressionToken : ExpressionScopeToken
    {
        public override string Value => "(";

        public override TokenType Type => TokenType.StartSubExpression;

        public override string ToString()
        {
            return Value;
        }
    }
}