namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class EndSubExpressionToken : ExpressionScopeToken
    {
        public override string Value => ")";

        public override TokenType Type => TokenType.EndSubExpression;

        public override string ToString()
        {
            return Value;
        }
    }
}

