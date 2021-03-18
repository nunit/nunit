// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.IO;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// InternalTrace provides facilities for tracing the execution
    /// of the NUnit framework. Tests and classes under test may make use
    /// of Console writes, System.Diagnostics.Trace or various loggers and
    /// NUnit itself traps and processes each of them. For that reason, a
    /// separate internal trace is needed.
    ///
    /// Note:
    /// InternalTrace uses a global lock to allow multiple threads to write
    /// trace messages. This can easily make it a bottleneck so it must be
    /// used sparingly. Keep the trace Level as low as possible and only
    /// insert InternalTrace writes where they are needed.
    /// TODO: add some buffering and a separate writer thread as an option.
    /// TODO: figure out a way to turn on trace in specific classes only.
    /// </summary>
    public static class InternalTrace
    {
        private static InternalTraceLevel traceLevel;
        private static InternalTraceWriter traceWriter;

        /// <summary>
        /// Gets a flag indicating whether the InternalTrace is initialized
        /// </summary>
        public static bool Initialized { get; private set; }

        /// <summary>
        /// Initialize the internal trace facility using the name of the log
        /// to be written to and the trace level.
        /// </summary>
        /// <param name="logName">The log name</param>
        /// <param name="level">The trace level</param>
        public static void Initialize(string logName, InternalTraceLevel level)
        {
            if (!Initialized)
            {
                traceLevel = level;

                if (traceWriter == null && traceLevel > InternalTraceLevel.Off)
                {
                    traceWriter = new InternalTraceWriter(logName);
                    traceWriter.WriteLine("InternalTrace: Initializing at level {0}", traceLevel);
                }

                Initialized = true;
            }
            else
                traceWriter.WriteLine("InternalTrace: Ignoring attempted re-initialization at level {0}", level);
        }

        /// <summary>
        /// Initialize the internal trace using a provided TextWriter and level
        /// </summary>
        /// <param name="writer">A TextWriter</param>
        /// <param name="level">The InternalTraceLevel</param>
        public static void Initialize(TextWriter writer, InternalTraceLevel level)
        {
            if (!Initialized)
            {
                traceLevel = level;

                if (traceWriter == null && traceLevel > InternalTraceLevel.Off)
                {
                    traceWriter = new InternalTraceWriter(writer);
                    traceWriter.WriteLine("InternalTrace: Initializing at level " + traceLevel.ToString());
                }

                Initialized = true;
            }
        }

        /// <summary>
        /// Get a named Logger
        /// </summary>
        /// <returns></returns>
        public static Logger GetLogger(string name)
        {
            return new Logger(name, traceLevel, traceWriter);
        }

        /// <summary>
        /// Get a logger named for a particular Type.
        /// </summary>
        public static Logger GetLogger(Type type)
        {
            return GetLogger(type.FullName);
        }
    }
}
