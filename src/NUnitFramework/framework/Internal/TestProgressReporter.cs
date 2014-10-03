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

#if !NETCF && !SILVERLIGHT
using System;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Xml;
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

        ///// <summary>
        ///// Called when a test run has just started
        ///// </summary>
        ///// <param name="test">The test that is starting</param>
        //public void RunStarted(ITest test)
        //{
        //    try
        //    {
        //        string report = string.Format(
        //            "<start-run id=\"{0}\" name=\"{1}\" fullname=\"{2}\"/>",
        //            test.Id,
        //            XmlHelper.FormatAttributeValue(test.Name),
        //            XmlHelper.FormatAttributeValue(test.FullName));

        //        handler.RaiseCallbackEvent(report);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Exception processing " + test.FullName + NUnit.Env.NewLine + ex.ToString());
        //    }
        //}

        ///// <summary>
        ///// Called when a test has finished. Sends a result summary to the callback.
        ///// to 
        ///// </summary>
        ///// <param name="result">The result of the test</param>
        //public void RunFinished(ITestResult result)
        //{
        //    try
        //    {
        //        handler.RaiseCallbackEvent(result.ToXml(false).OuterXml);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Exception processing " + result.FullName + NUnit.Env.NewLine + ex.ToString());
        //    }
        //}

        ///// <summary>
        ///// Called when a test has just started
        ///// </summary>
        ///// <param name="test">The test that is starting</param>
        //public void SuiteStarted(ITest test)
        //{
        //    try
        //    {
        //        string report = string.Format(
        //            "<start-suite id=\"{0}\" name=\"{1}\" fullname=\"{2}\"/>",
        //            test.Id,
        //            XmlHelper.FormatAttributeValue(test.Name),
        //            XmlHelper.FormatAttributeValue(test.FullName));

        //        handler.RaiseCallbackEvent(report);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Exception processing " + test.FullName + NUnit.Env.NewLine + ex.ToString());
        //    }
        //}

        ///// <summary>
        ///// Called when a test has finished. Sends a result summary to the callback.
        ///// to 
        ///// </summary>
        ///// <param name="result">The result of the test</param>
        //public void SuiteFinished(ITestResult result)
        //{
        //    try
        //    {
        //        handler.RaiseCallbackEvent(result.ToXml(false).OuterXml);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Exception processing " + result.FullName + NUnit.Env.NewLine + ex.ToString());
        //    }
        //}

        /// <summary>
        /// Called when a test has just started
        /// </summary>
        /// <param name="test">The test that is starting</param>
        public void TestStarted(ITest test)
        {
            string startElement = test is TestSuite
                ? "start-suite"
                : "start-test";

            try
            {
                string report = string.Format(
                    "<{0} id=\"{1}\" name=\"{2}\" fullname=\"{3}\"/>",
                    startElement,
                    test.Id,
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
                handler.RaiseCallbackEvent(result.ToXml(false).OuterXml);
            }
            catch (Exception ex)
            {
                log.Error("Exception processing " + result.FullName + NUnit.Env.NewLine + ex.ToString());
            }
        }

        #endregion

        #region Helper Methods

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
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");
        }

        #endregion
    }
}
#endif
