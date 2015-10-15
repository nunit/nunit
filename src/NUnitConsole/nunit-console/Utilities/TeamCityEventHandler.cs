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
        private TextWriter _outWriter;

        /// <summary>
        /// Construct a TeamCityEventHandler
        /// </summary>
        /// <param name="outWriter">TextWriter to which output should be directed</param>
        public TeamCityEventHandler(TextWriter outWriter)
        {
            _outWriter = outWriter;
        }

        public void TestStarted(XmlNode node)
        {
            string name = node.GetAttribute("fullname");
            string testId = node.GetAttribute("id");

            _outWriter.WriteLine("##teamcity[testStarted name='{0}' captureStandardOutput='true' flowId='{1}']", Escape(name), Escape(testId));
        }

        /// <summary>
        /// Called when a test has finished
        /// </summary>
        /// <param name="result">The result of the test</param>
        public void TestFinished(XmlNode result)
        {
            string name = result.GetAttribute("fullname");
            double duration = result.GetAttribute("duration", 0.0);
            string testId = result.GetAttribute("id");

            switch (result.GetAttribute("result"))
            {
                case "Passed":
                    TC_TestFinished(name, duration, testId);
                    break;
                case "Inconclusive":
                    TC_TestIgnored(name, "Inconclusive", testId);
                    break;
                case "Skipped":
                    XmlNode reason = result.SelectSingleNode("reason/message");
                    TC_TestIgnored(name, reason == null ? "" : reason.InnerText, testId);
                    break;
                case "Failed":
                    XmlNode message = result.SelectSingleNode("failure/message");
                    XmlNode stackTrace = result.SelectSingleNode("failure/stack-trace");
                    TC_TestFailed(name, message == null ? "" : message.InnerText, stackTrace == null ? "" : stackTrace.InnerText, testId);
                    TC_TestFinished(name, duration, testId);
                    break;
            }
        }

        #region Helper Methods

        private void TC_TestFinished(string name, double duration, string flowId)
        {
            // TeamCity expects the duration to be in milliseconds
            int milliseconds = (int)(duration * 1000d);
            _outWriter.WriteLine("##teamcity[testFinished name='{0}' duration='{1}' flowId='{2}']", Escape(name), milliseconds, Escape(flowId));
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
