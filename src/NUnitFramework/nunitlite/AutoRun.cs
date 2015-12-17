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

#if !SILVERLIGHT
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NUnit.Common;
using NUnit.Framework.Internal;

namespace NUnitLite
{
    /// <summary>
    /// The AutoRun class is used by executable test
    /// assemblies to control their own execution.
    /// 
    /// Call it from your executable test like this:
    ///    new AutoRun().Execute(args);
    /// The arguments can be those passed into your exe
    /// or constructed for the purpose in your code.
    /// 
    /// If the tests are in a dll, you can write a stub
    /// executable that runs them like this:
    ///    new Autorun().Execute(testAssembly, args);
    ///
    /// When running tests compiled against the portable
    /// framework, the methods above are not available.
    /// Run your tests like this:
    ///    new AutoRun().Execute(testAssembly, args, output, input);
    /// Where output is an ExtendedTextWriter (normally a
    /// ColorConsoleWriter) and input is usually Console.In
    /// and is used by the --wait option.
    /// </summary>
    public class AutoRun
    {
#if !PORTABLE
        /// <summary>
        /// Execute the tests in the assembly, passing in
        /// a list of arguments. The calling assembly itself
        /// must contain the tests to be executed.
        /// </summary>
        /// <param name="args">Arguments for NUnitLite to use</param>
        public int Execute(string[] args)
        {
            return Execute(Assembly.GetCallingAssembly(), args);
        }

        /// <summary>
        /// Execute the tests in the assembly, passing in
        /// a list of arguments and a test assembly.
        /// This is for use on platforms where the there is
        /// no Assembly.GetCallingAssembly() method or when
        /// it is desired to pass in a different assembly.
        /// </summary>
        /// <param name="testAssembly">The test assembly</param>
        /// <param name="args">arguments for NUnitLite to use</param>
        public int Execute(Assembly testAssembly, string[] args)
        {
            var options = new NUnitLiteOptions(args);

            InitializeInternalTrace(testAssembly, options);

            ExtendedTextWriter outWriter = null;
            if (options.OutFile != null)
            {
                outWriter = new ExtendedTextWrapper(new StreamWriter(Path.Combine(options.WorkDirectory, options.OutFile)));
                Console.SetOut(outWriter);
            }
            else
            {
                outWriter = new ColorConsoleWriter();
            }

            TextWriter errWriter = null;
            if (options.ErrFile != null)
            {
                errWriter = new StreamWriter(Path.Combine(options.WorkDirectory, options.ErrFile));
                Console.SetError(errWriter);
            }

            try
            {
                var textUI = new TextUI(outWriter, Console.In, options);
                return new TextRunner(textUI, options).Execute(testAssembly);
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

        /// <summary>
        /// Execute the tests in the assembly, passing in
        /// a list of arguments, a test assembly a writer
        /// and a reader. For use in the portable build.
        /// </summary>
        /// <param name="testAssembly">The test assembly</param>
        /// <param name="args">Arguments passed to NUnitLite</param>
        /// <param name="writer">An ExtendedTextWriter to which output will be written</param>
        /// <param name="reader">A TextReader used when waiting for input</param>
        public int Execute(Assembly testAssembly, string[] args, ExtendedTextWriter writer, TextReader reader)
        {
            var options = new NUnitLiteOptions(args);
            var textUI = new TextUI(writer, reader, options);
            return new TextRunner(textUI, options).Execute(testAssembly);
        }

#if !PORTABLE
        private void InitializeInternalTrace(Assembly testAssembly, NUnitLiteOptions _options)
        {
#if NETCF  // NETCF: Try to unify
            var assemblyPath = testAssembly.GetName().CodeBase;
#else
            var assemblyPath = testAssembly.Location;
#endif
            
            var traceLevel = (InternalTraceLevel)Enum.Parse(typeof(InternalTraceLevel), _options.InternalTraceLevel ?? "Off", true);

            if (traceLevel != InternalTraceLevel.Off)
            {
                var logName = GetLogFileName(assemblyPath);

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

        private string GetLogFileName(string assemblyPath)
        {
            // Some mobiles don't have an Open With menu item,
            // so we use .txt, which is opened easily.
            const string LOG_FILE_FORMAT =
#if NETCF
                "InternalTrace.{0}.{1}.txt";
#else
                "InternalTrace.{0}.{1}.log";
#endif

            return string.Format(LOG_FILE_FORMAT, Process.GetCurrentProcess().Id, Path.GetFileName(assemblyPath));
        }
#endif
    }
}
#endif
