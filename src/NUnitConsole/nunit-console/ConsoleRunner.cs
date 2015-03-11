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
using System.IO;
using System.Xml;
using NUnit.Common;
using NUnit.Engine;
using NUnit.Engine.Extensibility;

namespace NUnit.ConsoleRunner
{
    using Utilities;
    
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

        private ITestEngine _engine;
        private ConsoleOptions _options;
        private IResultService _resultService;

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
        }

        #endregion

        #region Execute Method

        /// <summary>
        /// Executes tests according to the provided commandline options.
        /// </summary>
        /// <returns></returns>
        public int Execute()
        {
            _outWriter.WriteLine(ColorStyle.SectionHeader, "Test Files");
            foreach (string file in _options.InputFiles)
                _outWriter.WriteLine(ColorStyle.Default, "    " + file);
            _outWriter.WriteLine();

            WriteRuntimeEnvironment(_outWriter);

            TestPackage package = MakeTestPackage(_options);

            TestFilter filter = CreateTestFilter(_options);

            if (_options.Explore)
                return ExploreTests(package, filter);
            else
                return RunTests(package, filter);
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
                    _resultService.GetResultWriter(spec.Format, new object[] { spec.Transform }).WriteResultFile(result, spec.OutputPath);
                    _outWriter.WriteLine("Results ({0}) saved as {1}", spec.Format, spec.OutputPath);
                }
            }

            return ConsoleRunner.OK;
        }

        private int RunTests(TestPackage package, TestFilter filter)
        {
            // TODO: We really need options as resolved by engine for most of  these
            DisplayRequestedOptions();

            foreach (var spec in _options.ResultOutputSpecifications)
                GetResultWriter(spec).CheckWritability(spec.OutputPath);

            // TODO: Incorporate this in EventCollector?
            RedirectOutputAsRequested();

            var labels = _options.DisplayTestLabels != null
                ? _options.DisplayTestLabels.ToUpperInvariant()
                : "ON";

            var eventHandler = new TestEventHandler(_outWriter, labels, _options.TeamCity);

            XmlNode result;
            try
            {
                using (new SaveConsoleOutput())
                using (new ColorConsole(ColorStyle.Output))
                using (ITestRunner runner = _engine.GetRunner(package))
                {
                    result = runner.Run(eventHandler, filter);
                }
            }
            finally
            {
                RestoreOutput();
            }

            var writer = new ColorConsoleWriter(!_options.NoColor);
            var reporter = new ResultReporter(result, writer, _options);
            reporter.ReportResults();

            foreach (var spec in _options.ResultOutputSpecifications)
            {
                GetResultWriter(spec).WriteResultFile(result, spec.OutputPath);
                _outWriter.WriteLine("Results ({0}) saved as {1}", spec.Format, spec.OutputPath);
            }

            return reporter.Summary.FailureCount + reporter.Summary.ErrorCount + reporter.Summary.InvalidCount;
        }

        private void WriteRuntimeEnvironment(ExtendedTextWriter OutWriter)
        {
            OutWriter.WriteLine(ColorStyle.SectionHeader, "Runtime Environment");
            OutWriter.WriteLabelLine("   OS Version: ", Environment.OSVersion.ToString());
            OutWriter.WriteLabelLine("  CLR Version: ", Environment.Version.ToString());
            OutWriter.WriteLine();
        }

        private void DisplayRequestedOptions()
        {
            _outWriter.WriteLine(ColorStyle.SectionHeader, "Options");
            _outWriter.WriteLabel("    ProcessModel: ", _options.ProcessModel ?? "Default");
            _outWriter.WriteLabelLine("    DomainUsage: ", _options.DomainUsage ?? "Default");
            _outWriter.WriteLabelLine("    Execution Runtime: ", _options.Framework ?? "Not Specified");
            if (_options.DefaultTimeout >= 0)
                _outWriter.WriteLabelLine("    Default timeout: ", _options.DefaultTimeout.ToString());
            if (_options.NumWorkers > 0)
                _outWriter.WriteLabelLine("    Worker Threads: ", _options.NumWorkers.ToString());
            _outWriter.WriteLabelLine("    Work Directory: ", _workDirectory);
            _outWriter.WriteLabelLine("    Internal Trace: ", _options.InternalTraceLevel ?? "Off");
            if (_options.TeamCity)
                _outWriter.WriteLine(ColorStyle.Label, "    Display TeamCity Service Messages");
            _outWriter.WriteLine();

            if (_options.TestList.Count > 0)
            {
                _outWriter.WriteLine(ColorStyle.Label, "Selected test(s):");
                using (new ColorConsole(ColorStyle.Default))
                    foreach (string testName in _options.TestList)
                        _outWriter.WriteLine("    " + testName);
            }

            if (!string.IsNullOrEmpty( _options.Include ))
                _outWriter.WriteLabelLine("Included categories: ", _options.Include);

            if (!string.IsNullOrEmpty( _options.Exclude ))
                _outWriter.WriteLabelLine("Excluded categories: ", _options.Exclude);
        }

        private void RedirectOutputAsRequested()
        {
            if (_options.OutFile != null)
            {
                var outStreamWriter = new StreamWriter(Path.Combine(_workDirectory, _options.OutFile));
                outStreamWriter.AutoFlush = true;
                _outWriter = new ExtendedTextWrapper(outStreamWriter);
            }

            if (_options.ErrFile != null)
            {
                var errorStreamWriter = new StreamWriter(Path.Combine(_workDirectory, _options.ErrFile));
                errorStreamWriter.AutoFlush = true;
                _errorWriter = errorStreamWriter;
            }
        }

        private void RestoreOutput()
        {
            _outWriter.Flush();
            if (_options.OutFile != null)
                _outWriter.Close();

            _errorWriter.Flush();
            if (_options.ErrFile != null)
                _errorWriter.Close();
        }

        private IResultWriter GetResultWriter(OutputSpecification spec)
        {
            return _resultService.GetResultWriter(spec.Format, new object[] { spec.Transform });
        }

        // This is public static for ease of testing
        public static TestPackage MakeTestPackage( ConsoleOptions options )
        {
            TestPackage package = new TestPackage(options.InputFiles);

            if (options.ProcessModel != null)//ProcessModel.Default)
                package.Settings[PackageSettings.ProcessModel] = options.ProcessModel;

            if (options.DomainUsage != null)
                package.Settings[PackageSettings.DomainUsage] = options.DomainUsage;

            if (options.Framework != null)
                package.Settings[PackageSettings.RuntimeFramework] = options.Framework;

            if (options.RunAsX86)
                package.Settings[PackageSettings.RunAsX86] = true;

            if (options.DisposeRunners)
                package.Settings[PackageSettings.DisposeRunners] = true;

            if (options.DefaultTimeout >= 0)
                package.Settings[PackageSettings.DefaultTimeout] = options.DefaultTimeout;

            if (options.InternalTraceLevel != null)
                package.Settings[PackageSettings.InternalTraceLevel] = options.InternalTraceLevel;

            if (options.ActiveConfig != null)
                package.Settings[PackageSettings.ActiveConfig] = options.ActiveConfig;
            
            if (options.WorkDirectory != null)
                package.Settings[PackageSettings.WorkDirectory] = options.WorkDirectory;

            if (options.StopOnError)
                package.Settings[PackageSettings.StopOnError] = true;

            if (options.NumWorkers >= 0)
                package.Settings[PackageSettings.NumberOfTestWorkers] = options.NumWorkers;

            if (options.RandomSeed > 0)
                package.Settings[PackageSettings.RandomSeed] = options.RandomSeed;

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

