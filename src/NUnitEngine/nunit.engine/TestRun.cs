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
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Xml;

namespace NUnit.Engine
{
    /// <summary>
    /// The TestRun class encapsulates an ongoing test run.
    /// </summary>
    public class TestRun : ITestRun
    {
        private volatile TestEngineResult _result;
        private readonly ManualResetEvent _waitHandle;
        private readonly ITestEngineRunner _runner;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestRun"/> class.
        /// </summary>
        public TestRun(ITestEngineRunner runner)
        {
            _runner = runner;
            _waitHandle = new ManualResetEvent(initialState: false);
        }

        /// <summary>
        /// Get the result of this run.
        /// </summary>
        /// <exception cref="InvalidOperationException">Cannot retrieve Result from an incomplete or cancelled TestRun.</exception>
        public XmlNode Result
        {
            get
            {
                if (_result == null)
                    throw new InvalidOperationException("Cannot retrieve Result from an incomplete or cancelled TestRun.");

                return _result.Xml;
            }
        }
        
        public void SetResult(TestEngineResult result)
        {
            Guard.ArgumentNotNull(result, "result");
            Guard.OperationValid(_result == null, "Cannot set the Result of an TestRun more than once");
            
            _result = result;
            _waitHandle.Set();
        }

        /// <summary>
        /// Stop the current test run, specifying whether to force cancellation. 
        /// If no test is running, the method returns without error.
        /// </summary>
        /// <param name="force">If true, force the stop by cancelling all threads.</param>
        /// <remarks>
        /// Note that cancelling the threads is intrinsically unsafe and is only
        /// provided on the assumption that tests do not impact production data.
        /// </remarks>
        public void Stop(bool force)
        {
            _runner.StopRun(force);
        }

        /// <summary>
        /// Blocks the current thread until the current test run completes
        /// or the timeout is reached
        /// </summary>
        /// <param name="timeout">A <see cref="T:System.TimeSpan"/> that represents the number of milliseconds to wait, or a <see cref="T:System.TimeSpan"/> that represents -1 milliseconds to wait indefinitely. </param>
        /// <returns>True if the run completed</returns>
        public bool Wait(TimeSpan timeout)
        {
            return _waitHandle.WaitOne(timeout);
        }

        /// <summary>
        /// True if the test run has completed
        /// </summary>
        public bool IsComplete { get { return _result != null; } }
    }
}
