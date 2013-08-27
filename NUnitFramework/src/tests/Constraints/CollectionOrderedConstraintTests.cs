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

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class CollectionOrderedConstraintTests : NUnit.Framework.Assertions.MessageChecker
    {
        [Test]
        public void IsOrdered()
        {
            var al = new ObjectList();
            al.Add("x");
            al.Add("y");
            al.Add("z");

            Assert.That(al, Is.Ordered);
        }

        [Test]
        public void IsOrdered_2()
        {
            var al = new ObjectList();
            al.Add(1);
            al.Add(2);
            al.Add(3);

            Assert.That(al, Is.Ordered);
        }

        [Test]
        public void IsOrderedDescending()
        {
            var al = new ObjectList();
            al.Add("z");
            al.Add("y");
            al.Add("x");

            Assert.That(al, Is.Ordered.Descending);
        }

        [Test]
        public void IsOrderedDescending_2()
        {
            var al = new ObjectList();
            al.Add(3);
            al.Add(2);
            al.Add(1);

            Assert.That(al, Is.Ordered.Descending);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void IsOrdered_Fails()
        {
            var al = new ObjectList();
            al.Add("x");
            al.Add("z");
            al.Add("y");

            expectedMessage =
                "  Expected: collection ordered" + NL +
                "  But was:  < \"x\", \"z\", \"y\" >" + NL;

            Assert.That(al, Is.Ordered);
        }

        [Test]
        public void IsOrdered_Allows_adjacent_equal_values()
        {
            var al = new ObjectList();
            al.Add("x");
            al.Add("x");
            al.Add("z");

            Assert.That(al, Is.Ordered);
        }

        [Test, ExpectedException(typeof(ArgumentNullException),
            ExpectedMessage = "index 1", MatchType = MessageMatch.Contains)]
        public void IsOrdered_Handles_null()
        {
            var al = new ObjectList();
            al.Add("x");
            al.Add(null);
            al.Add("z");

            Assert.That(al, Is.Ordered);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void IsOrdered_TypesMustBeComparable()
        {
            var al = new ObjectList();
            al.Add(1);
            al.Add("x");

            Assert.That(al, Is.Ordered);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void IsOrdered_AtLeastOneArgMustImplementIComparable()
        {
            var al = new ObjectList();
            al.Add(new object());
            al.Add(new object());

            Assert.That(al, Is.Ordered);
        }

        [Test]
        public void IsOrdered_Handles_custom_comparison()
        {
            var al = new ObjectList();
            al.Add(new object());
            al.Add(new object());

            AlwaysEqualComparer comparer = new AlwaysEqualComparer();
            Assert.That(al, Is.Ordered.Using(comparer));
            Assert.That(comparer.Called, "TestComparer was not called");
        }

        [Test]
        public void IsOrdered_Handles_custom_comparison2()
        {
            var al = new ObjectList();
            al.Add(2);
            al.Add(1);

            TestComparer comparer = new TestComparer();
            Assert.That(al, Is.Ordered.Using(comparer));
            Assert.That(comparer.Called, "TestComparer was not called");
        }

        [Test]
        public void UsesProvidedComparerOfT()
        {
            var al = new ObjectList();
            al.Add(1);
            al.Add(2);

            MyComparer<int> comparer = new MyComparer<int>();
            Assert.That(al, Is.Ordered.Using(comparer));
            Assert.That(comparer.Called, "Comparer was not called");
        }

        class MyComparer<T> : IComparer<T>
        {
            public bool Called;

            public int Compare(T x, T y)
            {
                Called = true;
                return Comparer<T>.Default.Compare(x, y);
            }
        }

        [Test]
        public void UsesProvidedComparisonOfT()
        {
            var al = new ObjectList();
            al.Add(1);
            al.Add(2);

            MyComparison<int> comparer = new MyComparison<int>();
            Assert.That(al, Is.Ordered.Using(new Comparison<int>(comparer.Compare)));
            Assert.That(comparer.Called, "Comparer was not called");
        }

        class MyComparison<T>
        {
            public bool Called;

            public int Compare(T x, T y)
            {
                Called = true;
                return Comparer<T>.Default.Compare(x, y);
            }
        }

        [Test]
        public void UsesProvidedLambda()
        {
            var al = new ObjectList();
            al.Add(1);
            al.Add(2);

            Comparison<int> comparer = (x, y) => x.CompareTo(y);
            Assert.That(al, Is.Ordered.Using(comparer));
        }

        [Test]
        public void IsOrderedBy()
        {
            var al = new ObjectList();
            al.Add(new OrderedByTestClass(1));
            al.Add(new OrderedByTestClass(2));

            Assert.That(al, Is.Ordered.By("Value"));
        }

        [Test]
        public void IsOrderedBy_Comparer()
        {
            var al = new ObjectList();
            al.Add(new OrderedByTestClass(1));
            al.Add(new OrderedByTestClass(2));

            Assert.That(al, Is.Ordered.By("Value").Using(SimpleObjectComparer.Default));
        }

        [Test]
        public void IsOrderedBy_Handles_heterogeneous_classes_as_long_as_the_property_is_of_same_type()
        {
            var al = new ObjectList();
            al.Add(new OrderedByTestClass(1));
            al.Add(new OrderedByTestClass2(2));

            Assert.That(al, Is.Ordered.By("Value"));
        }

        class OrderedByTestClass
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