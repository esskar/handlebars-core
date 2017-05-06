using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace Handlebars.Core.Test.Providers
{
    public class FileSystemTemplateContentProviderTests
    {
        [Fact]
        public void CanLoadAViewWithALayout()
        {
            //Given a layout in a subfolder
            var configuration = new HandlebarsConfiguration
            {
                TemplateContentProvider = new FakeFileSystemTemplateContentProvider
                {
                    {"views\\somelayout.hbs", "layout start\r\n{{{body}}}\r\nlayout end"},
                    //And a view in the same folder which uses that layout
                    {"views\\someview.hbs", "{{!< somelayout}}This is the body"}
                }
            };

            //When a viewengine renders that view
            var handleBars = new HandlebarsEngine(configuration);
            var renderView = handleBars.CompileView("views\\someview.hbs");
            var output = renderView.Render(null);
            
            //Then the correct output should be rendered
            Assert.Equal("layout start\r\nThis is the body\r\nlayout end", output);
        }
        [Fact]
        public void CanLoadAViewWithALayoutInTheRoot()
        {
            //Given a layout in the root
            var configuration = new HandlebarsConfiguration
            {
                TemplateContentProvider = new FakeFileSystemTemplateContentProvider
                {
                    {"somelayout.hbs", "layout start\r\n{{{body}}}\r\nlayout end"},
                    //And a view in a subfolder folder which uses that layout
                    {"views\\someview.hbs", "{{!< somelayout}}This is the body"}
                }
            };

            //When a viewengine renders that view
            var handlebars = new HandlebarsEngine(configuration);
            var render = handlebars.CompileView("views\\someview.hbs");
            var output = render.Render(null);

            //Then the correct output should be rendered
            Assert.Equal("layout start\r\nThis is the body\r\nlayout end", output);
        }

        [Fact]
        public void CanRenderAGlobalVariable()
        {
            //Given a layout in the root which contains an @ variable
            var configuration = new HandlebarsConfiguration
            {
                TemplateContentProvider = new FakeFileSystemTemplateContentProvider
                {
                    {"views\\someview.hbs", "This is the {{@body.title}}"}
                }
            };

            //When a viewengine renders that view
            var handlebars = new HandlebarsEngine(configuration);
            var render = handlebars.CompileView("views\\someview.hbs");
            var output = render.Render(new {@body = new {title = "THING"}});

            //Then the correct output should be rendered
            Assert.Equal("This is the THING", output);
        }

        //We have a fake file system. Difference frameworks and apps will use 
        //different file systems.
        private class FakeFileSystemTemplateContentProvider : FileSystemTemplateContentProvider, IEnumerable
        {
            private readonly SortedDictionary<string, string> _files = new SortedDictionary<string, string>();

            public void Add(string fileName, string fileContent)
            {
                _files[Sanitise(fileName)] = fileContent;
            }

            protected override string GetFileContent(string filename)
            {
                _files.TryGetValue(Sanitise(filename), out string content);
                return content;
            }

            private static string Sanitise(string filename)
            {
                return filename.Replace("\\", "/");
            }

            protected override string CombinePath(string dir, string otherFileName)
            {
                var fullFileName = dir + "/" + otherFileName;
                fullFileName = fullFileName.TrimStart('/');
                return fullFileName;
            }

            protected override bool FileExists(string filePath)
            {
                return _files.ContainsKey(Sanitise(filePath));
            }

            public IEnumerator GetEnumerator()
            {
                return _files.GetEnumerator();
            }
        }
    }
}
