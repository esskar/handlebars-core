namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class PartialToken : Token
    {
        public override TokenType Type => TokenType.Partial;

        public override string Value => ">";

        public override string ToString()
        {
            return Value;
        }
    }
}