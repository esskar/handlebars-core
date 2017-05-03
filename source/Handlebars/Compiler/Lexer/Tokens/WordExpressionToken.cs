﻿namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class WordExpressionToken : ExpressionToken
    {
        public WordExpressionToken(string word)
        {
            Value = word;
        }

        public override TokenType Type => TokenType.Word;

        public override string Value { get; }
    }
}