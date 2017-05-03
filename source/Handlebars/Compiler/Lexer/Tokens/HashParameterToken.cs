namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class HashParameterToken : ExpressionToken
    {
        public HashParameterToken(string parameter)
        {
            Value = parameter;
        }

        public override TokenType Type => TokenType.HashParameter;

        public override string Value { get; }
    }
}

