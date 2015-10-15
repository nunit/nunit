// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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
    /// Provides methods to support legacy string comparison methods.
    /// </summary>
    public class StringUtil
    {
        /// <summary>
        /// Compares two strings for equality, ignoring case if requested.
        /// </summary>
        /// <param name="strA">The first string.</param>
        /// <param name="strB">The second string..</param>
        /// <param name="ignoreCase">if set to <c>true</c>, the case of the letters in the strings is ignored.</param>
        /// <returns>Zero if the strings are equivalent, a negative number if strA is sorted first, a positive number if
        /// strB is sorted first</returns>
        public static int Compare(string strA, string strB, bool ignoreCase)
        {
#if NETCF
            return string.Compare(strA, strB, ignoreCase);
#else
            var comparison = ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
            return string.Compare(strA, strB, comparison);
#endif
        }

        /// <summary>
        /// Compares two strings for equality, ignoring case if requested.
        /// </summary>
        /// <param name="strA">The first string.</param>
        /// <param name="strB">The second string..</param>
        /// <param name="ignoreCase">if set to <c>true</c>, the case of the letters in the strings is ignored.</param>
        /// <returns>True if the strings are equivalent, false if not.</returns>
        public static bool StringsEqual(string strA, string strB, bool ignoreCase)
        {
            return Compare(strA, strB, ignoreCase) == 0;
        }
    }
}