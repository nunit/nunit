// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class GreaterThanOrEqualConstraintTests : ComparisonConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = ComparisonConstraint = new GreaterThanOrEqualConstraint(5);
            ExpectedDescription = "greater than or equal to 5";
            StringRepresentation = "<greaterthanorequal 5>";
        }

        static object[] SuccessData = new object[] { 6, 5 };

        static object[] FailureData = new object[] { new object[] { 4, "4" } };

        [Test]
        public void CanCompareIComparables()
        {
            ClassWithIComparable expected = new ClassWithIComparable(0);
            ClassWithIComparable actual = new ClassWithIComparable(42);
            Assert.That(actual, Is.GreaterThanOrEqualTo(expected));
        }

        [Test]
        public void CanCompareIComparablesOfT()
        {
            ClassWithIComparableOfT expected = new ClassWithIComparableOfT(0);
            ClassWithIComparableOfT actual = new ClassWithIComparableOfT(42);
            Assert.That(actual, Is.GreaterThanOrEqualTo(expected));
        }

        [Test]
        public void CanCompareIComparablesOfInt()
        {
            int expected = 0;
            ClassWithIComparableOfT actual = new ClassWithIComparableOfT(42);
            Assert.That(actual, Is.GreaterThanOrEqualTo(expected));
        }

        [TestCase(6.0, 5.0, 0.05)]
        [TestCase(5.05, 5.0, 0.05)] // upper range bound
        [TestCase(5.0001, 5.0, 0.05)]
        [TestCase(4.9999, 5.0, 0.05)]
        [TestCase(4.9501, 5.0, 0.05)] // lower range bound + .01
        [TestCase(4.95, 5.0, 0.05)] // lower range bound
        [TestCase(210, 200, 5)]
        [TestCase(205, 200, 5)] // upper range bound
        [TestCase(202, 200, 5)]
        [TestCase(198, 200, 5)]
        [TestCase(196, 200, 5)] // lower range bound + 1
        [TestCase(195, 200, 5)] // lower range bound
        public void SimpleTolerance(object actual, object expected, object tolerance)
        {
            Assert.That(actual, Is.GreaterThanOrEqualTo(expected).Within(tolerance));
        }

        [TestCase(4.9, 5.0, 0.05)]
        [TestCase(190, 200, 5)]
        public void SimpleTolerance_Failure(object actual, object expected, object tolerance)
        {
            var ex = Assert.Throws<AssertionException>(
                () => Assert.That(actual, Is.GreaterThanOrEqualTo(expected).Within(tolerance)),
                "Assertion should have failed");

            Assert.That(ex.Message, Contains.Substring("Expected: greater than or equal to " + expected.ToString()));
        }

        [TestCase(6.0, 5.0, 1)]
        [TestCase(5.05, 5.0, 1)] // upper range bound
        [TestCase(5.0001, 5.0, 1)]
        [TestCase(4.9999, 5.0, 1)]
        [TestCase(4.9501, 5.0, 1)] // lower range bound + .01
        [TestCase(4.95, 5.0, 1)] // lower range bound
        [TestCase(210, 200, 2.5)]
        [TestCase(205, 200, 2.5)] // upper range bound
        [TestCase(202, 200, 2.5)]
        [TestCase(198, 200, 2.5)]
        [TestCase(196, 200, 2.5)] // lower range bound + 1
        [TestCase(195, 200, 2.5)] // lower range bound
        public void PercentTolerance(object actual, object expected, object tolerance)
        {
            Assert.That(actual, Is.GreaterThanOrEqualTo(expected).Within(tolerance).Percent);
        }

        [TestCase(4.9, 5.0, 1)]
        [TestCase(190, 200, 2.5)]
        public void PercentTolerance_Failure(object actual, object expected, object tolerance)
        {
            var ex = Assert.Throws<AssertionException>(
                () => Assert.That(actual, Is.GreaterThanOrEqualTo(expected).Within(tolerance).Percent),
                "Assertion should have failed");

            Assert.That(ex.Message, Contains.Substring("Expected: greater than or equal to " + MsgUtils.FormatValue(expected) + " within " + MsgUtils.FormatValue(tolerance) + " percent" ));
        }
    }
}
