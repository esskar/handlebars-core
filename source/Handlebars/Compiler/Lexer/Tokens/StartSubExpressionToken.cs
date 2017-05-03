namespace HandlebarsDotNet.Compiler.Lexer
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