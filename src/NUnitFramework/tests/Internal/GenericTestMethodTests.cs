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
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.TestData;
using NUnit.TestUtilities;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    class GenericTestMethodTests
    {
        [TestCase(5, 2, "ABC")]
        [TestCase(5.0, 2.0, "ABC")]
        [TestCase(5, 2.0, "ABC")]
        [TestCase(5.0, 2L, "ABC")]
        public void TestCase_OneTypeParameterOnTwoArgs<T>(T x, T y, string label)
        {
            Assert.AreEqual(5, x);
            Assert.AreEqual(2, y);
            Assert.AreEqual("ABC", label);
        }

        [Test]
        public void TestCase_IncompatibleArgsAreNotRunnable()
        {
            var result = TestBuilder.RunTestFixture(typeof(IncompatibleGenericTestCaseData));
            Assert.That(result.PassCount, Is.EqualTo(2), "PassCount");
            Assert.That(result.FailCount, Is.EqualTo(2), "FailCount");

            int invalid = 0;
            // Examine grandchildren - child is parameterized method suite
            var suiteResult = result.Children.ToArray()[0];
            foreach (var childResult in suiteResult.Children)
                if (childResult.ResultState == ResultState.NotRunnable)
                    invalid++;

            Assert.That(invalid, Is.EqualTo(2), "Invalid count");
        }

        [TestCase(5, 2, "ABC")]
        [TestCase(5.0, 2.0, "ABC")]
        [TestCase(5, 2.0, "ABC")]
        [TestCase(5.0, 2L, "ABC")]
        public void TestCase_TwoTypeParameters<T1, T2>(T1 x, T2 y, string label)
        {
            Assert.AreEqual(5, x);
            Assert.AreEqual(2, y);
            Assert.AreEqual("ABC", label);
        }

        [TestCase(5, 2, "ABC")]
        [TestCase(5.0, 2.0, "ABC")]
        [TestCase(5, 2.0, "ABC")]
        [TestCase(5.0, 2L, "ABC")]
        public void TestCase_TwoTypeParameters_Reversed<T1, T2>(T2 x, T1 y, string label)
        {
            Assert.AreEqual(5, x);
            Assert.AreEqual(2, y);
            Assert.AreEqual("ABC", label);
        }

        [TestCaseSource("Source")]
        public void TestCaseSource_OneTypeParameterOnTwoArgs<T>(T x, T y, string label)
        {
            Assert.AreEqual(5, x);
            Assert.AreEqual(2, y);
            Assert.AreEqual("ABC", label);
        }

        [Test]
        public void TestCaseSource_IncompatibleArgsAreNotRunnable()
        {
            var result = TestBuilder.RunTestFixture(typeof(IncompatibleGenericTestCaseSourceData));
            Assert.That(result.PassCount, Is.EqualTo(2), "PassCount");
            Assert.That(result.FailCount, Is.EqualTo(2), "FailCount");

            int invalid = 0;
            // Examine grandchildren - child is parameterized method suite
            var suiteResult = result.Children.ToArray()[0];
            foreach (var childResult in suiteResult.Children)
                if (childResult.ResultState == ResultState.NotRunnable)
                    invalid++;

            Assert.That(invalid, Is.EqualTo(2), "Invalid count");
        }

        [TestCaseSource("Source")]
        public void TestCaseSource_TwoTypeParameters<T1, T2>(T1 x, T2 y, string label)
        {
            Assert.AreEqual(5, x);
            Assert.AreEqual(2, y);
            Assert.AreEqual("ABC", label);
        }

        [Test]
        public void Combinatorial_OneTypeParameterOnTwoArgs<T>(
            [Values(5, 5.0)] T x,
            [Values(2.0, 2)] T y)
        {
            Assert.AreEqual(5, x);
            Assert.AreEqual(2, y);
        }

        [Test]
        public void Combinatorial_TwoTypeParameters<T1, T2>(
            [Values(5, 5.0)] T1 x,
            [Values(2.0, 2)] T2 y)
        {
            Assert.AreEqual(5, x);
            Assert.AreEqual(2, y);
        }

        [Test]
        public void Combinatorial_IncompatibleArgsAreNotRunnable()
        {
            var result = TestBuilder.RunTestFixture(typeof(IncompatibleGenericCombinatorialData));
            Assert.That(result.PassCount, Is.EqualTo(2), "PassCount");
            Assert.That(result.FailCount, Is.EqualTo(2), "FailCount");

            int invalid = 0;
            // Examine grandchildren - child is parameterized method suite
            var suiteResult = result.Children.ToArray()[0];
            foreach (var childResult in suiteResult.Children)
                if (childResult.ResultState == ResultState.NotRunnable)
                    invalid++;

            Assert.That(invalid, Is.EqualTo(2), "Invalid count");
        }

        private static readonly object[] Source = new object[] {
            new object[] { 5, 2, "ABC" },
            new object[] { 5.0, 2.0, "ABC" },
            new object[] { 5, 2.0, "ABC" },
            new object[] { 5.0, 2L, "ABC" }
        };

        //[TestCaseSource("SequenceCases")]
        //public void SequenceEquality<TSource>(IEnumerable<TSource> left, IEnumerable<TSource> right)
        //{
        //    Assert.That(left, Is.EqualTo(right));
        //}

        //static ITestCaseData[] SequenceCases = {
        //    new TestCaseData(new List<int> { 1, 2 }, new List<int> { 1, 2 }) };
    }
}
