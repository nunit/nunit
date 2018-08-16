// ***********************************************************************
// Copyright (c) 2007 Charlie Poole, Rob Prouse
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
using NUnit.Framework.Internal;
using NUnit.TestUtilities;
using NUnit.TestUtilities.Comparers;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class CollectionOrderedConstraintTests
    {
        private readonly string NL = Environment.NewLine;

        #region Ordering Tests

        [TestCaseSource(nameof(OrderedByData))]
        public void IsOrderedBy(IEnumerable collection, Constraint constraint)
        {
            Assert.That(collection, constraint);
        }

        static readonly object[] OrderedByData = new[]
        {
            // Simple Ordering
            new TestCaseData(
                new[] { "x", "y", "z" },
                Is.Ordered),
            new TestCaseData(
                new[] { 1, 2, 3 },
                Is.Ordered),
            new TestCaseData(
                new[] { "x", "y", "z" },
                Is.Ordered.Ascending),
            new TestCaseData(
                new[] { 1, 2, 3 },
                Is.Ordered.Ascending),
            new TestCaseData(
                new[] { "z", "y", "x" },
                Is.Ordered.Descending),
            new TestCaseData(
                new[] { 3, 2, 1 },
                Is.Ordered.Descending),
            new TestCaseData(
                new[] { "x", "x", "z" },
                Is.Ordered),
            new TestCaseData(
                new[] { null, "x", "y" },
                Is.Ordered),
            new TestCaseData(
                new[] {"y", "x", null},
                Is.Ordered.Descending),
            new TestCaseData(
                new[] { "x", null, "y" },
                Is.Not.Ordered),
            // Ordered By Single Property
            new TestCaseData(
                new[] { new TestClass1(1), new TestClass1(2), new TestClass1(3) },
                Is.Ordered.By("Value") ),
            new TestCaseData(
                new[] { new TestClass1(1), new TestClass1(2), new TestClass1(3) },
                Is.Ordered.By("Value").Ascending ),
            new TestCaseData(
                new[] { new TestClass1(1), new TestClass1(2), new TestClass1(3) },
                Is.Ordered.Ascending.By("Value") ),
            new TestCaseData(
                new[] { new TestClass1(3), new TestClass1(2), new TestClass1(1) },
                Is.Ordered.By("Value").Descending ),
            new TestCaseData(
                new[] { new TestClass1(3), new TestClass1(2), new TestClass1(1) },
                Is.Ordered.Descending.By("Value") ),
            new TestCaseData(
                new[] { new TestClass1(1), new TestClass1(2), new TestClass1(3) },
                Is.Ordered.By("Value").Using(ObjectComparer.Default) ),
            new TestCaseData(
                new object[] { new TestClass1(1), new TestClass2(2) },
                Is.Ordered.By("Value") ),
            // Ordered By Two Properties
            new TestCaseData(
                new [] { new TestClass3("ABC", 1), new TestClass3("ABC", 42), new TestClass3("XYZ", 2) },
                Is.Ordered.By("A").By("B") ),
            new TestCaseData(
                new [] { new TestClass3("ABC", 1), new TestClass3("ABC", 42), new TestClass3("XYZ", 2) },
                Is.Ordered.By("A").Then.By("B") ),
            new TestCaseData(
                new [] { new TestClass3("ABC", 1), new TestClass3("ABC", 42), new TestClass3("XYZ", 2) },
                Is.Ordered.Ascending.By("A").Then.Ascending.By("B") ),
            new TestCaseData(
                new [] { new TestClass3("ABC", 1), new TestClass3("ABC", 42), new TestClass3("XYZ", 2) },
                Is.Ordered.By("A").Ascending.Then.By("B").Ascending ),
            new TestCaseData(
                new [] { new TestClass3("ABC", 42), new TestClass3("XYZ", 99), new TestClass3("XYZ", 2) },
                Is.Not.Ordered.By("A").Then.By("B") ),
            new TestCaseData(
                new [] {  new TestClass3("XYZ", 2), new TestClass3("ABC", 1), new TestClass3("ABC", 42) },
                Is.Ordered.By("A").Descending.Then.By("B") ),
            new TestCaseData(
                new [] {  new TestClass3("XYZ", 2), new TestClass3("ABC", 1), new TestClass3("ABC", 42) },
                Is.Ordered.Descending.By("A").Then.By("B") ),
            new TestCaseData(
                new [] { new TestClass3("ABC", 42), new TestClass3("ABC", 1), new TestClass3("XYZ", 2) },
                Is.Ordered.By("A").Ascending.Then.By("B").Descending ),
            new TestCaseData(
                new [] { new TestClass3("ABC", 42), new TestClass3("ABC", 1), new TestClass3("XYZ", 2) },
                Is.Ordered.Ascending.By("A").Then.Descending.By("B") ),
            new TestCaseData(
                new [] { new TestClass3("ABC", 42), new TestClass3("ABC", 1), new TestClass3("XYZ", 2) },
                Is.Not.Ordered.By("A").Then.By("B") ),
            new TestCaseData(
                new[] { new TestClass3("XYZ", 2), new TestClass3("ABC", 42), new TestClass3("ABC", 1) },
                Is.Ordered.By("A").Descending.Then.By("B").Descending ),
            new TestCaseData(
                new[] { new TestClass3("XYZ", 2), new TestClass3("ABC", 42), new TestClass3("ABC", 1) },
                Is.Ordered.Descending.By("A").Then.Descending.By("B") )
        };

        #endregion

        #region Error Message Tests

        [Test]
        public void IsOrdered_Fails()
        {
            var expectedMessage =
                "  Expected: collection ordered" + NL +
                "  But was:  < \"x\", \"z\", \"y\" >" + NL;

            var ex = Assert.Throws<AssertionException>(() => Assert.That(new[] { "x", "z", "y" }, Is.Ordered));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        #endregion

        #region Custom Comparer Tests

        [Test]
        public void IsOrdered_HandlesCustomComparison()
        {
            AlwaysEqualComparer comparer = new AlwaysEqualComparer();
            Assert.That(new[] { new object(), new object() }, Is.Ordered.Using(comparer));
            Assert.That(comparer.CallCount, Is.GreaterThan(0), "TestComparer was not called");
        }

        [Test]
        public void ExceptionThrownForMultipleComparersInStep()
        {
            Assert.That(() => Is.Ordered.Using(new TestComparer()).Using(new AlwaysEqualComparer()), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void MultipleComparersUsedInDifferentSteps()
        {
            var comparer1 = new TestComparer();
            var comparer2 = new AlwaysEqualComparer();
            var collection = new[] { new TestClass3("XYZ", 2), new TestClass3("ABC", 42), new TestClass3("ABC", 1) };

            Assert.That(collection, Is.Ordered.By("A").Using(comparer1).Then.By("B").Using(comparer2));

            // First comparer is called for every pair of items in the collection
            Assert.That(comparer1.CallCount, Is.EqualTo(2), "First comparer should be called twice");

            // Second comparer is only called where the first property matches
            Assert.That(comparer2.CallCount, Is.EqualTo(1), "Second comparer should be called once");
        }

        [Test]
        public void IsOrdered_HandlesCustomComparison2()
        {
            TestComparer comparer = new TestComparer();
            Assert.That(new[] { 2, 1 }, Is.Ordered.Using(comparer));
            Assert.That(comparer.CallCount, Is.GreaterThan(0), "TestComparer was not called");
        }

        [Test]
        public void UsesProvidedGenericComparer()
        {
            var comparer = new GenericComparer<int>();
            Assert.That(new[] { 1, 2 }, Is.Ordered.Using(comparer));
            Assert.That(comparer.WasCalled, "Comparer was not called");
        }

        [Test]
        public void UsesProvidedGenericComparison()
        {
            var comparer = new GenericComparison<int>();
            Assert.That(new[] { 1, 2 }, Is.Ordered.Using(comparer.Delegate));
            Assert.That(comparer.WasCalled, "Comparer was not called");
        }

        [Test]
        public void UsesProvidedLambda()
        {
            Comparison<int> comparer = (x, y) => x.CompareTo(y);
            Assert.That(new[] { 1, 2 }, Is.Ordered.Using(comparer));
        }

        #endregion

        #region Exception Tests

        [Test]
        public void ExceptionThrownForRepeatedAscending()
        {
            Assert.That(() => Is.Ordered.Ascending.Ascending, Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void ExceptionThrownForRepeatedDescending()
        {
            Assert.That(() => Is.Ordered.Descending.Descending, Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void ExceptionThrownForAscendingPlusDescending()
        {
            Assert.That(() => Is.Ordered.Ascending.Descending, Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void ExceptionThrownForAscendingByDescending()
        {
            Assert.That(() => Is.Ordered.Ascending.By("A").Descending, Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void IsOrderedByProperty_ThrowsOnNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Assert.That(new[] { new TestClass4("x"), null, new TestClass4("z") }, Is.Ordered.By("Value")));
            Assert.That(ex.Message, Does.Contain("index 1"));
        }

        [Test]
        public void IsOrdered_TypesMustBeComparable()
        {
            Assert.Throws<ArgumentException>(() => Assert.That(new object[] { 1, "x" }, Is.Ordered));
        }

        [Test]
        public void IsOrdered_AtLeastOneArgMustImplementIComparable()
        {
            Assert.Throws<ArgumentException>(() => Assert.That(new [] { new object(), new object() }, Is.Ordered));
        }

        [TestCaseSource(nameof(InvalidOrderedByData))]
        public void IsOrdered_ThrowsOnMissingProperty(object[] collection, string property, string expectedIndex)
        {
            Assert.That(() => Assert.That(collection, Is.Ordered.By(property)), Throws.ArgumentException.With.Message.Contain(expectedIndex));
        }

        static readonly object[] InvalidOrderedByData = new[]
        {
            new TestCaseData(
                new object [] { "a", "b" },
                "A",
                "index 0"),
            new TestCaseData(
                new object [] { new TestClass3("a", 1), "b" },
                "A",
                "index 1"),
            new TestCaseData(
                new object [] { new TestClass3("a", 1), new TestClass3("b", 1), new TestClass4("c") },
                "A",
                "index 2"),
        };

        #endregion

        #region Test Classes

        public class TestClass1
        {
            public int Value { get; }

            public TestClass1(int value)
            {
                Value = value;
            }

            public override string ToString()
            {
                return Value.ToString();
            }
        }

        class TestClass2
        {
            public int Value { get; }

            public TestClass2(int value)
            {
                Value = value;
            }

            public override string ToString()
            {
                return Value.ToString();
            }
        }

        public class TestClass3
        {
            public string A { get; }
            public int B { get; }

            public TestClass3(string a, int b)
            {
                A = a;
                B = b;
            }

            public override string ToString()
            {
                return A.ToString() + "," + B.ToString();
            }
        }

        public class TestClass4
        {
            public readonly string A;

            public TestClass4(string a)
            {
                A = a;
            }

            public override string ToString()
            {
                return A;
            }
        }

        #endregion
    }
}
