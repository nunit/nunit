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

namespace NUnitLite.Runner
{
    /// <summary>
    /// The AutoRun class is used by executable test
    /// assemblies to control their own execution.
    /// </summary>
    public class AutoRun
    {
#if NETCF // NETCF: Any harm in using txt everywhere?
          // Some mobiles don't have an Open With menu item
        private const string LOG_FILE_FORMAT = "InternalTrace.{0}.{1}.txt";
#else
        private const string LOG_FILE_FORMAT = "InternalTrace.{0}.{1}.log";
#endif

        /// <summary>
        /// Execute the tests in the assembly, passing in
        /// a list of arguments.
        /// </summary>
        /// <param name="args">Execution options</param>
        public int Execute(string[] args)
        {
            var options = new ConsoleOptions(args);
            var callingAssembly = Assembly.GetCallingAssembly();

            var level = (InternalTraceLevel)Enum.Parse(typeof(InternalTraceLevel), options.InternalTraceLevel ?? "Off", true);
#if NETCF  // NETCF: Try to unify
            InitializeInternalTrace(callingAssembly.GetName().CodeBase, level);
#else
            InitializeInternalTrace(callingAssembly.Location, level);
#endif

            ExtendedTextWriter outWriter = null;
            if (options.OutFile != null)
            {
                outWriter = new ExtendedTextWrapper(new StreamWriter(Path.Combine(options.WorkDirectory, options.OutFile)));
                Console.SetOut(outWriter);
            }

            TextWriter errWriter = null;
            if (options.ErrFile != null)
            {
                errWriter = new StreamWriter(Path.Combine(options.WorkDirectory, options.ErrFile));
                Console.SetError(errWriter);
            }

            var _textUI = new TextUI(outWriter, options);

            if (!options.NoHeader)
                _textUI.DisplayHeader();

            if (options.ShowHelp)
            {
                _textUI.DisplayHelp();
                return TextRunner.OK;
            }

            if (options.ErrorMessages.Count > 0)
            {
                _textUI.DisplayErrors(options.ErrorMessages);
                _textUI.DisplayHelp();

                return TextRunner.INVALID_ARG;
            }

            if (options.InputFiles.Count > 0)
            {
                _textUI.DisplayError("Input assemblies may not be specified when using the NUnitLite AutoRunner");
                return TextRunner.INVALID_ARG;
            }

            _textUI.DisplayTestFiles(new string[] { callingAssembly.GetName().Name });
            _textUI.DisplayRuntimeEnvironment();
            _textUI.DisplayRequestedOptions();

            if (options.WaitBeforeExit && options.OutFile != null)
                _textUI.DisplayWarning("Ignoring /wait option - only valid for Console");

            try
            {
                return new TextRunner(_textUI, options).Execute(callingAssembly);
            }
            finally
            {
                if (options.WaitBeforeExit)
                    _textUI.WaitForUser("Press Enter key to continue . . .");

                if (outWriter != null)
                    outWriter.Close();

                if (errWriter != null)
                    errWriter.Close();
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
    }
}
#endif
