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
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// TestProgressReporter translates ITestListener events into
    /// the async callbacks that are used to inform the client
    /// software about the progress of a test run.
    /// </summary>
    public class TestProgressReporter : ITestListener
    {
        private AsyncCallback callback;
        private StringWriter writer;
        private XmlTextWriter xml;
        private StringBuilder sb;
        XmlDocument doc = new XmlDocument();

        /// <summary>
        /// Initializes a new instance of the <see cref="TestProgressReporter"/> class.
        /// </summary>
        /// <param name="callback">The callback to be used for reporting progress.</param>
        public TestProgressReporter(AsyncCallback callback)
        {
            this.callback = callback;
            this.writer = new System.IO.StringWriter();
            this.xml = new System.Xml.XmlTextWriter(writer);
            this.sb = writer.GetStringBuilder();
        }

        #region ITestListener Members

        /// <summary>
        /// Called when a test has just started
        /// </summary>
        /// <param name="test">The test that is starting</param>
        public void TestStarted(ITest test)
        {
            xml.WriteStartElement("start");
            xml.WriteAttributeString("id", test.ID.ToString());
            xml.WriteAttributeString("name", test.Name);
            xml.WriteAttributeString("fullname", test.FullName);
            xml.WriteEndElement();
            SendReport();
        }

        /// <summary>
        /// Called when a test has finished. Sends a result summary to the callback.
        /// to 
        /// </summary>
        /// <param name="result">The result of the test</param>
        public void TestFinished(ITestResult result)
        {
            callback(new ProgressReport(result.ToXml(false)));
        }

        /// <summary>
        /// Called when the test creates text output.
        /// </summary>
        /// <param name="testOutput">A console message</param>
        public void TestOutput(TestOutput testOutput)
        {
            xml.WriteStartElement("output");
            xml.WriteAttributeString("type", testOutput.Type.ToString());
            xml.WriteStartElement("text");
            xml.WriteCData(testOutput.Text);
            xml.WriteEndElement();
            xml.WriteEndElement();
            SendReport();
        }

        #endregion

        private void SendReport()
        {
            doc.LoadXml(sb.ToString());
            callback(new ProgressReport(doc.FirstChild));
            sb.Remove(0, sb.Length);
        }
    }
}
