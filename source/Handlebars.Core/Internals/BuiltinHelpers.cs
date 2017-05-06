using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Handlebars.Core.Internals
{
    internal static class BuiltinHelpers
    {
        [Description("with")]
        public static void With(HandlebarsConfiguration configuration, TextWriter output, HandlebarsBlockHelperOptions options, dynamic context, params object[] arguments)
        {
            if (arguments.Length != 1)
            {
                throw new HandlebarsException("{{with}} helper must have exactly one argument");
            }

            if (HandlebarsUtils.IsTruthyOrNonEmpty(arguments[0]))
            {
                options.Template(output, arguments[0]);
            }
            else
            {
                options.Inverse(output, context);
            }
        }

        public static IEnumerable<KeyValuePair<string, HandlebarsHelperV2>> Helpers => GetHelpers<HandlebarsHelperV2>();

        public static IEnumerable<KeyValuePair<string, HandlebarsBlockHelperV2>> BlockHelpers => GetHelpers<HandlebarsBlockHelperV2>();

        private static IEnumerable<KeyValuePair<string, T>> GetHelpers<T>()
        {
            var builtInHelpersType = typeof(BuiltinHelpers);
            foreach (var method in builtInHelpersType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public))
            {
                Delegate possibleDelegate;
                try
                {
#if netstandard
                    possibleDelegate = method.CreateDelegate(typeof(T));
#else
                    possibleDelegate = Delegate.CreateDelegate(typeof(T), method);
#endif
                }
                catch
                {
                    possibleDelegate = null;
                }
                if (possibleDelegate != null)
                {
#if netstandard
                    yield return new KeyValuePair<string, T>(
                        method.GetCustomAttribute<DescriptionAttribute>().Description,
                        (T)(object)possibleDelegate);
#else
                    yield return new KeyValuePair<string, T>(
                        ((DescriptionAttribute)Attribute.GetCustomAttribute(method, typeof(DescriptionAttribute))).Description,
                        (T)(object)possibleDelegate);
#endif
                }
            }
        }
    }
}

