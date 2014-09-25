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

namespace NUnit.ConsoleRunner
{
    using Common;

    public class OutputManager
    {
        private XmlNode result;
        private string workDirectory;

        public OutputManager(XmlNode result, string workDirectory)
        {
            this.result = result;
            this.workDirectory = workDirectory;
        }

        public void WriteResultFile(OutputSpecification spec, DateTime startTime)
        {
            string outputPath = Path.Combine(workDirectory, spec.OutputPath);
            IResultWriter outputWriter = null;

            switch (spec.Format)
            {
                case "nunit3":
                    outputWriter = new NUnit3XmlOutputWriter();
                    break;

                case "nunit2":
                    outputWriter = new NUnit2XmlOutputWriter();
                    break;

                case "user":
                    Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
                    string dir = Path.GetDirectoryName(uri.LocalPath);
                    outputWriter = new XmlTransformOutputWriter(Path.Combine(dir, spec.Transform));
                    break;

                default:
                    throw new ArgumentException(
                        string.Format("Invalid XML output format '{0}'", spec.Format),
                        "spec");
            }

            outputWriter.WriteResultFile(result, outputPath);
            Console.WriteLine("Results ({0}) saved as {1}", spec.Format, outputPath);
        }

        public void WriteTestFile(OutputSpecification spec)
        {
            string outputPath = Path.Combine(workDirectory, spec.OutputPath);
            IResultWriter outputWriter = null;

            switch (spec.Format)
            {
                case "nunit3":
                    outputWriter = new NUnit3XmlOutputWriter();
                    break;

                case "cases":
                    outputWriter = new TestCaseOutputWriter();
                    break;

                case "user":
                    Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
                    string dir = Path.GetDirectoryName(uri.LocalPath);
                    outputWriter = new XmlTransformOutputWriter(Path.Combine(dir, spec.Transform));
                    break;

                default:
                    throw new ArgumentException(
                        string.Format("Invalid XML output format '{0}'", spec.Format),
                        "spec");
            }

            outputWriter.WriteResultFile(result, outputPath);
            Console.WriteLine("Tests ({0}) saved as {1}", spec.Format, outputPath);
        }
    }
}
