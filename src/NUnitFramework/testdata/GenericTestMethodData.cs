// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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
using System.Text;
using NUnit.Framework;

namespace NUnit.TestData
{
    public class IncompatibleGenericTestCaseData
    {
        [TestCase(5, 2, "ABC")]
        [TestCase(5, "Y", "ABC")]
        [TestCase("X", 2, "ABC")]
        [TestCase("X", "Y", "ABC")]
        public void TestCase_OneTypeParameterOnTwoArgs<T>(T x, T y, string label)
        {
        }
    }

    public class IncompatibleGenericTestCaseSourceData
    {
        [TestCaseSource("Source")]
        public void TestCaseSource_OneTypeParameterOnTwoArgs<T>(T x, T y, string label)
        {
        }

#pragma warning disable 414
        private static readonly object[] Source = new object[] {
            new object[] { 5, 2, "ABC" },
            new object[] { 5, "Y", "ABC" },
            new object[] { "X", 2, "ABC" },
            new object[] { "X", "Y", "ABC" }
        };
#pragma warning restore 414
    }

    public class IncompatibleGenericCombinatorialData
    {
        [Test]
        public void Combinatorial_OneTypeParameterOnTwoArgs<T>(
            [Values(5, "X")] T x,
            [Values(2, "Y")] T y)
        {
        }
    }
}
