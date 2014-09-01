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

        [Test]
        public void IsOrdered()
        {
            var al = new List<string>();
            al.Add("x");
            al.Add("y");
            al.Add("z");

            Assert.That(al, Is.Ordered);
        }

        [Test]
        public void IsOrdered_2()
        {
            var al = new List<int>();
            al.Add(1);
            al.Add(2);
            al.Add(3);

            Assert.That(al, Is.Ordered);
        }

        [Test]
        public void IsOrderedDescending()
        {
            var al = new List<string>();
            al.Add("z");
            al.Add("y");
            al.Add("x");

            Assert.That(al, Is.Ordered.Descending);
        }

        [Test]
        public void IsOrderedDescending_2()
        {
            var al = new List<int>();
            al.Add(3);
            al.Add(2);
            al.Add(1);

            Assert.That(al, Is.Ordered.Descending);
        }

        [Test]
        public void IsOrdered_Fails()
        {
            var al = new List<string>();
            al.Add("x");
            al.Add("z");
            al.Add("y");

            var expectedMessage =
                "  Expected: collection ordered" + NL +
                "  But was:  < \"x\", \"z\", \"y\" >" + NL;

            var ex = Assert.Throws<AssertionException>(() => Assert.That(al, Is.Ordered));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsOrdered_Allows_adjacent_equal_values()
        {
            var al = new List<string>();
            al.Add("x");
            al.Add("x");
            al.Add("z");

            Assert.That(al, Is.Ordered);
        }

        [Test]
        public void IsOrdered_Handles_null()
        {
            var al = new List<object>();
            al.Add("x");
            al.Add(null);
            al.Add("z");

            var ex = Assert.Throws<ArgumentNullException>(() => Assert.That(al, Is.Ordered));
            Assert.That(ex.Message, Does.Contain("index 1"));
        }

        [Test]
        public void IsOrdered_TypesMustBeComparable()
        {
            var al = new List<object>();
            al.Add(1);
            al.Add("x");

            Assert.Throws<ArgumentException>(() => Assert.That(al, Is.Ordered));
        }

        [Test]
        public void IsOrdered_AtLeastOneArgMustImplementIComparable()
        {
            var al = new List<object>();
            al.Add(new object());
            al.Add(new object());

            Assert.Throws<ArgumentException>(() => Assert.That(al, Is.Ordered));
        }

        [Test]
        public void IsOrdered_Handles_custom_comparison()
        {
            var al = new List<object>();
            al.Add(new object());
            al.Add(new object());

            AlwaysEqualComparer comparer = new AlwaysEqualComparer();
            Assert.That(al, Is.Ordered.Using(comparer));
            Assert.That(comparer.Called, "TestComparer was not called");
        }

        [Test]
        public void IsOrdered_Handles_custom_comparison2()
        {
            var al = new List<int>();
            al.Add(2);
            al.Add(1);

            TestComparer comparer = new TestComparer();
            Assert.That(al, Is.Ordered.Using(comparer));
            Assert.That(comparer.Called, "TestComparer was not called");
        }

        [Test]
        public void UsesProvidedGenericComparer()
        {
            var al = new List<int>();
            al.Add(1);
            al.Add(2);

            var comparer = new GenericComparer<int>();
            Assert.That(al, Is.Ordered.Using(comparer));
            Assert.That(comparer.WasCalled, "Comparer was not called");
        }

        [Test]
        public void UsesProvidedGenericComparison()
        {
            var al = new List<int>();
            al.Add(1);
            al.Add(2);

            var comparer = new GenericComparison<int>();
            Assert.That(al, Is.Ordered.Using(comparer.Delegate));
            Assert.That(comparer.WasCalled, "Comparer was not called");
        }

        [Test]
        public void UsesProvidedLambda()
        {
            var al = new List<int>();
            al.Add(1);
            al.Add(2);

            Comparison<int> comparer = (x, y) => x.CompareTo(y);
            Assert.That(al, Is.Ordered.Using(comparer));
        }

        [Test]
        public void IsOrderedBy()
        {
            var al = new List<OrderedByTestClass>();
            al.Add(new OrderedByTestClass(1));
            al.Add(new OrderedByTestClass(2));

            Assert.That(al, Is.Ordered.By("Value"));
        }

        [Test]
        public void IsOrderedBy_Comparer()
        {
            var al = new List<OrderedByTestClass>();
            al.Add(new OrderedByTestClass(1));
            al.Add(new OrderedByTestClass(2));

            Assert.That(al, Is.Ordered.By("Value").Using(ObjectComparer.Default));
        }

        [Test]
        public void IsOrderedBy_Handles_heterogeneous_classes_as_long_as_the_property_is_of_same_type()
        {
            var al = new List<object>();
            al.Add(new OrderedByTestClass(1));
            al.Add(new OrderedByTestClass2(2));

            Assert.That(al, Is.Ordered.By("Value"));
        }

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
    }
}