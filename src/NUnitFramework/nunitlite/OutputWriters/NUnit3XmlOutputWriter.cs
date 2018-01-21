// ***********************************************************************
// Copyright (c) 2011 Charlie Poole, Rob Prouse
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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using NUnit.Common;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnitLite
{
    /// <summary>
    /// NUnit3XmlOutputWriter is responsible for writing the results
    /// of a test to a file in NUnit 3 format.
    /// </summary>
    public class NUnit3XmlOutputWriter : OutputWriter
    {
        /// <summary>
        /// Writes test info to the specified TextWriter
        /// </summary>
        /// <param name="test">The test to be written</param>
        /// <param name="writer">A TextWriter to which the test info is written</param>
        public override void WriteTestFile(ITest test, TextWriter writer)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter xmlWriter = XmlWriter.Create(writer, settings))
            {
                test.ToXml(true).WriteTo(xmlWriter);
            }
        }

        /// <summary>
        /// Writes the test result to the specified TextWriter
        /// </summary>
        /// <param name="result">The result to be written to a file</param>
        /// <param name="writer">A TextWriter to which the result is written</param>
        /// <param name="runSettings"></param>
        /// <param name="filter"></param>
        public override void WriteResultFile(ITestResult result, TextWriter writer, IDictionary<string, object> runSettings, TestFilter filter)
        {
            XmlWriterSettings xmlSettings = new XmlWriterSettings();
            xmlSettings.Indent = true;

            using (XmlWriter xmlWriter = XmlWriter.Create(writer, xmlSettings))
            {
                WriteXmlResultOutput(result, xmlWriter, runSettings, filter);
            }
        }

        private void WriteXmlResultOutput(ITestResult result, XmlWriter xmlWriter, IDictionary<string, object> runSettings, TestFilter filter)
        {
            TNode resultNode = result.ToXml(true);

            // Insert elements as first child in reverse order
            if (runSettings != null) // Some platforms don't have settings
                FrameworkController.InsertSettingsElement(resultNode, runSettings);
            FrameworkController.InsertEnvironmentElement(resultNode);

            TNode testRun = MakeTestRunElement(result);

#if !NETSTANDARD1_6
            testRun.ChildNodes.Add(MakeCommandLineElement());
#endif
            testRun.ChildNodes.Add(MakeTestFilterElement(filter));
            testRun.ChildNodes.Add(resultNode);

            testRun.WriteTo(xmlWriter);
        }

        private TNode MakeTestRunElement(ITestResult result)
        {
            TNode testRun = new TNode("test-run");

            testRun.AddAttribute("id", "2");
            testRun.AddAttribute("name", result.Name);
            testRun.AddAttribute("fullname", result.FullName);
            testRun.AddAttribute("testcasecount", result.Test.TestCaseCount.ToString());

            testRun.AddAttribute("result", result.ResultState.Status.ToString());
            if (result.ResultState.Label != string.Empty)
                testRun.AddAttribute("label", result.ResultState.Label);

            testRun.AddAttribute("start-time", result.StartTime.ToString("u"));
            testRun.AddAttribute("end-time", result.EndTime.ToString("u"));
            testRun.AddAttribute("duration", result.Duration.ToString("0.000000", NumberFormatInfo.InvariantInfo));

            testRun.AddAttribute("total", (result.PassCount + result.FailCount + result.SkipCount + result.InconclusiveCount).ToString());
            testRun.AddAttribute("passed", result.PassCount.ToString());
            testRun.AddAttribute("failed", result.FailCount.ToString());
            testRun.AddAttribute("inconclusive", result.InconclusiveCount.ToString());
            testRun.AddAttribute("skipped", result.SkipCount.ToString());
            testRun.AddAttribute("asserts", result.AssertCount.ToString());

            testRun.AddAttribute("random-seed", Randomizer.InitialSeed.ToString());

            // NOTE: The console runner adds attributes for engine-version and clr-version
            // Neither of these is needed under nunitlite since there is no engine involved
            // and we are running under the same runtime as the tests.

            return testRun;
        }

#if !NETSTANDARD1_6
        private static TNode MakeCommandLineElement()
        {
            return new TNode("command-line", Environment.CommandLine, true);
        }
#endif

        private static TNode MakeTestFilterElement(TestFilter filter)
        {
            TNode result = new TNode("filter");
            if (!filter.IsEmpty)
                filter.AddToXml(result, true);
            return result;
        }
    }
}
