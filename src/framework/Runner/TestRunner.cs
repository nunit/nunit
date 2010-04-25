// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using System.Reflection;
using System.Collections;
using NUnit.Framework.Api;
using NUnit.Framework.Internal;

namespace NUnitLite.Runner
{
    //[Serializable]
    class TestRunnerException : Exception
    {
        public TestRunnerException(string message) : base(message) { }
    }

    /// <summary>
    /// The TestRunner class knows how to execute tests
    /// </summary>
    public class TestRunner : ITestListener
    {
        private NUnit.ObjectList listeners = new NUnit.ObjectList();

        /// <summary>
        /// Runs the specified test.
        /// </summary>
        /// <param name="test">The test.</param>
        public ITestResult Run(Test test)
        {
            return test.Run(this);
        }

        /// <summary>
        /// Adds a listener to the TestRunner.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public void AddListener(ITestListener listener)
        {
            listeners.Add(listener);
        }

        /// <summary>
        /// Removes a listener from the TestRunner.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public void RemoveListener(ITestListener listener)
        {
            listeners.Remove(listener);
        }

        /// <summary>
        /// Forwards the TestStarted event to all listeners.
        /// </summary>
        /// <param name="test">The test that just started.</param>
        public void TestStarted(ITest test)
        {
            foreach (ITestListener listener in listeners)
                listener.TestStarted(test);
        }

        /// <summary>
        /// Forwards the TestFinished event to all listeners.
        /// </summary>
        /// <param name="result">The result of the test that just finished.</param>
        public void TestFinished(ITestResult result)
        {
            foreach (ITestListener listener in listeners)
                listener.TestFinished(result);
        }

        /// <summary>
        /// Forwards the TestOutput event to all listeners.
        /// </summary>
        /// <param name="testOutput">A TestOutput instance.</param>
        public void TestOutput(TestOutput testOutput)
        {
            foreach (ITestListener listener in listeners)
                listener.TestOutput(testOutput);
        }
    }
}
