// *****************************************************
// Copyright 2007, Charlie Poole
//
// Licensed under the Open Software License version 3.0
// *****************************************************

using System;
using System.IO;
using System.Reflection;
using System.Collections;
using NUnit.Framework;

namespace NUnitLite.Runner
{
    //[Serializable]
    class TestRunnerException : Exception
    {
        public TestRunnerException(string message) : base(message) { }
    }

    public class TestRunner : TestListener
    {
        private IList listeners = new ArrayList();

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
            return test.Run(this);
        }

        /// <summary>
        /// Adds a listener to the TestRunner.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public void AddListener(TestListener listener)
        {
            listeners.Add(listener);
        }

        /// <summary>
        /// Removes a listener from the TestRunner.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public void RemoveListener(TestListener listener)
        {
            listeners.Remove(listener);
        }

        /// <summary>
        /// Forwards the TestStarted event to all listeners.
        /// </summary>
        /// <param name="test">The test that just started.</param>
        public void TestStarted(ITest test)
        {
            foreach (TestListener listener in listeners)
                listener.TestStarted(test);
        }

        /// <summary>
        /// Forwards the TestFinished event to all listeners.
        /// </summary>
        /// <param name="result">The result of the test that just finished.</param>
        public void TestFinished(TestResult result)
        {
            foreach (TestListener listener in listeners)
                listener.TestFinished(result);
        }
    }
}
