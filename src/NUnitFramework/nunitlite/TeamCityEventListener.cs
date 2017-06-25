// ***********************************************************************
// Copyright (c) 2014 Charlie Poole, Rob Prouse
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
using NUnit.Framework.Interfaces;

namespace NUnitLite
{
    /// <summary>
    /// TeamCityEventListener class handles ITestListener events
    /// by issuing TeamCity service messages on the Console.
    /// </summary>
    public class TeamCityEventListener : ITestListener
    {
        readonly TextWriter _outWriter;

        /// <summary>
        /// Default constructor using Console.Out
        /// </summary>
        /// <remarks>
        /// This constructor must be called before Console.Out is
        /// redirected in order to work correctly under TeamCity.
        /// </remarks>
        public TeamCityEventListener() : this(Console.Out) { }

        /// <summary>
        /// Construct a TeamCityEventListener specifying a TextWriter. Used for testing.
        /// </summary>
        /// <param name="outWriter">The TextWriter to receive normal messages.</param>
        public TeamCityEventListener(TextWriter outWriter)
        {
            _outWriter = outWriter;
        }

        /// <summary>
        /// Called when a test has just started
        /// </summary>
        /// <param name="test">The test that is starting</param>
        public void TestStarted(ITest test)
        {
            if (test.IsSuite)
                TC_TestSuiteStarted(test.Name);
            else
                TC_TestStarted(test.Name);
        }

        /// <summary>
        /// Called when a test has finished
        /// </summary>
        /// <param name="result">The result of the test</param>
        public void TestFinished(ITestResult result)
        {
            string testName = result.Test.Name;

            if (result.Test.IsSuite)
                TC_TestSuiteFinished(testName);
            else
                switch (result.ResultState.Status)
                {
                    case TestStatus.Passed:
                        TC_TestFinished(testName, result.Duration);
                        break;
                    case TestStatus.Inconclusive:
                        TC_TestIgnored(testName, "Inconclusive");
                        break;
                    case TestStatus.Skipped:
                        TC_TestIgnored(testName, result.Message);
                        break;
                    case TestStatus.Warning:
                        // TODO: No action at this time. May need to be added.
                        break;
                    case TestStatus.Failed:
                        TC_TestFailed(testName, result.Message, result.StackTrace);
                        TC_TestFinished(testName, result.Duration);
                        break;
                }
        }

        /// <summary>
        /// Called when a test produces output for immediate display
        /// </summary>
        /// <param name="output">A TestOutput object containing the text to display</param>
        public void TestOutput(TestOutput output) { }

#region Helper Methods

        private void TC_TestSuiteStarted(string name)
        {
            _outWriter.WriteLine("##teamcity[testSuiteStarted name='{0}']", Escape(name));
        }

        private void TC_TestSuiteFinished(string name)
        {
            _outWriter.WriteLine("##teamcity[testSuiteFinished name='{0}']", Escape(name));
        }

        private void TC_TestStarted(string name)
        {
            _outWriter.WriteLine("##teamcity[testStarted name='{0}' captureStandardOutput='true']", Escape(name));
        }

        private void TC_TestFinished(string name, double duration)
        {
            // TeamCity expects the duration to be in milliseconds
            int milliseconds = (int)(duration * 1000d);
            _outWriter.WriteLine("##teamcity[testFinished name='{0}' duration='{1}']", Escape(name), milliseconds);
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
