using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet.Compiler.Lexer;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal class HelperConverter : ITokenConverter
    {
        private static readonly string[] BuiltInHelpers = { "else", "each" };

        public static IEnumerable<object> Convert(
            IEnumerable<object> sequence,
            HandlebarsConfiguration configuration)
        {
            return new HelperConverter(configuration).ConvertTokens(sequence).ToList();
        }

        private readonly HandlebarsConfiguration _configuration;

        private HelperConverter(HandlebarsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            var enumerator = sequence.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if (item is StartExpressionToken)
                {
                    yield return item;
                    item = GetNext(enumerator);
                    if (item is Expression)
                    {
                        yield return item;
                        continue;
                    }
                    if (item is WordExpressionToken word && IsRegisteredHelperName(word.Value))
                    {
                        yield return HandlebarsExpression.HelperExpression(word.Value);
                    }
                    else
                    {
                        yield return item;
                    }
                }
                else
                {
                    yield return item;
                }
            }
        }


        private bool IsRegisteredHelperName(string name)
        {
            name = name.Replace("#", "");
            return _configuration.Helpers.ContainsKey(name)
                || _configuration.BlockHelpers.ContainsKey(name)
                || BuiltInHelpers.Contains(name);
        }

        private static object GetNext(IEnumerator<object> enumerator)
        {
            enumerator.MoveNext();
            return enumerator.Current;
        }
    }
}

