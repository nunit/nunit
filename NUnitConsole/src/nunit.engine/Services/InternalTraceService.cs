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
using System.Collections.Generic;
using System.Text;
using NUnit.Engine.Internal;

namespace NUnit.Engine.Services
{
    public interface ITraceService : IService
    {
        Logger GetLogger(string name);
        void Log(InternalTraceLevel level, string message, string category);
        void Log(InternalTraceLevel level, string message, string category, Exception ex);
    }

    public class InternalTraceService : ITraceService
    {
        private readonly static string TIME_FMT = "HH:mm:ss.fff";

        private static bool initialized;

        private InternalTraceWriter writer;

        public InternalTraceLevel Level;

        public InternalTraceService(InternalTraceLevel level)
        {
            this.Level = level;
        }

        public void Initialize(string logName, InternalTraceLevel level)
        {
            if (!initialized)
            {
                Level = level;

                if (writer == null && Level > InternalTraceLevel.Off)
                {
                    writer = new InternalTraceWriter(logName);
                    writer.WriteLine("InternalTrace: Initializing at level " + Level.ToString());
                }

                initialized = true;
            }
        }

        public Logger GetLogger(string name)
        {
            return new Logger(this, name);
        }

        public Logger GetLogger(Type type)
        {
            return new Logger(this, type.FullName);
        }

        public void Log(InternalTraceLevel level, string message, string category)
        {
            Log(level, message, category, null);
        }

        public void Log(InternalTraceLevel level, string message, string category, Exception ex)
        {
            if (writer != null)
            {
                writer.WriteLine("{0} {1,-5} [{2,2}] {3}: {4}",
                DateTime.Now.ToString(TIME_FMT),
                level == InternalTraceLevel.Verbose ? "Debug" : level.ToString(),
                System.Threading.Thread.CurrentThread.ManagedThreadId,
                category,
                message);

                if (ex != null)
                    writer.WriteLine(ex.ToString());
            }
        }

        #region IService Members

        private ServiceContext services;
        public ServiceContext ServiceContext
        {
            get { return services; }
            set { services = value; }
        }

        public void InitializeService()
        {
        }

        public void UnloadService()
        {
        }

        #endregion
    }
}
