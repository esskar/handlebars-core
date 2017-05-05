using Handlebars.Compiler.Structure;

namespace Handlebars.Compiler.Translation.Expressions
{
    internal class UnencodedStatementVisitor : HandlebarsExpressionVisitor
    {
        public static System.Linq.Expressions.Expression Visit(System.Linq.Expressions.Expression expr, CompilationContext context)
        {
            return new UnencodedStatementVisitor(context).Visit(expr);
        }

        private UnencodedStatementVisitor(CompilationContext context)
            : base(context)
        {
        }

        protected override System.Linq.Expressions.Expression VisitStatementExpression(StatementExpression sex)
        {
            if (sex.IsEscaped == false)
            {
                return System.Linq.Expressions.Expression.Block(
                    typeof(void),
                    System.Linq.Expressions.Expression.Assign(
                        System.Linq.Expressions.Expression.Property(CompilationContext.BindingContext, "SuppressEncoding"),
                        System.Linq.Expressions.Expression.Constant(true)),
                    sex,
                    System.Linq.Expressions.Expression.Assign(
                        System.Linq.Expressions.Expression.Property(CompilationContext.BindingContext, "SuppressEncoding"),
                        System.Linq.Expressions.Expression.Constant(false)),
                    System.Linq.Expressions.Expression.Empty());
            }
            else
            {
                return sex;
            }
        }
    }
}

