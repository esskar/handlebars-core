using Handlebars.Core.Compiler.Resolvers;
using Handlebars.Core.Encoders;
using Newtonsoft.Json;
using System;
using Xunit;

namespace Handlebars.Core.Test
{
    public class CustomConfigurationTests
    {
        public IHandlebarsEngine HandlebarsInstance { get; }
        public const string ExpectedOutput = "Hello Eric Sharp from Japan. You're <b>AWESOME</b>.";
        public object Value = new
                    {
                        Person = new { Name = "Eric", Surname = "Sharp", Address = new { HomeCountry = "Japan" } },
                        Description = @"<b>AWESOME</b>"
                    };

        public CustomConfigurationTests()
        {
            var configuration = new HandlebarsConfiguration
                                    {
                                        ExpressionNameResolver =
                                            new UpperCamelCaseExpressionNameResolver()
                                    };
                        
            this.HandlebarsInstance = new HandlebarsEngine(configuration);
        }

        #region UpperCamelCaseExpressionNameResolver Tests

        [Fact]
        public void LowerCamelCaseInputModelNaming()
        {
            var template = "Hello {{person.name}} {{person.surname}} from {{person.address.homeCountry}}. You're {{{description}}}.";
            var output = this.HandlebarsInstance.Compile(template).Render(Value);

            Assert.Equal(output, ExpectedOutput);
        }

        [Fact]
        public void UpperCamelCaseInputModelNaming()
        {
            var template = "Hello {{person.name}} {{person.surname}} from {{person.address.homeCountry}}. You're {{{description}}}.";
            var output = this.HandlebarsInstance.Compile(template).Render(Value);

            Assert.Equal(output, ExpectedOutput);
        }

        [Fact]
        public void SnakeCaseInputModelNaming()
        {
            var template = "Hello {{person.name}} {{person.surname}} from {{person.address.home_Country}}. You're {{{description}}}.";
            var output = this.HandlebarsInstance.Compile(template).Render(Value);

            Assert.Equal(output, ExpectedOutput);
        }

        #endregion

        #region Custom IOutputEncoding

        private class JsonEncoder : ITextEncoder
        {
            public string Encode(string value)
            {
                return value != null ? JsonConvert.ToString(value, '"').Trim('"') : String.Empty;
            }
        }


        [Fact]
        public void NoOutputEncoding()
        {
            var template =
                "Hello {{person.name}} {{person.surname}} from {{person.address.homeCountry}}. You're {{description}}.";


            var configuration = new HandlebarsConfiguration
                                    {
                                        TextEncoder = null
                                    };

            var handlebarsInstance = new HandlebarsEngine(configuration);

            var output = handlebarsInstance.Compile(template).Render(Value);

            Assert.Equal(ExpectedOutput, output);
        }

        [Fact]
        public void JsonEncoding()
        {
            var template = "No html entities, {{Username}}.";


            var configuration = new HandlebarsConfiguration
                                    {
                                        TextEncoder = new JsonEncoder()
                                    };

            var handlebarsInstance = new HandlebarsEngine(configuration);

            var value = new {Username = "\"<Eric>\"\n<Sharp>"};
            var output = handlebarsInstance.Compile(template).Render(value);

            Assert.Equal(@"No html entities, \""<Eric>\""\n<Sharp>.", output);
        }

        #endregion
    }
}