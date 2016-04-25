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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
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

        private Assembly _testAssembly;
        private readonly List<Assembly> _assemblies = new List<Assembly>();
        private ITestAssemblyRunner _runner;

        private NUnitLiteOptions _options;
        private ITestListener _teamCity = null;

        private TextUI _textUI;

#if SILVERLIGHT
        private List<ITestResult> _results = new List<ITestResult>();
#endif

        #region Constructors

        //// <summary>
        //// Initializes a new instance of the <see cref="TextRunner"/> class.
        //// </summary>
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

#if !SILVERLIGHT && !PORTABLE
        public int Execute(string[] args)
        {
            var options = new NUnitLiteOptions(args);

            InitializeInternalTrace(options);

            ExtendedTextWriter outWriter = null;
            if (options.OutFile != null)
            {
                outWriter = new ExtendedTextWrapper(TextWriter.Synchronized(new StreamWriter(Path.Combine(options.WorkDirectory, options.OutFile))));
                Console.SetOut(outWriter);
            }
            else
            {
                outWriter = new ColorConsoleWriter();
            }

            TextWriter errWriter = null;
            if (options.ErrFile != null)
            {
                errWriter = TextWriter.Synchronized(new StreamWriter(Path.Combine(options.WorkDirectory, options.ErrFile)));
                Console.SetError(errWriter);
            }

            try
            {
                return Execute(outWriter, Console.In, options);
            }
            finally
            {
                if (options.OutFile != null && outWriter != null)
                    outWriter.Close();

                if (options.ErrFile != null && errWriter != null)
                    errWriter.Close();
            }
        }
#endif

        public int Execute(ExtendedTextWriter writer, TextReader reader, string[] args)
        {
            var options = new NUnitLiteOptions(args);
            var textUI = new TextUI(writer, reader, options);
            return Execute(textUI, options);
        }

        public int Execute(ExtendedTextWriter writer, TextReader reader, NUnitLiteOptions options)
        {
            var textUI = new TextUI(writer, reader, options);
            return Execute(textUI, options);
        }

        /// <summary>
        /// Execute a test run
        /// </summary>
        /// <param name="callingAssembly">The assembly from which tests are loaded</param>
        public int Execute(TextUI textUI, NUnitLiteOptions options)
        {
            _textUI = textUI;
            _options = options;
            _runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());

            try
            {
#if !SILVERLIGHT && !PORTABLE
                if (!Directory.Exists(_options.WorkDirectory))
                    Directory.CreateDirectory(_options.WorkDirectory);

#if !NETCF
                if (_options.TeamCity)
                    _teamCity = new TeamCityEventListener();
#endif
#endif

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
                    _textUI.DisplayHelp();

                    return TextRunner.INVALID_ARG;
                }

                _textUI.DisplayRuntimeEnvironment();

                var testFile = _testAssembly != null
                    ? AssemblyHelper.GetAssemblyPath(_testAssembly)
                    : _options.InputFiles.Count > 0
                        ? _options.InputFiles[0]
                        : null;

                if (testFile != null)
                {
                    _textUI.DisplayTestFiles(new string[] { testFile });
                    if (_testAssembly == null)
                        _testAssembly = AssemblyHelper.Load(testFile);
                }


                if (_options.WaitBeforeExit && _options.OutFile != null)
                    _textUI.DisplayWarning("Ignoring /wait option - only valid for Console");

                foreach (string nameOrPath in _options.InputFiles)
                    _assemblies.Add(AssemblyHelper.Load(nameOrPath));

                var runSettings = MakeRunSettings(_options);

                // We display the filters at this point so  that any exception message
                // thrown by CreateTestFilter will be understandable.
                _textUI.DisplayTestFilters();

                TestFilter filter = CreateTestFilter(_options);

                _runner.Load(_testAssembly, runSettings);
                return _options.Explore ? ExploreTests() : RunTests(filter, runSettings);
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
#if !SILVERLIGHT
            finally
            {
                if (_options.WaitBeforeExit)
                    _textUI.WaitForUser("Press Enter key to continue . . .");
            }
#endif
        }

#endregion

        #region Helper Methods

        public int RunTests(TestFilter filter, IDictionary runSettings)
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

            _textUI.DisplayRunSettings();

            _textUI.DisplaySummaryReport(Summary);
        }

        private int ExploreTests()
        {
#if !PORTABLE && !SILVERLIGHT
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

#if !PORTABLE && !SILVERLIGHT
        private void InitializeInternalTrace(NUnitLiteOptions _options)
        {
            var traceLevel = (InternalTraceLevel)Enum.Parse(typeof(InternalTraceLevel), _options.InternalTraceLevel ?? "Off", true);

            if (traceLevel != InternalTraceLevel.Off)
            {
                var logName = GetLogFileName();

#if NETCF // NETCF: Try to encapsulate this
                InternalTrace.Initialize(Path.Combine(NUnit.Env.DocumentFolder, logName), traceLevel);
#else
                StreamWriter streamWriter = null;
                if (traceLevel > InternalTraceLevel.Off)
                {
                    string logPath = Path.Combine(Environment.CurrentDirectory, logName);
                    streamWriter = new StreamWriter(new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.Write));
                    streamWriter.AutoFlush = true;
                }
                InternalTrace.Initialize(streamWriter, traceLevel);
#endif
            }
        }

        private string GetLogFileName()
        {
            const string LOG_FILE_FORMAT = "InternalTrace.{0}.{1}.{2}";

            // Some mobiles don't have an Open With menu item,
            // so we use .txt, which is opened easily.
#if NETCF
            const string ext = "txt";
#else
            const string ext = "log";
#endif
            var baseName = _testAssembly != null
                ? _testAssembly.GetName().Name
                : _options.InputFiles.Count > 0
                    ? Path.GetFileNameWithoutExtension(_options.InputFiles[0])
                    : "NUnitLite";

            return string.Format(LOG_FILE_FORMAT, Process.GetCurrentProcess().Id, baseName, ext);
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
            if (_teamCity != null)
                _teamCity.TestStarted(test);
        }

        /// <summary>
        /// Called when a test has finished
        /// </summary>
        /// <param name="result">The result of the test</param>
        public void TestFinished(ITestResult result)
        {
            if (_teamCity != null)
                _teamCity.TestFinished(result);

#if !SILVERLIGHT
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
