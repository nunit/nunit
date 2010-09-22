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
            try
            {
                XmlNode node = XmlHelper.CreateTopLevelElement("start");
                XmlHelper.AddAttribute(node, "id", test.Id.ToString());
                XmlHelper.AddAttribute(node, "name", test.Name);
                XmlHelper.AddAttribute(node, "fullname", test.FullName);
                callback(new ProgressReport(node));
            }
            catch(Exception ex)
            {
                InternalTrace.Error("Exception processing " + test.FullName + NUnit.Env.NewLine + ex.ToString());
            }
        }

        /// <summary>
        /// Called when a test has finished. Sends a result summary to the callback.
        /// to 
        /// </summary>
        /// <param name="result">The result of the test</param>
        public void TestFinished(ITestResult result)
        {
            try
            {
                callback(new ProgressReport(result.ToXml(false)));
            }
            catch (Exception ex)
            {
                InternalTrace.Error("Exception processing " + result.FullName + NUnit.Env.NewLine + ex.ToString());
            }
        }

        /// <summary>
        /// Called when the test creates text output.
        /// </summary>
        /// <param name="testOutput">A console message</param>
        public void TestOutput(TestOutput testOutput)
        {
            try
            {
                XmlNode node = XmlHelper.CreateTopLevelElement("output");
                XmlHelper.AddAttribute(node, "type", testOutput.Type.ToString());
                XmlHelper.AddElementWithCDataSection(node, "text", testOutput.Text);
                callback(new ProgressReport(node));
            }
            catch (Exception ex)
            {
                InternalTrace.Error("Exception processing: " + testOutput.ToString() + NUnit.Env.NewLine + ex.ToString());
            }

        }

        #endregion
    }
}
