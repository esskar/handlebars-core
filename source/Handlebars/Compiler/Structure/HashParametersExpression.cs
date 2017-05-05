using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Handlebars.Compiler.Structure
{
    internal class HashParametersExpression : HandlebarsExpression
    {
        public Dictionary<string, object> Parameters { get; set; }

        public HashParametersExpression(Dictionary<string, object> parameters)
        {
            Parameters = parameters;
        }

        public override ExpressionType NodeType => (ExpressionType)HandlebarsExpressionType.HashParametersExpression;

        public override Type Type => typeof(object);
    }
}

