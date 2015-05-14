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
using System.Xml;

namespace NUnit.Engine
{
    /// <summary>
    /// The TestRun class encapsulates an ongoing test run.
    /// </summary>
    public class TestRun : ITestRun
    {
        private BackgroundWorker _worker;
        private ITestEngineRunner _runner;
        private TestEngineResult _result;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestRun"/> class.
        /// </summary>
        /// <param name="runner">The <see cref="ITestEngineRunner"/> to use for this run.</param>
        public TestRun(ITestEngineRunner runner)
        {
            _runner = runner;

            _worker = new BackgroundWorker();
            _worker.WorkerSupportsCancellation = true;
            _worker.WorkerReportsProgress = true; // ?
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

        /// <summary>
        /// Start asynchronous execution of a test.
        /// </summary>
        /// <param name="listener">The ITestEventListener to use for this run</param>
        /// <param name="filter">The TestFilter to use for this run</param>
        public void Start(ITestEventListener listener, TestFilter filter)
        {
            _worker.DoWork += (s, ea) => 
                _result = _runner.Run(listener, filter);

            _worker.RunWorkerAsync();
        }

        /// <summary>
        /// Stop the current test run. 
        /// </summary>
        /// <param name="force">If true, force the stop by cancelling all threads.</param>
        public void Stop(bool force)
        {
            _worker.CancelAsync();
        }
    }
}
