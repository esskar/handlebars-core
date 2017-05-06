using System.Collections.Generic;
using System.Linq;
using Handlebars.Core.Compiler.Lexer.Tokens;
using Handlebars.Core.Compiler.Structure;

namespace Handlebars.Core.Compiler.Lexer.Converter
{
    internal class HashParametersConverter : ITokenConverter
    {
        public IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            var enumerator = sequence.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;

                if (item is HashParameterToken)
                {
                    var parameters = AccumulateParameters(enumerator);

                    if (parameters.Any())
                    {
                        yield return HandlebarsExpression.HashParametersExpression(parameters);
                    }

                    yield return enumerator.Current;
                }
                else
                {
                    yield return item;
                }
            }
        }

        private static Dictionary<string, object> AccumulateParameters(IEnumerator<object> enumerator)
        {
            var parameters = new Dictionary<string, object>();

            var item = enumerator.Current;

            while (!(item is EndExpressionToken))
            {
                if (item is HashParameterToken parameter)
                {
                    var segments = parameter.Value.Split('=');
                    var value = ParseValue(segments[1]);
                    parameters.Add(segments[0], value);
                }

                if (item is EndSubExpressionToken)
                {
                    break;
                }

                item = GetNext(enumerator);
            }

            return parameters;
        }

        private static object ParseValue(string value)
        {
            if (value.StartsWith("'") || value.StartsWith("\""))
            {
                return value.Trim('\'', '"');
            }
            if (bool.TryParse(value, out bool boolValue))
            {
                return boolValue;
            }
            if (int.TryParse(value, out int intValue))
            {
                return intValue;
            }

            return HandlebarsExpression.PathExpression(value);
        }

        private static object GetNext(IEnumerator<object> enumerator)
        {
            enumerator.MoveNext();
            return enumerator.Current;
        }
    }
}

