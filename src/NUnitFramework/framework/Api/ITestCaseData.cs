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
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OFn
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Collections;

namespace NUnit.Framework.Api
{
    /// <summary>
    /// The ITestCaseData interface is implemented by a class
    /// that is able to return complete testcases for use by
    /// a parameterized test method.
    /// 
    /// NOTE: This interface is used in both the framework
    /// and the core, even though that results in two different
    /// types. However, sharing the source code guarantees that
    /// the various implementations will be compatible and that
    /// the core is able to reflect successfully over the
    /// framework implementations of ITestCaseData.
    /// </summary>
    public interface ITestCaseData
    {
        /// <summary>
        /// Gets the argument list to be provided to the test
        /// </summary>
        object[] Arguments { get; }

#if !NUNITLITE
        /// <summary>
        /// Gets the expected result
        /// </summary>
        object Result { get; }

        /// <summary>
        ///  Gets the expected exception Type
        /// </summary>
        Type ExpectedException { get; }

        /// <summary>
        /// Gets the FullName of the expected exception
        /// </summary>
        string ExpectedExceptionName { get; }

        /// <summary>
        /// Gets  the expected message of the expected exception
        /// </summary>
        string ExpectedMessage { get; }

        /// <summary>
        ///  Gets the type of match to be performed on the expected message
        /// </summary>
        MessageMatch MatchType { get; }

        /// <summary>
        /// Gets the name to be used for the test
        /// </summary>
        string TestName { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ITestCaseData"/> is ignored.
        /// </summary>
        /// <value><c>true</c> if ignored; otherwise, <c>false</c>.</value>
        bool Ignored { get; }

        /// <summary>
        /// Gets the property dictionary for the test case
        /// </summary>
        IPropertyBag Properties { get; }

#endif
    }
}
