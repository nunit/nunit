﻿// ***********************************************************************
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
        private readonly TextWriter _outWriter;

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
            string name = node.GetAttribute("name");
            _outWriter.WriteLine("##teamcity[testStarted name='{0}' captureStandardOutput='true']", Escape(name));
        }

        /// <summary>
        /// Called when a test has finished
        /// </summary>
        /// <param name="result">The result of the test</param>
        public void TestFinished(XmlNode result)
        {
            string name = result.GetAttribute("name");
            double duration = result.GetAttribute("duration", 0.0);

            switch (result.GetAttribute("result"))
            {
                case "Passed":
                    TC_TestFinished(name, duration);
                    break;
                case "Inconclusive":
                    TC_TestIgnored(name, "Inconclusive");
                    break;
                case "Skipped":
                    XmlNode reason = result.SelectSingleNode("reason/message");
                    TC_TestIgnored(name, reason == null ? "" : reason.InnerText);
                    break;
                case "Failed":
                    XmlNode message = result.SelectSingleNode("failure/message");
                    XmlNode stackTrace = result.SelectSingleNode("failure/stack-trace");
                    TC_TestFailed(name, message == null ? "" : message.InnerText, stackTrace == null ? "" : stackTrace.InnerText);
                    TC_TestFinished(name, duration);
                    break;
            }
        }

        public void TestSuiteStarted(XmlNode node)
        {
            string name = node.GetAttribute("name");
            _outWriter.WriteLine("##teamcity[testSuiteStarted name='{0}']", Escape(name));
        }

        public void TestSuiteFinished(XmlNode node)
        {
            string name = node.GetAttribute("name");
            _outWriter.WriteLine("##teamcity[testSuiteFinished name='{0}']", Escape(name));
        }

        #region Helper Methods

        private void TC_TestFinished(string name, double duration)
        {
            _outWriter.WriteLine("##teamcity[testFinished name='{0}' duration='{1}']", Escape(name),
                             duration.ToString("0.000", NumberFormatInfo.InvariantInfo));
        }

        private void TC_TestIgnored(string name, string reason)
        {
            _outWriter.WriteLine("##teamcity[testIgnored name='{0}' message='{1}']", Escape(name), Escape(reason));
        }

        private void TC_TestFailed(string name, string message, string details)
        {
            _outWriter.WriteLine("##teamcity[testFailed name='{0}' message='{1}' details='{2}']", Escape(name), Escape(message), Escape(details));
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
