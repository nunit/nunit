// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Api;

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

            testRun.ChildNodes.Add(MakeCommandLineElement());
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

            testRun.AddAttribute("start-time", result.StartTime.ToString("o"));
            testRun.AddAttribute("end-time", result.EndTime.ToString("o"));
            testRun.AddAttribute("duration", result.Duration.ToString("0.000000", NumberFormatInfo.InvariantInfo));

            testRun.AddAttribute("total", result.TotalCount.ToString());
            testRun.AddAttribute("passed", result.PassCount.ToString());
            testRun.AddAttribute("failed", result.FailCount.ToString());
            testRun.AddAttribute("inconclusive", result.InconclusiveCount.ToString());
            testRun.AddAttribute("skipped", result.SkipCount.ToString());
            testRun.AddAttribute("warnings", result.WarningCount.ToString());
            testRun.AddAttribute("asserts", result.AssertCount.ToString());
            testRun.AddAttribute("random-seed", Randomizer.InitialSeed.ToString());

            // NOTE: The console runner adds attributes for engine-version and clr-version
            // Neither of these is needed under nunitlite since there is no engine involved
            // and we are running under the same runtime as the tests.

            return testRun;
        }

        private static TNode MakeCommandLineElement()
        {
            return new TNode("command-line", Environment.CommandLine, true);
        }

        private static TNode MakeTestFilterElement(TestFilter filter)
        {
            TNode result = new TNode("filter");
            if (filter != null && !filter.IsEmpty)
                filter.AddToXml(result, true);
            return result;
        }
    }
}
