// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using NUnit.Framework.Constraints;
using NUnit.Framework.Tests.TestUtilities.Comparers;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class RangeConstraintTest : ConstraintTestBase
    {
        private readonly RangeConstraint _rangeConstraint = new RangeConstraint(5, 42);

        protected override Constraint TheConstraint => _rangeConstraint;

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "in range (5,42)";
            StringRepresentation = "<range 5 42>";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = [5, 23, 42];
        private static readonly object[] FailureData = [new object[] { 4, "4" }, new object[] { 43, "43" }];
#pragma warning restore IDE0052 // Remove unread private members

        [TestCase(null)]
        [TestCase("xxx")]
        public void InvalidDataThrowsArgumentException(object? data)
        {
            Assert.Throws<ArgumentException>(() => TheConstraint.ApplyTo(data));
        }

        [Test]
        public void UsesProvidedIComparer()
        {
            var comparer = new ObjectComparer();
            Assert.That(_rangeConstraint.Using(comparer).ApplyTo(19).IsSuccess);
            Assert.That(comparer.WasCalled, "Comparer was not called");
        }

        [Test]
        public void UsesProvidedGenericComparer()
        {
            var comparer = new GenericComparer<int>();
            Assert.That(_rangeConstraint.Using(comparer).ApplyTo(19).IsSuccess);
            Assert.That(comparer.WasCalled, "Comparer was not called");
        }

        [Test]
        public void UsesProvidedGenericComparison()
        {
            var comparer = new GenericComparison<int>();
            Assert.That(_rangeConstraint.Using(comparer.Delegate).ApplyTo(19).IsSuccess);
            Assert.That(comparer.WasCalled, "Comparer was not called");
        }

        [Test]
        public void UsesProvidedLambda()
        {
            Comparison<int> comparer = (x, y) => x.CompareTo(y);
            Assert.That(_rangeConstraint.Using(comparer).ApplyTo(19).IsSuccess);
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
            var test = new RangeConstraint(5, 42);
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
        public void RangeConstructorComparerThrowExceptionIfFromIsLessThanTo(object testObj, object from, object to, System.Collections.IComparer comparer)
        {
            var test = new RangeConstraint(from, to);
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
