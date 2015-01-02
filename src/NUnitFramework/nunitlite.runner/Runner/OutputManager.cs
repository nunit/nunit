// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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
using System.Reflection;
using System.Xml;
using NUnit.Common;
using NUnit.Framework.Interfaces;

namespace NUnitLite.Runner
{
    /// <summary>
    /// OutputManager is responsible for creating output files
    /// from a test run in various formats.
    /// </summary>
    public class OutputManager
    {
        private string _workDirectory;

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
        public void WriteResultFile(ITestResult result, OutputSpecification spec)
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

                //case "user":
                //    Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
                //    string dir = Path.GetDirectoryName(uri.LocalPath);
                //    outputWriter = new XmlTransformOutputWriter(Path.Combine(dir, spec.Transform));
                //    break;

                default:
                    throw new ArgumentException(
                        string.Format("Invalid XML output format '{0}'", spec.Format),
                        "spec");
            }

            outputWriter.WriteResultFile(result, outputPath);
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

                //case "user":
                //    Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
                //    string dir = Path.GetDirectoryName(uri.LocalPath);
                //    outputWriter = new XmlTransformOutputWriter(Path.Combine(dir, spec.Transform));
                //    break;

                default:
                    throw new ArgumentException(
                        string.Format("Invalid XML output format '{0}'", spec.Format),
                        "spec");
            }

            outputWriter.WriteTestFile(test, outputPath);
            Console.WriteLine("Tests ({0}) saved as {1}", spec.Format, outputPath);
        }
    }
}
