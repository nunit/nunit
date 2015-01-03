// ***********************************************************************
// Copyright (c) 2008-2014 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.IO;
using System.Collections;
using NUnit.Engine.Tests.resources;
using NUnit.Framework;

namespace NUnit.Engine.Services.ProjectLoaders.Tests
{
    [TestFixture]
    public class VSProjectTests
    {
        private string invalidFile = Path.Combine(Path.GetTempPath(), "invalid.csproj");

        private void WriteInvalidFile( string text )
        {
            StreamWriter writer = new StreamWriter( invalidFile );
            writer.WriteLine( text );
            writer.Close();
        }

        [TearDown]
        public void EraseInvalidFile()
        {
            if ( File.Exists( invalidFile ) )
                File.Delete( invalidFile );
        }

        [Test]
        public void NotWebProject()
        {
            Assert.IsFalse(VSProject.IsProjectFile(@"http://localhost/web.csproj"));
            Assert.IsFalse(VSProject.IsProjectFile(@"\MyProject\http://localhost/web.csproj"));
        }

        [Test]
        public void LoadInvalidFileType()
        {
            Assert.Throws<ArgumentException>(() => new VSProject(@"/test.junk"));
        }

        [Test]
        public void FileNotFoundError()
        {
            Assert.Throws<FileNotFoundException>(() => new VSProject(@"/junk.csproj"));
        }

        [TestCase("<VisualStudioProject><junk></VisualStudioProject>")]
        [TestCase("<VisualStudioProject><junk></junk></VisualStudioProject>")]
        public void InvalidXmlFormat(string invalidXml)
        {
            WriteInvalidFile(invalidXml);
            Assert.Throws<ArgumentException>(() => new VSProject(Path.Combine(Path.GetTempPath(), "invalid.csproj")));
        }

        [Test]
        public void NoConfigurations()
        {
            WriteInvalidFile("<VisualStudioProject><CSharp><Build><Settings AssemblyName=\"invalid\" OutputType=\"Library\"></Settings></Build></CSharp></VisualStudioProject>");
            VSProject project = new VSProject(Path.Combine(Path.GetTempPath(), "invalid.csproj"));
            Assert.AreEqual(0, project.ConfigNames.Count);
        }
    }
}
