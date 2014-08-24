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
        private readonly bool _teamcity;
        private readonly string _labels;
        private readonly TeamCityServiceMessages _teamcityMessages;

        private TextWriter _outWriter;

        public RunTestsCallbackHandler(IDictionary<string, object> settings)
        {
            _teamcity = settings.ContainsKey("DisplayTeamCityServiceMessages")
                ? (bool)settings["DisplayTeamCityServiceMessages"]
                : false;
            _labels = settings.ContainsKey("DisplayTestLabels")
                ? ((string)settings["DisplayTestLabels"]).ToUpperInvariant()
                : "OFF";
            _outWriter = Console.Out;

            if (_teamcity)
                _teamcityMessages = new TeamCityServiceMessages();
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
            }
        }

        private void OnSuiteStart(XmlNode suiteNode)
        {
            XmlAttribute name = suiteNode.Attributes["name"];
            XmlAttribute fullname = suiteNode.Attributes["fullname"];

            if (_teamcity)
                _teamcityMessages.TestSuiteStarted(name.Value);
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

            if (_teamcity)
                _teamcityMessages.TestSuiteFinished(name.Value);
        }

        private void OnTestStart(XmlNode startNode)
        {
            XmlAttribute id = startNode.Attributes["id"];
            XmlAttribute name = startNode.Attributes["name"];
            //XmlAttribute fullname = startNode.Attributes["fullname"];
            XmlAttribute testcase = startNode.Attributes["testcase"];

            Debug.Assert(id != null);
            Debug.Assert(name != null);

            if (_teamcity)
                _teamcityMessages.TestStarted(name.Value);
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
                    if (_teamcity)
                        _teamcityMessages.TestFinished(name.Value, duration);
                    break;
                case "Inconclusive":
                    if (_teamcity)
                        _teamcityMessages.TestIgnored(name.Value, "Inconclusive");
                    break;
                case "Skipped":
                    XmlElement reason = testNode["reason"];
                    if (_teamcity)
                        _teamcityMessages.TestIgnored(name.Value, reason["message"].InnerText);
                    break;
                case "Failed":
                    XmlElement failure = testNode["failure"];
                    XmlElement message = failure["message"];
                    XmlElement stackTrace = failure["stack-trace"];
                    if (_teamcity)
                    {
                        _teamcityMessages.TestFailed(name.Value, message.InnerText, stackTrace.InnerText);
                        _teamcityMessages.TestFinished(name.Value, duration);
                    }
                    break;
            }

            var outputNode = testNode.SelectSingleNode("output");
            if (_labels == "ALL")
                WriteTestLabel(name.Value);
            if (outputNode != null)
            {
                if (_labels == "ON")
                    WriteTestLabel(name.Value);
                _outWriter.Write(outputNode.InnerText);
            }
        }

        private void WriteTestLabel(string name)
        {
            _outWriter.WriteLine("***** {0}", name);
        }
    }
}