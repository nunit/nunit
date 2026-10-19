// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class GreaterThanConstraintTests : ComparisonConstraintTestBase
    {
        protected override Constraint TheConstraint { get; } = new GreaterThanConstraint(5);

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "greater than 5";
            StringRepresentation = "<greaterthan 5>";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = new object[] { 6, 5.001 };
        private static readonly object[] FailureData = new object[] { new object[] { 4, "4" }, new object[] { 5, "5" } };
#pragma warning restore IDE0052 // Remove unread private members

        [Test]
        public void CanCompareIComparables()
        {
            ClassWithIComparable expected = new ClassWithIComparable(0);
            ClassWithIComparable actual = new ClassWithIComparable(42);
            Assert.That(actual, Is.GreaterThan(expected));
        }

        [Test]
        public void CanCompareIComparablesOfT()
        {
            ClassWithIComparableOfT expected = new ClassWithIComparableOfT(0);
            ClassWithIComparableOfT actual = new ClassWithIComparableOfT(42);
            Assert.That(actual, Is.GreaterThan(expected));
        }

        [Test]
        public void CanCompareIComparablesOfInt()
        {
            int expected = 0;
            ClassWithIComparableOfT actual = new ClassWithIComparableOfT(42);
            Assert.That(actual, Is.GreaterThan(expected));
        }

        private static readonly DateTime ConstantDateTime = new(2024, 1, 1, 1, 1, 1);

        private static IEnumerable<object[]> GetLinearToleranceData()
        {
            yield return new object[] { 6.0, 5.0, 0.05 };
            yield return new object[] { 5.05, 5.0, 0.05 }; // upper range bound
            yield return new object[] { 5.0001, 5.0, 0.05 };
            yield return new object[] { 4.9999, 5.0, 0.05 };
            yield return new object[] { 4.9501, 5.0, 0.05 }; // lower range bound + .01
            yield return new object[] { 210, 200, 5 };
            yield return new object[] { 205, 200, 5 }; // upper range bound
            yield return new object[] { 202, 200, 5 };
            yield return new object[] { 198, 200, 5 };
            yield return new object[] { 196, 200, 5 }; // lower range bound + 1
            yield return new object[] { ConstantDateTime, ConstantDateTime.AddSeconds(1), TimeSpan.FromSeconds(2) };

            yield return new object[] { (nint)5, (nint)5, (nint)1 };
            yield return new object[] { (nint)6, (nint)5, (nint)1 };
            yield return new object[] { (nuint)15, (nuint)15, (nuint)1 };
            yield return new object[] { (nuint)16, (nuint)15, (nuint)1 };
#if !NETFRAMEWORK
            yield return new object[] { (Half)25, (Half)25, (Half)1 };
            yield return new object[] { (Half)26, (Half)25, (Half)1 };
            yield return new object[] { (Int128)35, (Int128)35, (Int128)1 };
            yield return new object[] { (Int128)36, (Int128)35, (Int128)1 };
            yield return new object[] { (UInt128)45, (UInt128)45, (UInt128)1 };
            yield return new object[] { (UInt128)46, (UInt128)45, (UInt128)1 };
#endif
        }

        [TestCaseSource(nameof(GetLinearToleranceData))]
        public void LinearTolerance(object actual, object expected, object tolerance)
        {
#pragma warning disable NUnit2042 // Comparison constraint on object
            Assert.That(actual, Is.GreaterThan(expected).Within(tolerance));
#pragma warning restore NUnit2042 // Comparison constraint on object
        }

        private static IEnumerable<object[]> GetLinearToleranceData_FailureData()
        {
            yield return new object[] { 4.95, 5.0, 0.05 }; // lower range bound
            yield return new object[] { 4.9, 5.0, 0.05 };
            yield return new object[] { 195, 200, 5 }; // lower range bound
            yield return new object[] { 190, 200, 5 };
            yield return new object[] { ConstantDateTime, ConstantDateTime.AddSeconds(2), TimeSpan.FromSeconds(1) };
            yield return new object[] { ConstantDateTime, ConstantDateTime.AddSeconds(2), TimeSpan.FromSeconds(2) };

            yield return new object[] { (nint)4, (nint)5, (nint)1 };
            yield return new object[] { (nuint)14, (nuint)15, (nuint)1 };
#if !NETFRAMEWORK
            yield return new object[] { (Half)24, (Half)25, (Half)1 };
            yield return new object[] { (Int128)34, (Int128)35, (Int128)1 };
            yield return new object[] { (UInt128)44, (UInt128)45, (UInt128)1 };
#endif
        }

        [TestCaseSource(nameof(GetLinearToleranceData_FailureData))]
        public void LinearTolerance_Failure(object actual, object expected, object tolerance)
        {
#pragma warning disable NUnit2042 // Comparison constraint on object
            var ex = Assert.Throws<AssertionException>(
                () => Assert.That(actual, Is.GreaterThan(expected).Within(tolerance)),
                "Assertion should have failed");
#pragma warning restore NUnit2042 // Comparison constraint on object

            Assert.That(ex?.Message, Contains.Substring("Expected: greater than " + MsgUtils.FormatValue(expected)));
        }

        private static IEnumerable<object[]> GetPercentToleranceData()
        {
            yield return new object[] { 6.0, 5.0, 1 };
            yield return new object[] { 5.05, 5.0, 1 }; // upper range bound
            yield return new object[] { 5.0001, 5.0, 1 };
            yield return new object[] { 4.9999, 5.0, 1 };
            yield return new object[] { 4.9501, 5.0, 1 }; // lower range bound + .01
            yield return new object[] { 210, 200, 2.5 };
            yield return new object[] { 205, 200, 2.5 }; // upper range bound
            yield return new object[] { 202, 200, 2.5 };
            yield return new object[] { 198, 200, 2.5 };
            yield return new object[] { 196, 200, 2.5 }; // lower range bound + 1
        }

        [TestCaseSource(nameof(GetPercentToleranceData))]
        public void PercentTolerance(object actual, object expected, object tolerance)
        {
#pragma warning disable NUnit2042 // Comparison constraint on object
            Assert.That(actual, Is.GreaterThan(expected).Within(tolerance).Percent);
#pragma warning restore NUnit2042 // Comparison constraint on object
        }

        private static IEnumerable<object[]> GetPercentTolerance_FailureData()
        {
            yield return new object[] { 4.95, 5.0, 1 }; // lower range bound
            yield return new object[] { 4.9, 5.0, 1 };
            yield return new object[] { 195, 200, 2.5 }; // lower range bound
            yield return new object[] { 190, 200, 2.5 };
        }

        [TestCaseSource(nameof(GetPercentTolerance_FailureData))]
        public void PercentTolerance_Failure(object actual, object expected, object tolerance)
        {
#pragma warning disable NUnit2042 // Comparison constraint on object
            var ex = Assert.Throws<AssertionException>(
                () => Assert.That(actual, Is.GreaterThan(expected).Within(tolerance).Percent),
                "Assertion should have failed");
#pragma warning restore NUnit2042 // Comparison constraint on object

            Assert.That(ex?.Message, Contains.Substring("Expected: greater than " + MsgUtils.FormatValue(expected) + " within " + MsgUtils.FormatValue(tolerance) + " percent"));
        }
    }
}
