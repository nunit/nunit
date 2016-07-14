// ***********************************************************************
// Copyright (c) 2008-2013 Charlie Poole
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

#if !PORTABLE
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
#endif

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
