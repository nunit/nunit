// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using NUnit.Common;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using NUnit.Framework.Internal.Filters;
using System.Globalization;

namespace NUnitLite.Runner
{
    /// <summary>
    /// TextUI is a general purpose class that runs tests and
    /// outputs to a TextWriter.
    ///
    /// Call it from your Main like this:
    ///   new TextUI(textWriter).Execute(args);
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
    public class TextUI : ITestListener
    {
        #region TextUI Return Codes

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

#if NETCF // NETCF: Any harm in using txt everywhere?
          // Some mobiles don't have an Open With menu item
        private const string LOG_FILE_FORMAT = "InternalTrace.{0}.{1}.txt";
#else
        private const string LOG_FILE_FORMAT = "InternalTrace.{0}.{1}.log";
#endif
        private ConsoleOptions _options;
        private string _workDirectory;
        private readonly List<Assembly> _assemblies = new List<Assembly>();
        private ExtendedTextWriter _outWriter;
        private TextWriter _errWriter;
        private ITestAssemblyRunner _runner;
#if !SILVERLIGHT && !NETCF
        private TeamCityEventListener _teamCity;
#endif
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TextUI"/> class.
        /// </summary>
        public TextUI() : this(Console.Out)
        {
            ColorConsole.Enabled = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextUI"/> class.
        /// </summary>
        /// <param name="writer">The TextWriter to use.</param>
        public TextUI(TextWriter writer)
        {
            // Set the default writer - may be overridden by the args specified
            _outWriter = new ExtendedTextWriter(writer);
            _errWriter = new ExtendedTextWriter(writer);
            ColorConsole.Enabled = false;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Execute a test run based on the aruments passed
        /// from Main.
        /// </summary>
        /// <param name="args">An array of arguments</param>
        public int Execute(string[] args)
        {
            // NOTE: Execute must be directly called from the
            // test assembly in order for the mechanism to work.

            _options = new ConsoleOptions(args);

            _workDirectory = _options.WorkDirectory;
            if (_workDirectory == null)
                _workDirectory = NUnit.Env.DefaultWorkDirectory;
            else if (!Directory.Exists(_workDirectory))
                Directory.CreateDirectory(_workDirectory);

#if !SILVERLIGHT
#if !NETCF
            if (_options.TeamCity)
                _teamCity = new TeamCityEventListener();
#endif

            if (_options.OutFile != null)
            {
                _outWriter = new ExtendedTextWriter(new StreamWriter(Path.Combine(_workDirectory, _options.OutFile)));
                Console.SetOut(_outWriter);
                ColorConsole.Enabled = false;
            }

            if (_options.ErrFile != null)
            {
                _errWriter = new StreamWriter(Path.Combine(_workDirectory, _options.ErrFile));
                Console.SetError(_errWriter);
            }
#endif

            if (_options.NoColor)
                ColorConsole.Enabled = false;

            if (!_options.NoHeader)
                WriteHeader(_outWriter);

            if (_options.ShowHelp)
            {
                WriteHelpText();
                return OK;
            }

            if (_options.ErrorMessages.Count > 0)
            {
                foreach (string line in _options.ErrorMessages)
                    _outWriter.WriteLine(line);

                _options.WriteOptionDescriptions(_outWriter);

                return INVALID_ARG;
            }
            Assembly callingAssembly = Assembly.GetCallingAssembly();

            // We must call this before creating the runner so that any internal logging is initialized
            var level = (InternalTraceLevel)Enum.Parse(typeof(InternalTraceLevel), _options.InternalTraceLevel ?? "Off", true);
#if NETCF  // NETCF: Try to unify
            InitializeInternalTrace(callingAssembly.GetName().CodeBase, level);
#else
            InitializeInternalTrace(callingAssembly.Location, level);
#endif

            _runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());

            _outWriter.WriteLine(ColorStyle.SectionHeader, "Test Files:");

            DisplayRequestedOptions(_outWriter);

            WriteRuntimeEnvironment(_outWriter);

            if (_options.WaitBeforeExit && _options.OutFile != null)
                _outWriter.WriteLine("Ignoring /wait option - only valid for Console");

            var runSettings = MakeRunSettings(_options);

            TestFilter filter = CreateTestFilter(_options);

            try
            {
                foreach (string nameOrPath in _options.InputFiles)
                    _assemblies.Add(AssemblyHelper.Load(nameOrPath));

                if (_assemblies.Count == 0)
                    _assemblies.Add(callingAssembly);

                // TODO: For now, ignore all but first assembly
                Assembly assembly = _assemblies[0];

                // Randomizer.InitialSeed = _commandLineOptions.InitialSeed;

                if (_runner.Load(assembly, runSettings) != null)
                    return _options.Explore ? ExploreTests() : RunTests(filter);

                var assemblyName = AssemblyHelper.GetAssemblyName(assembly);
                Console.WriteLine("No tests found in assembly {0}", assemblyName.Name);
                return OK;
            }
            catch (FileNotFoundException ex)
            {
                _outWriter.WriteLine(ColorStyle.Error, ex.Message);
                return FILE_NOT_FOUND;
            }
            catch (Exception ex)
            {
                _outWriter.WriteLine(ColorStyle.Error, ex.ToString());
                return UNEXPECTED_ERROR;
            }
            finally
            {
                if (_options.OutFile == null)
                {
                    if (_options.WaitBeforeExit)
                    {
                        _outWriter.WriteLine(ColorStyle.Label, "Press Enter key to continue . . .");
                        Console.ReadLine();
                    }
                }
                else
                    _outWriter.Close();

                if (_options.ErrFile != null)
                    _errWriter.Close();
            }
        }

        #endregion

        #region Helper Methods

        private int RunTests(ITestFilter filter)
        {
            var startTime = DateTime.UtcNow;

            ITestResult result = _runner.Run(this, filter);
            var reporter = new ResultReporter(result, _outWriter, _options.StopOnError);
            reporter.ReportResults();

            if (_options.ResultOutputSpecifications.Count > 0)
            {
                var outputManager = new OutputManager(_workDirectory);

                foreach (var spec in _options.ResultOutputSpecifications)
                    outputManager.WriteResultFile(result, spec);
            }

            var summary = reporter.Summary;
            return summary.FailureCount + summary.ErrorCount + summary.InvalidCount;
        }

        private int ExploreTests()
        {
            ITest testNode = _runner.LoadedTest;

            var specs = _options.ExploreOutputSpecifications;

            if (specs.Count == 0)
                new TestCaseOutputWriter().WriteTestFile(testNode, Console.Out);
            else
            {
                var outputManager = new OutputManager(_workDirectory);

                foreach (var spec in _options.ExploreOutputSpecifications)
                    outputManager.WriteTestFile(testNode, spec);
            }

            return OK;
        }

        private void InitializeInternalTrace(string assemblyPath, InternalTraceLevel traceLevel)
        {
            if (traceLevel != InternalTraceLevel.Off)
            {
#if !SILVERLIGHT
                var logName = string.Format(LOG_FILE_FORMAT, Process.GetCurrentProcess().Id, Path.GetFileName(assemblyPath));
#else
                var logName = string.Format(LOG_FILE_FORMAT, DateTime.Now.ToString("o"), Path.GetFileName(assemblyPath));
#endif

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

        /// <summary>
        /// Writes the header.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public static void WriteHeader(ExtendedTextWriter writer)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = AssemblyHelper.GetAssemblyName(executingAssembly);
            Version version = assemblyName.Version;
            string copyright = "Copyright (C) 2012, Charlie Poole";
            string build = "";

            object[] attrs = executingAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (attrs.Length > 0)
            {
                var copyrightAttr = (AssemblyCopyrightAttribute)attrs[0];
                copyright = copyrightAttr.Copyright;
            }

            attrs = executingAssembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);
            if (attrs.Length > 0)
            {
                var configAttr = (AssemblyConfigurationAttribute)attrs[0];
                build = string.Format("({0})", configAttr.Configuration);
            }

            writer.WriteLine(ColorStyle.Header, String.Format("NUnitLite {0} {1}", version.ToString(3), build));
            writer.WriteLine(ColorStyle.SubHeader, copyright);
            writer.WriteLine();
        }

        private void WriteHelpText()
        {
            // TODO: The NETCF/Silverlight code is just a placeholder. Figure out how to do it correctly.
#if NETCF || SILVERLIGHT
            const string name = "NUNITLITE";
#else
            string name = Assembly.GetEntryAssembly().GetName().Name.ToUpper();
#endif
            _outWriter.WriteLine(ColorStyle.Header, "Usage: " + name + " [assembly] [options]");
            _outWriter.WriteLine();
            _outWriter.WriteLine(ColorStyle.Help, "Runs a set of NUnitLite tests from the console.");
            _outWriter.WriteLine();

            _outWriter.WriteLine(ColorStyle.SectionHeader, "Assembly:");
            _outWriter.WriteLine(ColorStyle.Help, "      An alternate assembly from which to execute tests. Normally, the tests");
            _outWriter.WriteLine(ColorStyle.Help, "      contained in the executable test assembly itself are run. An alternate");
            _outWriter.WriteLine(ColorStyle.Help, "      assembly is specified using the assembly name, without any path or.");
            _outWriter.WriteLine(ColorStyle.Help, "      extension. It must be in the same in the same directory as the executable");
            _outWriter.WriteLine(ColorStyle.Help, "      or on the probing path.");
            _outWriter.WriteLine();

            _outWriter.WriteLine(ColorStyle.SectionHeader, "Options:");
            using (var sw = new StringWriter())
            {
                _options.WriteOptionDescriptions(sw);
                _outWriter.Write(ColorStyle.Help, sw.ToString());
            }

            _outWriter.WriteLine(ColorStyle.SectionHeader, "Notes:");
            using (new ColorConsole(ColorStyle.Help))
            {
                _outWriter.WriteLine("    * File names may be listed by themselves, with a relative path or ");
                _outWriter.WriteLine("      using an absolute path. Any relative path is based on the current ");
                _outWriter.WriteLine("      directory or on the Documents folder if running on a under the ");
                _outWriter.WriteLine("      compact framework.");
                _outWriter.WriteLine();
                if (System.IO.Path.DirectorySeparatorChar != '/')
                {
                    _outWriter.WriteLine("    * On Windows, options may be prefixed by a '/' character if desired");
                    _outWriter.WriteLine();
                }
                _outWriter.WriteLine("    * Options that take values may use an equal sign or a colon");
                _outWriter.WriteLine("      to separate the option from its value.");
                _outWriter.WriteLine();
                _outWriter.WriteLine("    * Several options that specify processing of XML output take");
                _outWriter.WriteLine("      an output specification as a value. A SPEC may take one of");
                _outWriter.WriteLine("      the following forms:");
                _outWriter.WriteLine("          --OPTION:filename");
                _outWriter.WriteLine("          --OPTION:filename;format=formatname");
                _outWriter.WriteLine("          --OPTION:filename;transform=xsltfile");
                _outWriter.WriteLine();
                _outWriter.WriteLine("      The --result option may use any of the following formats:");
                _outWriter.WriteLine("          nunit3 - the native XML format for NUnit 3.0");
                _outWriter.WriteLine("          nunit2 - legacy XML format used by earlier releases of NUnit");
                _outWriter.WriteLine();
                _outWriter.WriteLine("      The --explore option may use any of the following formats:");
                _outWriter.WriteLine("          nunit3 - the native XML format for NUnit 3.0");
                _outWriter.WriteLine("          cases  - a text file listing the full names of all test cases.");
                _outWriter.WriteLine("      If --explore is used without any specification following, a list of");
                _outWriter.WriteLine("      test cases is output to the console.");
                _outWriter.WriteLine();
            }
        }

        private void DisplayRequestedOptions(ExtendedTextWriter writer)
        {
            writer.WriteLine(ColorStyle.SectionHeader, "Options -");

            if (_options.DefaultTimeout >= 0)
                writer.WriteLabelLine("    Default timeout: ", _options.DefaultTimeout);

            writer.WriteLabelLine("    Work Directory: ", _workDirectory);

            writer.WriteLabelLine("    Internal Trace: ", _options.InternalTraceLevel ?? "Off");

            if (_options.TeamCity)
                writer.WriteLine(ColorStyle.Value, "    Display TeamCity Service Messages");

            writer.WriteLine();

            if (_options.TestList.Count > 0)
            {
                writer.WriteLine(ColorStyle.SectionHeader, "Selected test(s) -");
                foreach (string testName in _options.TestList)
                    writer.WriteLine(ColorStyle.Value, "    " + testName);
                writer.WriteLine();
            }

            if (!string.IsNullOrEmpty(_options.Include))
            {
                writer.WriteLabelLine("Included categories: ", _options.Include);
                writer.WriteLine();
            }

            if (!string.IsNullOrEmpty(_options.Exclude))
            {
                writer.WriteLabelLine("Excluded categories: ", _options.Exclude);
                writer.WriteLine();
            }
        }

        /// <summary>
        /// Writes the runtime environment.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public static void WriteRuntimeEnvironment(ExtendedTextWriter writer)
        {
            writer.WriteLine(ColorStyle.SectionHeader, "Runtime Environment -");
            writer.WriteLabelLine("   OS Version: ", Environment.OSVersion);
            writer.WriteLabelLine("  CLR Version: ", Environment.Version);
            writer.WriteLine();
        }

        /// <summary>
        /// Make the settings for this run - this is public for testing
        /// </summary>
        public static Dictionary<string, object> MakeRunSettings(ConsoleOptions options)
        {
            // Transfer command line options to run settings
            var runSettings = new Dictionary<string, object>();

            if (options.RandomSeed >= 0)
                runSettings[PackageSettings.RandomSeed] = options.RandomSeed;

            if (options.WorkDirectory != null)
                runSettings[PackageSettings.WorkDirectory] = Path.GetFullPath(options.WorkDirectory);

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
        public static TestFilter CreateTestFilter(ConsoleOptions options)
        {
            TestFilter namefilter = options.TestList.Count > 0
                ? new SimpleNameFilter(options.TestList)
                : TestFilter.Empty;

            TestFilter includeFilter = string.IsNullOrEmpty(options.Include)
                ? TestFilter.Empty
                : new SimpleCategoryExpression(options.Include).Filter;

            TestFilter excludeFilter = string.IsNullOrEmpty(options.Exclude)
                ? TestFilter.Empty
                : new NotFilter(new SimpleCategoryExpression(options.Exclude).Filter);

            TestFilter catFilter = includeFilter.IsEmpty
                ? excludeFilter
                : excludeFilter.IsEmpty
                    ? includeFilter
                    : new AndFilter(includeFilter, excludeFilter);

            return namefilter.IsEmpty
                ? catFilter
                : catFilter.IsEmpty
                    ? namefilter
                    : new AndFilter(namefilter, catFilter);
        }

        #endregion

        #region ITestListener Members

        /// <summary>
        /// Called when a test or suite has just started
        /// </summary>
        /// <param name="test">The test that is starting</param>
        public void TestStarted(ITest test)
        {
#if !SILVERLIGHT && !NETCF
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
#if !SILVERLIGHT && !NETCF
            if (_teamCity != null)
                _teamCity.TestFinished(result);
#endif

            bool isSuite = result.Test.IsSuite;
            var labels = _options.DisplayTestLabels != null
                ? _options.DisplayTestLabels.ToUpper(CultureInfo.InvariantCulture)
                : "ON";

            if (!isSuite && labels == "ALL")
                WriteTestLabel(result);

            if (result.Output.Length > 0)
            {
                if (!isSuite && labels == "ON")
                    WriteTestLabel(result);
                _outWriter.Write(ColorStyle.Output, result.Output);
                if (!result.Output.EndsWith("\n"))
                    _outWriter.WriteLine();
            }
        }

        private void WriteTestLabel(ITestResult result)
        {
            _outWriter.WriteLine(ColorStyle.SectionHeader, "=> " + result.Test.Name);
        }

        #endregion
    }
}
