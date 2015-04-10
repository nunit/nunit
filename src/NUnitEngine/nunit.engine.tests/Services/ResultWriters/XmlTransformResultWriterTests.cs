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

using System.IO;
using NUnit.Engine.Services.ResultWriters;
using NUnit.Framework;

namespace NUnit.Engine.Tests.Services.ResultWriters
{
    public class XmlTransformResultWriterTests : XmlOutputTest
    {
        [Test]
        public void SummaryTransformTest()
        {
            var transformPath = GetLocalPath("TextSummary.xslt");
            var writer = new StringWriter();
            new XmlTransformResultWriter(new object[] { transformPath }).WriteResultFile(EngineResult.Xml, writer);

            var summary = string.Format(
                "Tests Run: {0}, Passed: {1}, Failed: {2}, Inconclusive: {3}, Skipped: {4}",
                EngineResult.Xml.Attributes["total"].Value,
                EngineResult.Xml.Attributes["passed"].Value,
                EngineResult.Xml.Attributes["failed"].Value,
                EngineResult.Xml.Attributes["inconclusive"].Value,
                EngineResult.Xml.Attributes["skipped"].Value);

            var output = writer.GetStringBuilder().ToString();
    
            Assert.That(output, Contains.Substring(summary));
        }
    }
}
