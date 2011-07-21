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

        private ConsoleOptions options;

        TextWriter outWriter = Console.Out;
        TextWriter errorWriter = Console.Error;

        private string workDirectory;

        #endregion

        #region Constructor

        public ConsoleRunner(ConsoleOptions options)
        {
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
            // TODO: We really need options as resolved by engine for most of  these
            DisplayRequestedOptions();

            // Create the test package
            TestPackage package = MakeTestPackage(options);

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
                ITestEngine engine = TestEngineActivator.CreateInstance();

                
#if true
                engineResult = engine.Run(package, eventHandler, testFilter );
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

                if (!options.noxml)
                {
                    var xmlManager = new XmlOutputManager(engineResult.Xml, options.WorkDirectory);

                    foreach (var outputSpec in options.XmlOutputSpecifications)
                        xmlManager.WriteXmlOutput(outputSpec);
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

        private void DisplayRequestedOptions()
        {
            Console.WriteLine("ProcessModel: {0}    DomainUsage: {1}", options.processModel, options.domainUsage);
            Console.WriteLine("Execution Runtime: {0}", options.framework == null ? "Not Specified" : options.framework);
            Console.WriteLine();

            if (options.RunList.Length > 0)
            {
                Console.WriteLine("Selected test(s):");
                foreach (string testName in options.RunList)
                    Console.WriteLine("    " + testName);
            }

            if (options.include != null && options.include != string.Empty)
                Console.WriteLine("Included categories: " + options.include);

            if (options.exclude != null && options.exclude != string.Empty)
                Console.WriteLine("Excluded categories: " + options.exclude);
        }

        private void RedirectOutputAsRequested()
        {
            if (options.outputPath != null)
            {
                StreamWriter outStreamWriter = new StreamWriter(Path.Combine(workDirectory, options.outputPath));
                outStreamWriter.AutoFlush = true;
                this.outWriter = outStreamWriter;
            }

            if (options.errorPath != null)
            {
                StreamWriter errorStreamWriter = new StreamWriter(Path.Combine(workDirectory, options.errorPath));
                errorStreamWriter.AutoFlush = true;
                this.errorWriter = errorStreamWriter;
            }
        }

        private void RestoreOutput()
        {
            outWriter.Flush();
            if (options.outputPath != null)
                outWriter.Close();

            errorWriter.Flush();
            if (options.errorPath != null)
                errorWriter.Close();
        }

        // This is public static for ease of testing
        public static TestPackage MakeTestPackage( ConsoleOptions options )
        {
            TestPackage package = options.InputFiles.Length == 1
                ? new TestPackage(options.InputFiles[0])
                : new TestPackage(options.InputFiles);

            if (options.processModel != ProcessModel.Default)
                package.Settings["ProcessModel"] = options.processModel;

            if (options.domainUsage != DomainUsage.Default)
                package.Settings["DomainUsage"] = options.domainUsage;

            if (options.framework != null)
                package.Settings["RuntimeFramework"] = options.framework;

            if (options.defaultTimeout >= 0)
                package.Settings["DefaultTimeout"] = options.defaultTimeout;

            if (options.internalTraceLevel != InternalTraceLevel.Default)
                package.Settings["InternalTraceLevel"] = options.internalTraceLevel;

            if (options.activeConfig != null)
                package.Settings["ActiveConfig"] = options.activeConfig;
            
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

