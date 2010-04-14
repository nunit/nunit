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

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// ParamterizedFixtureResult represents the result of running
    /// a set of parameterized fixtures.
    /// </summary>
    public class ParameterizedFixtureResult : TestSuiteResult
    {
        private ParameterizedFixtureSuite suite;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterizedFixtureResult"/> class.
        /// </summary>
        /// <param name="suite">The suite.</param>
        public ParameterizedFixtureResult(ParameterizedFixtureSuite suite) : base(suite) 
        {
            this.suite = suite;
        }

        /// <summary>
        /// Adds the top level test-suite element for this result.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <returns>The new top level element node.</returns>
        protected override System.Xml.XmlNode AddTopLevelElement(System.Xml.XmlNode parentNode)
        {
#if CLR_2_0
            if (suite.ParameterizedType.ContainsGenericParameters)
                return XmlHelper.AddElement(parentNode, "generic-fixture");
#endif
            return XmlHelper.AddElement(parentNode, "parameterized-fixture");
        }
    }
}
