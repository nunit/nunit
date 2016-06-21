// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
        private readonly string NL = NUnit.Env.NewLine;

        #region Simple Ordering

        [Test]
        public void IsOrdered_Strings()
        {
            Assert.That(new[] { "x", "y", "z" }, Is.Ordered);
        }

        [Test]
        public void IsOrdered_Ints()
        {
            Assert.That(new[] { 1, 2, 3 }, Is.Ordered);
        }

        [Test]
        public void IsOrderedAscending_Strings()
        {
            Assert.That(new[] { "x", "y", "z" }, Is.Ordered.Ascending);
        }

        [Test]
        public void IsOrderedAscending_Ints()
        {
            Assert.That(new[] { 1, 2, 3 }, Is.Ordered.Ascending);
        }

        [Test]
        public void IsOrderedDescending_Strings()
        {
            Assert.That(new[] { "z", "y", "x" }, Is.Ordered.Descending);
        }

        [Test]
        public void IsOrderedDescending_Ints()
        {
            Assert.That(new[] { 3, 2, 1 }, Is.Ordered.Descending);
        }

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
        public void IsOrdered_Fails()
        {
            var expectedMessage =
                "  Expected: collection ordered" + NL +
                "  But was:  < \"x\", \"z\", \"y\" >" + NL;

            var ex = Assert.Throws<AssertionException>(() => Assert.That(new[] { "x", "z", "y" }, Is.Ordered));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsOrdered_AllowsAdjacentEqualValues()
        {
            Assert.That(new[] { "x", "x", "z" }, Is.Ordered);
        }

        [Test]
        public void IsOrdered_ThrowsOnNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Assert.That(new[] { "x", null, "z" }, Is.Ordered));
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

        [Test]
        public void IsOrdered_HandlesCustomComparison()
        {
            AlwaysEqualComparer comparer = new AlwaysEqualComparer();
            Assert.That(new[] { new object(), new object() }, Is.Ordered.Using(comparer));
            Assert.That(comparer.Called, "TestComparer was not called");
        }

        [Test]
        public void ExceptionThrownForMultipleComparers()
        {
            Assert.That(() => Is.Ordered.Using(new TestComparer()).Using(new AlwaysEqualComparer()), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void IsOrdered_HandlesCustomComparison2()
        {
            TestComparer comparer = new TestComparer();
            Assert.That(new[] { 2, 1 }, Is.Ordered.Using(comparer));
            Assert.That(comparer.Called, "TestComparer was not called");
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

        #region Ordered By One Property

        [Test]
        public void IsOrderedBy()
        {
            Assert.That(new[] { new OrderedByTestClass(1), new OrderedByTestClass(2) }, Is.Ordered.By("Value"));
        }

        [Test]
        public void IsOrderedByAscending()
        {
            Assert.That(new[] { new OrderedByTestClass(1), new OrderedByTestClass(2) }, Is.Ordered.By("Value").Ascending);
        }

        [Test]
        public void IsOrderedAscendingBy()
        {
            Assert.That(new[] { new OrderedByTestClass(1), new OrderedByTestClass(2) }, Is.Ordered.Ascending.By("Value"));
        }

        [Test]
        public void IsOrderedByDescending()
        {
            Assert.That(new[] { new OrderedByTestClass(2), new OrderedByTestClass(1) }, Is.Ordered.By("Value").Descending);
        }

        [Test]
        public void IsOrderedDescendingBy()
        {
            Assert.That(new[] { new OrderedByTestClass(2), new OrderedByTestClass(1) }, Is.Ordered.Descending.By("Value"));
        }

        [Test]
        public void IsOrderedBy_Comparer()
        {
            Assert.That(new[] { new OrderedByTestClass(1), new OrderedByTestClass(2) }, Is.Ordered.By("Value").Using(ObjectComparer.Default));
        }

        [Test]
        public void IsOrderedBy_HandlesHeterogeneousClassesIfPropertyIsOfSameType()
        {
            Assert.That(new object[] { new OrderedByTestClass(1), new OrderedByTestClass2(2) }, Is.Ordered.By("Value"));
        }

        [Test]
        public void ExceptionThrownForAscendingPlusByPlusDescending()
        {
            Assert.That(() => Is.Ordered.Ascending.By("A").Descending, Throws.TypeOf<InvalidOperationException>());
        }

        #endregion

        #region Test Classes

        // Public to avoid a MethodAccessException under CF 2.0
        public class OrderedByTestClass
        {
            private int myValue;

            public int Value
            {
                get { return myValue; }
                set { myValue = value; }
            }

            public OrderedByTestClass(int value)
            {
                Value = value;
            }
        }

        class OrderedByTestClass2
        {
            private int myValue;
            public int Value
            {
                get { return myValue; }
                set { myValue = value; }
            }

            public OrderedByTestClass2(int value)
            {
                Value = value;
            }
        }

        #endregion
    }
}