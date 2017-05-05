using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Handlebars.Compiler.Structure
{
    internal class HelperExpression : HandlebarsExpression
    {
        public HelperExpression(string helperName, IEnumerable<Expression> arguments)
            : this(helperName)
        {
            Arguments = arguments;
        }

        public HelperExpression(string helperName)
        {
            HelperName = helperName;
            Arguments = Enumerable.Empty<Expression>();
        }

        public override ExpressionType NodeType => (ExpressionType)HandlebarsExpressionType.HelperExpression;

        public override Type Type => typeof(void);

        public string HelperName { get; }

        public IEnumerable<Expression> Arguments { get; }
    }
}

