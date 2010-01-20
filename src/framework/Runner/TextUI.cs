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
using System.IO;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Api;
using NUnit.Framework.Internal;

namespace NUnitLite.Runner
{
    /// <summary>
    /// A version of TextUI that outputs to the console.
    /// If you use it on a device without a console like
    /// PocketPC or SmartPhone you won't see anything!
    /// 
    /// Call it from your Main like this:
    ///   new ConsoleUI().Execute(args);
    /// </summary>
    public class ConsoleUI : TextUI
    {
        /// <summary>
        /// Construct an instance of ConsoleUI
        /// </summary>
#if NETCF_1_0
        public ConsoleUI() : base(ConsoleWriter.Out) { }
#else
        public ConsoleUI() : base(Console.Out) { }
#endif
    }

    /// <summary>
    /// A version of TextUI that writes to a file.
    /// 
    /// Call it from your Main like this:
    ///   new FileUI(filePath).Execute(args);
    /// </summary>
    public class FileUI : TextUI
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileUI"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public FileUI(string path) : base(new StreamWriter(path)) { }
    }

    /// <summary>
    /// A version of TextUI that displays to debug.
    /// 
    /// Call it from your Main like this:
    ///   new DebugUI().Execute(args);
    /// </summary>
    public class DebugUI : TextUI
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DebugUI"/> class.
        /// </summary>
        public DebugUI() : base(DebugWriter.Out) { }
    }

    /// <summary>
    /// A version of TextUI that writes to a TcpWriter
    /// </summary>
    public class TcpUI : TextUI
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TcpUI"/> class.
        /// </summary>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="port">The port.</param>
        public TcpUI(string hostName, int port) : base( new TcpWriter(hostName, port) ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpUI"/> class.
        /// </summary>
        /// <param name="hostName">Name of the host.</param>
        public TcpUI(string hostName) : this(hostName, 9000) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpUI"/> class.
        /// </summary>
        public TcpUI() : this("localhost", 9000) { }
    }

    /// <summary>
    /// TextUI is a general purpose class that runs tests and
    /// outputs to a TextWriter.
    /// 
    /// Call it from your Main like this:
    ///   new TextUI(textWriter).Execute(args);
    /// </summary>
    public class TextUI
    {
        private CommandLineOptions options;
        private int reportCount = 0;

        private NUnit.ObjectList assemblies = new NUnit.ObjectList();

        private TextWriter writer;

        private TestRunner runner = new TestRunner();

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TextUI"/> class.
        /// </summary>
        /// <param name="writer">The TextWriter to use.</param>
        public TextUI(TextWriter writer)
        {
            this.writer = writer;
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
            // NOTE: This must be directly called from the
            // test assembly in order for the mechanism to work.
            Assembly callingAssembly = Assembly.GetCallingAssembly();

            this.options = ProcessArguments( args );

            if (!options.Help && !options.Error)
            {
                if (options.Wait && !(this is ConsoleUI))
                    writer.WriteLine("Ignoring /wait option - only valid for Console");

                try
                {
                    foreach (string name in options.Parameters)
                        assemblies.Add(Assembly.Load(name));

                    if (assemblies.Count == 0)
                        assemblies.Add(callingAssembly);

                    foreach (Assembly assembly in assemblies)
                    {
                        Test suite = options.TestCount == 0
                            ? TestLoader.Load(assembly)
                            : TestLoader.Load(assembly, options.Tests);

                        ReportResults( runner.Run(suite) );
                    }
                }
                catch (TestRunnerException ex)
                {
                    writer.WriteLine(ex.Message);
                }
                catch (FileNotFoundException ex)
                {
                    writer.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    writer.WriteLine(ex.ToString());
                }
                finally
                {
                    if (options.Wait && this is ConsoleUI)
                    {
                        Console.WriteLine("Press Enter key to continue . . .");
                        Console.ReadLine();
                    }
                }
            }
        }

        /// <summary>
        /// Runs all tests in the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public void Run(Assembly assembly)
        {
            ReportResults( runner.Run(assembly) );
        }

        /// <summary>
        /// Runs selected tests in the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="tests">The tests.</param>
        public void Run(Assembly assembly, string[] tests)
        {
            ReportResults( runner.Run(assembly, tests) );
        }

        /// <summary>
        /// Reports the results.
        /// </summary>
        /// <param name="result">The result.</param>
        private void ReportResults( ITestResult result )
        {
            ResultSummary summary = new ResultSummary(result);

            writer.WriteLine("{0} Tests : {1} Errors, {2} Failures, {3} Not Run",
                summary.TestCount, summary.ErrorCount, summary.FailureCount, summary.NotRunCount);

            if (summary.ErrorCount + summary.FailureCount > 0)
                PrintErrorReport(result);

            if (summary.NotRunCount > 0)
                PrintNotRunReport(result);

            if (options.Full)
                PrintFullReport(result);
        }
        #endregion

        #region Helper Methods
        private CommandLineOptions ProcessArguments(string[] args)
        {
            this.options = new CommandLineOptions();
            options.Parse(args);

            if (!options.Nologo)
                WriteCopyright();

            if (options.Help)
                writer.Write(options.HelpText);
            else if (options.Error)
                writer.WriteLine(options.ErrorMessage);

            return options;
        }

        private void WriteCopyright()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            System.Version version = executingAssembly.GetName().Version;

#if NETCF_1_0
            writer.WriteLine("NUnitLite version {0}", version.ToString() );
            writer.WriteLine("Copyright 2007, Charlie Poole");
#else
            object[] objectAttrs = executingAssembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            AssemblyProductAttribute productAttr = (AssemblyProductAttribute)objectAttrs[0];

            objectAttrs = executingAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            AssemblyCopyrightAttribute copyrightAttr = (AssemblyCopyrightAttribute)objectAttrs[0];

            writer.WriteLine(String.Format("{0} version {1}", productAttr.Product, version.ToString(3)));
            writer.WriteLine(copyrightAttr.Copyright);
#endif
            writer.WriteLine();

            string clrPlatform = Type.GetType("Mono.Runtime", false) == null ? ".NET" : "Mono";
            writer.WriteLine("Runtime Environment -");
            writer.WriteLine("    OS Version: {0}", Environment.OSVersion);
            writer.WriteLine("  {0} Version: {1}", clrPlatform, Environment.Version);
            writer.WriteLine();
        }

        private void PrintErrorReport(ITestResult result)
        {
            reportCount = 0;
            writer.WriteLine();
            writer.WriteLine("Errors and Failures:");
            PrintErrorResults(result);
        }

        private void PrintErrorResults(ITestResult result)
        {
            if (result.Results != null)
                foreach (ITestResult r in result.Results)
                    PrintErrorResults(r);
            else if (result.ResultState == ResultState.Error || result.ResultState == ResultState.Failure)
            {
                writer.WriteLine();
                writer.WriteLine("{0}) {1} ({2})", ++reportCount, result.Name, result.FullName);
                //if (options.ListProperties)
                //    PrintTestProperties(result.Test);
                writer.WriteLine(result.Message);
#if !NETCF_1_0
                writer.WriteLine(result.StackTrace);
#endif
            }
        }

        private void PrintNotRunReport(ITestResult result)
        {
            reportCount = 0;
            writer.WriteLine();
            writer.WriteLine("Tests Not Run:");
            PrintNotRunResults(result);
        }

        private void PrintNotRunResults(ITestResult result)
        {
            if (result.Results != null)
                foreach (ITestResult r in result.Results)
                    PrintNotRunResults(r);
            else if (result.ResultState == ResultState.Ignored || result.ResultState == ResultState.NotRunnable || result.ResultState == ResultState.Skipped)
            {
                writer.WriteLine();
                writer.WriteLine("{0}) {1} ({2}) : {3}", ++reportCount, result.Name, result.FullName, result.Message);
                //if (options.ListProperties)
                //    PrintTestProperties(result.Test);
            }
        }

        private void PrintTestProperties(ITest test)
        {
            foreach (DictionaryEntry entry in test.Properties)
                writer.WriteLine("  {0}: {1}", entry.Key, entry.Value);            
        }

        private void PrintFullReport(ITestResult result)
        {
            writer.WriteLine();
            writer.WriteLine("All Test Results:");
            PrintAllResults(result, " ");
        }

        private void PrintAllResults(ITestResult result, string indent)
        {
            string status = null;
            switch (result.ResultState)
            {
                case ResultState.Error:
                case ResultState.NotRunnable:
                    status = "ERROR ";
                    break;
                case ResultState.Failure:
                    status = "FAILED";
                    break;
                case ResultState.Cancelled:
                    status = "CANCEL ";
                    break;
                case ResultState.Skipped:
                    status = "SKIP  ";
                    break;
                case ResultState.Ignored:
                    status = "IGNORE";
                    break;
                case ResultState.Inconclusive:
                    status = "INC   ";
                    break;
                case ResultState.Success:
                    status = "OK    ";
                    break;
            }

            writer.Write(status);
            writer.Write(indent);
            writer.WriteLine(result.Name);

            if (result.Results != null)
                foreach (ITestResult r in result.Results)
                    PrintAllResults(r, indent + "  ");
        }
        #endregion
    }
}
