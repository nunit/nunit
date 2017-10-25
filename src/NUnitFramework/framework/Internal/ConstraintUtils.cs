// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
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
using System.Globalization;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Provides methods to support consistent checking for constaints methods.
    /// </summary>
    internal static class ConstraintUtils
    {
        /// <summary>
        /// Require that the provided object is actually of the type required,
        /// and that it is not null.
        /// </summary>
        /// <param name="actual">The object to verify.</param>
        /// <param name="paramName">Name of the parameter as passed into the checking method.</param>
        /// <typeparam name="T">The type to require.</typeparam>  
        /// <returns>A properly typed object, or throws. Never null.</returns>
        public static T RequireActual<T>(object actual, string paramName)
        {
            if (actual is T) return (T)actual;

            var actualDisplay = actual == null ? "null" : actual.GetType().Name;
            throw new ArgumentException($"Expected: {typeof(T).Name} But was: {actualDisplay}", paramName);
        }
    }
}
