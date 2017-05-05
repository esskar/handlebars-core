using System.Collections.Generic;
using System.Linq.Expressions;

namespace Handlebars.Compiler.Structure
{
    internal class BlockHelperExpression : HelperExpression
    {
        public BlockHelperExpression(
            string helperName,
            IEnumerable<Expression> arguments,
            Expression body,
            Expression inversion)
            : base(helperName, arguments)
        {
            Body = body;
            Inversion = inversion;
        }

        public Expression Body { get; }

        public Expression Inversion { get; }

        public override ExpressionType NodeType => (ExpressionType)HandlebarsExpressionType.BlockExpression;
    }
}

