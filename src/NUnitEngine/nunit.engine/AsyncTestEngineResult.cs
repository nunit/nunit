// ***********************************************************************
// Copyright (c) 2011-2015 Charlie Poole
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

namespace NUnit.Engine
{
    /// <summary>
    /// Encapsulates an ongoing <see cref="TestEngineResult"/> 
    /// </summary>
    public class AsyncTestEngineResult
    {
        private readonly ManualResetEvent _waitHandle;
        private volatile TestEngineResult _result;

        public AsyncTestEngineResult()
        {
            _waitHandle = new ManualResetEvent(initialState: false);
        }

        public TestEngineResult Result 
        {
            get
            {
                if (!IsComplete)
                {
                    throw new InvalidOperationException("Cannot retrieve Result from an incomplete or cancelled AsyncTestEngineResult.");
                }

                return _result;
            }
        }

        public void SetResult(TestEngineResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (_result != null)
            {
                throw new InvalidOperationException("Cannot set the Result of an AsyncTestEngineResult more than once");
            }

            _result = result;
            _waitHandle.Set();
        }

        /// <summary>
        /// Blocks the current thread until the current test run completes
        /// or the timeout is reached.
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
