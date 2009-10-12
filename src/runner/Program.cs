// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
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
using System.Reflection;
using System.IO;
using NUnit.Core;

namespace NUnit.TestRunner
{
    /// <summary>
    /// NUnit.TestRunner is an adoc runner used in testing the framework.
    /// It is based partly on the NUnitLite embedded runner but is not 
    /// is only available in the source code for use by developers and
    /// is not distributed as part of NUnit.
    /// </summary>
    public class Program
    {
        static int reportCount;

        static void Main(string[] args)
        {
            CommandLineOptions options = new CommandLineOptions();
            options.Parse(args);

            if (!options.Nologo)
                WriteCopyright();

            if (options.Help)
                Console.Write(options.HelpText);
            else if (options.Error)
                Console.WriteLine(options.ErrorMessage);
            else if (options.Parameters.Length == 0)
                Console.WriteLine("No Test assembly was specified");
            else
            {
                try
                {
                    NUnit.Core.TestRunner runner = new RemoteTestRunner();

                    TestPackage package = new TestPackage("Top Level Suite", options.Parameters);
                    package.Settings["AutoNamespaceSuites"] = false;
                    package.Settings["MergeAssemblies"] = true;

                    if (!runner.Load(package))
                        Console.WriteLine("Unable to load package");
                    else
                    {
                        TextWriter savedOut = Console.Out;
                        TextWriter savedError = Console.Error;

                        TestEventListener listener = new TestEventListener(options, Console.Out);

                        TestResult result = runner.Run(listener, TestFilter.Empty);

                        Console.SetOut(savedOut);
                        Console.SetError(savedError);

                        ReportResults(result);
                    }
                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    if (options.Wait)
                    {
                        Console.WriteLine("Press Enter key to continue . . .");
                        Console.ReadLine();
                    }
                }
            }
        }

        private static void WriteCopyright()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            System.Version version = executingAssembly.GetName().Version;

            object[] objectAttrs = executingAssembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            AssemblyProductAttribute productAttr = (AssemblyProductAttribute)objectAttrs[0];

            objectAttrs = executingAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            AssemblyCopyrightAttribute copyrightAttr = (AssemblyCopyrightAttribute)objectAttrs[0];

            Console.WriteLine(String.Format("{0} version {1}", productAttr.Product, version.ToString(3)));
            Console.WriteLine(copyrightAttr.Copyright);
            Console.WriteLine();

            string clrPlatform = Type.GetType("Mono.Runtime", false) == null ? ".NET" : "Mono";
            Console.WriteLine("Runtime Environment -");
            Console.WriteLine("    OS Version: {0}", Environment.OSVersion);
            Console.WriteLine("  {0} Version: {1}", clrPlatform, Environment.Version);
            Console.WriteLine();
        }

        /// <summary>
        /// Reports the results.
        /// </summary>
        /// <param name="result">The result.</param>
        private static void ReportResults(TestResult result)
        {
            ResultSummary summary = new ResultSummary(result);
            WriteSummaryReport(summary);

            if (summary.ErrorsAndFailures > 0)
                WriteErrorsAndFailuresReport(result);

            if (summary.TestsNotRun > 0)
                WriteNotRunReport(result);
        }

        private static void WriteSummaryReport(ResultSummary summary)
        {
            Console.WriteLine(
                "   Tests run: {0}, Errors: {1}, Failures: {2}, Inconclusive: {3}",
                summary.TestsRun, summary.Errors, summary.Failures, summary.Inconclusive);
            Console.WriteLine(
                "     Not run: {0}, Invalid: {1}, Ignored: {2}, Skipped: {3}",
                summary.TestsNotRun, summary.NotRunnable, summary.Ignored, summary.Skipped);
            Console.WriteLine(
                "        Time: {0} seconds", summary.Time);
            Console.WriteLine();
        }

        private static void WriteErrorsAndFailuresReport(TestResult result)
        {
            reportIndex = 0;
            Console.WriteLine("Errors and Failures:");
            WriteErrorsAndFailures(result);
            Console.WriteLine();
        }

        private static void WriteErrorsAndFailures(TestResult result)
        {
            if (result.Executed)
            {
                if (result.HasResults)
                {
                    if ((result.IsFailure || result.IsError) && result.FailureSite == FailureSite.SetUp)
                        WriteSingleResult(result);

                    foreach (TestResult childResult in result.Results)
                        WriteErrorsAndFailures(childResult);
                }
                else if (result.IsFailure || result.IsError)
                {
                    WriteSingleResult(result);
                }
            }
        }

        private static void WriteNotRunReport(TestResult result)
        {
            reportIndex = 0;
            Console.WriteLine("Tests Not Run:");
            WriteNotRunResults(result);
            Console.WriteLine();
        }

        private static int reportIndex = 0;
        private static void WriteNotRunResults(TestResult result)
        {
            if (result.HasResults)
                foreach (TestResult childResult in result.Results)
                    WriteNotRunResults(childResult);
            else if (!result.Executed)
                WriteSingleResult(result);
        }

        private static void WriteSingleResult(TestResult result)
        {
            string status = result.IsFailure || result.IsError
                ? string.Format("{0} {1}", result.FailureSite, result.ResultState)
                : result.ResultState.ToString();

            Console.WriteLine("{0}) {1} : {2}", ++reportIndex, status, result.FullName);

            if (result.Message != null && result.Message != string.Empty)
                Console.WriteLine(result.Message);

            if (result.StackTrace != null && result.StackTrace != string.Empty)
                Console.WriteLine(result.IsFailure
                    ? StackTraceFilter.Filter(result.StackTrace)
                    : result.StackTrace + Environment.NewLine);
        }        
    }
}
