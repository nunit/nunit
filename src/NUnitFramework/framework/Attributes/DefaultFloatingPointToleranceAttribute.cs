// ***********************************************************************
// Copyright (c) 2016 Charlie Poole, Rob Prouse
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

#nullable enable

using System;
using NUnit.Framework.Constraints;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Sets the tolerance used by default when checking the equality of floating point values
    /// within the test assembly, fixture or method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class DefaultFloatingPointToleranceAttribute : NUnitAttribute, IApplyToContext
    {
        private readonly Tolerance _tolerance;

        /// <summary>
        /// Construct specifying an amount
        /// </summary>
        /// <param name="amount"></param>
        public DefaultFloatingPointToleranceAttribute(double amount)
        {
            _tolerance = new Tolerance(amount);
        }

        #region IApplyToContext Members

        /// <summary>
        /// Apply changes to the TestExecutionContext
        /// </summary>
        /// <param name="context">The TestExecutionContext</param>
        public void ApplyToContext(TestExecutionContext context)
        {
            context.DefaultFloatingPointTolerance = _tolerance;
        }

        #endregion
    }
}
