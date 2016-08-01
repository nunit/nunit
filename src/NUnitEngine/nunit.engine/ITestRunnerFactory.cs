// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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

namespace NUnit.Engine
{
    /// <summary>
    /// A Test Runner factory can supply a suitable test runner for a given package
    /// </summary>
    public interface ITestRunnerFactory
    {
        /// <summary>
        /// Return a suitable runner for the package provided as an argument
        /// </summary>
        /// <param name="package">The test package to be loaded by the runner</param>
        /// <returns>A TestRunner</returns>
        ITestEngineRunner MakeTestRunner(TestPackage package);

        /// <summary>
        /// Return true if the provided runner is suitable for reuse in loading
        /// the test package provided. Otherwise, return false. Runners that
        /// cannot be reused must always return false.
        /// </summary>
        /// <param name="runner">An ITestRunner to possibly be used.</param>
        /// <param name="package">The TestPackage to be loaded.</param>
        /// <returns>True if the runner may be reused for the provided package.</returns>
        bool CanReuse(ITestEngineRunner runner, TestPackage package);
    }
}
