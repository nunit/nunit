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
        /// Runs all tests in the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly for which tests are to be run.</param>
        /// <returns>TestResult representing the result of the run</returns>
        public virtual TestResult Run(Assembly assembly)
        {
            return Run( TestLoader.Load(assembly) );
        }

        /// <summary>
        /// Runs a set of tests specified by name
        /// </summary>
        /// <param name="assembly">The assembly containing the tests</param>
        /// <param name="tests">Array of test names to be run</param>
        /// <returns>TestResult representing the result of the run</returns>
        public virtual TestResult Run(Assembly assembly, string[] tests)
        {
            return Run( TestLoader.Load( assembly, tests ) );
        }


        /// <summary>
        /// Runs the specified test.
        /// </summary>
        /// <param name="test">The test.</param>
        public virtual TestResult Run(ITest test)
        {
            // TODO: Temporary
            if (test is TestCase)
                return ((TestCase)test).Run(this);
            else
                return ((TestSuite)test).Run(this);
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
        public void TestFinished(TestResult result)
        {
            foreach (ITestListener listener in listeners)
                listener.TestFinished(result);
        }

        /// <summary>
        /// Forwards the TestOutput event to all listeners.
        /// </summary>
        /// <param name="result">The result of the test that just finished.</param>
        public void TestOutput(TestOutput testOutput)
        {
            foreach (ITestListener listener in listeners)
                listener.TestOutput(testOutput);
        }
    }
}
