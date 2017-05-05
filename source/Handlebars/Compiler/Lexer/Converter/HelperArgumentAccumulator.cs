using System.Linq.Expressions;
using System.Collections.Generic;
using HandlebarsDotNet.Compiler.Lexer;

namespace HandlebarsDotNet.Compiler
{
    internal class HelperArgumentAccumulator : ITokenConverter
    {
        public IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            var enumerator = sequence.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if (item is HelperExpression helperExpression)
                {
                    var helperArguments = AccumulateArguments(enumerator);
                    yield return HandlebarsExpression.HelperExpression(
                        helperExpression.HelperName,
                        helperArguments);
                    yield return enumerator.Current;
                }
                else if (item is PathExpression pathExpression)
                {
                    var helperArguments = AccumulateArguments(enumerator);
                    if (helperArguments.Count > 0)
                    {
                        yield return HandlebarsExpression.HelperExpression(
                            pathExpression.Path,
                            helperArguments);
                        yield return enumerator.Current;
                    }
                    else
                    {
                        yield return item;
                        yield return enumerator.Current;
                    }
                }
                else
                {
                    yield return item;
                }
            }
        }

        private static List<Expression> AccumulateArguments(IEnumerator<object> enumerator)
        {
            var item = GetNext(enumerator);
            var helperArguments = new List<Expression>();
            while (!(item is EndExpressionToken))
            {
                if (!(item is Expression))
                    throw new HandlebarsCompilerException(string.Format("Token '{0}' could not be converted to an expression", item));
                helperArguments.Add((Expression)item);
                item = GetNext(enumerator);
            }
            return helperArguments;
        }

        private static object GetNext(IEnumerator<object> enumerator)
        {
            enumerator.MoveNext();
            return enumerator.Current;
        }
    }
}

