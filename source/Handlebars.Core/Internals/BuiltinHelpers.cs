using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Handlebars.Core.Internals
{
    internal static class BuiltinHelpers
    {
        private static IList<Type> _builtinTypes;

        public static IEnumerable<IHandlebarsHelper> Helpers => GetHelpers<IHandlebarsHelper>();

        public static IEnumerable<IHandlebarsBlockHelper> BlockHelpers => GetHelpers<IHandlebarsBlockHelper>();

        private static IEnumerable<T> GetHelpers<T>()
        {
            if (_builtinTypes == null)
            {
                Assembly assembly;
#if netstandard
                assembly = typeof(BuiltinHelpers).GetTypeInfo().Assembly;
#else
                assembly = typeof(BuiltinHelpers).Assembly;
#endif
                _builtinTypes = assembly.GetTypes();
            }

            var instances = new List<T>();

            var type = typeof(T);
            foreach (var builtinType in _builtinTypes.Where(t =>
#if netstandard
                t.GetTypeInfo().IsClass
#else
                t.IsClass
#endif
                && type.IsAssignableFrom(t)))
            {
                var instance = CreateInstance(builtinType);
                if (instance != null)
                    instances.Add((T)instance);
            }

            return instances;
        }

        private static object CreateInstance(Type type)
        {
            try
            {
                var constructor = type.GetConstructor(new Type[] {});
                return constructor?.Invoke(new object[] { });
            }
            catch
            {
                return null;
            }
        }
    }
}

