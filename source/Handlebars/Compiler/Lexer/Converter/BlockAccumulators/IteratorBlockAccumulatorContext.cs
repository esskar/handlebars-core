using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Handlebars.Core.Compiler.Structure;

namespace Handlebars.Core.Compiler.Lexer.Converter.BlockAccumulators
{
    internal class IteratorBlockAccumulatorContext : BlockAccumulatorContext
    {
        private readonly HelperExpression _startingNode;
        private Expression _accumulatedExpression;
        private List<Expression> _body = new List<Expression>();

        public IteratorBlockAccumulatorContext(Expression startingNode)
        {
            _startingNode = (HelperExpression)UnwrapStatement(startingNode);
        }

        public string BlockName => _startingNode.HelperName;

        public override void HandleElement(Expression item)
        {
            if (IsElseBlock(item))
            {
                _accumulatedExpression = HandlebarsExpression.IteratorExpression(
                    _startingNode.Arguments.Single(),
                    Expression.Block(_body));
                _body = new List<Expression>();
            }
            else
            {
                _body.Add(item);
            }
        }

        public override bool IsClosingElement(Expression item)
        {
            if (!IsClosingNode(item))
                return false;

            if (_accumulatedExpression == null)
            {
                _accumulatedExpression = HandlebarsExpression.IteratorExpression(
                    _startingNode.Arguments.Single(),
                    Expression.Block(_body));
            }
            else
            {
                _accumulatedExpression = HandlebarsExpression.IteratorExpression(
                    ((IteratorExpression)_accumulatedExpression).Sequence,
                    ((IteratorExpression)_accumulatedExpression).Template,
                    Expression.Block(_body));
            }
            return true;
        }

        public override Expression GetAccumulatedBlock()
        {
            return _accumulatedExpression;
        }

        private static bool IsClosingNode(Expression item)
        {
            var pathExpression = UnwrapStatement(item) as PathExpression;
            return pathExpression != null && pathExpression.Path.Replace("#", "") == "/each";
        }

        private static bool IsElseBlock(Expression item)
        {
            var helperExpression = UnwrapStatement(item) as HelperExpression;
            return helperExpression != null && helperExpression.HelperName == "else";
        }
    }
}

