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
using System.Diagnostics;

namespace NUnit.Framework.Internal
{
    public class Logger
    {
        private string name;
        private string fullname;

        public Logger(string name)
        {
            this.fullname = this.name = name;
            int index = fullname.LastIndexOf('.');
            if (index >= 0)
                this.name = fullname.Substring(index + 1);
        }

        #region Error
        public void Error(string message)
        {
            Log(TraceLevel.Error, message);
        }

        public void Error(string message, params object[] args)
        {
            Log(TraceLevel.Error, message, args);
        }

        public void Error(string message, Exception ex)
        {
            if (InternalTrace.Level >= TraceLevel.Error)
            {
                InternalTrace.Log(TraceLevel.Error, message, name, ex);
            }
        }
        #endregion

        #region Warning
        public void Warning(string message)
        {
            Log(TraceLevel.Warning, message);
        }

        public void Warning(string message, params object[] args)
        {
            Log(TraceLevel.Warning, message, args);
        }
        #endregion

        #region Info
        public void Info(string message)
        {
            Log(TraceLevel.Info, message);
        }

        public void Info(string message, params object[] args)
        {
            Log(TraceLevel.Info, message, args);
        }
        #endregion

        #region Debug
        public void Debug(string message)
        {
            Log(TraceLevel.Verbose, message);
        }

        public void Debug(string message, params object[] args)
        {
            Log(TraceLevel.Verbose, message, args);
        }
        #endregion

        #region Helper Methods
        public void Log(TraceLevel level, string message)
        {
            if (InternalTrace.Level >= level)
                InternalTrace.Log(level, message, name);
        }

        private void Log(TraceLevel level, string format, params object[] args)
        {
            if (InternalTrace.Level >= level)
                Log(level, string.Format( format, args ) );
        }
        #endregion
    }
}
