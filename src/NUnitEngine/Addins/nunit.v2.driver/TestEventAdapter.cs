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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using NUnit.Core;

namespace NUnit.Engine.Drivers
{
    /// <summary>
    /// The TestEventAdapter class receives NUnit v2 events
    /// and passes the equivalent v3 event to the engine.
    /// </summary>
    public class TestEventAdapter : MarshalByRefObject, EventListener
    {
        private ITestEventListener _listener;
        private StringBuilder _testOutput = new StringBuilder();

        public TestEventAdapter(ITestEventListener listener)
        {
            _listener = listener;
        }

        #region NUnit.Core.EventListener Implementation

        void EventListener.RunFinished(Exception exception)
        {
            // Currently not used
        }

        void EventListener.RunFinished(TestResult result)
        {
            // Currently not used
        }

        void EventListener.RunStarted(string name, int testCount)
        {
            // Currently not used
        }

        void EventListener.SuiteFinished(TestResult result)
        {
            OnTestFinished(result);
        }

        void EventListener.SuiteStarted(TestName testName)
        {
            OnTestStarted(testName, true);
        }

        void EventListener.TestFinished(TestResult result)
        {
            OnTestFinished(result);
        }

        void EventListener.TestOutput(TestOutput testOutput)
        {
            switch (testOutput.Type)
            {
                case TestOutputType.Out:
                    _testOutput.Append(testOutput.Text);
                    break;

                case TestOutputType.Error:
                    Console.Error.Write(testOutput.Text);
                    break;

                default:
                    // Ignore any other output type
                    break;
            }
        }

        void EventListener.TestStarted(TestName testName)
        {
            OnTestStarted(testName, false);
        }

        void EventListener.UnhandledException(Exception exception)
        {
            // Currently not used
        }

        #endregion

        #region InitializeLifetimeService

        public override object InitializeLifetimeService()
        {
            return null;
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
                .Replace("&", "&amp;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");
        }

        private void OnTestStarted(TestName testName, bool isSuite)
        {
            string report = string.Format(
                "<{0} id=\"{1}-{2}\" name=\"{3}\" fullname=\"{4}\"/>",
                isSuite ? "start-suite" : "start-test",
                testName.RunnerID,
                testName.TestID,
                FormatAttributeValue(testName.Name),
                FormatAttributeValue(testName.FullName));

            _listener.OnTestEvent(report);
        }

        private void OnTestFinished(TestResult result)
        {
            var resultNode = result.ToXml(false);

            if (_testOutput.Length > 0)
            {
                var outputNode = resultNode.AddElement("output");
                outputNode.InnerText = _testOutput.ToString();
                _testOutput.Remove(0, _testOutput.Length);
            }

            _listener.OnTestEvent(resultNode.OuterXml);
        }

        #endregion
    }
}
