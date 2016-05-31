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
using System.Web.UI;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// TestProgressReporter translates ITestListener events into
    /// the async callbacks that are used to inform the client
    /// software about the progress of a test run.
    /// </summary>
    public class TestProgressReporter : ITestListener
    {
        static Logger log = InternalTrace.GetLogger("TestProgressReporter");

        private ICallbackEventHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestProgressReporter"/> class.
        /// </summary>
        /// <param name="handler">The callback handler to be used for reporting progress.</param>
        public TestProgressReporter(ICallbackEventHandler handler)
        {
            this.handler = handler;
        }

        #region ITestListener Members

        /// <summary>
        /// Called when a test has just started
        /// </summary>
        /// <param name="test">The test that is starting</param>
        public void TestStarted(ITest test)
        {
            string startElement = test is TestSuite
                ? "start-suite"
                : "start-test";

            var parent = GetParent(test);
            try
            {
                string report = string.Format(
                    "<{0} id=\"{1}\" parentId=\"{2}\" name=\"{3}\" fullname=\"{4}\"/>",
                    startElement,
                    test.Id,
                    parent != null ? parent.Id : string.Empty,
                    FormatAttributeValue(test.Name),
                    FormatAttributeValue(test.FullName));

                handler.RaiseCallbackEvent(report);
            }
            catch (Exception ex)
            {
                log.Error("Exception processing " + test.FullName + NUnit.Env.NewLine + ex.ToString());
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
                var node = result.ToXml(false);
                var parent = GetParent(result.Test);
                node.Attributes.Add("parentId", parent != null ? parent.Id : string.Empty);
                handler.RaiseCallbackEvent(node.OuterXml);                
            }
            catch (Exception ex)
            {
                log.Error("Exception processing " + result.FullName + NUnit.Env.NewLine + ex.ToString());
            }
        }

        /// <summary>
        /// Called when a test produces output for immediate display
        /// </summary>
        /// <param name="output">A TestOutput object containing the text to display</param>
        public void TestOutput(TestOutput output)
        {
            try
            {
                handler.RaiseCallbackEvent(output.ToXml());
            }
            catch (Exception ex)
            {
                log.Error("Exception processing TestOutput event" + NUnit.Env.NewLine + ex.ToString());
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Returns the parent test item for the targer test item if it exists
        /// </summary>
        /// <param name="test"></param>
        /// <returns>parent test item</returns>
        private static ITest GetParent(ITest test)
        {
            if (test == null || test.Parent == null)
            {
                return null;
            }

            return test.Parent.IsSuite ? test.Parent : GetParent(test.Parent);
        }

        /// <summary>
        /// Makes a string safe for use as an attribute, replacing
        /// characters characters that can't be used with their
        /// corresponding xml representations.
        /// </summary>
        /// <param name="original">The string to be used</param>
        /// <returns>A new string with the _values replaced</returns>
        private static string FormatAttributeValue(string original)
        {
            return original
                .Replace("&", "&amp;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");
        }

        #endregion
    }
}
