// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.IO;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Provides internal logging to the NUnit framework
    /// </summary>
    public class Logger : ILogger
    {
        private readonly static string TIME_FMT = "HH:mm:ss.fff";
        private readonly static string TRACE_FMT = "{0} {1,-5} [{2,2}] {3}: {4}";

        private readonly string _name;
        private readonly string _fullname;
        private readonly InternalTraceLevel _maxLevel;
        private readonly TextWriter? _writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="level">The log level.</param>
        /// <param name="writer">The writer where logs are sent.</param>
        public Logger(string name, InternalTraceLevel level, TextWriter? writer)
        {
            _maxLevel = level;
            _writer = writer;
            _fullname = _name = name;
            int index = _fullname.LastIndexOf('.');
            if (index >= 0)
                _name = _fullname.Substring(index + 1);
        }

        #region Error
        /// <summary>
        /// Logs the message at error level.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(string message)
        {
            Log(InternalTraceLevel.Error, message);
        }

        /// <summary>
        /// Logs the message at error level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The message arguments.</param>
        public void Error(string message, params object[] args)
        {
            Log(InternalTraceLevel.Error, message, args);
        }

        //public void Error(string message, Exception ex)
        //{
        //    if (service.Level >= InternalTraceLevel.Error)
        //    {
        //        service.Log(InternalTraceLevel.Error, message, name, ex);
        //    }
        //}
        #endregion

        #region Warning
        /// <summary>
        /// Logs the message at warm level.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warning(string message)
        {
            Log(InternalTraceLevel.Warning, message);
        }

        /// <summary>
        /// Logs the message at warning level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The message arguments.</param>
        public void Warning(string message, params object[] args)
        {
            Log(InternalTraceLevel.Warning, message, args);
        }
        #endregion

        #region Info
        /// <summary>
        /// Logs the message at info level.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(string message)
        {
            Log(InternalTraceLevel.Info, message);
        }

        /// <summary>
        /// Logs the message at info level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The message arguments.</param>
        public void Info(string message, params object[] args)
        {
            Log(InternalTraceLevel.Info, message, args);
        }
        #endregion

        #region Debug
        /// <summary>
        /// Logs the message at debug level.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Debug(string message)
        {
            Log(InternalTraceLevel.Verbose, message);
        }

        /// <summary>
        /// Logs the message at debug level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The message arguments.</param>
        public void Debug(string message, params object[] args)
        {
            Log(InternalTraceLevel.Verbose, message, args);
        }
        #endregion

        #region Helper Methods
        private void Log(InternalTraceLevel level, string message)
        {
            if (_maxLevel >= level)
                WriteLog(level, message);
        }

        private void Log(InternalTraceLevel level, string format, params object[] args)
        {
            if (_maxLevel >= level)
                WriteLog(level, string.Format( format, args ) );
        }

        private void WriteLog(InternalTraceLevel level, string message)
        {
            _writer?.WriteLine(TRACE_FMT,
                DateTime.Now.ToString(TIME_FMT),
                level == InternalTraceLevel.Verbose ? "Debug" : level.ToString(),
                System.Threading.Thread.CurrentThread.ManagedThreadId,
                _name,
                message);
        }

        #endregion
    }
}
