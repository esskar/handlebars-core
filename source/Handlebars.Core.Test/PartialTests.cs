using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Handlebars.Core.Test
{
    public class PartialTests
    {
        [Fact]
        public void BasicPartial()
        {
            string source = "Hello, {{>person}}!";

            var engine = new HandlebarsEngine();
            var template = engine.Compile(source);

            var data = new {
                name = "Marc"
            };

            var partialSource = "{{name}}";
            using(var reader = new StringReader(partialSource))
            {
                var partialTemplate = engine.Compile(reader);
                engine.RegisterTemplate("person", partialTemplate);
            }

            var result = template.Render(data);
            Assert.Equal("Hello, Marc!", result);
        }

        [Fact]
        public void BasicStringOnlyPartial()
        {
            string source = "Hello, {{>person}}!";

            var engine = new HandlebarsEngine();
            var template = engine.Compile(source);

            var data = new {
                name = "Marc"
            };

            var partialSource = "{{name}}";           
            engine.RegisterTemplate("person", partialSource);            

            var result = template.Render(data);
            Assert.Equal("Hello, Marc!", result);
        }

        [Fact]
        public void BasicPartialWithContext()
        {
            string source = "Hello, {{>person leadDev}}!";

            var engine = new HandlebarsEngine();
            var template = engine.Compile(source);

            var data = new {
                leadDev = new {
                    name = "Marc"
                }
            };

            var partialSource = "{{name}}";
            using(var reader = new StringReader(partialSource))
            {
                var partialTemplate = engine.Compile(reader);
                engine.RegisterTemplate("person", partialTemplate);
            }

            var result = template.Render(data);
            Assert.Equal("Hello, Marc!", result);
        }

        [Fact]
        public void BasicPartialWithStringParameter()
        {
            string source = "Hello, {{>person first='Pete'}}!";

            var engine = new HandlebarsEngine();
            var template = engine.Compile(source);

            var partialSource = "{{first}}";
            using (var reader = new StringReader(partialSource))
            {
                var partialTemplate = engine.Compile(reader);
                engine.RegisterTemplate("person", partialTemplate);
            }

            var result = template.Render(null);
            Assert.Equal("Hello, Pete!", result);
        }

        [Fact]
        public void BasicPartialWithMultipleStringParameters()
        {
            string source = "Hello, {{>person first='Pete' last=\"Sampras\"}}!";

            var engine = new HandlebarsEngine();
            var template = engine.Compile(source);

            var partialSource = "{{first}} {{last}}";
            using (var reader = new StringReader(partialSource))
            {
                var partialTemplate = engine.Compile(reader);
                engine.RegisterTemplate("person", partialTemplate);
            }

            var result = template.Render(null);
            Assert.Equal("Hello, Pete Sampras!", result);
        }

        [Fact]
        public void BasicPartialWithContextParameter()
        {
            string source = "Hello, {{>person first=leadDev.marc}}!";

            var engine = new HandlebarsEngine();
            var template = engine.Compile(source);

            var data = new
            {
                leadDev = new
                {
                    marc = new
                    {
                        name = "Marc"
                    }
                }
            };

            var partialSource = "{{first.name}}";
            using (var reader = new StringReader(partialSource))
            {
                var partialTemplate = engine.Compile(reader);
                engine.RegisterTemplate("person", partialTemplate);
            }

            var result = template.Render(data);
            Assert.Equal("Hello, Marc!", result);
        }

        [Fact]
        public void BasicPartialWithContextAndStringParameters()
        {
            string source = "Hello, {{>person first=leadDev.marc last='Smith'}}!";

            var engine = new HandlebarsEngine();
            var template = engine.Compile(source);

            var data = new
            {
                leadDev = new
                {
                    marc = new
                    {
                        name = "Marc"
                    }
                }
            };

            var partialSource = "{{first.name}} {{last}}";
            using (var reader = new StringReader(partialSource))
            {
                var partialTemplate = engine.Compile(reader);
                engine.RegisterTemplate("person", partialTemplate);
            }

            var result = template.Render(data);
            Assert.Equal("Hello, Marc Smith!", result);
        }

        [Fact]
        public void BasicPartialWithTypedParameters()
        {
            string source = "Hello, {{>person first=1 last=true}}!";

            var engine = new HandlebarsEngine();
            var template = engine.Compile(source);

            var partialSource = "{{first}} {{last}}";
            using (var reader = new StringReader(partialSource))
            {
                var partialTemplate = engine.Compile(reader);
                engine.RegisterTemplate("person", partialTemplate);
            }

            var result = template.Render(null);
            Assert.Equal("Hello, 1 True!", result);
        }

        [Fact]
        public void BasicPartialWithStringParameterIncludingExpressionChars()
        {
            string source = "Hello, {{>person first='Pe ({~te~}) '}}!";

            var engine = new HandlebarsEngine();
            var template = engine.Compile(source);

            var partialSource = "{{first}}";
            using (var reader = new StringReader(partialSource))
            {
                var partialTemplate = engine.Compile(reader);
                engine.RegisterTemplate("person", partialTemplate);
            }

            var result = template.Render(null);
            Assert.Equal("Hello, Pe ({~te~}) !", result);
        }

        [Fact]
        public void DynamicPartial()
        {
            string source = "Hello, {{> (partialNameHelper)}}!";

            var engine = new HandlebarsEngine();
            engine.RegisterHelper("partialNameHelper", (writer, context, args) =>
            {
                writer.WriteSafeString("partialName");
            });

            using (var reader = new StringReader("world"))
            {
                var partial = engine.Compile(reader);
                engine.RegisterTemplate("partialName", partial);
            }

            var template = engine.Compile(source);
            var data = new { };
            var result = template.Render(data);
            Assert.Equal("Hello, world!", result);
        }

        [Fact]
        public void DynamicPartialWithHelperArguments()
        {
            string source = "Hello, {{> (concat 'par' 'tial' item1='Na' item2='me')}}!";

            var engine = new HandlebarsEngine();
            engine.RegisterHelper("concat", (writer, context, args) =>
            {
                var hash = args[2] as Dictionary<string, object>;
                writer.WriteSafeString(string.Concat(args[0], args[1], hash["item1"], hash["item2"]));
            });

            using (var reader = new StringReader("world"))
            {
                var partial = engine.Compile(reader);
                engine.RegisterTemplate("partialName", partial);
            }

            var template = engine.Compile(source);
            var data = new { };
            var result = template.Render(data);
            Assert.Equal("Hello, world!", result);
        }

        [Fact]
        public void DynamicPartialWithContext()
        {
            var source = "Hello, {{> (lookup name) context }}!";

            var engine = new HandlebarsEngine();
            engine.RegisterHelper("lookup", (output, context, arguments) =>
            {
                output.WriteSafeString(arguments[0]);
            });

            var template = engine.Compile(source);

            using (var reader = new StringReader("{{first}} {{last}}"))
            {
                var partialTemplate = engine.Compile(reader);
                engine.RegisterTemplate("test", partialTemplate);
            }

            var data = new
            {
                name = "test",
                context = new
                {
                    first = "Marc",
                    last = "Smith"
                }
            };

            var result = template.Render(data);
            Assert.Equal("Hello, Marc Smith!", result);
        }

        [Fact]
        public void DynamicPartialWithParameters()
        {
            var source = "Hello, {{> (lookup name) first='Marc' last='Smith' }}!";

            var engine = new HandlebarsEngine();
            engine.RegisterHelper("lookup", (output, context, arguments) =>
            {
                output.WriteSafeString(arguments[0]);
            });

            var template = engine.Compile(source);

            using (var reader = new StringReader("{{first}} {{last}}"))
            {
                var partialTemplate = engine.Compile(reader);
                engine.RegisterTemplate("test", partialTemplate);
            }

            var data = new
            {
                name = "test"
            };

            var result = template.Render(data);
            Assert.Equal("Hello, Marc Smith!", result);
        }

        [Fact]
        public void SuperfluousWhitespace()
        {
            string source = "Hello, {{  >  person  }}!";

            var engine = new HandlebarsEngine();
            var template = engine.Compile(source);

            var data = new {
                name = "Marc"
            };

            var partialSource = "{{name}}";
            using(var reader = new StringReader(partialSource))
            {
                var partialTemplate = engine.Compile(reader);
                engine.RegisterTemplate("person", partialTemplate);
            }

            var result = template.Render(data);
            Assert.Equal("Hello, Marc!", result);
        }

        [Fact]
        public void BasicPartialWithStringParametersAndImplicitContext()
        {
            string source = "Hello, {{>person lastName='Smith'}}!";

            var engine = new HandlebarsEngine();
            var template = engine.Compile(source);

            var data = new
            {
                firstName = "Marc",
                lastName = "Jones"
            };

            var partialSource = "{{firstName}} {{lastName}}";
            using (var reader = new StringReader(partialSource))
            {
                var partialTemplate = engine.Compile(reader);
                engine.RegisterTemplate("person", partialTemplate);
            }

            var result = template.Render(data);
            Assert.Equal("Hello, Marc Smith!", result);
        }

        [Fact]
        public void BasicPartialWithEmptyParameterDoesNotFallback()
        {
            string source = "Hello, {{>person lastName=test}}!";

            var engine = new HandlebarsEngine();
            var template = engine.Compile(source);

            var data = new
            {
                firstName = "Marc",
                lastName = "Jones"
            };

            var partialSource = "{{firstName}} {{lastName}}";
            using (var reader = new StringReader(partialSource))
            {
                var partialTemplate = engine.Compile(reader);
                engine.RegisterTemplate("person", partialTemplate);
            }

            var result = template.Render(data);
            Assert.Equal("Hello, Marc !", result);
        }

        [Fact]
        public void BasicPartialWithIncompleteChildContextDoesNotFallback()
        {
            string source = "Hello, {{>person leadDev}}!";

            var engine = new HandlebarsEngine();
            var template = engine.Compile(source);

            var data = new
            {
                firstName = "Pete",
                lastName = "Jones",
                leadDev = new
                {
                    firstName = "Marc"
                }
            };

            var partialSource = "{{firstName}} {{lastName}}";
            using (var reader = new StringReader(partialSource))
            {
                var partialTemplate = engine.Compile(reader);
                engine.RegisterTemplate("person", partialTemplate);
            }

            var result = template.Render(data);
            Assert.Equal("Hello, Marc !", result);
        }

        [Fact]
        public void BasicBlockPartial()
        {
            string source = "Hello, {{#>person1}}friend{{/person1}}!";

            var engine = new HandlebarsEngine();
            var template = engine.Compile(source);

            var data = new {
                firstName = "Pete",
                lastName = "Jones"
            };

            var result1 = template.Render(data);
            Assert.Equal ("Hello, friend!", result1);

            var partialSource = "{{firstName}} {{lastName}}";
            using (var reader = new StringReader(partialSource)) {
                var partialTemplate = engine.Compile(reader);
                engine.RegisterTemplate("person1", partialTemplate);
            }

            var result2 = template.Render(data);
            Assert.Equal("Hello, Pete Jones!", result2);
        }

        [Fact]
        public void BasicBlockPartialWithArgument()
        {
            string source = "Hello, {{#>person2 arg='Todd'}}friend{{/person2}}!";

            var engine = new HandlebarsEngine();
            var template = engine.Compile (source);

            var data = new {
                firstName = "Pete",
                lastName = "Jones"
            };

            var result1 = template.Render(data);
            Assert.Equal ("Hello, friend!", result1);

            var partialSource = "{{arg}}";
            using (var reader = new StringReader (partialSource)) {
                var partialTemplate = engine.Compile (reader);
                engine.RegisterTemplate ("person2", partialTemplate);
            }

            var result2 = template.Render(data);
            Assert.Equal ("Hello, Todd!", result2);
        }
    }
}

