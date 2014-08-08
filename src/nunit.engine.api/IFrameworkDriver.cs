// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
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
using System.Xml;

namespace NUnit.Engine
{
    /// <summary>
    /// The IFrameworkDriver interface is implemented by a class that
    /// is able to use an external framework to explore or run tests
    /// under the engine.
    /// </summary>
    public interface IFrameworkDriver
    {
        /// <summary>
        /// Loads the tests in an assembly.
        /// </summary>
        /// <returns>An Xml string representing the loaded test</returns>
        string Load();

        /// <summary>
        /// Count the test cases that would be executed.
        /// </summary>
        /// <param name="filter">A TestFilter to use in counting the tests</param>
        /// <returns>The number of test cases counted</returns>
        int CountTestCases(TestFilter filter);

        /// <summary>
        /// Executes the tests in an assembly.
        /// </summary>
        /// <param name="listener">An ITestEventHandler that receives progress notices</param>
        /// <param name="filter">A filter that controls which tests are executed</param>
        /// <returns>An Xml string representing the result</returns>
        string Run(ITestEventListener listener, TestFilter filter);

        /// <summary>
        /// Returns information about the tests in an assembly.
        /// </summary>
        /// <param name="filter">A filter indicating which tests to include</param>
        /// <returns>An Xml string representing the tests</returns>
        string Explore(TestFilter filter);

        /// <summary>
        /// Cancel the ongoing test run. If no  test is running, the call is ignored.
        /// </summary>
        /// <param name="force">If true, cancel any ongoing test threads, otherwise wait for them to complete.</param>
        void StopRun(bool force);
    }
}
