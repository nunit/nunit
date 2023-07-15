// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
        private readonly TextWriter _outWriter;

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
            {
                TC_TestSuiteFinished(testName);
            }
            else
            {
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
        }

        /// <summary>
        /// Called when a test produces output for immediate display
        /// </summary>
        /// <param name="output">A TestOutput object containing the text to display</param>
        public void TestOutput(TestOutput output) { }

        /// <summary>
        /// Called when a test produces a message to be sent to listeners
        /// </summary>
        /// <param name="message">A TestMessage object containing the text to send</param>
        public void SendMessage(TestMessage message) { }

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
            return input is not null
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
