using System;
using System.Linq.Expressions;

namespace Handlebars.Compiler.Structure
{
    internal class PathExpression : HandlebarsExpression
    {
        public PathExpression(string path)
        {
            Path = path;
        }

        public string Path { get; }

        public override ExpressionType NodeType => (ExpressionType)HandlebarsExpressionType.PathExpression;

        public override Type Type => typeof(object);
    }
}

