using System.Linq.Expressions;
using System.Collections.Generic;

namespace HandlebarsDotNet.Compiler
{
    internal enum HandlebarsExpressionType
    {
        StaticExpression = 6000,
        StatementExpression = 6001,
        BlockExpression = 6002,
        HelperExpression = 6003,
        PathExpression = 6004,
        IteratorExpression = 6005,
        DeferredSection = 6006,
        PartialExpression = 6007,
        BoolishExpression = 6008,
        SubExpression = 6009,
        HashParametersExpression = 6010,
		CommentExpression = 6011
    }

    internal abstract class HandlebarsExpression : Expression
    {
        public static HelperExpression HelperExpression(string helperName, IEnumerable<Expression> arguments)
        {
            return new HelperExpression(helperName, arguments);
        }

        public static HelperExpression HelperExpression(string helperName)
        {
            return new HelperExpression(helperName);
        }

        public static BlockHelperExpression BlockHelperExpression(
            string helperName,
            IEnumerable<Expression> arguments,
            Expression body,
            Expression inversion)
        {
            return new BlockHelperExpression(helperName, arguments, body, inversion);
        }

        public static PathExpression PathExpression(string path)
        {
            return new PathExpression(path);
        }

        public static StaticExpression StaticExpression(string value)
        {
            return new StaticExpression(value);
        }

        public static StatementExpression StatementExpression(Expression body, bool isEscaped, bool trimBefore, bool trimAfter)
        {
            return new StatementExpression(body, isEscaped, trimBefore, trimAfter);
        }

        public static IteratorExpression IteratorExpression(
            Expression sequence,
            Expression template)
        {
            return new IteratorExpression(sequence, template);
        }

        public static IteratorExpression IteratorExpression(
            Expression sequence,
            Expression template,
            Expression ifEmpty)
        {
            return new IteratorExpression(sequence, template, ifEmpty);
        }

        public static DeferredSectionExpression DeferredSectionExpression(
            PathExpression path,
            BlockExpression body,
            BlockExpression inversion)
        {
            return new DeferredSectionExpression(path, body, inversion);
        }

        public static PartialExpression PartialExpression(Expression partialName)
        {
            return PartialExpression(partialName, null);
        }

        public static PartialExpression PartialExpression(Expression partialName, Expression argument)
        {
            return new PartialExpression(partialName, argument, null);
        }

        public static PartialExpression PartialExpression(Expression partialName, Expression argument, Expression fallback)
        {
            return new PartialExpression(partialName, argument, fallback);
        }

        public static BoolishExpression BoolishExpression(Expression condition)
        {
            return new BoolishExpression(condition);
        }

        public static SubExpressionExpression SubExpressionExpression(Expression expression)
        {
            return new SubExpressionExpression(expression);
        }

        public static HashParametersExpression HashParametersExpression(Dictionary<string, object> parameters)
        {
            return new HashParametersExpression(parameters);
        }

	    public static CommentExpression CommentExpression(string value)
	    {
		    return new CommentExpression(value);
	    }
    }
}

