using System;
using System.Globalization;
using System.IO;
using NUnit.Framework.Interfaces;

namespace NUnitLite.Runner
{
    /// <summary>
    /// TeamCityEventListener class handles ITestListener events
    /// by issuing TeamCity service messages on the Console.
    /// </summary>
    public class TeamCityEventListener : ITestListener
    {
        readonly TextWriter _outWriter;
        readonly TextWriter _errWriter;

        /// <summary>
        /// Default constructor using Console.Out and Console.Error
        /// </summary>
        /// <remarks>
        /// This constructor must be called before Console.Out and
        /// Console.Error are redirected in order to work correctly
        /// under TeamCity.
        /// </remarks>
        public TeamCityEventListener() : this(Console.Out, Console.Error) { }

        /// <summary>
        /// Construct a TeamCityEventListener specifying two TextWriters. Used for testing.
        /// </summary>
        /// <param name="outWriter">The TextWriter to receive normal messages.</param>
        /// <param name="errWriter">The TextWriter to receive error messages.</param>
        public TeamCityEventListener(TextWriter outWriter, TextWriter errWriter)
        {
            _outWriter = outWriter;
            _errWriter = errWriter;
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
                        TC_TestFinished(testName, result.Duration.TotalSeconds);
                        break;
                    case TestStatus.Inconclusive:
                        TC_TestIgnored(testName, "Inconclusive");
                        break;
                    case TestStatus.Skipped:
                        TC_TestIgnored(testName, result.Message);
                        break;
                    case TestStatus.Failed:
                        TC_TestFailed(testName, result.Message, result.StackTrace);
                        TC_TestFinished(testName, result.Duration.TotalSeconds);
                        break;
                }
        }

        /// <summary>
        /// Called when the test creates text output.
        /// </summary>
        /// <param name="testOutput">A console message</param>
        public void TestOutput(TestOutput testOutput)
        {
            switch (testOutput.Type)
            {
                case TestOutputType.Out:
                    TC_StandardOutput(testOutput.Text);
                    break;
                case TestOutputType.Error:
                    TC_ErrorOutput(testOutput.Text);
                    break;
            }
        }

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

        private void TC_StandardOutput(string text)
        {
            _outWriter.WriteLine(Escape(text));
        }

        private void TC_ErrorOutput(string text)
        {
            _errWriter.WriteLine(Escape(text));
        }

        private static string Escape(string input)
        {
            return input.Replace("|", "||")
                        .Replace("'", "|'")
                        .Replace("\n", "|n")
                        .Replace("\r", "|r")
                        .Replace(char.ConvertFromUtf32(int.Parse("0086", NumberStyles.HexNumber)), "|x")
                        .Replace(char.ConvertFromUtf32(int.Parse("2028", NumberStyles.HexNumber)), "|l")
                        .Replace(char.ConvertFromUtf32(int.Parse("2029", NumberStyles.HexNumber)), "|p")
                        .Replace("[", "|[")
                        .Replace("]", "|]");
        }

        #endregion
    }
}
