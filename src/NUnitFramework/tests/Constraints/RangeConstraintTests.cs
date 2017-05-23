// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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
using NUnit.TestUtilities.Comparers;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class RangeConstraintTest : ConstraintTestBase
    {
        RangeConstraint rangeConstraint;

        [SetUp]
        public void SetUp()
        {
            theConstraint = rangeConstraint = new RangeConstraint(5, 42);
            expectedDescription = "in range (5,42)";
            stringRepresentation = "<range 5 42>";
        }

        static object[] SuccessData = new object[] { 5, 23, 42 };

        static object[] FailureData = new object[] { new object[] { 4, "4" }, new object[] { 43, "43" } };
        
        [TestCase(null)]
        [TestCase("xxx")]
        public void InvalidDataThrowsArgumentException(object data)
        {
            Assert.Throws<ArgumentException>(() => theConstraint.ApplyTo(data));
        }

        [Test]
        public void UsesProvidedIComparer()
        {
            var comparer = new ObjectComparer();
            Assert.That(rangeConstraint.Using(comparer).ApplyTo(19).IsSuccess);
            Assert.That(comparer.WasCalled, "Comparer was not called");
        }

        [Test]
        public void UsesProvidedGenericComparer()
        {
            var comparer = new GenericComparer<int>();
            Assert.That(rangeConstraint.Using(comparer).ApplyTo(19).IsSuccess);
            Assert.That(comparer.WasCalled, "Comparer was not called");
        }

        [Test]
        public void UsesProvidedGenericComparison()
        {
            var comparer = new GenericComparison<int>();
            Assert.That(rangeConstraint.Using(comparer.Delegate).ApplyTo(19).IsSuccess);
            Assert.That(comparer.WasCalled, "Comparer was not called");
        }

        [Test]
        public void UsesProvidedLambda()
        {
            Comparison<int> comparer = (x, y) => x.CompareTo(y);
            Assert.That(rangeConstraint.Using(comparer).ApplyTo(19).IsSuccess);
        }

        // Test on Issue #21 - https://github.com/nunit/nunit-framework/issues/21
        [Test]
        public void ShouldThrowExceptionIfFromIsLessThanTo()
        {
            Assert.Throws<ArgumentException>(() => new RangeConstraint( 42, 5 ));
        }

        #if !PORTABLE
        [Test, TestCaseSource("ObjectsWithNoIComparable")]
        public void InRangeNoIComparableTest(object testObj, object from, object to,System.Collections.IComparer comparer) {
           
            Assert.Throws<ArgumentException>(() =>
            {
                Assert.That(testObj, Is.InRange(from, to));
            });
            Assert.That(testObj, Is.InRange(from, to).Using(comparer));
            Assert.Throws<AssertionException>(() =>
            {
                Assert.That(from, Is.InRange(testObj, to).Using(comparer));
            });
        }

        [Test,TestCaseSource("ObjectsWithNoIComparable")]
        public void NotInRangeTest(object testObj, object from, object to, System.Collections.IComparer comparer)
        {
            Assert.That(to, Is.Not.InRange(testObj, from).Using(comparer));
        }

        private static System.Collections.IEnumerable ObjectsWithNoIComparable()
        {
            var obj1 = new ExampleTest.NoComparer(1);
            var obj = new ExampleTest.NoComparer(30);
            var obj46 = new ExampleTest.NoComparer(46);
            var comparer = new ExampleTest.myObjectComparer();
            yield return new object[] { obj, obj1, obj46 , comparer };
        }
        #endif
    }

    #if !PORTABLE
    namespace ExampleTest
    {
        public class myObjectComparer : System.Collections.IComparer
        {
            public readonly System.Collections.IComparer Default = new System.Collections.CaseInsensitiveComparer();
            int System.Collections.IComparer.Compare(object x, object y)
            {
                return Default.Compare(x.ToString(), y.ToString());

            }
        }

        public class myComparer : NoComparer , IComparable
        {
            public myComparer(int value) : base(value){
            }

            int IComparable.CompareTo(object obj)
            {
                if (obj == null) return 1;

                myComparer otherObj = obj as myComparer;
                if (otherObj != null)
                    return this._value.CompareTo(otherObj._value);
                return 0;
            }
        }
        public class NoComparer
        {
            public readonly int _value;
            public NoComparer(int value) {
                _value = value;
            }
            public override string ToString() {
                return _value.ToString();
            }
        }
    }
    #endif
}