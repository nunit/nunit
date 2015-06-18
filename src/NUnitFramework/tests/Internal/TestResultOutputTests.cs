// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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
using System.Collections.Generic;

namespace NUnit.Framework.Internal
{
    public class TestResultOutputTests
    {
        private const string SOME_TEXT = "This text should be part of the result.";
        private static readonly string NL = NUnit.Env.NewLine;
        private TestResult _result;

        [SetUp]
        public void SetUp()
        {
            _result = TestUtilities.Fakes.GetTestMethod(this, "FakeMethod").MakeTestResult();
        }

        [Test]
        public void IfNothingIsWritten_OutputIsEmpty()
        {
            Assert.That(_result.Output, Is.EqualTo(string.Empty));
        }

        [Test]
        public void CanWriteOutputToResult()
        {
            _result.OutWriter.WriteLine(SOME_TEXT);

            Assert.That(_result.Output, Is.EqualTo(SOME_TEXT + NL));
        }

        [Test]
        public void CanWriteOutputToResult_Multiple()
        {
            _result.OutWriter.WriteLine(SOME_TEXT);
            _result.OutWriter.Write("More text ");
            _result.OutWriter.Write("written in ");
            _result.OutWriter.Write("segments.");
            _result.OutWriter.WriteLine();
            _result.OutWriter.WriteLine("Last line!");

            Assert.That(_result.Output, Is.EqualTo(SOME_TEXT + NL +
                "More text written in segments." + NL + "Last line!" + NL));
        }

        [Test]
        public void IfNothingIsWritten_XmlOutputIsEmpty()
        {
            Assert.That(_result.ToXml(false).SelectSingleNode("output"), Is.Null);
        }

        [Test]
        public void CanWriteOutputToResult_XmlOutput()
        {
            _result.OutWriter.WriteLine(SOME_TEXT);

            var outputNode = _result.ToXml(false).SelectSingleNode("output");
            Assert.NotNull(outputNode, "No output node found in XML");
            Assert.That(outputNode.Value, Is.EqualTo(SOME_TEXT + NL));
        }

        [Test]
        public void CanWriteOutputToResult_Multiple_XmlOutput()
        {
            _result.OutWriter.WriteLine(SOME_TEXT);
            _result.OutWriter.Write("More text ");
            _result.OutWriter.Write("written in ");
            _result.OutWriter.Write("segments.");
            _result.OutWriter.WriteLine();
            _result.OutWriter.WriteLine("Last line!");

            var outputNode = _result.ToXml(false).SelectSingleNode("output");
            Assert.NotNull(outputNode, "No output node found in XML");
            Assert.That(outputNode.Value, Is.EqualTo(SOME_TEXT + NL +
                "More text written in segments." + NL + "Last line!" + NL));
        }

        [Test]
        public void WriteToTestContext()
        {
            TestContext.WriteLine("This is a test!");
            TestContext.WriteLine("The characters &, ', \", < and > must be escaped.");
        }

        private void FakeMethod() { }
    }
}
