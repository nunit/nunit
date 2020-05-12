// ***********************************************************************
// Copyright (c) 2020 Charlie Poole, Rob Prouse
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

namespace NUnit.Framework.Internal.Extensions
{
    /// <summary>
    /// Extensions to <see cref="IPropertyBag"/>.
    /// </summary>
    internal static class IPropertyBagDataExtensions
    {
        /// <summary>
        /// Adds the skip reason to tests that are ignored until a specific date.
        /// </summary>
        /// <param name="properties">The test properties to add the skip reason to</param>
        /// <param name="untilDate">The date that the test is being ignored until</param>
        /// <param name="reason">The reason the test is being ignored until that date</param>
        internal static void AddIgnoreUntilReason(this IPropertyBag properties, DateTimeOffset untilDate, string reason)
        {
            string skipReason = string.Format("Ignoring until {0}. {1}", untilDate.ToString("u"), reason);
            properties.Set(PropertyNames.SkipReason, skipReason);
        }
    }
}
