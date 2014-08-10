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
        private readonly List<Assembly> _assemblies = new List<Assembly>();
        private TextWriter _writer;
        private ITestAssemblyRunner _runner;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TextUI"/> class.
        /// </summary>
        public TextUI() : this(ConsoleWriter.Out) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextUI"/> class.
        /// </summary>
        /// <param name="writer">The TextWriter to use.</param>
        public TextUI(TextWriter writer)
        {
            // Set the default writer - may be overridden by the args specified
            _writer = writer;
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

            this._commandLineOptions = new CommandLineOptions();
            _commandLineOptions.Parse(args);

            if (_commandLineOptions.OutFile != null)
                this._writer = new StreamWriter(_commandLineOptions.OutFile);

            if (!_commandLineOptions.NoHeader)
                WriteHeader(this._writer);

            if (_commandLineOptions.ShowHelp)
                _writer.Write(_commandLineOptions.HelpText);
            else if (_commandLineOptions.Error)
            {
                _writer.WriteLine(_commandLineOptions.ErrorMessage);
                _writer.WriteLine(_commandLineOptions.HelpText);
            }
            else
            {
                Assembly callingAssembly = Assembly.GetCallingAssembly();

                // We must call this before creating the runner so that any internal logging is initialized
                InitializeInternalTrace(callingAssembly.Location, _commandLineOptions.InternalTraceLevel);

                _runner = new NUnitLiteTestAssemblyRunner(new DefaultTestAssemblyBuilder());

                WriteRuntimeEnvironment(this._writer);

                if (_commandLineOptions.Wait && _commandLineOptions.OutFile != null)
                    _writer.WriteLine("Ignoring /wait option - only valid for Console");

                // We only have one commandline option that has to be passed
                // to the runner, so we do it here for convenience.
                var runnerSettings = new Dictionary<string, object>();
                if (_commandLineOptions.InitialSeed >= 0)
                    runnerSettings[DriverSettings.RandomSeed] = _commandLineOptions.InitialSeed;
                
                TestFilter filter = _commandLineOptions.Tests.Count > 0
                    ? new SimpleNameFilter(_commandLineOptions.Tests)
                    : TestFilter.Empty;

                try
                {
                    foreach (string name in _commandLineOptions.Parameters)
                        _assemblies.Add(Assembly.Load(name));

                    if (_assemblies.Count == 0)
                        _assemblies.Add(callingAssembly);

                    // TODO: For now, ignore all but first assembly
                    Assembly assembly = _assemblies[0];

                    //Randomizer.InitialSeed = _commandLineOptions.InitialSeed;

                    if (_runner.Load(assembly, runnerSettings) == null)
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
                    _writer.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    _writer.WriteLine(ex.ToString());
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
                        _writer.Close();
                    }
                }
            }
        }

#endregion

        #region Helper Methods

        private void RunTests(ITestFilter filter)
        {
            ITestResult result = _runner.Run(this, filter);
            new ResultReporter(result, _writer).ReportResults();

            string resultFile = _commandLineOptions.ResultFile;
            string resultFormat = _commandLineOptions.ResultFormat;
            if (resultFile != null || _commandLineOptions.ResultFormat != null)
            {
                if (resultFile == null)
                    resultFile = "TestResult.xml";

                if (resultFormat == "nunit2")
                    new NUnit2XmlOutputWriter().WriteResultFile(result, resultFile);
                else
                    new NUnit3XmlOutputWriter().WriteResultFile(result, resultFile);
                Console.WriteLine();
                Console.WriteLine("Results saved as {0}.", resultFile);
            }
        }

        private void ExploreTests()
        {
            XmlNode testNode = _runner.LoadedTest.ToXml(true);

            string listFile = _commandLineOptions.ExploreFile;
            TextWriter textWriter = listFile != null && listFile.Length > 0
                ? new StreamWriter(listFile)
                : Console.Out;

            System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;
            System.Xml.XmlWriter testWriter = System.Xml.XmlWriter.Create(textWriter, settings);

            testNode.WriteTo(testWriter);
            testWriter.Close();

            Console.WriteLine();
            Console.WriteLine("Test info saved as {0}.", listFile);
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


        #endregion

        #region ITestListener Members

        /// <summary>
        /// Called when a test has just started
        /// </summary>
        /// <param name="test">The test that is starting</param>
        public void TestStarted(ITest test)
        {
            if (_commandLineOptions.ShowLabels)
                _writer.WriteLine("***** {0}", test.Name);
        }

        /// <summary>
        /// Called when a test has finished
        /// </summary>
        /// <param name="result">The result of the test</param>
        public void TestFinished(ITestResult result)
        {
        }

        /// <summary>
        /// Called when the test creates text output.
        /// </summary>
        /// <param name="testOutput">A console message</param>
        public void TestOutput(TestOutput testOutput)
        {
        }

        #endregion
    }
}
