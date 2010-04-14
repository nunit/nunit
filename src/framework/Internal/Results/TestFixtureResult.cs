// ***********************************************************************
// Copyright (c) 2010 Charlie Poole
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
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// TestFixtureResult represents the result of running a TestFixture.
    /// </summary>
    public class TestFixtureResult : TestSuiteResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestFixtureResult"/> class.
        /// </summary>
        /// <param name="test">The test to be used</param>
        public TestFixtureResult(TestFixture test) : base(test) { }

        /// <summary>
        /// Adds the test-fixture top level element.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <returns>The newly created element node.</returns>
        protected override System.Xml.XmlNode AddTopLevelElement(System.Xml.XmlNode parentNode)
        {
            return XmlHelper.AddElement(parentNode, "test-fixture");
        }
    }
}
