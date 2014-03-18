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

namespace NUnit.ConsoleRunner
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Text;
    using NUnit.Engine;
    using NUnit.Util;
    
    /// <summary>
    /// ConsoleRunner provides the nunit-console text-based
    /// user interface, running the tests and reporting the results.
    /// </summary>
    public class ConsoleRunner
    {
        #region Console Runner Return Codes

        public static readonly int OK = 0;
        public static readonly int INVALID_ARG = -1;
        public static readonly int FILE_NOT_FOUND = -2;
        public static readonly int FIXTURE_NOT_FOUND = -3;
        public static readonly int UNEXPECTED_ERROR = -100;

        #endregion

        #region Instance Fields

        private ITestEngine engine;
        private ConsoleOptions options;

        private TextWriter outWriter = Console.Out;
        private TextWriter errorWriter = Console.Error;

        private string workDirectory;

        #endregion

        #region Constructor

        public ConsoleRunner(ITestEngine engine, ConsoleOptions options)
        {
            this.engine = engine;
            this.options = options;
            this.workDirectory = options.WorkDirectory;
            if (this.workDirectory == null)
                this.workDirectory = Environment.CurrentDirectory;
            else if (!Directory.Exists(this.workDirectory))
                Directory.CreateDirectory(this.workDirectory);
        }

        #endregion

        #region Execute Method

        /// <summary>
        /// Executes tests according to the provided commandline options.
        /// </summary>
        /// <returns></returns>
        public int Execute()
        {
            TestPackage package = MakeTestPackage(options);

            TestFilter filter = CreateTestFilter(options);

            if (options.Explore)
                return ExploreTests(package, filter);
            else
                return RunTests(package, filter);
        }

        #endregion

        #region Helper Methods

        private int ExploreTests(TestPackage package, TestFilter filter)
        {
            XmlNode result = engine.Explore(package, filter);

            if (options.ExploreOutputSpecifications.Count == 0)
            {
                new TestCaseOutputWriter().WriteResultFile(result, Console.Out);
            }
            else
            {
                var outputManager = new OutputManager(result, this.workDirectory);

                foreach (OutputSpecification spec in options.ExploreOutputSpecifications)
                    outputManager.WriteTestFile(spec);
            }

            return ConsoleRunner.OK;
        }

        private int RunTests(TestPackage package, TestFilter filter)
        {
            // TODO: We really need options as resolved by engine for most of  these
            DisplayRequestedOptions();

            // TODO: Incorporate this in EventCollector?
            RedirectOutputAsRequested();

            TestEventHandler eventHandler = new TestEventHandler(options, outWriter, errorWriter);

            XmlNode result = null;

            // Save things that might be messed up by a bad test
            TextWriter savedOut = Console.Out;
            TextWriter savedError = Console.Error;

            DateTime startTime = DateTime.Now;

            try
            {
                using ( new ColorConsole( ColorStyle.Output ) )
#if true
                result = engine.Run(package, eventHandler, filter);
#else
                using (ITestRunner runner = engine.GetRunner(package))
                {
                    if (runner.Load(package))
                        engineResult = runner.Run(eventHandler, testFilter);
                }
#endif
            }
            finally
            {
                Console.SetOut(savedOut);
                Console.SetError(savedError);

                RestoreOutput();
            }

            //Console.WriteLine();

            ResultReporter reporter = new ResultReporter(result, options);
            reporter.ReportResults();

            // TODO: Inject this?
            var outputManager = new OutputManager(result, this.workDirectory);

            foreach (var outputSpec in options.ResultOutputSpecifications)
                outputManager.WriteResultFile(outputSpec, startTime);

            return reporter.Summary.ErrorsAndFailures;
        }

        private void DisplayRequestedOptions()
        {
            ColorConsole.WriteLine(ColorStyle.SectionHeader, "Options -");
            ColorConsole.WriteLabel("    ProcessModel: ", options.ProcessModel ?? "Default", false);
            ColorConsole.WriteLabel("    DomainUsage: ", options.DomainUsage ?? "Default", true);
            ColorConsole.WriteLabel("    Execution Runtime: ", options.Framework ?? "Not Specified", true);
            if (options.DefaultTimeout >= 0)
                ColorConsole.WriteLabel("    Default timeout: ", options.DefaultTimeout.ToString(), true);
            if (options.NumWorkers > 0)
                ColorConsole.WriteLabel("    Worker Threads: ", options.NumWorkers.ToString(), true);
            ColorConsole.WriteLabel("    Work Directory: ", workDirectory, true);
            ColorConsole.WriteLabel("    Internal Trace: ", options.InternalTraceLevel ?? "Off", true);
            //if (options.DisplayTeamCityServiceMessages)
            //    Console.WriteLine("    Display TeamCity Service Messages");
            Console.WriteLine();

            if (options.TestList.Count > 0)
            {
                ColorConsole.WriteLine(ColorStyle.Label, "Selected test(s):");
                using (new ColorConsole(ColorStyle.Default))
                    foreach (string testName in options.TestList)
                        Console.WriteLine("    " + testName);
            }

            if (!string.IsNullOrEmpty( options.Include ))
                ColorConsole.WriteLabel("Included categories: ", options.Include, true);

            if (!string.IsNullOrEmpty( options.Exclude ))
                ColorConsole.WriteLabel("Excluded categories: ", options.Exclude, true);
        }

        private void RedirectOutputAsRequested()
        {
            if (options.OutFile != null)
            {
                StreamWriter outStreamWriter = new StreamWriter(Path.Combine(workDirectory, options.OutFile));
                outStreamWriter.AutoFlush = true;
                this.outWriter = outStreamWriter;
            }

            if (options.ErrFile != null)
            {
                StreamWriter errorStreamWriter = new StreamWriter(Path.Combine(workDirectory, options.ErrFile));
                errorStreamWriter.AutoFlush = true;
                this.errorWriter = errorStreamWriter;
            }
        }

        private void RestoreOutput()
        {
            outWriter.Flush();
            if (options.OutFile != null)
                outWriter.Close();

            errorWriter.Flush();
            if (options.ErrFile != null)
                errorWriter.Close();
        }

        // This is public static for ease of testing
        public static TestPackage MakeTestPackage( ConsoleOptions options )
        {
            TestPackage package = options.InputFiles.Count == 1
                ? new TestPackage(options.InputFiles[0])
                : new TestPackage(options.InputFiles);

            if (options.ProcessModel != null)//ProcessModel.Default)
                package.Settings["ProcessModel"] = options.ProcessModel;

            if (options.DomainUsage != null)
                package.Settings["DomainUsage"] = options.DomainUsage;

            if (options.Framework != null)
                package.Settings["RuntimeFramework"] = options.Framework;

            if (options.DefaultTimeout >= 0)
                package.Settings["DefaultTimeout"] = options.DefaultTimeout;

            if (options.InternalTraceLevel != null)
                package.Settings["InternalTraceLevel"] = options.InternalTraceLevel;

            if (options.ActiveConfig != null)
                package.Settings["ActiveConfig"] = options.ActiveConfig;
            
            if (options.WorkDirectory != null)
                package.Settings["WorkDirectory"] = options.WorkDirectory;

            if (options.StopOnError)
                package.Settings["StopOnError"] = true;

            if (options.NumWorkers > 0)
                package.Settings["NumberOfTestWorkers"] = options.NumWorkers;

            if (options.RandomSeed > 0)
                package.Settings["RandomSeed"] = options.RandomSeed;

            if (options.Verbose)
                package.Settings["Verbose"] = true;

#if DEBUG
            //foreach (KeyValuePair<string, object> entry in package.Settings)
            //    if (!(entry.Value is string || entry.Value is int || entry.Value is bool))
            //        throw new Exception(string.Format("Package setting {0} is not a valid type", entry.Key));
#endif
            
            return package;
        }

        // This is public static for ease of testing
        public static TestFilter CreateTestFilter(ConsoleOptions options)
        {
            TestFilterBuilder builder = new TestFilterBuilder();
            foreach (string testName in options.TestList)
                builder.Tests.Add(testName);

            // TODO: Support multiple include / exclude options

            if (options.Include != null)
                builder.Include.Add(options.Include);

            if (options.Exclude != null)
                builder.Exclude.Add(options.Exclude);

            return builder.GetFilter();
        }

        #endregion
    }
}

