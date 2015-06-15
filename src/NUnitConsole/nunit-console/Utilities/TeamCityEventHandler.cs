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
using System.Globalization;
using System.IO;
using System.Xml;

namespace NUnit.ConsoleRunner.Utilities
{
    /// <summary>
    /// TeamCityEventHandler contains methods for issuing
    /// TeamCity service messages on the Console.
    /// </summary>
    public class TeamCityEventHandler
    {
        private readonly TeamCityAssemblyNameProvider _assemblyNameProvider = new TeamCityAssemblyNameProvider();
        private TextWriter _outWriter;

        /// <summary>
        /// Construct a TeamCityEventHandler
        /// </summary>
        /// <param name="outWriter">TextWriter to which output should be directed</param>
        public TeamCityEventHandler(TextWriter outWriter)
        {
            _outWriter = outWriter;
        }

        public void SuiteStarted(XmlNode testEvent)
        {
            this._assemblyNameProvider.RegisterSuite(testEvent.GetAttribute("id"), testEvent.GetAttribute("fullname"));
        }

        public void SuiteFinished(XmlNode testEvent)
        {
            this._assemblyNameProvider.UnregisterSuite(testEvent.GetAttribute("id"), testEvent.GetAttribute("fullname"));
        }

        public void TestStarted(XmlNode testEvent)
        {
            string testId = testEvent.GetAttribute("id");
            string name = this.EnrichTestName(testId, testEvent.GetAttribute("fullname"));

            _outWriter.WriteLine("##teamcity[testStarted name='{0}' captureStandardOutput='true' flowId='{1}']", Escape(name), Escape(testId));
        }

        /// <summary>
        /// Called when a test has finished
        /// </summary>
        /// <param name="testEvent">The testEvent of the test</param>
        public void TestFinished(XmlNode testEvent)
        {
            string testId = testEvent.GetAttribute("id");
            string name = EnrichTestName(testId, testEvent.GetAttribute("fullname"));
            double duration = testEvent.GetAttribute("duration", 0.0);

            switch (testEvent.GetAttribute("result"))
            {
                case "Passed":
                    TC_TestFinished(name, duration, testId);
                    break;
                case "Inconclusive":
                    TC_TestIgnored(name, "Inconclusive", testId);
                    break;
                case "Skipped":
                    XmlNode reason = testEvent.SelectSingleNode("reason/message");
                    TC_TestIgnored(name, reason == null ? "" : reason.InnerText, testId);
                    break;
                case "Failed":
                    XmlNode message = testEvent.SelectSingleNode("failure/message");
                    XmlNode stackTrace = testEvent.SelectSingleNode("failure/stack-trace");
                    TC_TestFailed(name, message == null ? "" : message.InnerText, stackTrace == null ? "" : stackTrace.InnerText, testId);
                    TC_TestFinished(name, duration, testId);
                    break;
            }
        }

        #region Helper Methods
        private string EnrichTestName(string testId, string name)
        {
            if (name == null)
            {
                return null;
            }

            string assemblyName;
            if (this._assemblyNameProvider.TryGetAssemblyName(testId, out assemblyName))
            {
                name = string.Format("{0}: {1}", assemblyName, name);
            }

            return name;
        }

        private void TC_TestFinished(string name, double duration, string flowId)
        {
            _outWriter.WriteLine("##teamcity[testFinished name='{0}' duration='{1}' flowId='{2}']", Escape(name),
                             duration.ToString("0.000", NumberFormatInfo.InvariantInfo), Escape(flowId));
        }

        private void TC_TestIgnored(string name, string reason, string flowId)
        {
            _outWriter.WriteLine("##teamcity[testIgnored name='{0}' message='{1}' flowId='{2}']", Escape(name), Escape(reason), Escape(flowId));
        }

        private void TC_TestFailed(string name, string message, string details, string flowId)
        {
            _outWriter.WriteLine("##teamcity[testFailed name='{0}' message='{1}' details='{2}' flowId='{3}']", Escape(name), Escape(message), Escape(details), Escape(flowId));
        }

        private static string Escape(string input)
        {
            return input != null
                ? input.Replace("|", "||")
                       .Replace("'", "|'")
                       .Replace("\n", "|n")
                       .Replace("\r", "|r")
                       .Replace(char.ConvertFromUtf32(int.Parse("0086", NumberStyles.HexNumber)), "|x")
                       .Replace(char.ConvertFromUtf32(int.Parse("2028", NumberStyles.HexNumber)), "|l")
                       .Replace(char.ConvertFromUtf32(int.Parse("2029", NumberStyles.HexNumber)), "|p")
                       .Replace("[", "|[")
                       .Replace("]", "|]")
                : null;
        }
        #endregion
    }
}
