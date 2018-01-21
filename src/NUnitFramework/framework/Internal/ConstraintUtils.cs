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

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Provides methods to support consistent checking in constraints.
    /// </summary>
    internal static class ConstraintUtils
    {
        /// <summary>
        /// Requires that the provided object is actually of the type required.
        /// </summary>
        /// <param name="actual">The object to verify.</param>
        /// <param name="paramName">Name of the parameter as passed into the checking method.</param>
        /// <param name="allowNull">
        /// If <see langword="true"/> and <typeparamref name="T"/> can be null, returns null rather than throwing when <paramref name="actual"/> is null.
        /// If <typeparamref name="T"/> cannot be null, this parameter is ignored.</param>
        /// <typeparam name="T">The type to require.</typeparam>
        public static T RequireActual<T>(object actual, string paramName, bool allowNull = false)
        {
            T result;
            if (TypeHelper.TryCast(actual, out result) && (allowNull || result != null))
            {
                return result;
            }

            var actualDisplay = actual == null ? "null" : actual.GetType().Name;
            throw new ArgumentException($"Expected: {typeof(T).Name} But was: {actualDisplay}", paramName);
        }
    }
}
