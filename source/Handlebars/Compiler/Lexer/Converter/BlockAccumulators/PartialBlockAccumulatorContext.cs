using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace HandlebarsDotNet.Compiler
{
    internal class PartialBlockAccumulatorContext : BlockAccumulatorContext
    {
        private readonly PartialExpression _startingNode;
        private string _blockName;
        private readonly List<Expression> _body = new List<Expression>();

        public PartialBlockAccumulatorContext(Expression startingNode)
        {
            _startingNode = ConvertToPartialExpression(UnwrapStatement(startingNode));
        }

        public override void HandleElement(Expression item)
        {
            _body.Add(item);
        }

        public override Expression GetAccumulatedBlock()
        {
            return HandlebarsExpression.PartialExpression(
                _startingNode.PartialName,
                _startingNode.Argument,
                _body.Count > 1 ? Expression.Block(_body) : _body.First());
        }

        public override bool IsClosingElement(Expression item)
        {
            item = UnwrapStatement(item);
            return IsClosingNode(item);
        }

        private bool IsClosingNode(Expression item)
        {
            var pathExpression = item as PathExpression;
            return pathExpression != null && pathExpression.Path == "/" + _blockName;
        }

        private PartialExpression ConvertToPartialExpression(Expression expression)
        {
            if (expression is PathExpression pathExpression)
            {
                _blockName = pathExpression.Path.Replace ("#>", "");
                return HandlebarsExpression.PartialExpression(Expression.Constant(_blockName));
            }
            if (expression is HelperExpression helperExpression)
            {
                _blockName = helperExpression.HelperName.Replace ("#>", "");
                if (!helperExpression.Arguments.Any())
                {
                    return HandlebarsExpression.PartialExpression(Expression.Constant(_blockName));
                }
                if (helperExpression.Arguments.Count() == 1)
                {
                    return HandlebarsExpression.PartialExpression(
                        Expression.Constant(_blockName),
                        helperExpression.Arguments.First());
                }
                throw new InvalidOperationException("Cannot convert a multi-argument helper expression to a partial expression");
            }
            throw new InvalidOperationException(string.Format("Cannot convert '{0}' to a partial expression", expression));
        }
    }
}

