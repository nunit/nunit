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
    /// <summary>
    /// TestEventHandler processes events from the running
    /// test for the console runner.
    /// </summary>
    public class TestEventHandler : MarshalByRefObject, ITestEventListener
    {
        private int testRunCount;
        private int testIgnoreCount;
        private int failureCount;
        private int level;

        private string pendingLabel;

        private ConsoleOptions options;
        private TextWriter outWriter;
        private TextWriter errorWriter;

        private List<string> messages = new List<string>();
        
        private string currentTestName;

        public TestEventHandler( ConsoleOptions options, TextWriter outWriter, TextWriter errorWriter )
        {
            level = 0;
            this.options = options;
            this.outWriter = outWriter;
            this.errorWriter = errorWriter;
            this.currentTestName = string.Empty;
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
            if (level++ == 0)
            {
                testRunCount = 0;
                testIgnoreCount = 0;
                failureCount = 0;

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
                    testRunCount++;
                    failureCount++;
                  
                    messages.Add(string.Format("{0}) {1} :", failureCount, nameAttr.Value));
                    break;

                case "Inconclusive":
                case "Passed":
                    testRunCount++;
                    break;

                case "Skipped":
                    testIgnoreCount++;
                    break;
            }

            var outputNode = testResult.SelectSingleNode("output");
            var labels = options.DisplayTestLabels != null 
                ? options.DisplayTestLabels.ToUpperInvariant()
                : "ON";

            if (labels == "ALL")
                WriteTestLabel(nameAttr.Value);
            if (outputNode != null)
            {
                if (labels == "ON")
                    WriteTestLabel(nameAttr.Value);
                outWriter.Write(outputNode.InnerText);
            }

            //currentTestName = string.Empty;
        }

        private void WriteTestLabel(string name)
        {
            using (new ColorConsole(ColorStyle.Label))
                outWriter.WriteLine("***** {0}", name);
        }

        public void SuiteFinished(XmlNode suiteResult)
        {
            if (--level == 0)
            {
                XmlAttribute durationAttribute = suiteResult.Attributes["duration"];

                Trace.WriteLine("############################################################################");

                if (messages.Count == 0)
                {
                    Trace.WriteLine("##############                 S U C C E S S               #################");
                }
                else
                {
                    Trace.WriteLine("##############                F A I L U R E S              #################");

                    foreach (string s in messages)
                    {
                        Trace.WriteLine(s);
                    }
                }

                Trace.WriteLine("############################################################################");
                Trace.WriteLine("Executed tests       : " + testRunCount);
                Trace.WriteLine("Ignored tests        : " + testIgnoreCount);
                Trace.WriteLine("Failed tests         : " + failureCount);
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
