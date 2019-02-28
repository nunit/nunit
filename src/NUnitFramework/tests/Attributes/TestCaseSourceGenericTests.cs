// ***********************************************************************
// Copyright (c) 2019 Charlie Poole, Rob Prouse
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
using System.Collections.Generic;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Attributes
{
    [TestFixture]
    public class TestCaseSourceGenericTests
    {
        [TestCaseSourceGeneric(nameof(StringList), TypeArguments = new[] { typeof(string) })]
        [TestCaseSourceGeneric(nameof(IntList), TypeArguments = new[] { typeof(int) })]
        [TestCaseSourceGeneric(nameof(DateList), TypeArguments = new[] { typeof(DateTime) })]
        public void ListTest<T>(List<T> data)
        {
            Assert.AreEqual(3, data.Count);
        }

        public static IEnumerable<List<string>> StringList()
        {
                yield return new List<string> { "one", "two", "three" };
        }

        public static IEnumerable<List<int>> IntList()
        {
                yield return new List<int> { 1, 2, 3 };
        }

        public static IEnumerable<List<DateTime>> DateList()
        {
            yield return new List<DateTime> { DateTime.MinValue, DateTime.Now, DateTime.MaxValue };
        }

        [TestCaseSourceGeneric(nameof(SpecifiedList), 5, TypeArguments = new[] { typeof(int) })]
        public void SpecifiedListTest<T>(List<T> data, int expectedCount)
        {
            Assert.AreEqual(expectedCount, data.Count);
        }

        public static IEnumerable<object> SpecifiedList(int expectedCount)
        {
            var result = new List<int>();
            for (int i = 0; i < expectedCount; i++)
            {
                result.Add(i);
            }
            yield return new object[] { result, expectedCount };
        }
    }
}
