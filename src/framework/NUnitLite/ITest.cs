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

namespace NUnitLite
{
    /// <summary>
    /// Interface representing a test
    /// </summary>
    public interface ITest
    {
        /// <summary>
        /// Gets the name of the test.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Gets the full name of the test.
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Gets or sets the run state of the test.
        /// </summary>
        /// <value>The state of the run.</value>
        NUnit.Framework.Api.RunState RunState { get; set; }

        /// <summary>
        /// Gets or sets the ignore reason.
        /// </summary>
        string IgnoreReason { get; set; }

        /// <summary>
        /// Gets the test case count.
        /// </summary>
        int TestCaseCount { get; }

        /// <summary>
        /// Gets the properties of the test.
        /// </summary>
        System.Collections.IDictionary Properties { get; }

        /// <summary>
        /// Runs this test.
        /// </summary>
        /// <returns>A TestResult</returns>
        TestResult Run();

        /// <summary>
        /// Runs this test
        /// </summary>
        /// <param name="listener">A TestListener to handle test events</param>
        /// <returns>A TestResult</returns>
        TestResult Run(TestListener listener);
    }
}
