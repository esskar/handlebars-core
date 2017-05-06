using System;
using System.Collections.Generic;

namespace Handlebars.Core.Compiler.Translation.Expressions
{
    public class HashParameterDictionary : Dictionary<string, object>
    {
        public TValue GetConvertedValue<TValue>(string key, Func<object, TValue> converter, TValue defaultValue)
        {
            if (!TryGetValue(key, out object valueObject))
                return defaultValue;

            try
            {
                return converter(valueObject);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
    }
}