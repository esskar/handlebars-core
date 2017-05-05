using System;
using Handlebars.Compiler;
using System.Collections;
using System.Linq;
using Handlebars.Compiler.Structure;

namespace Handlebars
{
    public static class HandlebarsUtils
    {
        public static bool IsTruthy(object value)
        {
            return !IsFalsy(value);
        }

        public static bool IsFalsy(object value)
        {
            if (value is UndefinedBindingResult)
            {
                return true;
            }
            if (value == null)
            {
                return true;
            }
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            if (value is string stringValue)
            {
                return stringValue == "";
            }
            if (IsNumber(value))
            {
                return !Convert.ToBoolean(value);
            }
            return false;
        }

        public static bool IsTruthyOrNonEmpty(object value)
        {
            return !IsFalsyOrEmpty(value);
        }

        public static bool IsFalsyOrEmpty(object value)
        {
            if(IsFalsy(value))
            {
                return true;
            }
            var enumerable = value as IEnumerable;
            return enumerable != null && !enumerable.OfType<object>().Any();
        }

        private static bool IsNumber(object value)
        {
            return value is sbyte
                || value is byte
                || value is short
                || value is ushort
                || value is int
                || value is uint
                || value is long
                || value is ulong
                || value is float
                || value is double
                || value is decimal;
        }
    }
}

