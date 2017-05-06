using System.IO;
using Xunit;

namespace Handlebars.Core.Test
{
	public class TripleStashTests
	{
		[Fact]
		public void UnencodedPartial()
		{
			string source = "Hello, {{{>unenc_person}}}!";

            var engine = new HandlebarsEngine();
            var template = engine.Compile(source);

			var data = new {
				name = "Marc"
			};

			var partialSource = "<div>{{name}}</div>";
			using(var reader = new StringReader(partialSource))
			{
				var partialTemplate = engine.Compile(reader);
				engine.RegisterTemplate("unenc_person", partialTemplate);
			}

			var result = template.Render(data);
			Assert.Equal("Hello, <div>Marc</div>!", result);
		}

		[Fact]
		public void EncodedPartialWithUnencodedContents()
		{
			string source = "Hello, {{>enc_person}}!";

            var engine = new HandlebarsEngine();
            var template = engine.Compile(source);

			var data = new {
				name = "<div>Marc</div>"
			};

			var partialSource = "<div>{{{name}}}</div>";
			using(var reader = new StringReader(partialSource))
			{
				var partialTemplate = engine.Compile(reader);
				engine.RegisterTemplate("enc_person", partialTemplate);
			}

			var result = template.Render(data);
			Assert.Equal("Hello, <div><div>Marc</div></div>!", result);
		}

		[Fact]
		public void UnencodedObjectEnumeratorItems()
		{
			var source = "{{#each enumerateMe}}{{{this}}} {{/each}}";
            var engine = new HandlebarsEngine();
            var template = engine.Compile(source);
			var data = new
			{
				enumerateMe = new
				{
					foo = "<div>hello</div>",
					bar = "<div>world</div>"
				}
			};
			var result = template.Render(data);
			Assert.Equal("<div>hello</div> <div>world</div> ", result);
		}

        [Fact]
        public void FailingBasicTripleStash()
        {
            string source = "{{#if a_bool}}{{{dangerous_value}}}{{/if}}Hello, {{{dangerous_value}}}!";

            var engine = new HandlebarsEngine();
            var template = engine.Compile(source);

            var data = new
                {
                    a_bool = false,
                    dangerous_value = "<div>There's HTML here</div>"
                };

            var result = template.Render(data);
            Assert.Equal("Hello, <div>There's HTML here</div>!", result);
        }

		[Fact]
        public void UnencodedEncodedUnencoded()
        {
            string source = "{{{dangerous_value}}}...{{dangerous_value}}...{{{dangerous_value}}}!";

            var engine = new HandlebarsEngine();
            var template = engine.Compile(source);

            var data = new
                {
                    a_bool = false,
                    dangerous_value = "<div>There's HTML here</div>"
                };

            var result = template.Render(data);
            Assert.Equal("<div>There's HTML here</div>...&lt;div&gt;There's HTML here&lt;/div&gt;...<div>There's HTML here</div>!", result);
        }
	}
}

