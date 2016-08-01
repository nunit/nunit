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
using System.Xml;
using NUnit.Common;
using NUnit.Engine;
using NUnit.ConsoleRunner.Utilities;

namespace NUnit.ConsoleRunner
{    
    /// <summary>
    /// TestEventHandler processes events from the running
    /// test for the console runner.
    /// </summary>
    public class TestEventHandler : MarshalByRefObject, ITestEventListener
    {
        private readonly string _displayLabels;
        private readonly TextWriter _outWriter;

        public TestEventHandler(TextWriter outWriter, string displayLabels)
        {
            _displayLabels = displayLabels.ToUpperInvariant();
            _outWriter = outWriter;
        }

        #region ITestEventHandler Members

        public void OnTestEvent(string report)
        {
            var doc = new XmlDocument();
            doc.LoadXml(report);

            var testEvent = doc.FirstChild;
            switch (testEvent.Name)
            {
                case "test-case":
                    TestFinished(testEvent);
                    break;

                case "test-suite":
                    SuiteFinished(testEvent);
                    break;

                case "test-output":
                    TestOutput(testEvent);
                    break;
            }
        }

        #endregion

        #region Individual Handlers

        private void TestFinished(XmlNode testResult)
        {
            var testName = testResult.Attributes["fullname"].Value;
            var outputNode = testResult.SelectSingleNode("output");

            if (_displayLabels == "ALL")
                WriteLabelLine(testName);

            if (outputNode != null)
            {
                if (_displayLabels == "ON")
                    WriteLabelLine(testName);

                WriteOutputLine(outputNode.InnerText);
            }
        }

        private void SuiteFinished(XmlNode testResult)
        {
            var suiteName = testResult.Attributes["fullname"].Value;
            var outputNode = testResult.SelectSingleNode("output");

            if (outputNode != null)
            {
                if (_displayLabels == "ON" || _displayLabels == "ALL")
                    WriteLabelLine(suiteName);

                WriteOutputLine(outputNode.InnerText);
            }
        }

        private void TestOutput(XmlNode outputNode)
        {
            var testName = outputNode.GetAttribute("testname");
            var stream = outputNode.GetAttribute("stream");

            if (_displayLabels == "ON" && testName != null)
                WriteLabelLine(testName);

            WriteOutputLine(outputNode.InnerText, stream == "Error" ? ColorStyle.Error : ColorStyle.Output);
        }

        private string _currentLabel;

        private void WriteLabelLine(string label)
        {
            if (label != _currentLabel)
            {
                using (new ColorConsole(ColorStyle.SectionHeader))
                    _outWriter.WriteLine("=> {0}", label);

                _currentLabel = label;
            }
        }

        private void WriteOutputLine(string text)
        {
            WriteOutputLine(text, ColorStyle.Output);
        }

        private void WriteOutputLine(string text, ColorStyle color)
        {
            using (new ColorConsole(color))
            {
                _outWriter.Write(text);

                // Some labels were being shown on the same line as the previous output
                if (!text.EndsWith("\n"))
                {
                    _outWriter.WriteLine();
                }
            }
        }

        #endregion

        #region InitializeLifetimeService

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #endregion
    }
}
