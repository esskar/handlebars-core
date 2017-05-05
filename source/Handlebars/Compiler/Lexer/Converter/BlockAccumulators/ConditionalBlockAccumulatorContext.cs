using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Handlebars.Compiler.Structure;

namespace Handlebars.Compiler.Lexer.Converter.BlockAccumulators
{
    internal class ConditionalBlockAccumulatorContext : BlockAccumulatorContext
    {
        private readonly List<ConditionalExpression> _conditionalBlock = new List<ConditionalExpression>();
        private Expression _currentCondition;
        private List<Expression> _bodyBuffer = new List<Expression>();
        public string BlockName { get; }

        public ConditionalBlockAccumulatorContext(Expression startingNode)
        {
            var item = (HelperExpression)UnwrapStatement(startingNode);
            BlockName = item.HelperName.Replace("#", "");
            if (new [] { "if", "unless" }.Contains(BlockName) == false)
            {
                throw new HandlebarsCompilerException(string.Format(
                        "Tried to convert {0} expression to conditional block", BlockName));
            }
            var testType = BlockName == "if";
            var argument = HandlebarsExpression.BoolishExpression(item.Arguments.Single());
            _currentCondition = testType ? (Expression)argument : Expression.Not(argument);
        }

        public override void HandleElement(Expression item)
        {
            if (IsElseBlock(item))
            {
                _conditionalBlock.Add(Expression.IfThen(_currentCondition, SinglifyExpressions(_bodyBuffer)));
                _currentCondition = IsElseIfBlock(item) ? GetElseIfTestExpression(item) : null;
                _bodyBuffer = new List<Expression>();
            }
            else
            {
                _bodyBuffer.Add(item);
            }
        }

        public override bool IsClosingElement(Expression item)
        {
            if (!IsClosingNode(item))
                return false;

            if (_currentCondition != null)
            {
                _conditionalBlock.Add(Expression.IfThen(_currentCondition, SinglifyExpressions(_bodyBuffer)));
            }
            else
            {
                var lastCondition = _conditionalBlock.Last();
                _conditionalBlock[_conditionalBlock.Count - 1] = Expression.IfThenElse(
                    lastCondition.Test,
                    lastCondition.IfTrue,
                    SinglifyExpressions(_bodyBuffer));
            }
            return true;
        }

        public override Expression GetAccumulatedBlock()
        {
            return _conditionalBlock
                .AsEnumerable()
                .Reverse()
                .Aggregate<ConditionalExpression, ConditionalExpression>(
                    null, 
                    (current, condition) => 
                        Expression.IfThenElse(condition.Test, condition.IfTrue, current ?? condition.IfFalse));
        }

        private static bool IsElseBlock(Expression item)
        {
            item = UnwrapStatement(item);
            return item is HelperExpression && ((HelperExpression)item).HelperName == "else";
        }

        private static bool IsElseIfBlock(Expression item)
        {
            item = UnwrapStatement(item);
            return IsElseBlock(item) && ((HelperExpression)item).Arguments.Count() == 2;
        }

        private static Expression GetElseIfTestExpression(Expression item)
        {
            item = UnwrapStatement(item);
            return HandlebarsExpression.BoolishExpression(((HelperExpression)item).Arguments.Skip(1).Single());
        }

        private bool IsClosingNode(Expression item)
        {
            item = UnwrapStatement(item);
            var pathExpression = item as PathExpression;
            return pathExpression != null && pathExpression.Path == "/" + BlockName;
        }

        private static Expression SinglifyExpressions(IEnumerable<Expression> expressions)
        {
            Expression singleExpression = null;
            using (var enumerator = expressions.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (singleExpression == null)
                        singleExpression = enumerator.Current;
                    else // we have more than one expression
                        return Expression.Block(expressions);
                }
            }

            if (singleExpression != null)
                return singleExpression;
            
            throw new InvalidOperationException();
        }
    }
}

