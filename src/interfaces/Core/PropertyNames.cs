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

namespace NUnit.Core
{
    /// <summary>
    /// The PropertyNames struct lists common property names, which are
    /// accessed by reflection in the NUnit core. This provides a modicum 
    /// of type safety as opposed to using the strings directly.
    /// </summary>
    public struct PropertyNames
    {
        /// <summary>Exception Type expected from a test</summary>
        public static readonly string ExpectedException = "ExpectedException";
        /// <summary>FullName of the Exception Type expected from a test</summary>
        public static readonly string ExpectedExceptionName = "ExpectedExceptionName";
        /// <summary>ExpectedException Message</summary>
        public static readonly string ExpectedMessage = "ExpectedMessage";
        /// <summary>ExpectedException MatchType</summary>
        public static readonly string MatchType = "MatchType";
        /// <summary>Expected return result from test</summary>
        public static readonly string Result = "Result";
        /// <summary>Description of the test</summary>
        public static readonly string Description = "Description";
        /// <summary>Alternate test name</summary>
        public static readonly string TestName = "TestName";
        /// <summary>Arguments for the test</summary>
        public static readonly string Arguments = "Arguments";
        /// <summary>Indicates test case is ignored</summary>
        public static readonly string Ignored = "Ignored";
        /// <summary>The reason a test case is ignored</summary>
        public static readonly string IgnoreReason = "IgnoreReason";
        /// <summary>Properties of the test</summary>
        public static readonly string Properties = "Properties";
        /// <summary>Categories of the test</summary>
        public static readonly string Categories = "Categories";
    }
}
