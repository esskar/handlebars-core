using Handlebars.Compiler;
using Xunit;

namespace Handlebars.Test
{
    public class ExceptionTests
    {
        [Fact]
        public void TestNonClosingBlockExpressionException()
        {
            Assert.Throws<HandlebarsCompilerException>(() =>
            {
                Handlebars.Compile("{{#if 0}}test").Render(new { });
            });
        }
    }
}
