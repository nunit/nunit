// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class LessThanConstraintTests : ComparisonConstraintTestBase
    {
        protected override Constraint TheConstraint { get; } = new LessThanConstraint(5);

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "less than 5";
            StringRepresentation = "<lessthan 5>";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = new object[] { 4, 4.999 };
        private static readonly object[] FailureData = new object[] { new object[] { 6, "6" }, new object[] { 5, "5" } };
#pragma warning restore IDE0052 // Remove unread private members

        [Test]
        public void CanCompareIComparables()
        {
            ClassWithIComparable expected = new ClassWithIComparable(42);
            ClassWithIComparable actual = new ClassWithIComparable(0);
            Assert.That(actual, Is.LessThan(expected));
        }

        [Test]
        public void CanCompareIComparablesOfT()
        {
            ClassWithIComparableOfT expected = new ClassWithIComparableOfT(42);
            ClassWithIComparableOfT actual = new ClassWithIComparableOfT(0);
            Assert.That(actual, Is.LessThan(expected));
        }

        [Test]
        public void CanCompareIComparablesOfInt()
        {
            int expected = 42;
            ClassWithIComparableOfT actual = new ClassWithIComparableOfT(0);
            Assert.That(actual, Is.LessThan(expected));
        }

        [TestCase(4.0, 5.0, 0.05)]
        [TestCase(4.95, 5.0, 0.05)] // lower range bound
        [TestCase(4.9501, 5.0, 0.05)] // lower range bound + .01
        [TestCase(4.9999, 5.0, 0.05)]
        [TestCase(5.0001, 5.0, 0.05)]
        [TestCase(190, 200, 5)]
        [TestCase(195, 200, 5)] // lower range bound
        [TestCase(196, 200, 5)] // lower range bound + 1
        [TestCase(198, 200, 5)]
        [TestCase(202, 200, 5)]
        public void SimpleTolerance(object actual, object expected, object tolerance)
        {
#pragma warning disable NUnit2042 // Comparison constraint on object
            Assert.That(actual, Is.LessThan(expected).Within(tolerance));
#pragma warning restore NUnit2042 // Comparison constraint on object
        }

        [TestCase(5.05, 5.0, 0.05)] // upper range bound
        [TestCase(6.0, 5.0, 0.05)]
        [TestCase(205, 200, 5)] // upper range bound
        [TestCase(210, 200, 5)]
        public void SimpleTolerance_Failure(object actual, object expected, object tolerance)
        {
#pragma warning disable NUnit2042 // Comparison constraint on object
            var ex = Assert.Throws<AssertionException>(
                () => Assert.That(actual, Is.LessThan(expected).Within(tolerance)),
                "Assertion should have failed");
#pragma warning restore NUnit2042 // Comparison constraint on object

            Assert.That(ex?.Message, Contains.Substring("Expected: less than " + MsgUtils.FormatValue(expected) + " within " + MsgUtils.FormatValue(tolerance)));
        }

        [TestCase(4.0, 5.0, 1)]
        [TestCase(4.95, 5.0, 1)] // lower range bound
        [TestCase(4.9501, 5.0, 1)] // lower range bound + .01
        [TestCase(4.9999, 5.0, 1)]
        [TestCase(5.0001, 5.0, 1)]
        [TestCase(190, 200, 2.5)]
        [TestCase(195, 200, 2.5)] // lower range bound
        [TestCase(196, 200, 5)] // lower range bound + 1
        [TestCase(198, 200, 2.5)]
        [TestCase(202, 200, 2.5)]
        public void PercentTolerance(object actual, object expected, object percent)
        {
#pragma warning disable NUnit2042 // Comparison constraint on object
            Assert.That(actual, Is.LessThan(expected).Within(percent).Percent);
#pragma warning restore NUnit2042 // Comparison constraint on object
        }

        [TestCase(5.05, 5.0, 1)] // upper range bound
        [TestCase(6.0, 5.0, 1)]
        [TestCase(205, 200, 2.5)] // upper range bound
        [TestCase(210, 200, 2.5)]
        public void PercentTolerance_Failure(object actual, object expected, object tolerance)
        {
#pragma warning disable NUnit2042 // Comparison constraint on object
            var ex = Assert.Throws<AssertionException>(
                () => Assert.That(actual, Is.LessThan(expected).Within(tolerance).Percent),
                "Assertion should have failed");
#pragma warning restore NUnit2042 // Comparison constraint on object

            Assert.That(ex?.Message, Contains.Substring("Expected: less than " + MsgUtils.FormatValue(expected) + " within " + MsgUtils.FormatValue(tolerance) + " percent"));
        }
    }
}
