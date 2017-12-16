// ***********************************************************************
// Copyright (c) 2008 Charlie Poole, Rob Prouse
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
using System.Collections;

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
        [Test]
        public void ShouldThrowExceptionIfObjectHasNoComparer()
        {
            NoComparer from = new NoComparer(1), to = new NoComparer(10), testObj = new NoComparer(5);
            Assert.Throws<ArgumentException>(() => new RangeConstraint(from, to).ApplyTo(testObj));
        }
        [Test]
        public void ChangingComparerTest()
        {
            RangeConstraint test = new RangeConstraint(5, 42);
            Comparison<int> rComparer = (x, y) => y.CompareTo(x);
            Comparison<int> comparer = (x, y) => x.CompareTo(y);
            Assert.DoesNotThrow(() => test.ApplyTo(7));
            test.Using(rComparer);
            Assert.Throws<ArgumentException>(() => test.ApplyTo(7));
            Assert.Throws<ArgumentException>(() => test.Using(rComparer).ApplyTo(7));
            Assert.DoesNotThrow(() => test.Using(comparer).ApplyTo(7));
            test.Using(comparer);
            Assert.DoesNotThrow(() => test.ApplyTo(7));
        }
        [TestCaseSource(nameof(NoIComparableTestCase))]
        public void RangeConstructorComparerThrowExceptionIfFromIsLessThanTo(object testObj,object from, object to, System.Collections.IComparer comparer)
        {
            RangeConstraint test = new RangeConstraint(from, to);
            test.Using(comparer);
            test.ApplyTo(testObj);
        }
        private static IEnumerable NoIComparableTestCase()
        {
            IComparer comparer = new ObjectToStringComparer();
            yield return new object[] { new NoComparer(110), new NoComparer(10), new NoComparer(120), comparer };
            yield return new object[] { new NoComparer("M"), new NoComparer("A"), new NoComparer("Z"), comparer };
        }
    }

}
