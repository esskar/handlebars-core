using System.Collections.Generic;
using System.Linq.Expressions;
using HandlebarsDotNet.Compiler.Lexer;

namespace HandlebarsDotNet.Compiler
{
    internal class PartialConverter : ITokenConverter
    {
        public IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            var enumerator = sequence.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if (item is PartialToken)
                {
                    var arguments = AccumulateArguments(enumerator);
                    if (arguments.Count == 0)
                    {
                        throw new HandlebarsCompilerException("A partial must have a name");
                    }

                    var partialName = arguments[0];

                    if (partialName is PathExpression pathExpression)
                    {
                        partialName = Expression.Constant(pathExpression.Path);
                    }

                    switch (arguments.Count)
                    {
                        case 1:
                            yield return HandlebarsExpression.PartialExpression(partialName);
                            break;
                        case 2:
                            yield return HandlebarsExpression.PartialExpression(partialName, arguments [1]);
                            break;
                        default:
                            throw new HandlebarsCompilerException("A partial can only accept 0 or 1 arguments");
                    }
                    yield return enumerator.Current;
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
                {
                    throw new HandlebarsCompilerException($"Token '{item}' could not be converted to an expression");
                }
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

