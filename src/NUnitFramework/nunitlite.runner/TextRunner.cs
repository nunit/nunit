// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Common;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Filters;

namespace NUnitLite
{
    /// <summary>
    /// TextRunner is a general purpose class that runs tests and
    /// outputs to a text-based user interface (TextUI).
    ///
    /// Call it from your Main like this:
    ///   new TextRunner(textWriter).Execute(args);
    ///     OR
    ///   new TextUI().Execute(args);
    /// The provided TextWriter is used by default, unless the
    /// arguments to Execute override it using -out. The second
    /// form uses the Console, provided it exists on the platform.
    ///
    /// NOTE: When running on a platform without a Console, such
    /// as Windows Phone, the results will simply not appear if
    /// you fail to specify a file in the call itself or as an option.
    /// </summary>
    public class TextRunner : ITestListener
    {
        #region Runner Return Codes

        /// <summary>OK</summary>
        public const int OK = 0;
        /// <summary>Invalid Arguments</summary>
        public const int INVALID_ARG = -1;
        /// <summary>File not found</summary>
        public const int FILE_NOT_FOUND = -2;
        /// <summary>Test fixture not found</summary>
        public const int FIXTURE_NOT_FOUND = -3;
        /// <summary>Unexpected error occurred</summary>
        public const int UNEXPECTED_ERROR = -100;

        #endregion

        private readonly List<Assembly> _assemblies = new List<Assembly>();
        private ITestAssemblyRunner _runner;

#if !SILVERLIGHT
        private NUnitLiteOptions _options;
#if !NETCF && !PORTABLE
        private TeamCityEventListener _teamCity;
#endif
#endif

        private TextUI _textUI;

#if SILVERLIGHT
        private List<ITestResult> _results = new List<ITestResult>();
#endif

        #region Constructors

        //// <summary>
        //// Initializes a new instance of the <see cref="TextRunner"/> class.
        //// </summary>
        //// <param name="writer">The TextWriter to use.</param>
        //public TextRunner(ConsoleOptions options) : this(null, options) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextRunner"/> class.
        /// </summary>
        /// <param name="textUI">The text-based user interface to output results of the run</param>
#if SILVERLIGHT
        public TextRunner(TextUI textUI)
        {
            _textUI = textUI;
            //_workDirectory = NUnit.Env.DefaultWorkDirectory;
        }
#else
        /// <param name="options">The options to use when running the test</param>
        public TextRunner(TextUI textUI, NUnitLiteOptions options)
        {
            _textUI = textUI;
            _options = options;
            
#if !PORTABLE
            if (!Directory.Exists(_options.WorkDirectory))
                Directory.CreateDirectory(_options.WorkDirectory);

#if !NETCF
            if (_options.TeamCity)
                _teamCity = new TeamCityEventListener();
#endif
#endif
        }
#endif

        #endregion

        #region Properties

        public ResultSummary Summary { get; private set; }

        #endregion

        #region Public Methods

        public int Execute()
        {
            return Execute(null);
        }

        /// <summary>
        /// Execute a test run
        /// </summary>
        /// <param name="callingAssembly">The assembly from which tests are loaded</param>
        public int Execute(Assembly callingAssembly)
        {
            _runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());

            try
            {
#if !SILVERLIGHT
                foreach (string nameOrPath in _options.InputFiles)
                    _assemblies.Add(AssemblyHelper.Load(nameOrPath));

                if (_assemblies.Count == 0)
                    _assemblies.Add(callingAssembly);

                // TODO: For now, ignore all but first assembly
                Assembly assembly = _assemblies[0];

                var runSettings = MakeRunSettings(_options);

                // We display the filters at this point so  that any exception message
                // thrown by CreateTestFilter will be understandable.
                _textUI.DisplayTestFilters();

                TestFilter filter = CreateTestFilter(_options);

                if (_runner.Load(assembly, runSettings) != null)
                    return _options.Explore ? ExploreTests() : RunTests(filter, runSettings);
#else
                Assembly assembly = callingAssembly;
                if (_runner.Load(assembly, new Dictionary<string, object>()) != null)
                    return RunTests(TestFilter.Empty, null);
#endif

                var assemblyName = AssemblyHelper.GetAssemblyName(assembly);
                _textUI.DisplayError(string.Format("No tests found in assembly {0}", assemblyName.Name));
                return OK;
            }
            catch (FileNotFoundException ex)
            {
                _textUI.DisplayError(ex.Message);
                return FILE_NOT_FOUND;
            }
            catch (Exception ex)
            {
                _textUI.DisplayError(ex.ToString());
                return UNEXPECTED_ERROR;
            }
        }

        #endregion

        #region Helper Methods

        private int RunTests(TestFilter filter, IDictionary runSettings)
        {
            var startTime = DateTime.UtcNow;

            ITestResult result = _runner.Run(this, filter);

#if SILVERLIGHT
            // Silverlight can't display results while the test is running
            // so we do it afterwards.
            foreach(ITestResult testResult in _results)
                _textUI.TestFinished(testResult);
#endif
            ReportResults(result);

#if !SILVERLIGHT && !PORTABLE
            if (_options.ResultOutputSpecifications.Count > 0)
            {
                var outputManager = new OutputManager(_options.WorkDirectory);

                foreach (var spec in _options.ResultOutputSpecifications)
                    outputManager.WriteResultFile(result, spec, runSettings, filter);
            }
#endif

            return Summary.FailureCount + Summary.ErrorCount + Summary.InvalidCount;
        }

        public void ReportResults(ITestResult result)
        {
            Summary = new ResultSummary(result);

            if (Summary.ExplicitCount + Summary.SkipCount + Summary.IgnoreCount > 0)
                _textUI.DisplayNotRunReport(result);

            if (result.ResultState.Status == TestStatus.Failed)
                _textUI.DisplayErrorsAndFailuresReport(result);

#if FULL
            if (_options.Full)
                _textUI.PrintFullReport(_result);
#endif

#if !SILVERLIGHT
            _textUI.DisplayRunSettings();
#endif

            _textUI.DisplaySummaryReport(Summary);
        }

#if !SILVERLIGHT
        private int ExploreTests()
        {
#if !PORTABLE
            ITest testNode = _runner.LoadedTest;

            var specs = _options.ExploreOutputSpecifications;

            if (specs.Count == 0)
                new TestCaseOutputWriter().WriteTestFile(testNode, Console.Out);
            else
            {
                var outputManager = new OutputManager(_options.WorkDirectory);

                foreach (var spec in _options.ExploreOutputSpecifications)
                    outputManager.WriteTestFile(testNode, spec);
            }
#endif

            return OK;
        }

        /// <summary>
        /// Make the settings for this run - this is public for testing
        /// </summary>
        public static Dictionary<string, object> MakeRunSettings(NUnitLiteOptions options)
        {
            // Transfer command line options to run settings
            var runSettings = new Dictionary<string, object>();

            if (options.RandomSeed >= 0)
                runSettings[PackageSettings.RandomSeed] = options.RandomSeed;

#if !PORTABLE
            if (options.WorkDirectory != null)
                runSettings[PackageSettings.WorkDirectory] = Path.GetFullPath(options.WorkDirectory);
#endif
            if (options.DefaultTimeout >= 0)
                runSettings[PackageSettings.DefaultTimeout] = options.DefaultTimeout;

            if (options.StopOnError)
                runSettings[PackageSettings.StopOnError] = true;

            return runSettings;
        }

        /// <summary>
        /// Create the test filter for this run - public for testing
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static TestFilter CreateTestFilter(NUnitLiteOptions options)
        {
            var filter = TestFilter.Empty;

            if (options.TestList.Count > 0)
            {
                var testFilters = new List<TestFilter>();
                foreach (var test in options.TestList)
                    testFilters.Add(new FullNameFilter(test));

                filter = testFilters.Count > 1
                    ? new OrFilter(testFilters.ToArray())
                    : testFilters[0];
            }
                

            if (options.WhereClauseSpecified)
            {
                string xmlText = new TestSelectionParser().Parse(options.WhereClause);
                var whereFilter = TestFilter.FromXml(TNode.FromXml(xmlText));
                filter = filter.IsEmpty
                    ? whereFilter
                    : new AndFilter(filter, whereFilter);
            }

            return filter;
        }
#endif

#endregion

        #region ITestListener Members

        /// <summary>
        /// Called when a test or suite has just started
        /// </summary>
        /// <param name="test">The test that is starting</param>
        public void TestStarted(ITest test)
        {
#if !SILVERLIGHT && !NETCF && !PORTABLE
            if (_teamCity != null)
                _teamCity.TestStarted(test);
#endif
        }

        /// <summary>
        /// Called when a test has finished
        /// </summary>
        /// <param name="result">The result of the test</param>
        public void TestFinished(ITestResult result)
        {
#if !SILVERLIGHT
#if !NETCF && !PORTABLE
            if (_teamCity != null)
                _teamCity.TestFinished(result);
#endif

            _textUI.TestFinished(result);
#else
            // For Silverlight, we can't display the results
            // until the run is completed. We don't save anything
            // unless there is associated output, since that's 
            // the only time we display anything in Silverlight.
            if (result.Output.Length > 0)
                _results.Add(result);
#endif
        }

        #endregion
    }
}
