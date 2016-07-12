// ***********************************************************************
// Copyright (c) 2016 Charlie Poole
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
using System.Text;
using NUnit.Framework;

namespace NUnit.ConsoleRunner.Tests
{
    public class TestEventHandlerTests
    {
        private StringBuilder _output;
        private TextWriter _writer;

        private string Output {  get { return _output.ToString(); } }

        [SetUp]
        public void CreateWriter()
        {
            _output = new StringBuilder();
            _writer = new StringWriter(_output);
        }

        [TestCaseSource("EventData")]
        public void EventsWriteExpectedOutput(string report, string labels, string expected)
        {
            var handler = new TestEventHandler(_writer, labels);

            handler.OnTestEvent(report);

            if (Environment.NewLine != "\r\n")
                expected = expected.Replace("\r\n", Environment.NewLine);
            Assert.That(Output, Is.EqualTo(expected));
        }

#pragma warning disable 414
        static TestCaseData[] EventData = new TestCaseData[]
        {
            // Start Events
            new TestCaseData("<start-test/>", "Off", ""),
            new TestCaseData("<start-test/>", "On", ""),
            new TestCaseData("<start-test/>", "All", ""),
            new TestCaseData("<start-suite/>", "Off", ""),
            new TestCaseData("<start-suite/>", "On", ""),
            new TestCaseData("<start-suite/>", "All", ""),
            // Finish Events - No Output
            new TestCaseData("<test-case fullname='SomeName'/>", "Off", ""),
            new TestCaseData("<test-case fullname='SomeName'/>", "On", ""),
            new TestCaseData("<test-case fullname='SomeName'/>", "All", "=> SomeName\r\n"),
            new TestCaseData("<test-suite fullname='SomeName'/>", "Off", ""),
            new TestCaseData("<test-suite fullname='SomeName'/>", "On", ""),
            new TestCaseData("<test-suite fullname='SomeName'/>", "All", ""),
            // Finish Events - With Output
            new TestCaseData(
                "<test-case fullname='SomeName'><output>OUTPUT</output></test-case>",
                "Off",
                "OUTPUT\r\n"),
            new TestCaseData(
                "<test-case fullname='SomeName'><output>OUTPUT</output></test-case>",
                "On",
                "=> SomeName\r\nOUTPUT\r\n"),
            new TestCaseData(
                "<test-case fullname='SomeName'><output>OUTPUT</output></test-case>",
                "All", 
                "=> SomeName\r\nOUTPUT\r\n"),
            new TestCaseData(
                "<test-suite fullname='SomeName'><output>OUTPUT</output></test-suite>",
                "Off", 
                "OUTPUT\r\n"),
            new TestCaseData(
                "<test-suite fullname='SomeName'><output>OUTPUT</output></test-suite>",
                "On",
                "=> SomeName\r\nOUTPUT\r\n"),
            new TestCaseData(
                "<test-suite fullname='SomeName'><output>OUTPUT</output></test-suite>",
                "All",
                "=> SomeName\r\nOUTPUT\r\n"),
            // Output Events
            new TestCaseData(
                "<test-output>OUTPUT</test-output>",
                "Off",
                "OUTPUT\r\n"),
            new TestCaseData(
                "<test-output>OUTPUT</test-output>",
                "On",
                "OUTPUT\r\n"),
            new TestCaseData(
                "<test-output>OUTPUT</test-output>",
                "All",
                "OUTPUT\r\n")
        };
#pragma warning restore 414
    }
}
