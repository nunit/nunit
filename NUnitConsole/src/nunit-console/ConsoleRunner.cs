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
	using System.Reflection;
	using System.Xml;
	using System.Resources;
	using System.Text;
    using NUnit.Engine;
	
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
        }

        #endregion

        #region Execute Method

        /// <summary>
        /// Executes tests according to the provided commandline options.
        /// </summary>
        /// <returns></returns>
        public int Execute()
		{
            // Create the test package
            TestPackage package = MakeTestPackage(options);

            if (options.Explore)
                return ExploreTests(package);
            else
                return RunTests(package);
        }

        #endregion

        #region Helper Methods

        private int ExploreTests(TestPackage package)
        {
            ITestEngineResult engineResult = engine.Explore(package);
            int returnCode = ConsoleRunner.OK;

            if (engineResult.HasErrors)
            {
                DisplayErrorMessages(engineResult);
                returnCode = ConsoleRunner.UNEXPECTED_ERROR;
            }
            else if (options.ExploreOutputSpecifications.Count == 0)
            {
                new TestCaseOutputWriter().WriteResultFile(engineResult.Xml, Console.Out);
            }
            else
            {
                var outputManager = new OutputManager(engineResult.Xml, workDirectory);

                foreach (OutputSpecification spec in options.ExploreOutputSpecifications)
                    outputManager.WriteTestFile(spec);
            }

            return returnCode;
        }

        private int RunTests(TestPackage package)
        {
            // TODO: We really need options as resolved by engine for most of  these
            DisplayRequestedOptions();

            // TODO: Incorporate this in EventCollector?
            RedirectOutputAsRequested();

            TestEventHandler eventHandler = new TestEventHandler(options, outWriter, errorWriter);

            TestFilter testFilter = CreateTestFilter(options);

            ITestEngineResult engineResult = null;

            // Save things that might be messed up by a bad test
            TextWriter savedOut = Console.Out;
            TextWriter savedError = Console.Error;

            try
            {
#if true
                engineResult = engine.Run(package, eventHandler, testFilter);
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

            int returnCode = UNEXPECTED_ERROR;

            if (engineResult.HasErrors)
                DisplayErrorMessages(engineResult);
            else
            {
                ResultReporter reporter = new ResultReporter(engineResult.Xml);
                reporter.ReportResults();

                // TODO: Inject this?
                var outputManager = new OutputManager(engineResult.Xml, options.WorkDirectory);

                foreach (var outputSpec in options.ResultOutputSpecifications)
                    outputManager.WriteResultFile(outputSpec);

                returnCode = reporter.Summary.ErrorsAndFailures;

                //if ( collector.HasExceptions )
                //{
                //    collector.WriteExceptions();
                //    returnCode = UNEXPECTED_ERROR;
                //}
            }

            return returnCode;
        }

        private void DisplayRequestedOptions()
        {
            Console.WriteLine("ProcessModel: {0}    DomainUsage: {1}", options.ProcessModel, options.DomainUsage);
            Console.WriteLine("Execution Runtime: {0}", options.Framework == null ? "Not Specified" : options.Framework);
            Console.WriteLine();

            if (options.TestList.Length > 0)
            {
                Console.WriteLine("Selected test(s):");
                foreach (string testName in options.TestList)
                    Console.WriteLine("    " + testName);
            }

            if (options.Include != null && options.Include != string.Empty)
                Console.WriteLine("Included categories: " + options.Include);

            if (options.Exclude != null && options.Exclude != string.Empty)
                Console.WriteLine("Excluded categories: " + options.Exclude);
        }

        private void RedirectOutputAsRequested()
        {
            if (options.OutputPath != null)
            {
                StreamWriter outStreamWriter = new StreamWriter(Path.Combine(workDirectory, options.OutputPath));
                outStreamWriter.AutoFlush = true;
                this.outWriter = outStreamWriter;
            }

            if (options.ErrorPath != null)
            {
                StreamWriter errorStreamWriter = new StreamWriter(Path.Combine(workDirectory, options.ErrorPath));
                errorStreamWriter.AutoFlush = true;
                this.errorWriter = errorStreamWriter;
            }
        }

        private void RestoreOutput()
        {
            outWriter.Flush();
            if (options.OutputPath != null)
                outWriter.Close();

            errorWriter.Flush();
            if (options.ErrorPath != null)
                errorWriter.Close();
        }

        // This is public static for ease of testing
        public static TestPackage MakeTestPackage( ConsoleOptions options )
        {
            TestPackage package = options.InputFiles.Length == 1
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
            
            return package;
		}

        // This is public static for ease of testing
        public static TestFilter CreateTestFilter(ConsoleOptions options)
        {
            // TODO: Implement filtering
            TestFilter testFilter = TestFilter.Empty;
            //if (options.RunList.Length > 0)
            //{
            //    testFilter = new SimpleNameFilter(TestNameParser.Parse(options.run));
            //}

            //if (options.include != null && options.include != string.Empty)
            //{
            //    TestFilter includeFilter = new CategoryExpression(options.include).Filter;
            //    if (testFilter.IsEmpty)
            //        testFilter = includeFilter;
            //    else
            //        testFilter = new AndFilter(testFilter, includeFilter);
            //}

            //if (options.exclude != null && options.exclude != string.Empty)
            //{
            //    TestFilter excludeFilter = new NotFilter(new CategoryExpression(options.exclude).Filter);
            //    if (testFilter.IsEmpty)
            //        testFilter = excludeFilter;
            //    else if (testFilter is AndFilter)
            //        ((AndFilter)testFilter).Add(excludeFilter);
            //    else
            //        testFilter = new AndFilter(testFilter, excludeFilter);
            //}

            //if (testFilter is NotFilter)
            //    ((NotFilter)testFilter).TopLevel = true;

            return testFilter;
        }

        private static string CreateXmlOutput(ITestEngineResult result)
        {
            StringBuilder builder = new StringBuilder();

            XmlTextWriter writer = new XmlTextWriter(new StringWriter(builder));
            writer.Formatting = Formatting.Indented;

            result.Xml.WriteTo(writer);
            writer.Close();

            return builder.ToString();
        }

        private static void DisplayErrorMessages(ITestEngineResult errorReport)
        {
            foreach (TestEngineError error in errorReport.Errors)
            {
                if (error.Message != null)
                {
                    Console.WriteLine("Load failure: {0}", error.Message);
                    if (error.StackTrace != null)
                        Console.WriteLine(error.StackTrace);
                }
            }
        }
        
        #endregion
	}
}

