using System.Collections.Generic;

namespace Handlebars.Core.Compiler.Lexer.Converter
{
    internal interface ITokenConverter
    {
        IEnumerable<object> ConvertTokens(IEnumerable<object> sequence);
    }
}

