// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Tests.TestUtilities.Comparers;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class RangeTests
    {
        [Test]
        public void InRangeSucceeds()
        {
            Assert.That(7, Is.InRange(5, 10));
            Assert.That(0.23, Is.InRange(-1.0, 1.0));
            Assert.That(DateTime.Parse("12-December-2008"),
                Is.InRange(DateTime.Parse("1-October-2008"), DateTime.Parse("31-December-2008")));
        }

        [Test]
        public void InRangeFails()
        {
            string expectedMessage = string.Format("  Expected: in range (5,10){0}  But was:  12{0}",
                    Environment.NewLine);

            Assert.That(
                new TestDelegate(FailingInRangeMethod),
                Throws.TypeOf(typeof(AssertionException)).With.Message.Contains(expectedMessage));
        }

        private void FailingInRangeMethod()
        {
            Assert.That(12, Is.InRange(5, 10));
        }

        [Test]
        public void NotInRangeSucceeds()
        {
            Assert.That(12, Is.Not.InRange(5, 10));
            Assert.That(2.57, Is.Not.InRange(-1.0, 1.0));
        }

        [Test]
        public void NotInRangeFails()
        {
            string expectedMessage = string.Format("  Expected: not in range (5,10){0}  But was:  7{0}",
                    Environment.NewLine);

            Assert.That(
                new TestDelegate(FailingNotInRangeMethod),
                Throws.TypeOf(typeof(AssertionException)).With.Message.Contains(expectedMessage));
        }

        private void FailingNotInRangeMethod()
        {
            Assert.That(7, Is.Not.InRange(5, 10));
        }

        // Test on Issue #21 - https://github.com/nunit/nunit-framework/issues/21
        [Test]
        public void ShouldThrowExceptionIfFromIsLessThanTo()
        {
            Assert.That(
                () => Assert.That(12, Is.InRange(10, 5)),
                Throws.ArgumentException);
        }

        [TestCase(9, 9, 10)]
        [TestCase(10, 9, 10)]
        [TestCase(9, 9, 9)]
        public void RangeBoundaryConditions(int actual, int from, int to)
        {
            Assert.That(actual, Is.InRange(from, to));
        }

        [TestCase(5, (short)10, (short)7)]
        [TestCase((short)5, 10, (short)7)]
        [TestCase(5, 10.0, 7)]
        [TestCase(5.0, 10, 7.0)]
        public void MixedRangeTests<TMin, TMax, TVal>(TMin min, TMax max, TVal val)
            where TMin : IComparable
            where TMax : IComparable
            where TVal : IComparable
        {
            Assert.That(val, Is.InRange(min, max));
        }

        [Test, TestCaseSource(nameof(InRangeObjectsWithNoIComparable))]
        public void InRangeNoIComparableTest(object testObj, object from, object to, ObjectToStringComparer comparer)
        {
            Assert.That(testObj, Is.InRange(from, to).Using(comparer));
            Assert.That(true, Is.EqualTo(comparer.WasCalled));
        }

        [Test, TestCaseSource(nameof(NotInRangeObjectsWithNoIComparable))]
        public void NotInRangeNoIComparableTest(object testObj, object from, object to, ObjectToStringComparer comparer)
        {
            Assert.That(testObj, Is.Not.InRange(from, to).Using(comparer));
            Assert.That(true, Is.EqualTo(comparer.WasCalled));
        }
        [TestCaseSource(nameof(InRangeObjectsWithNoIComparableAndNotUsingComparerException))]
        public void InRangeNoIComparableThrowsExceptionTest(object testObj, object from, object to)
        {
            Assert.Throws<ArgumentException>(() => Assert.That(testObj, Is.InRange(from, to)));
        }
        [TestCaseSource(nameof(InRangeObjectsWithNoIComparableAndNotUsingComparerException))]
        public void NotInRangeNoIComparableThrowsExceptionTest(object testObj, object from, object to)
        {
            Assert.Throws<ArgumentException>(() => Assert.That(testObj, Is.Not.InRange(from, to)));
        }

        #region TestCaseSources
        private static System.Collections.IEnumerable InRangeObjectsWithNoIComparableAndNotUsingComparerException()
        {
            var testObj = new NoComparer("M");
            var from = new NoComparer("A");
            var to = new NoComparer("Z");
            var obj1 = new NoComparer(1);
            var obj30 = new NoComparer(30);
            var obj46 = new NoComparer(46);
            yield return new object[] { testObj, from, to };
            yield return new object[] { obj30, obj1, obj46 };
            yield return new object[] { testObj, to, from };
            yield return new object[] { obj30, obj46, obj1 };
        }
        private static System.Collections.IEnumerable InRangeObjectsWithNoIComparable()
        {
            var objN7 = new NoComparer(-7);
            var objN5 = new NoComparer(-5);
            var obj0 = new NoComparer(0);
            var obj1 = new NoComparer(1);
            var obj30 = new NoComparer(30);
            var obj46 = new NoComparer(46);
            var objA = new NoComparer("A");
            var comparer = new ObjectToStringComparer();
            yield return new object[] { obj46, obj0, objA, comparer };
            yield return new object[] { obj30, obj1, obj46, comparer };
            yield return new object[] { obj0, obj0, obj0, comparer };
            yield return new object[] { obj0, objN5, obj46, comparer };
            yield return new object[] { objN5, objN5, objN5, comparer };
            yield return new object[] { objN5, objN7, obj0, comparer };
        }

        private static System.Collections.IEnumerable NotInRangeObjectsWithNoIComparable()
        {
            var objN5 = new NoComparer(-5);
            var obj0 = new NoComparer(0);
            var obj30 = new NoComparer(30);
            var objA = new NoComparer("A");
            var objM = new NoComparer("M");
            var objZ = new NoComparer("Z");
            var comparer = new ObjectToStringComparer();
            yield return new object[] { objN5, obj0, objA, comparer };
            yield return new object[] { objN5, obj0, obj30, comparer };
            yield return new object[] { objA, objM, objZ, comparer };
        }
        #endregion
    }
}
