// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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
using System.Text;
using System.Xml;
using NUnit.Framework;

namespace NUnit.Engine.Listeners
{
    using System;

    [TestFixture]
    
    public class TeamCityEventListenerTests
    {
        private StringBuilder _output;
        private StringWriter _outputWriter;

        [SetUp]
        public void SetUp()
        {
            _output = new StringBuilder();
            _outputWriter = new StringWriter(_output);            
        }

        [TearDown]
        public void TearDown()
        {
            _outputWriter.Dispose();
        }

        [Test]
        public void ShouldSendMessagesWithValidFlowIdWhenParallelizedTestsFromNUnit3()
        {
            // Given
            var publisher = CreateInstance();

            // When
            publisher.RegisterMessage(CreateStartRun(1));

            // Assembly 1
            publisher.RegisterMessage(CreateStartSuite("1-1", "", "aaa" + Path.DirectorySeparatorChar + "Assembly1"));
            publisher.RegisterMessage(CreateStartSuite("1-2", "1-1", "Assembly1.Namespace1"));
            publisher.RegisterMessage(CreateStartSuite("1-3", "1-2", "Assembly1.Namespace1.1"));

            // Assembly 2
            publisher.RegisterMessage(CreateStartSuite("1-6", "", "ddd" + Path.DirectorySeparatorChar + "Assembly2"));
            publisher.RegisterMessage(CreateStartSuite("1-7", "1-6", "Assembly2.Namespace2"));

            // Test Assembly1.Namespace1.1.Test1
            publisher.RegisterMessage(CreateStartTest("1-4", "1-3", "Assembly1.Namespace1.1.Test1"));
            publisher.RegisterMessage(CreateTestCaseSuccessful("1-4", "1-3", "Assembly1.Namespace1.1.Test1", "0.1", "Text output"));

            // Test Assembly2.Namespace2.1.Test1
            publisher.RegisterMessage(CreateStartTest("1-8", "1-7", "Assembly2.Namespace2.1.Test1"));
            publisher.RegisterMessage(CreateTestCaseSuccessful("1-8", "1-8", "Assembly2.Namespace2.1.Test1", "0.1", "Text output"));

            // Test Assembly1.Namespace1.1.Test2
            publisher.RegisterMessage(CreateStartTest("1-5", "1-3", "Assembly1.Namespace1.1.Test2"));
            publisher.RegisterMessage(CreateTestCaseFailed("1-5", "1-3", "Assembly1.Namespace1.1.Test2", "0.2", "Error output", "Stack trace"));

            publisher.RegisterMessage(CreateFinishSuite("1-7", "1-6", "Assembly2.Namespace2"));
            publisher.RegisterMessage(CreateFinishSuite("1-6", "", "Assembly2"));

            publisher.RegisterMessage(CreateFinishSuite("1-3", "1-2",  "Assembly1.Namespace1.1"));
            publisher.RegisterMessage(CreateFinishSuite("1-2", "1-1", "Assembly1.Namespace1"));
            publisher.RegisterMessage(CreateFinishSuite("1-1", "", "Assembly1"));
                        
            publisher.RegisterMessage(CreateTestRun());

            // Then
            Assert.AreEqual(                
                "##teamcity[testSuiteStarted name='Assembly1' flowId='1-1']" + Environment.NewLine
                + "##teamcity[testSuiteStarted name='Assembly2' flowId='1-6']" + Environment.NewLine
                
                + "##teamcity[flowStarted flowId='1-4' parent='1-1']" + Environment.NewLine
                + "##teamcity[testStarted name='Assembly1.Namespace1.1.Test1' captureStandardOutput='false' flowId='1-4']" + Environment.NewLine
                + "##teamcity[testStdOut name='Assembly1.Namespace1.1.Test1' out='Text output' flowId='1-4']" + Environment.NewLine
                + "##teamcity[testFinished name='Assembly1.Namespace1.1.Test1' duration='100' flowId='1-4']" + Environment.NewLine
                + "##teamcity[flowFinished flowId='1-4']" + Environment.NewLine

                + "##teamcity[flowStarted flowId='1-8' parent='1-6']" + Environment.NewLine
                + "##teamcity[testStarted name='Assembly2.Namespace2.1.Test1' captureStandardOutput='false' flowId='1-8']" + Environment.NewLine
                + "##teamcity[testStdOut name='Assembly2.Namespace2.1.Test1' out='Text output' flowId='1-8']" + Environment.NewLine
                + "##teamcity[testFinished name='Assembly2.Namespace2.1.Test1' duration='100' flowId='1-8']" + Environment.NewLine
                + "##teamcity[flowFinished flowId='1-8']" + Environment.NewLine

                + "##teamcity[flowStarted flowId='1-5' parent='1-1']" + Environment.NewLine
                + "##teamcity[testStarted name='Assembly1.Namespace1.1.Test2' captureStandardOutput='false' flowId='1-5']" + Environment.NewLine
                + "##teamcity[testFailed name='Assembly1.Namespace1.1.Test2' message='Error output' details='Stack trace' flowId='1-5']" + Environment.NewLine
                + "##teamcity[testFinished name='Assembly1.Namespace1.1.Test2' duration='200' flowId='1-5']" + Environment.NewLine
                + "##teamcity[flowFinished flowId='1-5']" + Environment.NewLine

                + "##teamcity[testSuiteFinished name='Assembly2' flowId='1-6']" + Environment.NewLine
                + "##teamcity[testSuiteFinished name='Assembly1' flowId='1-1']" + Environment.NewLine,
                _output.ToString());
        }

        [Test]
        public void ShouldSendMessagesWithValidFlowIdWhenHas1SuiteFromNUnit3()
        {
            // Given
            var publisher = CreateInstance();

            // When
            publisher.RegisterMessage(CreateStartRun(1));

            // Assembly 1
            publisher.RegisterMessage(CreateStartSuite("1-1", "", "aaa" + Path.DirectorySeparatorChar + "Assembly1"));
            
            // Test Assembly1.Namespace1.1.Test1
            publisher.RegisterMessage(CreateStartTest("1-2", "1-1", "Assembly1.Namespace1.1.Test1"));
            publisher.RegisterMessage(CreateTestCaseSuccessful("1-2", "1-1", "Assembly1.Namespace1.1.Test1", "0.1", "Text output"));

            publisher.RegisterMessage(CreateFinishSuite("1-1", "", "Assembly1"));

            publisher.RegisterMessage(CreateTestRun());

            // Then
            Assert.AreEqual(                
                "##teamcity[testSuiteStarted name='Assembly1' flowId='1-1']" + Environment.NewLine

                + "##teamcity[flowStarted flowId='1-2' parent='1-1']" + Environment.NewLine
                + "##teamcity[testStarted name='Assembly1.Namespace1.1.Test1' captureStandardOutput='false' flowId='1-2']" + Environment.NewLine
                + "##teamcity[testStdOut name='Assembly1.Namespace1.1.Test1' out='Text output' flowId='1-2']" + Environment.NewLine
                + "##teamcity[testFinished name='Assembly1.Namespace1.1.Test1' duration='100' flowId='1-2']" + Environment.NewLine
                + "##teamcity[flowFinished flowId='1-2']" + Environment.NewLine

                + "##teamcity[testSuiteFinished name='Assembly1' flowId='1-1']" + Environment.NewLine,
                _output.ToString());
        }

        [Test]
        public void ShouldSendMessagesWithValidFlowIdWhenHasNoSuiteFromNUnit3()
        {
            // Given
            var publisher = CreateInstance();

            // When
            publisher.RegisterMessage(CreateStartRun(1));

            // Test Assembly1.Namespace1.1.Test1
            publisher.RegisterMessage(CreateStartTest("1-1", "", "Assembly1.Namespace1.1.Test1"));
            publisher.RegisterMessage(CreateTestCaseSuccessful("1-1", "", "Assembly1.Namespace1.1.Test1", "0.1", "Text output"));

            publisher.RegisterMessage(CreateTestRun());

            // Then
            Assert.AreEqual(                
                "##teamcity[testStarted name='Assembly1.Namespace1.1.Test1' captureStandardOutput='false' flowId='1-1']" + Environment.NewLine
                + "##teamcity[testStdOut name='Assembly1.Namespace1.1.Test1' out='Text output' flowId='1-1']" + Environment.NewLine
                + "##teamcity[testFinished name='Assembly1.Namespace1.1.Test1' duration='100' flowId='1-1']" + Environment.NewLine,
                _output.ToString());
        }

        [Test]
        public void ShouldSendMessagesWithValidFlowIdWhenOutputIsEmpty()
        {
            // Given
            var publisher = CreateInstance();

            // When
            publisher.RegisterMessage(CreateStartRun(1));

            // Test Assembly1.Namespace1.1.Test1
            publisher.RegisterMessage(CreateStartTest("1-1", "", "Assembly1.Namespace1.1.Test1"));
            publisher.RegisterMessage(CreateTestCaseSuccessful("1-1", "", "Assembly1.Namespace1.1.Test1", "0.1", ""));

            publisher.RegisterMessage(CreateTestRun());

            // Then
            Assert.AreEqual(                
                "##teamcity[testStarted name='Assembly1.Namespace1.1.Test1' captureStandardOutput='false' flowId='1-1']" + Environment.NewLine
                + "##teamcity[testFinished name='Assembly1.Namespace1.1.Test1' duration='100' flowId='1-1']" + Environment.NewLine,
                _output.ToString());
        }

        [Test]
        public void ShouldSendMessagesWithValidFlowIdWhenHasNoOutput()
        {
            // Given
            var publisher = CreateInstance();

            // When
            publisher.RegisterMessage(CreateStartRun(1));

            // Test Assembly1.Namespace1.1.Test1
            publisher.RegisterMessage(CreateStartTest("1-1", "", "Assembly1.Namespace1.1.Test1"));
            publisher.RegisterMessage(CreateTestCaseSuccessful("1-1", "", "Assembly1.Namespace1.1.Test1", "10", null));

            publisher.RegisterMessage(CreateTestRun());

            // Then
            Assert.AreEqual(                
                "##teamcity[testStarted name='Assembly1.Namespace1.1.Test1' captureStandardOutput='false' flowId='1-1']" + Environment.NewLine
                + "##teamcity[testFinished name='Assembly1.Namespace1.1.Test1' duration='10000' flowId='1-1']" + Environment.NewLine,
                _output.ToString());
        }

        [Test]
        public void ShouldSendMessagesWithValidFlowIdWhenTestsFromNUnit2()
        {
            // Given
            var publisher = CreateInstance();

            // When
            publisher.RegisterMessage(CreateStartRun(1));

            // Assembly 1
            publisher.RegisterMessage(CreateStartSuite("1-1", null, "aaa" + Path.DirectorySeparatorChar + "Assembly1"));
            publisher.RegisterMessage(CreateStartSuite("1-2", null, "Assembly1.Namespace1"));
            publisher.RegisterMessage(CreateStartSuite("1-3", null, "Assembly1.Namespace1.1"));
            
            // Test Assembly1.Namespace1.1.Test1
            publisher.RegisterMessage(CreateStartTest("1-4", null, "Assembly1.Namespace1.1.Test1"));
            publisher.RegisterMessage(CreateTestCaseSuccessful("1-4", null, "Assembly1.Namespace1.1.Test1", "0.1", "Text output"));
            
            // Test Assembly1.Namespace1.1.Test2
            publisher.RegisterMessage(CreateStartTest("1-5", null, "Assembly1.Namespace1.1.Test2"));
            publisher.RegisterMessage(CreateTestCaseFailed("1-5", null, "Assembly1.Namespace1.1.Test2", "0.2", "Error output", "Stack trace"));
            
            publisher.RegisterMessage(CreateFinishSuite("1-3", null, "Assembly1.Namespace1.1"));
            publisher.RegisterMessage(CreateFinishSuite("1-2", null, "Assembly1.Namespace1"));
            publisher.RegisterMessage(CreateFinishSuite("1-1", null, "Assembly1"));

            // Assembly 2
            publisher.RegisterMessage(CreateStartSuite("1-6", null, "ddd//Assembly2"));
            publisher.RegisterMessage(CreateStartSuite("1-7", null, "Assembly2.Namespace2"));

            // Test Assembly2.Namespace2.1.Test1
            publisher.RegisterMessage(CreateStartTest("1-8", null, "Assembly2.Namespace2.1.Test1"));
            publisher.RegisterMessage(CreateTestCaseSuccessful("1-8", null, "Assembly2.Namespace2.1.Test1", "0.3", "Text output"));

            publisher.RegisterMessage(CreateFinishSuite("1-7", null, "Assembly2.Namespace2"));
            publisher.RegisterMessage(CreateFinishSuite("1-6", null, "Assembly2"));

            publisher.RegisterMessage(CreateTestRun());

            // Then
            Assert.AreEqual(                
                "##teamcity[testSuiteStarted name='Assembly1' flowId='1-1']" + Environment.NewLine

                + "##teamcity[testStarted name='Assembly1.Namespace1.1.Test1' captureStandardOutput='false' flowId='1-1']" + Environment.NewLine
                + "##teamcity[testStdOut name='Assembly1.Namespace1.1.Test1' out='Text output' flowId='1-1']" + Environment.NewLine
                + "##teamcity[testFinished name='Assembly1.Namespace1.1.Test1' duration='100' flowId='1-1']" + Environment.NewLine

                + "##teamcity[testStarted name='Assembly1.Namespace1.1.Test2' captureStandardOutput='false' flowId='1-1']" + Environment.NewLine
                + "##teamcity[testFailed name='Assembly1.Namespace1.1.Test2' message='Error output' details='Stack trace' flowId='1-1']" + Environment.NewLine
                + "##teamcity[testFinished name='Assembly1.Namespace1.1.Test2' duration='200' flowId='1-1']" + Environment.NewLine

                + "##teamcity[testSuiteFinished name='Assembly1' flowId='1-1']" + Environment.NewLine
                
                + "##teamcity[testSuiteStarted name='Assembly2' flowId='1-6']" + Environment.NewLine
                + "##teamcity[testStarted name='Assembly2.Namespace2.1.Test1' captureStandardOutput='false' flowId='1-6']" + Environment.NewLine
                + "##teamcity[testStdOut name='Assembly2.Namespace2.1.Test1' out='Text output' flowId='1-6']" + Environment.NewLine
                + "##teamcity[testFinished name='Assembly2.Namespace2.1.Test1' duration='300' flowId='1-6']" + Environment.NewLine

                + "##teamcity[testSuiteFinished name='Assembly2' flowId='1-6']" + Environment.NewLine,
                _output.ToString());
        }

        [Test]
        public void ShouldSendMessagesWithValidFlowIdWhenHas1SuiteFromNUnit2()
        {
            // Given
            var publisher = CreateInstance();

            // When
            publisher.RegisterMessage(CreateStartRun(1));

            // Assembly 1
            publisher.RegisterMessage(CreateStartSuite("1-1", null, "aaa" + Path.DirectorySeparatorChar + "Assembly1"));

            // Test Assembly1.Namespace1.1.Test1
            publisher.RegisterMessage(CreateStartTest("1-2", null, "Assembly1.Namespace1.1.Test1"));
            publisher.RegisterMessage(CreateTestCaseSuccessful("1-2", null, "Assembly1.Namespace1.1.Test1", "1.3", "Text output"));

            publisher.RegisterMessage(CreateFinishSuite("1-1", null, "Assembly1"));

            publisher.RegisterMessage(CreateTestRun());

            // Then
            Assert.AreEqual(                
                "##teamcity[testSuiteStarted name='Assembly1' flowId='1-1']" + Environment.NewLine

                + "##teamcity[testStarted name='Assembly1.Namespace1.1.Test1' captureStandardOutput='false' flowId='1-1']" + Environment.NewLine
                + "##teamcity[testStdOut name='Assembly1.Namespace1.1.Test1' out='Text output' flowId='1-1']" + Environment.NewLine
                + "##teamcity[testFinished name='Assembly1.Namespace1.1.Test1' duration='1300' flowId='1-1']" + Environment.NewLine
                
                + "##teamcity[testSuiteFinished name='Assembly1' flowId='1-1']" + Environment.NewLine,
                _output.ToString());
        }

        [Test]
        public void ShouldSendMessagesWithValidFlowIdWhenHasNoSuiteFromNUnit2()
        {
            // Given
            var publisher = CreateInstance();

            // When
            publisher.RegisterMessage(CreateStartRun(1));

            // Test Assembly1.Namespace1.1.Test1
            publisher.RegisterMessage(CreateStartTest("1-1", null, "Assembly1.Namespace1.1.Test1"));
            publisher.RegisterMessage(CreateTestCaseSuccessful("1-1", null, "Assembly1.Namespace1.1.Test1", "0.1", "Text output"));

            publisher.RegisterMessage(CreateTestRun());

            // Then
            Assert.AreEqual(                
                "##teamcity[testStarted name='Assembly1.Namespace1.1.Test1' captureStandardOutput='false' flowId='1-1']" + Environment.NewLine
                + "##teamcity[testStdOut name='Assembly1.Namespace1.1.Test1' out='Text output' flowId='1-1']" + Environment.NewLine
                + "##teamcity[testFinished name='Assembly1.Namespace1.1.Test1' duration='100' flowId='1-1']" + Environment.NewLine,
                _output.ToString());
        }

        private static XmlNode CreateMessage(string text)
        {
            var doc = new XmlDocument();
            doc.LoadXml(text);
            return doc.FirstChild;
        }

        private static XmlNode CreateStartRun(int count)
        {
            return CreateMessage(string.Format("<start-run count='{0}'/>", count));
        }

        private static XmlNode CreateTestRun()
        {
            return CreateMessage("<test-run></test-run>");
        }

        private static XmlNode CreateStartSuite(string id, string parentId, string name)
        {
            return CreateMessage(string.Format("<start-suite id=\"{0}\" {1} name=\"{2}\" fullname=\"{2}\"/>", id, GetNamedAttr("parentId", parentId), name));
        }

        private static XmlNode CreateFinishSuite(string id, string parentId, string name)
        {
            return CreateMessage(string.Format("<test-suite id=\"{0}\" {1} name=\"{2}\" fullname=\"{2}\" runstate=\"Runnable\" testcasecount=\"3\" result=\"Failed\" duration=\"0.251125\" total=\"3\" passed=\"0\" failed=\"0\" inconclusive=\"0\" skipped=\"0\" asserts=\"0\"><failure><message><![CDATA[One or more child tests had errors]]></message></failure></test-suite>", id, GetNamedAttr("parentId", parentId), name));
        }

        private static XmlNode CreateStartTest(string id, string parentId, string name)
        {
            return CreateMessage(string.Format("<start-test id=\"{0}\" {1} name=\"{2}\" fullname=\"{2}\"/>", id, GetNamedAttr("parentId", parentId), name));
        }

        private static XmlNode CreateTestCaseSuccessful(string id, string parentId, string name, string duration, string output)
        {
            return CreateMessage(string.Format("<test-case id=\"{0}\" {1} name=\"{2}\" fullname=\"{2}\" runstate=\"Runnable\" result=\"Passed\" duration=\"{3}\" asserts=\"0\">{4}</test-case>", id, GetNamedAttr("parentId", parentId), name, duration, GetNamedElement("output", output)));
        }

        private static XmlNode CreateTestCaseFailed(string id, string parentId, string name, string duration, string message, string stackTrace)
        {
            return CreateMessage(string.Format("<test-case id=\"{0}\" {1} name=\"{2}\" fullname=\"{2}\" runstate=\"Runnable\" result=\"Failed\" duration=\"{3}\" asserts=\"0\"><properties><property name=\"_CATEGORIES\" value=\"F\" /></properties><failure><message><![CDATA[{4}]]></message><stack-trace><![CDATA[{5}]]></stack-trace></failure></test-case>", id, GetNamedAttr("parentId", parentId), name, duration, message, stackTrace));
        }

        private TeamCityEventListener CreateInstance()
        {
            return new TeamCityEventListener(_outputWriter);
        }

        private static string GetNamedAttr(string attrName, string attrValue)
        {
            if (attrValue == null)
            {
                return string.Empty;
            }

            return string.Format("{0}=\"{1}\"", attrName, attrValue);
        }

        private static string GetNamedElement(string elementName, string elementValue)
        {
            if (elementValue == null)
            {
                return string.Empty;
            }

            return string.Format("<{0}>{1}</{0}>", elementName, elementValue);
        }
    }
}
