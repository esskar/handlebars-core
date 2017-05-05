using System;
using System.Linq.Expressions;

namespace Handlebars.Compiler.Structure
{
    internal class CommentExpression : HandlebarsExpression
    {
        public string Value { get; private set; }

        public override ExpressionType NodeType => (ExpressionType) HandlebarsExpressionType.CommentExpression;

        public override Type Type => typeof (void);

        public CommentExpression(string value)
        {
            Value = value;
        }
    }
}