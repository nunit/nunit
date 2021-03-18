// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
        [TestCaseSource(nameof(Source))]
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
