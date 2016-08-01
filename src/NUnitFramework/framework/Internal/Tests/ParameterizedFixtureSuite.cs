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
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// ParameterizedFixtureSuite serves as a container for the set of test 
    /// fixtures created from a given Type using various parameters.
    /// </summary>
    public class ParameterizedFixtureSuite : TestSuite
    {
        private bool _genericFixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterizedFixtureSuite"/> class.
        /// </summary>
        /// <param name="typeInfo">The ITypeInfo for the type that represents the suite.</param>
        public ParameterizedFixtureSuite(ITypeInfo typeInfo) : base(typeInfo.Namespace, typeInfo.GetDisplayName()) 
        {
            _genericFixture = typeInfo.ContainsGenericParameters;
        }

        /// <summary>
        /// Gets a string representing the type of test
        /// </summary>
        /// <value></value>
        public override string TestType
        {
            get
            {
                return _genericFixture
                    ? "GenericFixture"
                    : "ParameterizedFixture";
            }
        }
    }
}
