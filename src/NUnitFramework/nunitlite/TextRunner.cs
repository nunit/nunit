// ***********************************************************************
// Copyright (c) 2015 Charlie Poole, Rob Prouse
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NUnit.Common;
using NUnit;
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
        /// <summary>Test fixture not found - No longer in use</summary>
        //public const int FIXTURE_NOT_FOUND = -3;
        /// <summary>Invalid test suite</summary>
        public const int INVALID_TEST_FIXTURE = -4;
        /// <summary>Unexpected error occurred</summary>
        public const int UNEXPECTED_ERROR = -100;

        #endregion

        private Assembly _testAssembly;
        private ITestAssemblyRunner _runner;

        private NUnitLiteOptions _options;
        private ITestListener _teamCity = null;

        private TextUI _textUI;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TextRunner"/> class
        /// without specifying an assembly. This is interpreted as allowing
        /// a single input file in the argument list to Execute().
        /// </summary>
        public TextRunner() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextRunner"/> class
        /// specifying a test assembly whose tests are to be run.
        /// </summary>
        /// <param name="testAssembly"></param>
        /// </summary>
        public TextRunner(Assembly testAssembly)
        {
            _testAssembly = testAssembly;
        }

        #endregion

        #region Properties

        public ResultSummary Summary { get; private set; }

        #endregion

        #region Public Methods

        public int Execute(string[] args)
        {
            _options = new NUnitLiteOptions(_testAssembly == null, args);

            ExtendedTextWriter outWriter = null;
            if (_options.OutFile != null)
            {
                var outFile = Path.Combine(_options.WorkDirectory, _options.OutFile);
#if NETSTANDARD1_6
                var textWriter = File.CreateText(outFile);
#else
                var textWriter = TextWriter.Synchronized(new StreamWriter(outFile));
#endif
                outWriter = new ExtendedTextWrapper(textWriter);
                Console.SetOut(outWriter);
            }
            else
            {
                outWriter = new ColorConsoleWriter(!_options.NoColor);
            }

            using (outWriter)
            {
                TextWriter errWriter = null;
                if (_options.ErrFile != null)
                {
                    var errFile = Path.Combine(_options.WorkDirectory, _options.ErrFile);
#if NETSTANDARD1_6
                    errWriter = File.CreateText(errFile);
#else
                    errWriter = TextWriter.Synchronized(new StreamWriter(errFile));
#endif
                    Console.SetError(errWriter);
                }

                using (errWriter)
                {
                    _textUI = new TextUI(outWriter, Console.In, _options);
                    return Execute();
                }
            }
        }

        // Entry point called by AutoRun and by the .NET Standard nunitlite.runner
        public int Execute(ExtendedTextWriter writer, TextReader reader, string[] args)
        {
            _options = new NUnitLiteOptions(_testAssembly == null, args);

            _textUI = new TextUI(writer, reader, _options);

            return Execute();
        }

        // Internal Execute depends on _textUI and _options having been set already.
        private int Execute()
        {
            _runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());

            InitializeInternalTrace();

            try
            {
                if (!Directory.Exists(_options.WorkDirectory))
                    Directory.CreateDirectory(_options.WorkDirectory);

                if (_options.TeamCity)
                    _teamCity = new TeamCityEventListener(_textUI.Writer);

                if (_options.ShowVersion || !_options.NoHeader)
                    _textUI.DisplayHeader();

                if (_options.ShowHelp)
                {
                    _textUI.DisplayHelp();
                    return TextRunner.OK;
                }

                // We already showed version as a part of the header
                if (_options.ShowVersion)
                    return TextRunner.OK;

                if (_options.ErrorMessages.Count > 0)
                {
                    _textUI.DisplayErrors(_options.ErrorMessages);
                    _textUI.Writer.WriteLine();
                    _textUI.DisplayHelp();

                    return TextRunner.INVALID_ARG;
                }

                if (_testAssembly == null && _options.InputFile == null)
                {
                    _textUI.DisplayError("No test assembly was specified.");
                    _textUI.Writer.WriteLine();
                    _textUI.DisplayHelp();

                    return TextRunner.OK;
                }

                _textUI.DisplayRuntimeEnvironment();

                var testFile = _testAssembly != null
                    ? AssemblyHelper.GetAssemblyPath(_testAssembly)
                    : _options.InputFile;

                _textUI.DisplayTestFiles(new string[] { testFile });
                if (_testAssembly == null)
                    _testAssembly = AssemblyHelper.Load(testFile);

                if (_options.WaitBeforeExit && _options.OutFile != null)
                    _textUI.DisplayWarning("Ignoring /wait option - only valid for Console");

                var runSettings = MakeRunSettings(_options);

                // We display the filters at this point so that any exception message
                // thrown by CreateTestFilter will be understandable.
                _textUI.DisplayTestFilters();

                TestFilter filter = CreateTestFilter(_options);

                _runner.Load(_testAssembly, runSettings);
                return _options.Explore ? ExploreTests(filter) : RunTests(filter, runSettings);
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
            finally
            {
                if (_options.WaitBeforeExit)
                    _textUI.WaitForUser("Press Enter key to continue . . .");
            }
        }

        #endregion

        #region Helper Methods

        public int RunTests(TestFilter filter, IDictionary<string, object> runSettings)
        {
            var startTime = DateTime.UtcNow;

            ITestResult result = _runner.Run(this, filter);

            ReportResults(result);

            if (_options.ResultOutputSpecifications.Count > 0)
            {
                var outputManager = new OutputManager(_options.WorkDirectory);

                foreach (var spec in _options.ResultOutputSpecifications)
                    outputManager.WriteResultFile(result, spec, runSettings, filter);
            }
            if (Summary.InvalidTestFixtures > 0)
                return INVALID_TEST_FIXTURE;

            return Summary.FailureCount + Summary.ErrorCount + Summary.InvalidCount;
        }

        public void ReportResults(ITestResult result)
        {
            Summary = new ResultSummary(result);

            if (Summary.ExplicitCount + Summary.SkipCount + Summary.IgnoreCount > 0)
                _textUI.DisplayNotRunReport(result);

            if (result.ResultState.Status == TestStatus.Failed || result.ResultState.Status == TestStatus.Warning)
                _textUI.DisplayErrorsFailuresAndWarningsReport(result);

#if FULL
            if (_options.Full)
                _textUI.PrintFullReport(_result);
#endif

            _textUI.DisplayRunSettings();

            _textUI.DisplaySummaryReport(Summary);
        }

        private int ExploreTests(ITestFilter filter)
        {
            ITest testNode = _runner.ExploreTests(filter);

            var specs = _options.ExploreOutputSpecifications;

            if (specs.Count == 0)
                new TestCaseOutputWriter().WriteTestFile(testNode, Console.Out);
            else
            {
                var outputManager = new OutputManager(_options.WorkDirectory);

                foreach (var spec in _options.ExploreOutputSpecifications)
                    outputManager.WriteTestFile(testNode, spec);
            }

            return OK;
        }

        /// <summary>
        /// Make the settings for this run - this is public for testing
        /// </summary>
        public static Dictionary<string, object> MakeRunSettings(NUnitLiteOptions options)
        {
            // Transfer command line options to run settings
            var runSettings = new Dictionary<string, object>();

            if (options.NumberOfTestWorkers >= 0)
                runSettings[FrameworkPackageSettings.NumberOfTestWorkers] = options.NumberOfTestWorkers;

            if (options.InternalTraceLevel != null)
                runSettings[FrameworkPackageSettings.InternalTraceLevel] = options.InternalTraceLevel;

            if (options.RandomSeed >= 0)
                runSettings[FrameworkPackageSettings.RandomSeed] = options.RandomSeed;

            if (options.WorkDirectory != null)
                runSettings[FrameworkPackageSettings.WorkDirectory] = Path.GetFullPath(options.WorkDirectory);

            if (options.DefaultTimeout >= 0)
                runSettings[FrameworkPackageSettings.DefaultTimeout] = options.DefaultTimeout;

            if (options.StopOnError)
                runSettings[FrameworkPackageSettings.StopOnError] = true;

            if (options.DefaultTestNamePattern != null)
                runSettings[FrameworkPackageSettings.DefaultTestNamePattern] = options.DefaultTestNamePattern;

            if (options.TestParameters.Count != 0)
                runSettings[FrameworkPackageSettings.TestParametersDictionary] = options.TestParameters;

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

        private void InitializeInternalTrace()
        {
            var traceLevel = (InternalTraceLevel)Enum.Parse(typeof(InternalTraceLevel), _options.InternalTraceLevel ?? "Off", true);

            if (traceLevel != InternalTraceLevel.Off)
            {
                var logName = GetLogFileName();

                StreamWriter streamWriter = null;
                if (traceLevel > InternalTraceLevel.Off)
                {
                    string logPath = Path.Combine(Directory.GetCurrentDirectory(), logName);
                    streamWriter = new StreamWriter(new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.Write));
                    streamWriter.AutoFlush = true;
                }
                InternalTrace.Initialize(streamWriter, traceLevel);
            }
        }

        private string GetLogFileName()
        {
            const string LOG_FILE_FORMAT = "InternalTrace.{0}.{1}.{2}";

            // Some mobiles don't have an Open With menu item,
            // so we use .txt, which is opened easily.
            const string ext = "log";
            var baseName = _testAssembly != null
                ? _testAssembly.GetName().Name
                : _options.InputFile != null
                    ? Path.GetFileNameWithoutExtension(_options.InputFile)
                    : "NUnitLite";

#if NETSTANDARD1_6
            var id = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");
#else
            var id = Process.GetCurrentProcess().Id;
#endif

            return string.Format(LOG_FILE_FORMAT, id, baseName, ext);
        }

        #endregion

        #region ITestListener Members

        /// <summary>
        /// Called when a test or suite has just started
        /// </summary>
        /// <param name="test">The test that is starting</param>
        public void TestStarted(ITest test)
        {
            if (_teamCity != null)
                _teamCity.TestStarted(test);

            _textUI.TestStarted(test);
        }

        /// <summary>
        /// Called when a test has finished
        /// </summary>
        /// <param name="result">The result of the test</param>
        public void TestFinished(ITestResult result)
        {
            if (_teamCity != null)
                _teamCity.TestFinished(result);

            _textUI.TestFinished(result);
        }

        /// <summary>
        /// Called when a test produces output for immediate display
        /// </summary>
        /// <param name="output">A TestOutput object containing the text to display</param>
        public void TestOutput(TestOutput output)
        {
            _textUI.TestOutput(output);
        }

        #endregion
    }
}
