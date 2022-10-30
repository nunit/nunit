// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnitLite
{
    /// <summary>
    /// OutputWriter is an abstract class used to write test
    /// results to a file in various formats. Specific 
    /// OutputWriters are derived from this class.
    /// </summary>
    public abstract class OutputWriter
    {
        /// <summary>
        /// Writes a test result to a file
        /// </summary>
        /// <param name="result">The result to be written</param>
        /// <param name="outputPath">Path to the file to which the result is written</param>
        /// <param name="runSettings">A dictionary of settings used for this test run</param>
        public void WriteResultFile(ITestResult result, string outputPath, IDictionary<string, object> runSettings, TestFilter filter)
        {
            using (var stream = new FileStream(outputPath, FileMode.Create))
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                WriteResultFile(result, writer, runSettings, filter);
            }
        }

        /// <summary>
        /// Writes test info to a file
        /// </summary>
        /// <param name="test">The test to be written</param>
        /// <param name="outputPath">Path to the file to which the test info is written</param>
        public void WriteTestFile(ITest test, string outputPath)
        {
            using (var stream = new FileStream(outputPath, FileMode.Create))
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                WriteTestFile(test, writer);
            }
        }

        /// <summary>
        /// Abstract method that writes a test result to a TextWriter
        /// </summary>
        /// <param name="result">The result to be written</param>
        /// <param name="writer">A TextWriter to which the result is written</param>
        /// <param name="runSettings">A dictionary of settings used for this test run</param>
        /// <param name="filter"></param>
        public abstract void WriteResultFile(ITestResult result, TextWriter writer, IDictionary<string, object> runSettings, TestFilter filter);

        /// <summary>
        /// Abstract method that writes test info to a TextWriter
        /// </summary>
        /// <param name="test">The test to be written</param>
        /// <param name="writer">A TextWriter to which the test info is written</param>
        public abstract void WriteTestFile(ITest test, TextWriter writer);
    }
}
