// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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
using NUnit.Framework.Internal;

namespace NUnit.Framework.Internal
{
	/// <summary>
	/// InternalTrace provides static methods used for tracing the execution
    /// of the NUnit framework. Tests and classes under test may make use 
    /// of Console writes, System.Diagnostics.Trace or various loggers and
    /// NUnit itself traps and processes each of them. For that reason, a
    /// separate internal trace is needed.
    /// </summary>
	public class InternalTrace
    {
        #region TraceLevel Enumeration

        /// <summary>
        /// The TraceLevel enumeration defines the verbosity levels supported by InternalTrace.
        /// </summary>
        public enum TraceLevel
        {
            /// <summary>
            /// All tracing is off.
            /// </summary>
            Off,
            /// <summary>
            /// Report errors only.
            /// </summary>
            Error,
            /// <summary>
            /// Report warnings and above.
            /// </summary>
            Warning,
            /// <summary>
            /// Report informational entries and above.
            /// </summary>
            Info,
            /// <summary>
            /// Report debug entries and above (all entries)
            /// </summary>
            Debug
        }

        #endregion

        #region Static Fields
        private static string MY_NAME = "NUnit.Framework.Internal.InternalTrace";
        
        private readonly static string NL = NUnit.Env.NewLine;
        private readonly static string TIME_FMT = "HH:mm:ss.fff";
        private readonly static string TRACE_FMT = "{0} {1,-5} [{2,2}] {3} : {4}";

        private static StreamWriter writer;
        private static TraceLevel level = TraceLevel.Off;
        #endregion

        #region Static Properties

        /// <summary>
        /// Gets the writer used to output messages.
        /// </summary>
        /// <value>The writer.</value>
        public static StreamWriter Writer
        {
            get 
            {
                if (writer == null)
                    writer = new StreamWriter(Console.OpenStandardOutput());

                writer.AutoFlush = true;

                return writer;
            }
        }

        /// <summary>
        /// Gets or sets the TraceLevel
        /// </summary>
        /// <value>The level of messages to write.</value>
        public static TraceLevel Level
        {
            get { return level; }
            set { level = value; }
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Opens the specified log name for writing trace entries.
        /// </summary>
        /// <param name="logName">Name of the log.</param>
        public static void Open(string logName)
        {
            writer = new StreamWriter(
                new FileStream(logName, FileMode.Append, FileAccess.Write, FileShare.Write));
        }

        /// <summary>
        /// Flushes the output writer.
        /// </summary>
        public static void Flush()
        {
            if (writer != null)
                writer.Flush();
        }

        /// <summary>
        /// Closes the output writer.
        /// </summary>
        public static void Close()
        {
            if (writer != null)
                writer.Close();

            writer = null;
        }

        /// <summary>
        /// Issue a message at the Error level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        public static void Error(string message, params object[] args)
        {
            WriteTrace(TraceLevel.Error, message, args);
        }

        /// <summary>
        /// Issue a message at the Warning level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        public static void Warning(string message, params object[] args)
        {
            WriteTrace(TraceLevel.Warning, message, args);
        }

        /// <summary>
        /// Issue a message at the Info level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        public static void Info(string message, params object[] args)
        {
            WriteTrace(TraceLevel.Info, message, args);
        }

        /// <summary>
        /// Issue a message at the Debug level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        public static void Debug(string message, params object[] args)
        {
            WriteTrace(TraceLevel.Debug, message, args);
        }
        #endregion

        #region Private WriteTrace Method

        private static void WriteTrace(TraceLevel level, string message, params object[] args)
        {
            if (level <= InternalTrace.Level)
            {
                string caller = "UNKNOWN";
#if !NETCF
                string stack = System.Environment.StackTrace;
                int index1 = stack.LastIndexOf(MY_NAME);
                if (index1 >= 0)
                {
                    // Point to stack entry of the caller
                    index1 = stack.IndexOf(Env.NewLine, index1) + Env.NewLine.Length;
                    if (index1 >= 0 && index1 < stack.Length)
                    {
                        // Skip backwards over method name
                        int index2 = stack.IndexOf('(', index1);
                        if (index2 <= 0)
                            index2 = stack.LastIndexOf('.');
                        else
                            index2 = stack.LastIndexOf('.', index2);

                        // Skip over prefix word ('at' in English)
                        while (index1 < stack.Length && char.IsWhiteSpace(stack[index1])) index1++;
                        while (index1 < stack.Length && !char.IsWhiteSpace(stack[index1])) index1++;
                        while (index1 < stack.Length && char.IsWhiteSpace(stack[index1])) index1++;

                        // Skip past namespace, if any
                        index1 = Math.Max(stack.LastIndexOf('.', index2 - 1) + 1, index1);

                        // Set the caller's Type name
                        caller = stack.Substring(index1, index2 - index1);
                    }
#endif
                }


#if CLR_2_0 || CLR_4_0
                int threadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#else
                int threadID =AppDomain.GetCurrentThreadId();
#endif
                if (args != null)
                    message = string.Format(message, args);
                
                Writer.WriteLine(TRACE_FMT,
                    DateTime.Now.ToString(TIME_FMT),
                    level.ToString(),
                    threadID,
                    caller,
                    message);
            }
        }
        #endregion
    }
}
