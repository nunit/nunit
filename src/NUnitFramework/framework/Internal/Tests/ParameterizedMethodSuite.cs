// ***********************************************************************
// Copyright (c) 2008 Charlie Poole, Rob Prouse
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

using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// ParameterizedMethodSuite holds a collection of individual
    /// TestMethods with their arguments applied.
    /// </summary>
    public class ParameterizedMethodSuite : TestSuite
    {
        private readonly bool _isTheory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterizedMethodSuite"/> class.
        /// </summary>
        public ParameterizedMethodSuite(IMethodInfo method)
            : base(method.TypeInfo.FullName, method.Name)
        {
            Method = method;
            _isTheory = method.IsDefined<TheoryAttribute>(true);
            this.MaintainTestOrder = true;
        }

        /// <summary>
        /// Creates a copy of the given suite with only the descendants that pass the specified filter.
        /// </summary>
        /// <param name="suite">The <see cref="ParameterizedMethodSuite"/> to copy.</param>
        /// <param name="filter">Determines which descendants are copied.</param>
        public ParameterizedMethodSuite(ParameterizedMethodSuite suite, ITestFilter filter)
            : base(suite, filter)
        {
        }

        /// <summary>
        /// Gets a string representing the type of test
        /// </summary>
        public override string TestType
        {
            get
            {
                if (_isTheory)
                    return "Theory";

                if (this.Method.ContainsGenericParameters)
                    return "GenericMethod";

                return "ParameterizedMethod";
            }
        }

        /// <summary>
        /// Creates a filtered copy of the test suite.
        /// </summary>
        /// <param name="filter">Determines which descendants are copied.</param>
        public override TestSuite Copy(ITestFilter filter)
        {
            return new ParameterizedMethodSuite(this, filter);
        }
    }
}
