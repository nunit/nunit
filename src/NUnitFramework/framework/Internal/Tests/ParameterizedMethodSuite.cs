// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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
using NUnit.Framework.Internal.Commands;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// ParameterizedMethodSuite holds a collection of individual
    /// TestMethods with their arguments applied.
    /// </summary>
    public class ParameterizedMethodSuite : TestSuite
    {
        private bool _isTheory;

        /// <summary>
        /// Construct from a MethodInfo
        /// </summary>
        /// <param name="method"></param>
        public ParameterizedMethodSuite(IMethodInfo method)
            : base(method.TypeInfo.FullName, method.Name)
        {
            Method = method;
#if PORTABLE
            _isTheory = false;
#else
            _isTheory = method.IsDefined<TheoryAttribute>(true);
#endif
            this.MaintainTestOrder = true;
        }

        /// <summary>
        /// Gets a string representing the type of test
        /// </summary>
        /// <value></value>
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
    }
}
