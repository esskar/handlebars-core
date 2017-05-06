using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Handlebars.Core.Compiler.Structure;

namespace Handlebars.Core.Compiler.Lexer.Converter.BlockAccumulators
{
    internal class DeferredBlockAccumulatorContext : BlockAccumulatorContext
    {
        private readonly PathExpression _startingNode;
        private List<Expression> _body = new List<Expression>();
        private BlockExpression _accumulatedBody;
        private BlockExpression _accumulatedInversion;


        public DeferredBlockAccumulatorContext(Expression startingNode)
        {
            _startingNode = (PathExpression)UnwrapStatement(startingNode);
        }

        public override Expression GetAccumulatedBlock()
        {
            if (_accumulatedBody == null)
            {
                _accumulatedBody = Expression.Block(_body);
                _accumulatedInversion = Expression.Block(Expression.Empty());
            }
            else if (_accumulatedInversion == null && _body.Any())
            {
                _accumulatedInversion = Expression.Block(_body);
            }
            else
            {
                _accumulatedInversion = Expression.Block(Expression.Empty());
            }

            return HandlebarsExpression.DeferredSectionExpression(
                _startingNode,
                _accumulatedBody,
                _accumulatedInversion);
        }

        public override void HandleElement(Expression item)
        {
            if (IsInversionBlock(item))
            {
                _accumulatedBody = Expression.Block(_body);
                _body = new List<Expression>();
            }
            else
            {
                _body.Add(item);
            }
        }

        public override bool IsClosingElement(Expression item)
        {
            var blockName = _startingNode.Path.Replace("#", "").Replace("^", "");
            var pathExpression = UnwrapStatement(item) as PathExpression;
            return pathExpression != null && pathExpression.Path == "/" + blockName;
        }

        private static bool IsInversionBlock(Expression item)
        {
            var helperExpression = UnwrapStatement(item) as HelperExpression;
            return helperExpression != null && helperExpression.HelperName == "else";
        }
    }
}

