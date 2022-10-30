// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnitLite
{
    /// <summary>
    /// OutputManager is responsible for creating output files
    /// from a test run in various formats.
    /// </summary>
    public class OutputManager
    {
        private readonly string _workDirectory;

        /// <summary>
        /// Construct an OutputManager
        /// </summary>
        /// <param name="workDirectory">The directory to use for reports</param>
        public OutputManager(string workDirectory)
        {
            _workDirectory = workDirectory;
        }

        /// <summary>
        /// Write the result of a test run according to a spec.
        /// </summary>
        /// <param name="result">The test result</param>
        /// <param name="spec">An output specification</param>
        /// <param name="runSettings">Settings</param>
        /// <param name="filter">Filter</param>
        public void WriteResultFile(ITestResult result, OutputSpecification spec, IDictionary<string, object> runSettings, TestFilter filter)
        {
            string outputPath = Path.Combine(_workDirectory, spec.OutputPath);
            OutputWriter outputWriter = null;

            switch (spec.Format)
            {
                case "nunit3":
                    outputWriter = new NUnit3XmlOutputWriter();
                    break;

                case "nunit2":
                    outputWriter = new NUnit2XmlOutputWriter();
                    break;

                default:
                    throw new ArgumentException(
                        $"Invalid XML output format '{spec.Format}'",
                        nameof(spec));
            }

            outputWriter.WriteResultFile(result, outputPath, runSettings, filter);
            Console.WriteLine("Results ({0}) saved as {1}", spec.Format, outputPath);
        }

        /// <summary>
        /// Write out the result of exploring the tests
        /// </summary>
        /// <param name="test">The top-level test</param>
        /// <param name="spec">An OutputSpecification</param>
        public void WriteTestFile(ITest test, OutputSpecification spec)
        {
            string outputPath = Path.Combine(_workDirectory, spec.OutputPath);
            OutputWriter outputWriter = null;

            switch (spec.Format)
            {
                case "nunit3":
                    outputWriter = new NUnit3XmlOutputWriter();
                    break;

                case "cases":
                    outputWriter = new TestCaseOutputWriter();
                    break;

                default:
                    throw new ArgumentException(
                        $"Invalid output format '{spec.Format}'",
                        nameof(spec));
            }

            outputWriter.WriteTestFile(test, outputPath);
            Console.WriteLine("Tests ({0}) saved as {1}", spec.Format, outputPath);
        }
    }
}
