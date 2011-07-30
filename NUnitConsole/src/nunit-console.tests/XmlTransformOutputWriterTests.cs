using System;
// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Engine;
using NUnit.Framework;

namespace NUnit.ConsoleRunner.Tests
{
    public class XmlTransformOutputWriterTests : XmlOutputTest
    {
        private ITestEngineResult result;
        private string xsltFile;

        [TestFixtureSetUp]
        public void Initialize()
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string dir = Path.GetDirectoryName(uri.LocalPath);

            this.xsltFile = Path.Combine(dir, "TextSummary.xslt");

            this.result = TestEngine.Run(
                new TestPackage(Path.Combine(dir, "mock-assembly.dll")), 
                TestListener.Null, 
                TestFilter.Empty);
        }

        [Test]
        public void SummaryTransformTest()
        {
            StringWriter writer = new StringWriter();
            new XmlTransformOutputWriter(xsltFile).WriteResultFile(result.Xml, writer);

            string summary = string.Format(
                "Tests Run: {0}, Passed: {1}, Failed: {2}, Inconclusive: {3}, Skipped: {4}",
                result.Xml.Attributes["total"].Value,
                result.Xml.Attributes["passed"].Value,
                result.Xml.Attributes["failed"].Value,
                result.Xml.Attributes["inconclusive"].Value,
                result.Xml.Attributes["skipped"].Value);

            string output = writer.GetStringBuilder().ToString();
    
            Assert.That(output, Contains.Substring(summary));
        }
    }
}
