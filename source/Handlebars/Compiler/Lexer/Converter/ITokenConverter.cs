using System.Collections.Generic;

namespace HandlebarsDotNet.Compiler
{
    internal interface ITokenConverter
    {
        IEnumerable<object> ConvertTokens(IEnumerable<object> sequence);
    }
}

