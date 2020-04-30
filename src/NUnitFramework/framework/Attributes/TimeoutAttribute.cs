// ***********************************************************************
// Copyright (c) 2008-2018 Charlie Poole, Rob Prouse
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
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Applies a timeout in milliseconds to a test. 
    /// When applied to a method, the test is cancelled if the timeout is exceeded. 
    /// When applied to a class or assembly, the default timeout is set for all contained test methods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false, Inherited=false)]
    public class TimeoutAttribute : PropertyAttribute, IApplyToContext
    {
        private readonly int _timeout;

        /// <summary>
        /// Construct a TimeoutAttribute given a time in milliseconds
        /// </summary>
        /// <param name="timeout">The timeout value in milliseconds</param>
        public TimeoutAttribute(int timeout)
            : base(timeout)
        {
            _timeout = timeout;
        }

        #region IApplyToContext

        void IApplyToContext.ApplyToContext(TestExecutionContext context)
        {
            context.TestCaseTimeout = _timeout;
        }

        #endregion
    }
}
