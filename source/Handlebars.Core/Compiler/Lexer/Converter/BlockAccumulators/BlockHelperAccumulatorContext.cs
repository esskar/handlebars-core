using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Handlebars.Core.Compiler.Structure;

namespace Handlebars.Core.Compiler.Lexer.Converter.BlockAccumulators
{
    internal class BlockHelperAccumulatorContext : BlockAccumulatorContext
    {
        private readonly HelperExpression _startingNode;
        private Expression _accumulatedBody;
        private Expression _accumulatedInversion;
        private List<Expression> _body = new List<Expression>();

        public BlockHelperAccumulatorContext(Expression startingNode)
        {
            startingNode = UnwrapStatement(startingNode);
            _startingNode = (HelperExpression)startingNode;
        }

        public string HelperName => _startingNode.HelperName;

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

        private static bool IsInversionBlock(Expression item)
        {
            var helperExpression = UnwrapStatement(item) as HelperExpression;
            return helperExpression != null && helperExpression.HelperName == "else";
        }

        public override bool IsClosingElement(Expression item)
        {
            return IsClosingNode(UnwrapStatement(item));
        }

        private bool IsClosingNode(Expression item)
        {
            var helperName = _startingNode.HelperName.Replace("#", "");
            var pathExpression = item as PathExpression;
            return pathExpression != null && pathExpression.Path == "/" + helperName;
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
            return HandlebarsExpression.BlockHelperExpression(
                _startingNode.HelperName,
                _startingNode.Arguments,
                _accumulatedBody,
                _accumulatedInversion);
        }

    }
}

