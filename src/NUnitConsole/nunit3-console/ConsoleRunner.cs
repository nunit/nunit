// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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
using System.IO;
using System.Xml;
using NUnit.Common;
using NUnit.ConsoleRunner.Utilities;
using NUnit.Engine;
using NUnit.Engine.Extensibility;
using System.Runtime.InteropServices;

namespace NUnit.ConsoleRunner
{
    /// <summary>
    /// ConsoleRunner provides the nunit3-console text-based
    /// user interface, running the tests and reporting the results.
    /// </summary>
    public class ConsoleRunner
    {
        #region Console Runner Return Codes

        public static readonly int OK = 0;
        public static readonly int INVALID_ARG = -1;
        public static readonly int INVALID_ASSEMBLY = -2;
        public static readonly int FIXTURE_NOT_FOUND = -3;
        public static readonly int UNEXPECTED_ERROR = -100;

        #endregion

        #region Instance Fields

        private ITestEngine _engine;
        private ConsoleOptions _options;
        private IResultService _resultService;
        private ITestFilterService _filterService;

        private ExtendedTextWriter _outWriter;
        private TextWriter _errorWriter = Console.Error;

        private string _workDirectory;

        #endregion

        #region Constructor

        public ConsoleRunner(ITestEngine engine, ConsoleOptions options, ExtendedTextWriter writer)
        {
            _engine = engine;
            _options = options;
            _outWriter = writer;

            _workDirectory = options.WorkDirectory;

            if (_workDirectory == null)
                _workDirectory = Environment.CurrentDirectory;
            else if (!Directory.Exists(_workDirectory))
                Directory.CreateDirectory(_workDirectory);

            _resultService = _engine.Services.GetService<IResultService>();
            _filterService = _engine.Services.GetService<ITestFilterService>();
        }

        #endregion

        #region Execute Method

        /// <summary>
        /// Executes tests according to the provided commandline options.
        /// </summary>
        /// <returns></returns>
        public int Execute()
        {
            if (!VerifyEngineSupport(_options))
                return INVALID_ARG;

            DisplayRuntimeEnvironment(_outWriter);

            DisplayTestFiles();

            TestPackage package = MakeTestPackage(_options);

            // We display the filters at this point so  that any exception message
            // thrown by CreateTestFilter will be understandable.
            DisplayTestFilters();

            TestFilter filter = CreateTestFilter(_options);

            if (_options.Explore)
                return ExploreTests(package, filter);
            else
                return RunTests(package, filter);
        }

        private void DisplayTestFiles()
        {
            _outWriter.WriteLine(ColorStyle.SectionHeader, "Test Files");
            foreach (string file in _options.InputFiles)
                _outWriter.WriteLine(ColorStyle.Default, "    " + file);
            _outWriter.WriteLine();
        }

        #endregion

        #region Helper Methods

        private int ExploreTests(TestPackage package, TestFilter filter)
        {
            XmlNode result;

            using (var runner = _engine.GetRunner(package))
                result = runner.Explore(filter);

            if (_options.ExploreOutputSpecifications.Count == 0)
            {
                _resultService.GetResultWriter("cases", null).WriteResultFile(result, Console.Out);
            }
            else
            {
                foreach (OutputSpecification spec in _options.ExploreOutputSpecifications)
                {
                    _resultService.GetResultWriter(spec.Format, new object[] {spec.Transform}).WriteResultFile(result, spec.OutputPath);
                    _outWriter.WriteLine("Results ({0}) saved as {1}", spec.Format, spec.OutputPath);
                }
            }

            return ConsoleRunner.OK;
        }

        private int RunTests(TestPackage package, TestFilter filter)
        {
            foreach (var spec in _options.ResultOutputSpecifications)
            {
                var outputPath = Path.Combine(_workDirectory, spec.OutputPath);
                GetResultWriter(spec).CheckWritability(outputPath);
            }

            // TODO: Incorporate this in EventCollector?
            RedirectErrorOutputAsRequested();

            var labels = _options.DisplayTestLabels != null
                ? _options.DisplayTestLabels.ToUpperInvariant()
                : "ON";

            XmlNode result;

            try
            {
                using (new SaveConsoleOutput())
                using (new ColorConsole(ColorStyle.Output))
                using (ITestRunner runner = _engine.GetRunner(package))
                using (var output = CreateOutputWriter())
                {
                    var eventHandler = new TestEventHandler(output, labels, _options.TeamCity); 

                    result = runner.Run(eventHandler, filter);
                }
            }
            finally
            {
                RestoreErrorOutput();
            }

            var writer = new ColorConsoleWriter(!_options.NoColor);
            var reporter = new ResultReporter(result, writer, _options);
            reporter.ReportResults();

            foreach (var spec in _options.ResultOutputSpecifications)
            {
                var outputPath = Path.Combine(_workDirectory, spec.OutputPath);
                GetResultWriter(spec).WriteResultFile(result, outputPath);
                _outWriter.WriteLine("Results ({0}) saved as {1}", spec.Format, spec.OutputPath);
            }

            if (reporter.Summary.UnexpectedError)
                return ConsoleRunner.UNEXPECTED_ERROR;

            return reporter.Summary.InvalidAssemblies > 0
                    ? ConsoleRunner.INVALID_ASSEMBLY
                    : reporter.Summary.FailureCount + reporter.Summary.ErrorCount + reporter.Summary.InvalidCount;

        }

        private void DisplayRuntimeEnvironment(ExtendedTextWriter OutWriter)
        {
            OutWriter.WriteLine(ColorStyle.SectionHeader, "Runtime Environment");
            OutWriter.WriteLabelLine("   OS Version: ", GetOSVersion());
            OutWriter.WriteLabelLine("  CLR Version: ", Environment.Version.ToString());
            OutWriter.WriteLine();
        }

        private static string GetOSVersion()
        {
            OperatingSystem os = Environment.OSVersion;
            string osString = os.ToString();
            if (os.Platform == PlatformID.Unix)
            {
                IntPtr buf = Marshal.AllocHGlobal(8192);
                if (uname(buf) == 0)
                {
                    var unixVariant = Marshal.PtrToStringAnsi(buf);
                    if (unixVariant.Equals("Darwin"))
                        unixVariant = "MacOSX";
                    
                    osString = string.Format("{0} {1} {2}", unixVariant, os.Version, os.ServicePack); 
                }
                Marshal.FreeHGlobal(buf);
            }
            return osString;
        }

        [DllImport("libc")]
        static extern int uname(IntPtr buf);

        private void DisplayTestFilters()
        {
            if (_options.TestList.Count > 0 || _options.WhereClauseSpecified)
            {
                _outWriter.WriteLine(ColorStyle.SectionHeader, "Test Filters");

                if (_options.TestList.Count > 0)
                    foreach (string testName in _options.TestList)
                        _outWriter.WriteLabelLine("    Test: ", testName);

                if (_options.WhereClauseSpecified)
                    _outWriter.WriteLabelLine("    Where: ", _options.WhereClause.Trim());

                _outWriter.WriteLine();
            }
        }

        private void RedirectErrorOutputAsRequested()
        {
            if (_options.ErrFileSpecified)
            {
                var errorStreamWriter = new StreamWriter(Path.Combine(_workDirectory, _options.ErrFile));
                errorStreamWriter.AutoFlush = true;
                _errorWriter = errorStreamWriter;
            }
        }

        private ExtendedTextWriter CreateOutputWriter()
        {
            if (_options.OutFileSpecified)
            {
                var outStreamWriter = new StreamWriter(Path.Combine(_workDirectory, _options.OutFile));
                outStreamWriter.AutoFlush = true;

                return new ExtendedTextWrapper(outStreamWriter);
            }

            return _outWriter;
        }

        private void RestoreErrorOutput()
        {
            _errorWriter.Flush();
            if (_options.ErrFileSpecified)
                _errorWriter.Close();
        }

        private IResultWriter GetResultWriter(OutputSpecification spec)
        {
            return _resultService.GetResultWriter(spec.Format, new object[] {spec.Transform});
        }

        // This is public static for ease of testing
        public static TestPackage MakeTestPackage(ConsoleOptions options)
        {
            TestPackage package = new TestPackage(options.InputFiles);

            if (options.ProcessModelSpecified)
                package.AddSetting(PackageSettings.ProcessModel, options.ProcessModel);

            if (options.DomainUsageSpecified)
                package.AddSetting(PackageSettings.DomainUsage, options.DomainUsage);

            if (options.FrameworkSpecified)
                package.AddSetting(PackageSettings.RuntimeFramework, options.Framework);

            if (options.RunAsX86)
                package.AddSetting(PackageSettings.RunAsX86, true);

            if (options.DisposeRunners)
                package.AddSetting(PackageSettings.DisposeRunners, true);

            if (options.ShadowCopyFiles)
                package.AddSetting(PackageSettings.ShadowCopyFiles, true);

            if (options.DefaultTimeout >= 0)
                package.AddSetting(PackageSettings.DefaultTimeout, options.DefaultTimeout);

            if (options.InternalTraceLevelSpecified)
                package.AddSetting(PackageSettings.InternalTraceLevel, options.InternalTraceLevel);

            if (options.ActiveConfigSpecified)
                package.AddSetting(PackageSettings.ActiveConfig, options.ActiveConfig);

            // Always add work directory, in case current directory is changed
            var workDirectory = options.WorkDirectory ?? Environment.CurrentDirectory;
            package.AddSetting(PackageSettings.WorkDirectory, workDirectory);

            if (options.StopOnError)
                package.AddSetting(PackageSettings.StopOnError, true);

            if (options.MaxAgentsSpecified)
                package.AddSetting(PackageSettings.MaxAgents, options.MaxAgents);

            if (options.NumberOfTestWorkersSpecified)
                package.AddSetting(PackageSettings.NumberOfTestWorkers, options.NumberOfTestWorkers);

            if (options.RandomSeedSpecified)
                package.AddSetting(PackageSettings.RandomSeed, options.RandomSeed);

            if (options.DebugTests)
            {
                package.AddSetting(PackageSettings.DebugTests, true);

                if (!options.NumberOfTestWorkersSpecified)
                    package.AddSetting(PackageSettings.NumberOfTestWorkers, 0);
            }

            if (options.PauseBeforeRun)
                package.AddSetting(PackageSettings.PauseBeforeRun, true);

#if DEBUG
            if (options.DebugAgent)
                package.AddSetting(PackageSettings.DebugAgent, true);

            //foreach (KeyValuePair<string, object> entry in package.Settings)
            //    if (!(entry.Value is string || entry.Value is int || entry.Value is bool))
            //        throw new Exception(string.Format("Package setting {0} is not a valid type", entry.Key));
#endif

            return package;
        }

        private TestFilter CreateTestFilter(ConsoleOptions options)
        {
            ITestFilterBuilder builder = _filterService.GetTestFilterBuilder();

            foreach (string testName in options.TestList)
                builder.AddTest(testName);

            if (options.WhereClauseSpecified)
                builder.SelectWhere(options.WhereClause);

            return builder.GetFilter();
        }

        private bool VerifyEngineSupport(ConsoleOptions options)
        {
            foreach (var spec in options.ResultOutputSpecifications)
            {
                bool available = false;

                foreach (var format in _resultService.Formats)
                {
                    if (spec.Format == format)
                    {
                        available = true;
                        break;
                    }
                }

                if (!available)
                {
                    Console.WriteLine("Unknown result format: {0}", spec.Format);
                    return false;
                }
            }

            return true;
        }

        #endregion

    }
}

