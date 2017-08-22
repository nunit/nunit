// ***********************************************************************
// Copyright (c) 2011 Charlie Poole, Rob Prouse
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
using System.Collections.Generic;
using System.IO;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnitLite
{
    /// <summary>
    /// TestCaseOutputWriter lists test cases
    /// </summary>
    public class TestCaseOutputWriter : OutputWriter
    {
        /// <summary>
        /// Write a list of test cases to a file
        /// </summary>
        /// <param name="test"></param>
        /// <param name="writer"></param>
        public override void WriteTestFile(ITest test, TextWriter writer)
        {
            if (test.IsSuite)
                foreach (var child in test.Tests)
                    WriteTestFile(child, writer);
            else
                writer.WriteLine(test.FullName);
        }

        /// <summary>
        /// Write a list of test cases to a file
        /// </summary>
        /// <param name="result"></param>
        /// <param name="writer"></param>
        /// <param name="runSettings"></param>
        /// <param name="filter"></param>
        public override void WriteResultFile(ITestResult result, TextWriter writer, IDictionary<string, object> runSettings, TestFilter filter)
        {
            WriteTestFile(result.Test, writer);
        }
    }
}
