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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;

namespace NUnit.Framework.TestHarness
{
    public class RunTestsCallbackHandler : CallbackHandler
    {
        private readonly bool teamcity;
        private readonly string labels;
        private readonly TeamCityServiceMessages teamcityMessages;

        private TextWriter output;

        // TODO: Make this a separate file
        class TeamCityServiceMessages
        {
            readonly TextWriter output = Console.Out;
            readonly TextWriter error = Console.Error;

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
                output.WriteLine("##teamcity[testSuiteStarted name='{0}']", Escape(name));
            }

            public void TestSuiteFinished(string name)
            {
                output.WriteLine("##teamcity[testSuiteFinished name='{0}']", Escape(name));
            }

            public void TestStarted(string name)
            {
                output.WriteLine("##teamcity[testStarted name='{0}' captureStandardOutput='true']", Escape(name));
            }

            public void TestOutput(string text)
            {
                output.WriteLine(Escape(text));
            }

            public void TestError(string text)
            {
                error.WriteLine(Escape(text));
            }

            public void TestFailed(string name, string message, string details)
            {
                output.WriteLine("##teamcity[testFailed name='{0}' message='{1}' details='{2}']", Escape(name), Escape(message), Escape(details));
            }

            public void TestIgnored(string name, string message)
            {
                output.WriteLine("##teamcity[testIgnored name='{0}' message='{1}']", Escape(name), Escape(message));
            }

            public void TestFinished(string name, double duration)
            {
                output.WriteLine("##teamcity[testFinished name='{0}' duration='{1}']", Escape(name),
                                 duration.ToString("0.000", NumberFormatInfo.InvariantInfo));
            }
        }

        public RunTestsCallbackHandler(IDictionary<string, object> settings)
        {
            this.teamcity = settings.ContainsKey("DisplayTeamCityServiceMessages")
                ? (bool)settings["DisplayTeamCityServiceMessages"]
                : false;
            this.labels = settings.ContainsKey("DisplayTestLabels")
                ? (string)settings["DisplayTestLabels"]
                : "Off";
            this.output = Console.Out;

            if (teamcity)
                teamcityMessages = new TeamCityServiceMessages();
        }

        public override void ReportProgress(string report)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(report);
            XmlNode topNode = doc.FirstChild;

            switch (topNode.Name)
            {
                case "start-suite":
                    OnSuiteStart(topNode);
                    break;
                case "start-test":
                    OnTestStart(topNode);
                    break;
                case "test-case":
                    OnTestCaseFinished(topNode);
                    break;
                case "test-suite":
                    OnSuiteFinished(topNode);
                    break;
                case "output":
                    OnOutput(topNode);
                    break;
            }
        }

        private void OnSuiteStart(XmlNode suiteNode)
        {
            XmlAttribute name = suiteNode.Attributes["name"];
            XmlAttribute fullname = suiteNode.Attributes["fullname"];

            if (teamcity)
                teamcityMessages.TestSuiteStarted(name.Value);
        }

        private void OnSuiteFinished(XmlNode suiteNode)
        {
            //int id = int.Parse(suiteNode.Attributes["id"].Value);
            XmlAttribute name = suiteNode.Attributes["name"];
            XmlAttribute fullname = suiteNode.Attributes["fullname"];
            XmlAttribute result = suiteNode.Attributes["result"];

            Debug.Assert(name != null);
            //Debug.Assert(fullname != null);
            Debug.Assert(result != null);

            if (teamcity)
                teamcityMessages.TestSuiteFinished(name.Value);
        }

        private void OnTestStart(XmlNode startNode)
        {
            XmlAttribute id = startNode.Attributes["id"];
            XmlAttribute name = startNode.Attributes["name"];
            //XmlAttribute fullname = startNode.Attributes["fullname"];
            XmlAttribute testcase = startNode.Attributes["testcase"];

            Debug.Assert(id != null);
            Debug.Assert(name != null);

            if (teamcity)
                teamcityMessages.TestStarted(name.Value);

            if (labels == "On" || labels == "All")
                output.WriteLine("***** {0}", name.Value);
        }

        private void OnTestCaseFinished(XmlNode testNode)
        {
            //int id = int.Parse(testNode.Attributes["id"].Value);
            XmlAttribute name = testNode.Attributes["name"];
            //XmlAttribute fullname = testNode.Attributes["fullname"];
            XmlAttribute result = testNode.Attributes["result"];
            XmlAttribute durationString = testNode.Attributes["duration"];

            Debug.Assert(name != null);
            //Debug.Assert(fullname != null);
            Debug.Assert(result != null);
            Debug.Assert(durationString != null);

            // TODO: Handle an error here
            double duration = double.Parse(durationString.Value);

            switch (result.Value)
            {
                case "Passed":
                    if (teamcity)
                        teamcityMessages.TestFinished(name.Value, duration);
                    break;
                case "Inconclusive":
                    if (teamcity)
                        teamcityMessages.TestIgnored(name.Value, "Inconclusive");
                    break;
                case "Skipped":
                    XmlElement reason = testNode["reason"];
                    if (teamcity)
                        teamcityMessages.TestIgnored(name.Value, reason["message"].InnerText);
                    break;
                case "Failed":
                    XmlElement failure = testNode["failure"];
                    XmlElement message = failure["message"];
                    XmlElement stackTrace = failure["stack-trace"];
                    if (teamcity)
                    {
                        teamcityMessages.TestFailed(name.Value, message.InnerText, stackTrace.InnerText);
                        teamcityMessages.TestFinished(name.Value, duration);
                    }
                    break;
            }
        }

        private void OnOutput(XmlNode outputNode)
        {
            XmlAttribute type = outputNode.Attributes["type"];
            XmlNode textNode = outputNode.Attributes["text"];

            Debug.Assert(type != null);
            Debug.Assert(textNode != null);

            switch (type.Value)
            {
                case "Out":
                    if (teamcity)
                        teamcityMessages.TestOutput(textNode.InnerText);
                    output.Write(textNode.InnerText);
                    break;
                case "Error":
                    if (teamcity)
                        teamcityMessages.TestError(textNode.InnerText);
                    break;
            }
        }
    }
}