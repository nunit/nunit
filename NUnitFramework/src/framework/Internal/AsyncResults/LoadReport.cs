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

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// LoadReport is an AsyncResult returned after an assembly has
    /// been successfully loaded.
    /// </summary>
    [Serializable]
    public class LoadReport : FinalResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadReport"/> class
        /// for a particular assembly.
        /// </summary>
        /// <param name="assemblyFilename">The name of the assembly that was loaded.</param>
        /// <param name="testCaseCount">The number of test cases found in the assembly.</param>
        public LoadReport(string assemblyFilename, int testCaseCount)
        {
            this.xmlResult = string.Format("<loaded assembly=\"{0}\" testcases=\"{1}\"/>", assemblyFilename, testCaseCount);
        }
    }
}