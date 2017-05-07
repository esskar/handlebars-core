using Handlebars.Core.Logging;
using Moq;
using Xunit;

namespace Handlebars.Core.Test
{
    public class LoggerTests
    {
        [Fact]
        public void LogLevelIsInfoByDefault()
        {
            var configuration = new HandlebarsConfiguration();
            Assert.Equal(LogLevel.Info, configuration.LogLevel);
        }

        [Fact]
        public void LoggerIsNotNullByDefault()
        {
            var configuration = new HandlebarsConfiguration();
            Assert.NotNull(configuration.Logger);
        }

        [Fact]
        public void BasicLogging()
        {
            var loggerMock = new Mock<ILogger>();
            var configuration = new HandlebarsConfiguration
            {
                Logger = loggerMock.Object
            };
            var engine = new HandlebarsEngine(configuration);
            var template = engine.Compile("{{log \"Look at me!\"}}");
            template.Render(null);

            loggerMock.Verify(l => l.Log("Look at me!", LogLevel.Info));
        }

        [Fact]
        public void MultiMessageLogging()
        {
            var loggerMock = new Mock<ILogger>();
            var configuration = new HandlebarsConfiguration
            {
                Logger = loggerMock.Object
            };
            var engine = new HandlebarsEngine(configuration);
            var template = engine.Compile("{{log \"This is logged\" foo \"And so is this\"}}");
            template.Render(null);

            loggerMock.Verify(l => l.Log("This is logged", LogLevel.Info));
            loggerMock.Verify(l => l.Log("", LogLevel.Info));
            loggerMock.Verify(l => l.Log("And so is this", LogLevel.Info));
        }

        [Theory]
        [InlineData(LogLevel.Debug)]
        [InlineData(LogLevel.Info)]
        [InlineData(LogLevel.Warn)]
        [InlineData(LogLevel.Error)]
        public void LoglevelIsPassedCorrectly(LogLevel logLevel)
        {
            var loggerMock = new Mock<ILogger>();
            var configuration = new HandlebarsConfiguration
            {
                Logger = loggerMock.Object,
                LogLevel = LogLevel.Debug
            };
            var engine = new HandlebarsEngine(configuration);
            var templateData = $"{{{{log \"Log!\" level=\"{logLevel.ToString().ToLowerInvariant()}\"}}}}";
            var template = engine.Compile(templateData);
            template.Render(null);

            loggerMock.Verify(l => l.Log("Log!", logLevel));
        }

        [Fact]
        public void EverythingAfterLogLevelIsNotLogged()
        {
            var loggerMock = new Mock<ILogger>();
            var configuration = new HandlebarsConfiguration
            {
                Logger = loggerMock.Object
            };
            var engine = new HandlebarsEngine(configuration);
            var template = engine.Compile("{{log \"Logged!\" level=\"error\" \"Not logged!\" }}");
            template.Render(null);

            loggerMock.Verify(l => l.Log("Logged!", LogLevel.Error));
            loggerMock.Verify(l => l.Log("Not logged!", It.IsAny<LogLevel>()), Times.Never);
        }
    }
}
