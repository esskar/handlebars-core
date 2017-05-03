using System.Linq;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal abstract class BlockAccumulatorContext
    {
        public static BlockAccumulatorContext Create(Expression item, HandlebarsConfiguration configuration)
        {
            BlockAccumulatorContext context = null;
            if (IsConditionalBlock(item))
            {
                context = new ConditionalBlockAccumulatorContext(item);
            }
            else if (IsPartialBlock(item))
            {
                context = new PartialBlockAccumulatorContext(item);
            }
            else if (IsBlockHelper(item, configuration))
            {
                context = new BlockHelperAccumulatorContext(item);
            }
            else if (IsIteratorBlock(item))
            {
                context = new IteratorBlockAccumulatorContext(item);
            }
            else if (IsDeferredBlock(item))
            {
                context = new DeferredBlockAccumulatorContext(item);
            }
            return context;
        }

        public string Name
        {
            get
            {
                if (this is BlockHelperAccumulatorContext blockHelperAccumulatorContext)
                    return blockHelperAccumulatorContext.HelperName;

                if (this is ConditionalBlockAccumulatorContext conditionalBlockAccumulatorContext)
                    return conditionalBlockAccumulatorContext.BlockName;

                if (this is IteratorBlockAccumulatorContext iteratorBlockAccumulatorContext)
                    return iteratorBlockAccumulatorContext.BlockName;

                return null;
            }
        }

        private static bool IsConditionalBlock(Expression item)
        {
            item = UnwrapStatement(item);
            var helperExpression = item as HelperExpression;
            return helperExpression != null && new[] { "#if", "#unless" }.Contains(helperExpression.HelperName);
        }

        private static bool IsBlockHelper(Expression item, HandlebarsConfiguration configuration)
        {
            item = UnwrapStatement(item);
            var helperExpression = item as HelperExpression;
            return helperExpression != null && configuration.BlockHelpers.ContainsKey(helperExpression.HelperName.Replace("#", ""));
        }

        private static bool IsIteratorBlock(Expression item)
        {
            item = UnwrapStatement(item);
            var helperExpression = item as HelperExpression;
            return helperExpression != null && new[] { "#each" }.Contains(helperExpression.HelperName);
        }

        private static bool IsDeferredBlock(Expression item)
        {
            item = UnwrapStatement(item);
            var pathExpression = item as PathExpression;
            return pathExpression != null && (pathExpression.Path.StartsWith("#") || pathExpression.Path.StartsWith("^"));
        }

        private static bool IsPartialBlock(Expression item)
        {
            item = UnwrapStatement (item);
            if (item is PathExpression pathExpression)
            {
                return pathExpression.Path.StartsWith("#>");
            }
            if (item is HelperExpression helperExpression)
            {
                return helperExpression.HelperName.StartsWith("#>");
            }
            return false;
        }

        protected static Expression UnwrapStatement(Expression item)
        {
            var statementExpression = item as StatementExpression;
            return statementExpression != null ? statementExpression.Body : item;
        }

        public abstract void HandleElement(Expression item);

        public abstract bool IsClosingElement(Expression item);

        public abstract Expression GetAccumulatedBlock();
    }
}

