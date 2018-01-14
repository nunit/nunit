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
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
    public static class EqualityAdapterTests
    {
        public static IEnumerable<EqualityAdapter> EqualityAdapters()
        {
            return new[]
            {
                EqualityAdapter.For((IEqualityComparer)StringComparer.Ordinal),
                EqualityAdapter.For((IEqualityComparer<string>)StringComparer.Ordinal),
                EqualityAdapter.For<string, string>(StringComparer.Ordinal.Equals),
                EqualityAdapter.For((IComparer)StringComparer.Ordinal),
                EqualityAdapter.For((IComparer<string>)StringComparer.Ordinal),
                EqualityAdapter.For<string>(StringComparer.Ordinal.Compare)
            };
        }

        [TestCaseSource(nameof(EqualityAdapters))]
        public static void CanCompareWithNull(EqualityAdapter adapter)
        {
            Assert.That(adapter.AreEqual(null, "a"), Is.False);
            Assert.That(adapter.AreEqual("a", null), Is.False);
            Assert.That(adapter.AreEqual(null, null), Is.True);
        }
    }
}
