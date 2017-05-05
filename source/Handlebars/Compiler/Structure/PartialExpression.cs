using System;
using System.Linq.Expressions;

namespace Handlebars.Compiler.Structure
{
    internal class PartialExpression : HandlebarsExpression
    {
        public PartialExpression(Expression partialName, Expression argument, Expression fallback)
        {
            PartialName = partialName;
            Argument = argument;
            Fallback = fallback;
        }

        public override ExpressionType NodeType => (ExpressionType)HandlebarsExpressionType.PartialExpression;

        public override Type Type => typeof(void);

        public Expression PartialName { get; }

        public Expression Argument { get; }

        public Expression Fallback { get; }
    }
}

