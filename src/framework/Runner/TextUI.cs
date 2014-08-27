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
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using NUnit.Framework.Internal.Filters;

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
        private const string LOG_FILE_FORMAT = "InternalTrace.{0}.{1}.log";
        private CommandLineOptions _commandLineOptions;
        private string _workDirectory;
        private readonly List<Assembly> _assemblies = new List<Assembly>();
        private TextWriter _outWriter;
        private TextWriter _errWriter;
        private ITestAssemblyRunner _runner;
#if !SILVERLIGHT
        private TeamCityEventListener _teamCity;
#endif

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TextUI"/> class.
        /// </summary>
        public TextUI() : this(Console.Out) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextUI"/> class.
        /// </summary>
        /// <param name="writer">The TextWriter to use.</param>
        public TextUI(TextWriter writer)
        {
            // Set the default writer - may be overridden by the args specified
            _outWriter = writer;
            _errWriter = writer;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Execute a test run based on the aruments passed
        /// from Main.
        /// </summary>
        /// <param name="args">An array of arguments</param>
        public void Execute(string[] args)
        {
            // NOTE: Execute must be directly called from the
            // test assembly in order for the mechanism to work.

            _commandLineOptions = new CommandLineOptions(args);

            _workDirectory = _commandLineOptions.WorkDirectory;
            if (_workDirectory == null)
                _workDirectory = Environment.CurrentDirectory;
            else if (!Directory.Exists(_workDirectory))
                Directory.CreateDirectory(_workDirectory);

#if !SILVERLIGHT
            if (_commandLineOptions.DisplayTeamCityServiceMessages)
                _teamCity = new TeamCityEventListener();

            if (_commandLineOptions.OutFile != null)
            {
                _outWriter = new StreamWriter(Path.Combine(_workDirectory, _commandLineOptions.OutFile));
                Console.SetOut(_outWriter);
            }

            if (_commandLineOptions.ErrFile != null)
            {
                _errWriter = new StreamWriter(Path.Combine(_workDirectory, _commandLineOptions.ErrFile));
                Console.SetError(_errWriter);
            }
#endif

            if (!_commandLineOptions.NoHeader)
                WriteHeader(_outWriter);

            if (_commandLineOptions.ShowHelp)
                _outWriter.Write(_commandLineOptions.HelpText);
            else if (_commandLineOptions.ErrorMessages.Count > 0)
            {
                foreach(string line in _commandLineOptions.ErrorMessages)
                    _outWriter.WriteLine(line);

                _outWriter.WriteLine(_commandLineOptions.HelpText);
            }
            else
            {
                Assembly callingAssembly = Assembly.GetCallingAssembly();

                // We must call this before creating the runner so that any internal logging is initialized
                InternalTraceLevel level = (InternalTraceLevel)Enum.Parse(typeof(InternalTraceLevel), _commandLineOptions.InternalTraceLevel, true);
                InitializeInternalTrace(callingAssembly.Location, level);

                _runner = new NUnitLiteTestAssemblyRunner(new DefaultTestAssemblyBuilder());

                WriteRuntimeEnvironment(_outWriter);

                if (_commandLineOptions.Wait && _commandLineOptions.OutFile != null)
                    _outWriter.WriteLine("Ignoring /wait option - only valid for Console");

                var runSettings = MakeRunSettings(_commandLineOptions);
                
                TestFilter filter = _commandLineOptions.Tests.Count > 0
                    ? new SimpleNameFilter(_commandLineOptions.Tests)
                    : TestFilter.Empty;

                try
                {
                    foreach (string name in _commandLineOptions.InputFiles)
                        _assemblies.Add(Assembly.Load(name));

                    if (_assemblies.Count == 0)
                        _assemblies.Add(callingAssembly);

                    // TODO: For now, ignore all but first assembly
                    Assembly assembly = _assemblies[0];

                    //Randomizer.InitialSeed = _commandLineOptions.InitialSeed;

                    if (_runner.Load(assembly, runSettings) == null)
                    {
                        var assemblyName = AssemblyHelper.GetAssemblyName(assembly);
                        Console.WriteLine("No tests found in assembly {0}", assemblyName.Name);
                        return;
                    }

                    if (_commandLineOptions.Explore)
                        ExploreTests();
                    else
                    {
                        if (_commandLineOptions.Include != null && _commandLineOptions.Include != string.Empty)
                        {
                            TestFilter includeFilter = new SimpleCategoryExpression(_commandLineOptions.Include).Filter;

                            if (filter.IsEmpty)
                                filter = includeFilter;
                            else
                                filter = new AndFilter(filter, includeFilter);
                        }

                        if (_commandLineOptions.Exclude != null && _commandLineOptions.Exclude != string.Empty)
                        {
                            TestFilter excludeFilter = new NotFilter(new SimpleCategoryExpression(_commandLineOptions.Exclude).Filter);

                            if (filter.IsEmpty)
                                filter = excludeFilter;
                            else if (filter is AndFilter)
                                ((AndFilter)filter).Add(excludeFilter);
                            else
                                filter = new AndFilter(filter, excludeFilter);
                        }

                        RunTests(filter);
                    }
                }
                catch (FileNotFoundException ex)
                {
                    _outWriter.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    _outWriter.WriteLine(ex.ToString());
                }
                finally
                {
                    if (_commandLineOptions.OutFile == null)
                    {
                        if (_commandLineOptions.Wait)
                        {
                            Console.WriteLine("Press Enter key to continue . . .");
                            Console.ReadLine();
                        }
                    }
                    else
                    {
                        _outWriter.Close();
                    }

                    if (_commandLineOptions.ErrFile != null)
                        _errWriter.Close();
                }
            }
        }

#endregion

        #region Helper Methods

        private void RunTests(ITestFilter filter)
        {
            var startTime = DateTime.UtcNow;

            ITestResult result = _runner.Run(this, filter);
            new ResultReporter(result, _outWriter).ReportResults();

            if (_commandLineOptions.ResultOutputSpecifications.Count > 0)
            {
                var outputManager = new OutputManager(_workDirectory);

                foreach (var spec in _commandLineOptions.ResultOutputSpecifications)
                    outputManager.WriteResultFile(result, spec);
            }
        }

        private void ExploreTests()
        {
            ITest testNode = _runner.LoadedTest;

            var specs = _commandLineOptions.ExploreOutputSpecifications;

            if (specs.Count == 0)
            {
                new TestCaseOutputWriter().WriteTestFile(testNode, Console.Out);
            }
            else
            {
                var outputManager = new OutputManager(_workDirectory);

                foreach (var spec in _commandLineOptions.ExploreOutputSpecifications)
                    outputManager.WriteTestFile(testNode, spec);
            }
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
                InternalTrace.Initialize(Path.Combine(Environment.CurrentDirectory, logName), traceLevel);
            }
        }

        /// <summary>
        /// Writes the header.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public static void WriteHeader(TextWriter writer)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = AssemblyHelper.GetAssemblyName(executingAssembly);
#if NUNITLITE
            string title = "NUnitLite";
#else
            string title = "NUNit Framework";
#endif
            System.Version version = assemblyName.Version;
            string copyright = "Copyright (C) 2012, Charlie Poole";
            string build = "";

            object[] attrs = executingAssembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            if (attrs.Length > 0)
            {
                AssemblyTitleAttribute titleAttr = (AssemblyTitleAttribute)attrs[0];
                title = titleAttr.Title;
            }

            attrs = executingAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (attrs.Length > 0)
            {
                AssemblyCopyrightAttribute copyrightAttr = (AssemblyCopyrightAttribute)attrs[0];
                copyright = copyrightAttr.Copyright;      
            }

            attrs = executingAssembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);
            if (attrs.Length > 0)
            {
                AssemblyConfigurationAttribute configAttr = (AssemblyConfigurationAttribute)attrs[0];
                build = string.Format("({0})", configAttr.Configuration); 
            }

            writer.WriteLine(String.Format("{0} {1} {2}", title, version.ToString(3), build));
            writer.WriteLine(copyright);
            writer.WriteLine();
        }

        /// <summary>
        /// Writes the runtime environment.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public static void WriteRuntimeEnvironment(TextWriter writer)
        {
            string clrPlatform = Type.GetType("Mono.Runtime", false) == null ? ".NET" : "Mono";
            writer.WriteLine("Runtime Environment -");
            writer.WriteLine("    OS Version: {0}", Environment.OSVersion);
            writer.WriteLine("  {0} Version: {1}", clrPlatform, Environment.Version);
            writer.WriteLine();
        }

        /// <summary>
        /// Make the settings for this run - this is public for testing
        /// </summary>
        public static Dictionary<string, object> MakeRunSettings(CommandLineOptions options)
        {
            // Transfer command line options to run settings
            var runSettings = new Dictionary<string, object>();

            if (options.InitialSeed >= 0)
                runSettings[DriverSettings.RandomSeed] = options.InitialSeed;

            if (options.WorkDirectory != null)
                runSettings[DriverSettings.WorkDirectory] = Path.GetFullPath(options.WorkDirectory);

            if (options.DefaultTimeout >= 0)
                runSettings[DriverSettings.DefaultTimeout] = options.DefaultTimeout;

            return runSettings;
        }

        #endregion

        #region ITestListener Members

        /// <summary>
        /// Called when a test or suite has just started
        /// </summary>
        /// <param name="test">The test that is starting</param>
        public void TestStarted(ITest test)
        {
#if !SILVERLIGHT
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
            if (_teamCity != null)
                _teamCity.TestFinished(result);
#endif

            bool isSuite = result.Test.IsSuite;
            var labels = _commandLineOptions.DisplayTestLabels != null
                ? _commandLineOptions.DisplayTestLabels.ToUpperInvariant()
                : "ON";
                
            if (!isSuite && labels == "ALL")
                WriteTestLabel(result);

            if (result.Output.Length > 0)
            {
                if (!isSuite && labels == "ON")
                    WriteTestLabel(result);
                _outWriter.Write(result.Output);
            }
        }

        private void WriteTestLabel(ITestResult result)
        {
            _outWriter.WriteLine("***** " + result.Test.Name);
        }

        #endregion
    }
}
