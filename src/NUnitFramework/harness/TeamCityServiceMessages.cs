using System;
using System.Globalization;
using System.IO;

namespace NUnit.Framework.TestHarness
{
    public class TeamCityServiceMessages
    {
        readonly TextWriter _outWriter;

        /// <summary>
        /// Default constructor using Console.Out and Console.Error
        /// </summary>
        /// <remarks>
        /// This constructor must be called before Console.Out and
        /// Console.Error are redirected in order to work correctly
        /// under TeamCity.
        /// </remarks>
        public TeamCityServiceMessages() : this(Console.Out) { }

        /// <summary>
        /// Construct a TeamCityEventListener specifying a TextWriter. Used for testing.
        /// </summary>
        /// <param name="outWriter">The TextWriter to receive normal messages.</param>
        public TeamCityServiceMessages(TextWriter outWriter)
        {
            _outWriter = outWriter;
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

        public void TestSuiteStarted(string name)
        {
            _outWriter.WriteLine("##teamcity[testSuiteStarted name='{0}']", Escape(name));
        }

        public void TestSuiteFinished(string name)
        {
            _outWriter.WriteLine("##teamcity[testSuiteFinished name='{0}']", Escape(name));
        }

        public void TestStarted(string name)
        {
            _outWriter.WriteLine("##teamcity[testStarted name='{0}' captureStandardOutput='true']", Escape(name));
        }

        public void TestFailed(string name, string message, string details)
        {
            _outWriter.WriteLine("##teamcity[testFailed name='{0}' message='{1}' details='{2}']", Escape(name), Escape(message), Escape(details));
        }

        public void TestIgnored(string name, string message)
        {
            _outWriter.WriteLine("##teamcity[testIgnored name='{0}' message='{1}']", Escape(name), Escape(message));
        }

        public void TestFinished(string name, double duration)
        {
            _outWriter.WriteLine("##teamcity[testFinished name='{0}' duration='{1}']", Escape(name),
                             duration.ToString("0.000", NumberFormatInfo.InvariantInfo));
        }
    }
}
