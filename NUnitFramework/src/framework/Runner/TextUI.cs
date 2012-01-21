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
using System.Xml;
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
        private CommandLineOptions commandLineOptions;
        private int reportCount = 0;

        private NUnit.ObjectList assemblies = new NUnit.ObjectList();

        private TextWriter writer;

        private ITestAssemblyRunner runner;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TextUI"/> class.
        /// </summary>
        /// <param name="writer">The TextWriter to use.</param>
        public TextUI(TextWriter writer)
        {
            this.writer = writer;
            this.runner = new NUnitLiteTestAssemblyRunner(new NUnitLiteTestAssemblyBuilder());
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
            Assembly callingAssembly = Assembly.GetCallingAssembly();

            this.commandLineOptions = ProcessArguments( args );

            if (!commandLineOptions.Help && !commandLineOptions.Error)
            {
                if (commandLineOptions.Wait && !(this is ConsoleUI))
                    writer.WriteLine("Ignoring /wait option - only valid for Console");

                IDictionary loadOptions = new Hashtable();
                //if (options.Load.Count > 0)
                //    loadOptions["LOAD"] = options.Load;

                IDictionary runOptions = new Hashtable();
                if (commandLineOptions.TestCount > 0)
                    runOptions["RUN"] = commandLineOptions.Tests;

                try
                {
                    foreach (string name in commandLineOptions.Parameters)
                        assemblies.Add(Assembly.Load(name));

                    if (assemblies.Count == 0)
                        assemblies.Add(callingAssembly);

                    // TODO: For now, ignore all but first assembly
                    Assembly assembly = assemblies[0] as Assembly;

                    if (!runner.Load(assembly, loadOptions))
                    {
                        Console.WriteLine("No tests found in assembly {0}", assembly.GetName().Name);
                        return;
                    }

                    if (commandLineOptions.ListTests)
                        ListTests(runner.LoadedTest);
                    else
                        ReportResults(runner.Run(TestListener.NULL, TestFilter.Empty));
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
                    if (commandLineOptions.Wait && this is ConsoleUI)
                    {
                        Console.WriteLine("Press Enter key to continue . . .");
                        Console.ReadLine();
                    }
                }
            }
        }

        private void ListTests(ITest test)
        {
            XmlNode testNode = test.ToXml(true);

            string listFile = commandLineOptions.ListFile;
            XmlTextWriter testWriter = listFile != null && listFile.Length > 0
                ? new XmlTextWriter(listFile, System.Text.Encoding.UTF8)
                : new XmlTextWriter(Console.Out);
            testWriter.Formatting = Formatting.Indented;
            testNode.WriteTo(testWriter);
            testWriter.Close();
        }

        /// <summary>
        /// Reports the results.
        /// </summary>
        /// <param name="result">The result.</param>
        private void ReportResults( ITestResult result )
        {
            ResultSummary summary = new ResultSummary(result);

            writer.WriteLine("{0} Tests : {1} Failures, {2} Not Run",
                summary.TestCount, summary.FailureCount, summary.NotRunCount);

            if (summary.FailureCount > 0)
                PrintErrorReport(result);

            if (summary.NotRunCount > 0)
                PrintNotRunReport(result);

            if (commandLineOptions.Full)
                PrintFullReport(result);
        }
        #endregion

        #region Helper Methods
        private CommandLineOptions ProcessArguments(string[] args)
        {
            this.commandLineOptions = new CommandLineOptions();
            commandLineOptions.Parse(args);

            if (!commandLineOptions.Nologo)
                WriteCopyright();

            if (commandLineOptions.Help)
                writer.Write(commandLineOptions.HelpText);
            else if (commandLineOptions.Error)
                writer.WriteLine(commandLineOptions.ErrorMessage);

            return commandLineOptions;
        }

        private void WriteCopyright()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
#if NUNITLITE
            string title = "NUnitLite";
#else
            string title = "NUNit Framework";
#endif
            System.Version version = executingAssembly.GetName().Version;
            string copyright = "Copyright (C) 2011, Charlie Poole";
            string build = "";

#if !NETCF_1_0
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
#endif

            writer.WriteLine(String.Format("{0} {1} {2}", title, version.ToString(3), build));
            writer.WriteLine(copyright);
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
            if (result.HasChildren)
                foreach (ITestResult r in result.Children)
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
            if (result.HasChildren)
                foreach (ITestResult r in result.Children)
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
            foreach (PropertyEntry entry in test.Properties)
                writer.WriteLine("  {0}: {1}", entry.Name, entry.Value);            
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
            switch (result.ResultState.Status)
            {
                case TestStatus.Failed:
                    status = "FAIL";
                    break;
                case TestStatus.Skipped:
                    status = "SKIP";
                    break;
                case TestStatus.Inconclusive:
                    status = "INC ";
                    break;
                case TestStatus.Passed:
                    status = "OK  ";
                    break;
            }

            writer.Write(status);
            writer.Write(indent);
            writer.WriteLine(result.Name);

            if (result.HasChildren)
                foreach (ITestResult r in result.Children)
                    PrintAllResults(r, indent + "  ");
        }
        #endregion
    }
}
