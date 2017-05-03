using System.Linq.Expressions;
using System.Collections.Generic;

namespace HandlebarsDotNet.Compiler
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

