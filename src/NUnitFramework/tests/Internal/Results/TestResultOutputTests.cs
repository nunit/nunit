// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Internal.Results
{
    public class TestResultOutputTests
    {
        private const string SOME_TEXT = "This text should be part of the result.";
        private static readonly string NL = Environment.NewLine;
        private TestResult _result;

        [SetUp]
        public void SetUp()
        {
            _result = NUnit.TestUtilities.Fakes.GetTestMethod(this, nameof(FakeMethod)).MakeTestResult();
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
            Assert.That(outputNode, Is.Not.Null, "No output node found in XML");
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
            Assert.That(outputNode, Is.Not.Null, "No output node found in XML");
            Assert.That(outputNode.Value, Is.EqualTo(SOME_TEXT + NL +
                "More text written in segments." + NL + "Last line!" + NL));
        }

        [Test]
        public void WriteToTestContextOutput()
        {
            TestContext.WriteLine("This is a test!");
            TestContext.WriteLine("The characters &, ', \", < and > must be escaped.");
        }

        [Test]
        public void WriteToTestContextProgress()
        {
            TestContext.Progress.WriteLine("This is a test!");
            TestContext.Progress.WriteLine("The characters &, ', \", < and > must be escaped.");
        }

        private void FakeMethod() { }
    }
}
