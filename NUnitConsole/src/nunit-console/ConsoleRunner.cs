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
        #region Return Codes

        public static readonly int OK = 0;
		public static readonly int INVALID_ARG = -1;
		public static readonly int FILE_NOT_FOUND = -2;
		public static readonly int FIXTURE_NOT_FOUND = -3;
		public static readonly int UNEXPECTED_ERROR = -100;

        #endregion

        #region Execute Method

        /// <summary>
        /// Runs a set of tests using the provided options.
        /// </summary>
        /// <param name="options">An instance of ConsoleOptions</param>
        /// <returns></returns>
        public int Execute( ConsoleOptions options )
		{
            // Base directory for test output
            string workDir = options.WorkDirectory;

            // Redirect Console.Out if requested
			TextWriter outWriter = Console.Out;
			bool redirectOutput = options.outputPath != null && options.outputPath != string.Empty;
			if ( redirectOutput )
			{
				StreamWriter outStreamWriter = new StreamWriter( Path.Combine(workDir, options.outputPath) );
				outStreamWriter.AutoFlush = true;
				outWriter = outStreamWriter;
			}

            // Redirect Console.Error if requested
			TextWriter errorWriter = Console.Error;
            bool redirectError = options.errorPath != null && options.errorPath != string.Empty;
			if (redirectError )
			{
				StreamWriter errorStreamWriter = new StreamWriter( Path.Combine(workDir, options.errorPath ));
				errorStreamWriter.AutoFlush = true;
				errorWriter = errorStreamWriter;
			}

            // Create the test package
            TestPackage package = MakeTestPackage(options);

            // Display key options for the run
            // TODO: These are requested options - should be options as resolved by engine
            Console.WriteLine("ProcessModel: {0}    DomainUsage: {1}", options.processModel, options.domainUsage);
            Console.WriteLine("Execution Runtime: {0}", options.framework == null ? "Not Specified" : options.framework);
            Console.WriteLine();

            //testRunner.Load(package);

            //if (testRunner.Test == null)
            //{
            //    testRunner.Unload();
            //    Console.Error.WriteLine("Unable to locate fixture {0}", options.fixture);
            //    return FIXTURE_NOT_FOUND;
            //}

            //EventCollector collector = new EventCollector( options, outWriter, errorWriter );

            TestFilter testFilter = TestFilter.Empty;
            //if ( options.run != null && options.run != string.Empty )
            //{
            //    Console.WriteLine( "Selected test(s): " + options.run );
            //    testFilter = new SimpleNameFilter( TestNameParser.Parse(options.run) );
            //}

            //if ( options.include != null && options.include != string.Empty )
            //{
            //    Console.WriteLine( "Included categories: " + options.include );
            //    TestFilter includeFilter = new CategoryExpression( options.include ).Filter;
            //    if ( testFilter.IsEmpty )
            //        testFilter = includeFilter;
            //    else
            //        testFilter = new AndFilter( testFilter, includeFilter );
            //}

            //if ( options.exclude != null && options.exclude != string.Empty )
            //{
            //    Console.WriteLine( "Excluded categories: " + options.exclude );
            //    TestFilter excludeFilter = new NotFilter( new CategoryExpression( options.exclude ).Filter );
            //    if ( testFilter.IsEmpty )
            //        testFilter = excludeFilter;
            //    else if ( testFilter is AndFilter )
            //        ((AndFilter)testFilter).Add( excludeFilter );
            //    else
            //        testFilter = new AndFilter( testFilter, excludeFilter );
            //}

            //if (testFilter is NotFilter)
            //    ((NotFilter)testFilter).TopLevel = true;

			XmlNode result = null;
			string savedDirectory = Environment.CurrentDirectory;
			TextWriter savedOut = Console.Out;
			TextWriter savedError = Console.Error;

            try
            {
                ITestEngine engine = TestEngineActivator.CreateInstance();

                
#if false
                result = engine.Run(package, testFilter /*collector, testFilter*/ ).GetXml();
#else
                using (ITestRunner runner = engine.GetRunner(package))
                {
                    if (runner.Load(package))
                        result = runner.Run(testFilter).GetXml();
                }
#endif
            }
            finally
            {
                outWriter.Flush();
                errorWriter.Flush();

                if (redirectOutput)
                    outWriter.Close();
                if (redirectError)
                    errorWriter.Close();

                Environment.CurrentDirectory = savedDirectory;
                Console.SetOut(savedOut);
                Console.SetError(savedError);
            }

            //Console.WriteLine();

            int returnCode = UNEXPECTED_ERROR;

            if (result != null)
            {
                string xmlOutput = CreateXmlOutput(result);

                ResultReporter reporter = new ResultReporter(result);
                reporter.ReportResults();

                //ResultSummarizer summary = new ResultSummarizer(result);
                //WriteSummaryReport(summary);
                //if (summary.ErrorsAndFailures > 0 || result.IsError || result.IsFailure)
                //    WriteErrorsAndFailuresReport(result);
                //if (summary.TestsNotRun > 0)
                //    WriteNotRunReport(result);

                if (!options.noxml)
                {
                    // Write xml output here
                    string xmlResultFile = options.xmlPath == null || options.xmlPath == string.Empty
                        ? "TestResult.xml" : options.xmlPath;

                    using (StreamWriter writer = new StreamWriter(Path.Combine(workDir, xmlResultFile)))
                    {
                        writer.Write(xmlOutput);
                    }
                }

                returnCode = reporter.Summary.ErrorsAndFailures;

                //if ( collector.HasExceptions )
                //{
                //    collector.WriteExceptions();
                //    returnCode = UNEXPECTED_ERROR;
                //}
            }

            return returnCode;
        }

        #endregion

        #region Helper Methods
        // This is public for testing only
        public static TestPackage MakeTestPackage( ConsoleOptions options )
        {
            TestPackage package = new TestPackage();

            foreach (string testfile in options.InputFiles)
            {
                TestPackage subpackage = new TestPackage(testfile);

                package.Add(subpackage);

                if (options.processModel != ProcessModel.Default)
                    subpackage.Settings["ProcessModel"] = options.processModel;

                if (options.domainUsage != DomainUsage.Default)
                    subpackage.Settings["DomainUsage"] = options.domainUsage;

                if (options.framework != null)
                    subpackage.Settings["RuntimeFramework"] = options.framework;

                if (options.defaultTimeout >= 0)
                    subpackage.Settings["DefaultTimeout"] = options.defaultTimeout;

                if (options.internalTraceLevel != InternalTraceLevel.Default)
                    subpackage.Settings["InternalTraceLevel"] = options.internalTraceLevel;

                if (options.activeConfig != null)
                    subpackage.Settings["ActiveConfig"] = options.activeConfig;
            }

            return package.SubPackages.Length == 1 ? package.SubPackages[0] : package;
		}

        private static string CreateXmlOutput(XmlNode result)
        {
            StringBuilder builder = new StringBuilder();

            XmlTextWriter writer = new XmlTextWriter(new StringWriter(builder));
            writer.Formatting = Formatting.Indented;
            result.WriteTo(writer);
            writer.Close();

            return builder.ToString();
        }

        //private static void WriteSummaryReport(ResultSummary summary)
        //{
        //    Console.WriteLine(
        //        "Tests run: {0}, Errors: {1}, Failures: {2}, Inconclusive: {3}, Time: {4} seconds",
        //        summary.TestsRun, summary.Errors, summary.Failures, summary.Inconclusive, summary.Time);
        //    Console.WriteLine(
        //        "  Not run: {0}, Invalid: {1}, Ignored: {2}, Skipped: {3}",
        //        summary.TestsNotRun, summary.NotRunnable, summary.Ignored, summary.Skipped);
        //    Console.WriteLine();
        //}

        //private void WriteErrorsAndFailuresReport(ITestResult result)
        //{
        //    reportIndex = 0;
        //    Console.WriteLine("Errors and Failures:");
        //    WriteErrorsAndFailures(result);
        //    Console.WriteLine();
        //}

        //private void WriteErrorsAndFailures(ITestResult result)
        //{
        //    if (result.Executed)
        //    {
        //        if (result.HasResults)
        //        {
        //            if (result.IsFailure || result.IsError)
        //                if (result.FailureSite == FailureSite.SetUp || result.FailureSite == FailureSite.TearDown)
        //                    WriteSingleResult(result);

        //            foreach (ITestResult childResult in result.Results)
        //                WriteErrorsAndFailures(childResult);
        //        }
        //        else if (result.IsFailure || result.IsError)
        //        {
        //            WriteSingleResult(result);
        //        }
        //    }
        //}

        //private void WriteNotRunReport(ITestResult result)
        //{
        //    reportIndex = 0;
        //    Console.WriteLine("Tests Not Run:");
        //    WriteNotRunResults(result);
        //    Console.WriteLine();
        //}

        //private int reportIndex = 0;
        //private void WriteNotRunResults(ITestResult result)
        //{
        //    if (result.HasResults)
        //        foreach (ITestResult childResult in result.Results)
        //            WriteNotRunResults(childResult);
        //    else if (!result.Executed)
        //        WriteSingleResult( result );
        //}

        //private void WriteSingleResult( ITestResult result )
        //{
        //    string status = result.IsFailure || result.IsError
        //        ? string.Format("{0} {1}", result.FailureSite, result.ResultState)
        //        : result.ResultState.ToString();

        //    Console.WriteLine("{0}) {1} : {2}", ++reportIndex, status, result.FullName);

        //    if ( result.Message != null && result.Message != string.Empty )
        //         Console.WriteLine("   {0}", result.Message);

        //    if (result.StackTrace != null && result.StackTrace != string.Empty)
        //        Console.WriteLine( result.IsFailure
        //            ? StackTraceFilter.Filter(result.StackTrace)
        //            : result.StackTrace + Environment.NewLine );
        //}
	    #endregion
	}
}

