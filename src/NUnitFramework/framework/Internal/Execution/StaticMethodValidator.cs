// ***********************************************************************
// Copyright (c) 2012-2020 Charlie Poole, Rob Prouse
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
using System.Reflection;

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// Checks whether the method to execute is static.
    /// </summary>
    public class StaticMethodValidator : IMethodValidator
    {
        private readonly string _failMessage;

        /// <summary>
        /// Construct a StaticMethodValidator.
        /// </summary>
        /// <param name="failMessage">The error message to output in case the validation fails.</param>
        public StaticMethodValidator(string failMessage)
        {
            Guard.ArgumentNotNullOrEmpty(failMessage, nameof(failMessage));

            _failMessage = failMessage;
        }

        /// <summary>
        /// Determines whether a method to execute is static and throws an InvalidOperationException otherwise.
        /// </summary>
        /// <param name="method">The method to validate.</param>
        public void Validate(MethodInfo method)
        {
            if (!method.IsStatic)
                throw new InvalidOperationException(_failMessage);
        }
    }
}