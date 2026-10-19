// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

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
        private static readonly object[] SuccessData = [
            4, 4.999,
            (nint)4,
            (nuint)4,
#if !NETFRAMEWORK
            (Half)4,
            (Int128)4,
            (UInt128)4,
#endif
        ];
        private static readonly object[] FailureData = [
            new object[] { 6, "6" },
            new object[] { 5, "5" },
            new object[] { (nint)6, "6" },
            new object[] { (nint)5, "5" },
            new object[] { (nuint)6, "6" },
            new object[] { (nuint)5, "5" },
#if !NETFRAMEWORK
            new object[] { (Half)6, "6" },
            new object[] { (Half)5, "5" },
            new object[] { (Int128)6, "6" },
            new object[] { (Int128)5, "5" },
            new object[] { (UInt128)6, "6" },
            new object[] { (UInt128)5, "5" },
#endif
        ];
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

        private static readonly DateTime ConstantDateTime = new(2024, 1, 1, 1, 1, 1);

        private static IEnumerable<object[]> GetLinearToleranceData()
        {
            yield return new object[] { 4.0, 5.0, 0.05 };
            yield return new object[] { 4.95, 5.0, 0.05 }; // lower range bound
            yield return new object[] { 4.9501, 5.0, 0.05 }; // lower range bound + .01
            yield return new object[] { 4.9999, 5.0, 0.05 };
            yield return new object[] { 5.0001, 5.0, 0.05 };
            yield return new object[] { 190, 200, 5 };
            yield return new object[] { 195, 200, 5 }; // lower range bound
            yield return new object[] { 196, 200, 5 }; // lower range bound + 1
            yield return new object[] { 198, 200, 5 };
            yield return new object[] { 202, 200, 5 };
            yield return new object[] { ConstantDateTime, ConstantDateTime.AddSeconds(-1), TimeSpan.FromSeconds(2) };

            yield return new object[] { (nint)5, (nint)5, (nint)1 };
            yield return new object[] { (nint)4, (nint)5, (nint)1 };
            yield return new object[] { (nuint)15, (nuint)15, (nuint)1 };
            yield return new object[] { (nuint)14, (nuint)15, (nuint)1 };
#if !NETFRAMEWORK
            yield return new object[] { (Half)25, (Half)25, (Half)1 };
            yield return new object[] { (Half)24, (Half)25, (Half)1 };
            yield return new object[] { (Int128)35, (Int128)35, (Int128)1 };
            yield return new object[] { (Int128)34, (Int128)35, (Int128)1 };
            yield return new object[] { (UInt128)45, (UInt128)45, (UInt128)1 };
            yield return new object[] { (UInt128)44, (UInt128)45, (UInt128)1 };
#endif
        }

        [TestCaseSource(nameof(GetLinearToleranceData))]
        public void LinearTolerance(object actual, object expected, object tolerance)
        {
#pragma warning disable NUnit2042 // Comparison constraint on object
            Assert.That(actual, Is.LessThan(expected).Within(tolerance));
#pragma warning restore NUnit2042 // Comparison constraint on object
        }

        private static IEnumerable<object[]> GetLinearToleranceData_FailureData()
        {
            yield return new object[] { 5.05, 5.0, 0.05 }; // upper range bound
            yield return new object[] { 6.0, 5.0, 0.05 };
            yield return new object[] { 205, 200, 5 }; // upper range bound
            yield return new object[] { 210, 200, 5 };
            yield return new object[] { ConstantDateTime, ConstantDateTime.AddSeconds(-2), TimeSpan.FromSeconds(2) };
            yield return new object[] { ConstantDateTime, ConstantDateTime.AddSeconds(-2), TimeSpan.FromSeconds(1) };

            yield return new object[] { (nint)6, (nint)5, (nint)1 };
            yield return new object[] { (nuint)16, (nuint)15, (nuint)1 };
#if !NETFRAMEWORK
            yield return new object[] { (Half)26, (Half)25, (Half)1 };
            yield return new object[] { (Int128)36, (Int128)35, (Int128)1 };
            yield return new object[] { (UInt128)46, (UInt128)45, (UInt128)1 };
#endif
        }

        [TestCaseSource(nameof(GetLinearToleranceData_FailureData))]
        public void LinearTolerance_Failure(object actual, object expected, object tolerance)
        {
#pragma warning disable NUnit2042 // Comparison constraint on object
            var ex = Assert.Throws<AssertionException>(
                () => Assert.That(actual, Is.LessThan(expected).Within(tolerance)),
                "Assertion should have failed");
#pragma warning restore NUnit2042 // Comparison constraint on object

            Assert.That(ex?.Message, Contains.Substring("Expected: less than " + MsgUtils.FormatValue(expected) + " within " + MsgUtils.FormatValue(tolerance)));
        }

        private static IEnumerable<object[]> GetPercentToleranceData()
        {
            yield return new object[] { 4.0, 5.0, 1 };
            yield return new object[] { 4.95, 5.0, 1 }; // lower range bound
            yield return new object[] { 4.9501, 5.0, 1 }; // lower range bound + .01
            yield return new object[] { 4.9999, 5.0, 1 };
            yield return new object[] { 5.0001, 5.0, 1 };
            yield return new object[] { 190, 200, 2.5 };
            yield return new object[] { 195, 200, 2.5 }; // lower range bound
            yield return new object[] { 196, 200, 5 }; // lower range bound + 1
            yield return new object[] { 198, 200, 2.5 };
            yield return new object[] { 202, 200, 2.5 };
        }

        [TestCaseSource(nameof(GetPercentToleranceData))]
        public void PercentTolerance(object actual, object expected, object percent)
        {
#pragma warning disable NUnit2042 // Comparison constraint on object
            Assert.That(actual, Is.LessThan(expected).Within(percent).Percent);
#pragma warning restore NUnit2042 // Comparison constraint on object
        }

        private static IEnumerable<object[]> GetPercentTolerance_FailureData()
        {
            yield return new object[] { 5.05, 5.0, 1 }; // upper range bound
            yield return new object[] { 6.0, 5.0, 1 };
            yield return new object[] { 205, 200, 2.5 }; // upper range bound
            yield return new object[] { 210, 200, 2.5 };
        }

        [TestCaseSource(nameof(GetPercentTolerance_FailureData))]
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
