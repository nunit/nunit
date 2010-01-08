// ***********************************************************************
// Copyright (c) 2010 Charlie Poole
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
using System.Xml;
using NUnit.Framework.Internal;
using NUnit.TestUtilities;

namespace NUnit.Framework.Api
{
	/// <summary>
	/// Summary description for TestResultTests.
	/// </summary>
	[TestFixture]
	public class TestResultTests
	{
		private TestResult testResult;
        XmlDocument doc = new XmlDocument();

		[SetUp]
		public void SetUp()
		{
			testResult = new TestResult( new TestMethod(Reflect.GetNamedMethod( this.GetType(), "DummyMethod" ) ) );
		}

		public void DummyMethod() { }

        private void PerformCommonXmlTests(XmlNode element)
        {
            Assert.True(element is XmlElement);
            Assert.AreEqual("test", element.Name);
            Assert.AreEqual("DummyMethod", element.Attributes["name"].Value);
            Assert.AreEqual("NUnit.Framework.Api.TestResultTests.DummyMethod", element.Attributes["fullname"].Value);
            Assert.AreEqual("0.000", element.Attributes["time"].Value);
        }
		
		[Test]
		public void DefaultResult()
		{
			Assert.AreEqual( ResultState.Inconclusive, testResult.ResultState );
		}

        [Test]
        public void DefaultResultToXml()
        {
            doc.LoadXml(testResult.ToXml());
            XmlNode element = doc.FirstChild;
            PerformCommonXmlTests(element);
            Assert.AreEqual("Inconclusive", element.Attributes["result"].Value);
            Assert.Null(element.SelectSingleNode("message"));
            Assert.Null(element.SelectSingleNode("stacktrace"));
        }

        [Test]
		public void SuccessResult()
		{
			testResult.Success();
			Assert.IsTrue(testResult.IsSuccess, "result should be success");
        }

        [Test]
        public void SuccessResultToXml()
        {
            testResult.Success();
            doc.LoadXml(testResult.ToXml());
            XmlNode element = doc.FirstChild;
            PerformCommonXmlTests(element);
            Assert.AreEqual("Success", element.Attributes["result"].Value);
            Assert.Null(element.SelectSingleNode("message"));
            Assert.Null(element.SelectSingleNode("stacktrace"));
        }

        [Test]
		public void IgnoredResult()
		{
			testResult.Ignore( "because" );
			Assert.AreEqual( false, testResult.Executed );
			Assert.AreEqual( "because", testResult.Message );
        }

        [Test]
        public void IgnoredResultToXml()
        {
            testResult.Ignore("because");
            doc.LoadXml(testResult.ToXml());
            XmlNode element = doc.FirstChild;
            PerformCommonXmlTests(element);
            Assert.AreEqual("Ignored", element.Attributes["result"].Value);
            Assert.NotNull(element.SelectSingleNode("message"));
            Assert.AreEqual("because", element.SelectSingleNode("message").InnerText);
            Assert.Null(element.SelectSingleNode("stacktrace"));
        }

        [Test]
		public void FailedResult()
		{
			testResult.Failure("message", "stack trace");
			Assert.IsTrue(testResult.IsFailure);
			Assert.IsFalse(testResult.IsSuccess);
			Assert.AreEqual("message",testResult.Message);
			Assert.AreEqual("stack trace",testResult.StackTrace);
		}

        [Test]
        public void FailedResultToXml()
        {
            testResult.Failure("message", "stack trace");
            doc.LoadXml(testResult.ToXml());
            XmlNode element = doc.FirstChild;
            PerformCommonXmlTests(element);

            Assert.AreEqual("Failure", element.Attributes["result"].Value);
            XmlNode failureNode = element.SelectSingleNode("failure");
            Assert.NotNull(failureNode, "No failure element found");

            XmlNode messageNode = failureNode.SelectSingleNode("message");
            Assert.NotNull(messageNode, "No message element found");
            Assert.AreEqual("message", messageNode.InnerText);

            XmlNode stacktraceNode = failureNode.SelectSingleNode("stacktrace");
            Assert.NotNull(stacktraceNode, "No stacktrace element found");
            Assert.AreEqual("stack trace", stacktraceNode.InnerText);
        }
    }
}
