// ***********************************************************************
// Copyright (c) 2012 Charlie Poole
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

namespace NUnit.Engine.Internal
{
    public class Logger : ILogger
    {
        private readonly static string TIME_FMT = "HH:mm:ss.fff";

        private string name;
        private string fullname;
        private InternalTraceLevel level;
        private TextWriter writer;

        public Logger(string name, InternalTraceLevel level, TextWriter writer)
        {
            this.level = level;
            this.writer = writer;
            this.fullname = this.name = name;
            int index = fullname.LastIndexOf('.');
            if (index >= 0)
                this.name = fullname.Substring(index + 1);
        }

        #region Error
        public void Error(string message)
        {
            Log(InternalTraceLevel.Error, message);
        }

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
        public void Warning(string message)
        {
            Log(InternalTraceLevel.Warning, message);
        }

        public void Warning(string message, params object[] args)
        {
            Log(InternalTraceLevel.Warning, message, args);
        }
        #endregion

        #region Info
        public void Info(string message)
        {
            Log(InternalTraceLevel.Info, message);
        }

        public void Info(string message, params object[] args)
        {
            Log(InternalTraceLevel.Info, message, args);
        }
        #endregion

        #region Debug
        public void Debug(string message)
        {
            Log(InternalTraceLevel.Verbose, message);
        }

        public void Debug(string message, params object[] args)
        {
            Log(InternalTraceLevel.Verbose, message, args);
        }
        #endregion

        #region Helper Methods
        private void Log(InternalTraceLevel l, string message)
        {
            if (writer != null && this.level >= l)
            {
                writer.WriteLine("{0} {1,-5} [{2,2}] {3}: {4}",
                    DateTime.Now.ToString(TIME_FMT),
                    level == InternalTraceLevel.Verbose ? "Debug" : level.ToString(),
                    System.Threading.Thread.CurrentThread.ManagedThreadId,
                    name,
                    message);
            }
        }

        private void Log(InternalTraceLevel l, string format, params object[] args)
        {
            if (this.level >= l)
                Log(level, string.Format( format, args ) );
        }
        #endregion
    }
}
