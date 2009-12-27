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
using System.Reflection;

namespace NUnit.Framework.Api
{
    /// <summary>
    /// The ITestAssemblyRunner interface is implemented by classes
    /// that are able to execute a suite of tests loaded
    /// from an assembly.
    /// </summary>
    public interface ITestAssemblyRunner
    {
        #region Methods

        /// <summary>
        /// Loads the tests found in an Assembly
        /// </summary>
        /// <param name="assemblyName">File name of the assembly to load</param>
        /// <returns>True if the load was successful</returns>
        bool Load(string assemblyName);

        /// <summary>
        /// Count Test Cases using a filter
        /// </summary>
        /// <param name="filter">The filter to apply</param>
        /// <returns>The number of test cases found</returns>
        int CountTestCases(NUnit.Core.TestFilter filter);

        /// <summary>
        /// Run selected tests and return a test result. The test is run synchronously,
        /// and the listener interface is notified as it progresses.
        /// </summary>
        /// <param name="listener">Interface to receive EventListener notifications.</param>
        /// <param name="filter">The filter to apply when running the tests</param>
        NUnit.Core.TestResult Run(NUnit.Core.ITestListener listener, NUnit.Core.TestFilter filter);

        #endregion
    }
}
