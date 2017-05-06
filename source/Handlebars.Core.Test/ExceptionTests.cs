using Handlebars.Core.Compiler;
using Xunit;

namespace Handlebars.Core.Test
{
    public class ExceptionTests
    {
        [Fact]
        public void TestNonClosingBlockExpressionException()
        {
            Assert.Throws<HandlebarsCompilerException>(() =>
            {
                var engine = new HandlebarsEngine();
                engine.Compile("{{#if 0}}test").Render(new { });
            });
        }
    }
}
