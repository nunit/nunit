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

using System.Collections;

namespace NUnit.Framework.Api
{
	/// <summary>
	/// Common interface supported by all representations
	/// of a test. Only includes informational fields.
	/// The Run method is specifically excluded to allow
	/// for data-only representations of a test.
	/// </summary>
	public interface ITest
    {
        #region Properties

        /// <summary>
        /// Gets or sets the id of the test
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// Gets or sets the name of the test
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or sets the fully qualified name of the test
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Indicates whether the test can be run using
        /// the RunState enum.
        /// </summary>
		RunState RunState { get; set; }

        /// <summary>
        ///  Description for the test;
        /// </summary>
        string Description { get; set; }

		/// <summary>
		/// Reason for not running the test, if applicable
		/// </summary>
		string IgnoreReason { get; set; }

        /// <summary>
        /// Count of the test cases ( 1 if this is a test case )
        /// </summary>
		int TestCaseCount { get; }

		/// <summary>
		/// Gets the properties of the test
		/// </summary>
		IDictionary Properties { get; }

        /// <summary>
        /// Gets the categories of the test
        /// </summary>
        IList Categories { get; }

        /// <summary>
        /// True if this is a test case
        /// </summary>
        bool IsTestCase { get; }

        #endregion
    }
}

