// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Xml;
using NUnit.Engine;

namespace NUnit.ConsoleRunner
{
    using Utilities;

    /// <summary>
    /// TestEventHandler processes events from the running
    /// test for the console runner.
    /// </summary>
    public class TestEventHandler : MarshalByRefObject, ITestEventListener
    {
        private int _testRunCount;
        private int _testIgnoreCount;
        private int _failureCount;
        private int _level;

        private string _displayLabels;
        private TextWriter _outWriter;

        private List<string> _messages = new List<string>();
        

        public TestEventHandler(TextWriter outWriter, string displayLabels)
        {
            _level = 0;
            _displayLabels = displayLabels;
            _outWriter = outWriter;
        }

        #region ITestEventHandler Members

        public void OnTestEvent(string report)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(report);
            XmlNode testEvent = doc.FirstChild;

            switch (testEvent.Name)
            {
                case "start-test":
                    TestStarted(testEvent);
                    break;

                case "start-suite":
                    SuiteStarted(testEvent);
                    break;

                case "start-run":
                    //RunStarted(testEvent);
                    break;

                case "test-case":
                    TestFinished(testEvent);
                    break;

                case "test-suite":
                    SuiteFinished(testEvent);
                    break;
            }

        }

        #endregion

        #region Individual Handlers

        private void TestStarted(XmlNode startNode)
        {
            // Placeolder for TeamCity output
        }

        public void SuiteStarted(XmlNode startNode)
        {
            if (_level++ == 0)
            {
                _testRunCount = 0;
                _testIgnoreCount = 0;
                _failureCount = 0;

                XmlAttribute nameAttr = startNode.Attributes["fullname"];
                string suiteName = (nameAttr != null)
                    ? nameAttr.Value
                    : "<anonymous>";

                Trace.WriteLine("################################ UNIT TESTS ################################");
                Trace.WriteLine("Running tests in '" + suiteName + "'...");
            }
        }

        public void TestFinished(XmlNode testResult)
        {
            XmlAttribute resultAttr = testResult.Attributes["result"];
            string resultState = resultAttr == null
                ? "Unknown"
                : resultAttr.Value;
            XmlAttribute nameAttr = testResult.Attributes["fullname"];

            switch (resultState)
            {
                case "Failed":
                    _testRunCount++;
                    _failureCount++;
                  
                    _messages.Add(string.Format("{0}) {1} :", _failureCount, nameAttr.Value));
                    break;

                case "Inconclusive":
                case "Passed":
                    _testRunCount++;
                    break;

                case "Skipped":
                    _testIgnoreCount++;
                    break;
            }

            var outputNode = testResult.SelectSingleNode("output");

            if (_displayLabels == "ALL")
                WriteTestLabel(nameAttr.Value);
            if (outputNode != null)
            {
                if (_displayLabels == "ON")
                    WriteTestLabel(nameAttr.Value);

                WriteTestOutput(outputNode);
            }

            //currentTestName = string.Empty;
        }

        private void WriteTestLabel(string name)
        {
            using (new ColorConsole(ColorStyle.SectionHeader))
                outWriter.WriteLine("=> {0}", name);
        }

        private void WriteTestOutput(XmlNode outputNode)
        {
            using (new ColorConsole(ColorStyle.Output))
            {
                outWriter.Write(outputNode.InnerText);
                // Some labels were being shown on the same line as the previous output
                if (!outputNode.InnerText.EndsWith("\n"))
                {
                    outWriter.WriteLine();
                }
            }
        }

        public void SuiteFinished(XmlNode suiteResult)
        {
            if (--_level == 0)
            {
                XmlAttribute durationAttribute = suiteResult.Attributes["duration"];

                Trace.WriteLine("############################################################################");

                if (_messages.Count == 0)
                {
                    Trace.WriteLine("##############                 S U C C E S S               #################");
                }
                else
                {
                    Trace.WriteLine("##############                F A I L U R E S              #################");

                    foreach (string s in _messages)
                    {
                        Trace.WriteLine(s);
                    }
                }

                Trace.WriteLine("############################################################################");
                Trace.WriteLine("Executed tests       : " + _testRunCount);
                Trace.WriteLine("Ignored tests        : " + _testIgnoreCount);
                Trace.WriteLine("Failed tests         : " + _failureCount);
                if ( durationAttribute != null )
                Trace.WriteLine("Total duration       : " + durationAttribute.Value + " seconds");
                Trace.WriteLine("############################################################################");
            }
        }

        #endregion

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
