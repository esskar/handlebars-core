using System.Collections.Generic;
using System.Linq.Expressions;
using Handlebars.Core.Compiler.Lexer.Tokens;
using Handlebars.Core.Compiler.Structure;

namespace Handlebars.Core.Compiler.Lexer.Converter
{
    internal class ExpressionScopeConverter : ITokenConverter
    {
        public IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            var enumerator = sequence.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                var startExpression = item as StartExpressionToken;

                if (startExpression == null)
                {
                    yield return item;
                    continue;
                }

                var possibleBody = GetNext(enumerator);
                if (!(possibleBody is Expression))
                {
                    throw new HandlebarsCompilerException(string.Format("Token '{0}' could not be converted to an expression", possibleBody));
                }

                var endExpression = GetNext(enumerator) as EndExpressionToken;
                if (endExpression == null)
                {
                    throw new HandlebarsCompilerException("Handlebars statement was not reduced to a single expression");
                }

                if (endExpression.IsEscaped != startExpression.IsEscaped)
                {
                    throw new HandlebarsCompilerException("Starting and ending handlebars do not match");
                }

                yield return HandlebarsExpression.StatementExpression(
                    (Expression) possibleBody,
                    startExpression.IsEscaped,
                    startExpression.TrimPreceedingWhitespace,
                    endExpression.TrimTrailingWhitespace);
            }
        }

        private static object GetNext(IEnumerator<object> enumerator)
        {
            enumerator.MoveNext();
            return enumerator.Current;
        }
    }
}