// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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
using System.Threading;
using System.Xml;

namespace NUnit.Engine
{
    /// <summary>
    /// The TestRun class encapsulates an ongoing test run.
    /// </summary>
    [Serializable]
    public class AsyncTestEngineResult : ITestRun
    {
        private volatile TestEngineResult _result;
        private readonly ManualResetEvent _waitHandle = new ManualResetEvent(false);

        /// <summary>
        /// Get the result of this run.
        /// </summary>
        /// <exception cref="InvalidOperationException">Cannot retrieve Result from an incomplete or cancelled TestRun.</exception>
        public TestEngineResult EngineResult
        {
            get
            {
                Guard.OperationValid(_result != null, "Cannot retrieve Result from an incomplete or cancelled TestRun.");

                return _result;
            }
        }

        public EventWaitHandle WaitHandle
        {
            get { return _waitHandle; }
        }
        
        public void SetResult(TestEngineResult result)
        {
            Guard.ArgumentNotNull(result, "result");
            Guard.OperationValid(_result == null, "Cannot set the Result of an TestRun more than once");
            
            _result = result;
            _waitHandle.Set();
        }

        /// <summary>
        /// Blocks the current thread until the current test run completes
        /// or the timeout is reached
        /// </summary>
        /// <param name="timeout">A <see cref="T:System.Int32"/> that represents the number of milliseconds to wait, or -1 milliseconds to wait indefinitely. </param>
        /// <returns>True if the run completed</returns>
        public bool Wait(int timeout)
        {
            return _waitHandle.WaitOne(timeout);
        }

        /// <summary>
        /// True if the test run has completed
        /// </summary>
        public bool IsComplete { get { return _result != null; } }

        #region ITestRun Members

        XmlNode ITestRun.Result
        {
            get { return EngineResult.Xml; }
        }

        bool ITestRun.Wait(int timeout)
        {
            return Wait(timeout);
        }

        #endregion
    }
}
