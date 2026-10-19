// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class GreaterThanOrEqualConstraintTests : ComparisonConstraintTestBase
    {
        protected override Constraint TheConstraint { get; } = new GreaterThanOrEqualConstraint(5);

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "greater than or equal to 5";
            StringRepresentation = "<greaterthanorequal 5>";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = [
            6, 5,
            (nint)6, (nint)5,
            (nuint)6, (nuint)5,
#if !NETFRAMEWORK
            (Half)6, (Half)5,
            (Int128)6, (Int128)5,
            (UInt128)6, (UInt128)5,
#endif
        ];
        private static readonly object[] FailureData = [
            new object[] { 4, "4" },
            new object[] { (nint)4, "4" },
            new object[] { (nuint)4, "4" },
#if !NETFRAMEWORK
            new object[] { (Half)4, "4" },
            new object[] { (Int128)4, "4" },
            new object[] { (UInt128)4, "4" },
#endif
        ];
#pragma warning restore IDE0052 // Remove unread private members

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

        private static readonly DateTime ConstantDateTime = new(2024, 1, 1, 1, 1, 1);

        private static IEnumerable<object[]> GetLinearToleranceData()
        {
            yield return new object[] { 6.0, 5.0, 0.05 };
            yield return new object[] { 5.05, 5.0, 0.05 }; // upper range bound
            yield return new object[] { 5.0001, 5.0, 0.05 };
            yield return new object[] { 4.9999, 5.0, 0.05 };
            yield return new object[] { 4.9501, 5.0, 0.05 }; // lower range bound + .01
            yield return new object[] { 4.95, 5.0, 0.05 }; // lower range bound
            yield return new object[] { 210, 200, 5 };
            yield return new object[] { 205, 200, 5 }; // upper range bound
            yield return new object[] { 202, 200, 5 };
            yield return new object[] { 198, 200, 5 };
            yield return new object[] { 196, 200, 5 }; // lower range bound + 1
            yield return new object[] { 195, 200, 5 }; // lower range bound
            yield return new object[] { ConstantDateTime, ConstantDateTime.AddSeconds(1), TimeSpan.FromSeconds(2) };
            yield return new object[] { ConstantDateTime, ConstantDateTime.AddSeconds(2), TimeSpan.FromSeconds(2) };

            yield return new object[] { (nint)4, (nint)5, (nint)1 };
            yield return new object[] { (nint)5, (nint)5, (nint)1 };
            yield return new object[] { (nint)6, (nint)5, (nint)1 };
            yield return new object[] { (nuint)14, (nuint)15, (nuint)1 };
            yield return new object[] { (nuint)15, (nuint)15, (nuint)1 };
            yield return new object[] { (nuint)16, (nuint)15, (nuint)1 };
#if !NETFRAMEWORK
            yield return new object[] { (Half)24, (Half)25, (Half)1 };
            yield return new object[] { (Half)25, (Half)25, (Half)1 };
            yield return new object[] { (Half)26, (Half)25, (Half)1 };
            yield return new object[] { (Int128)34, (Int128)35, (Int128)1 };
            yield return new object[] { (Int128)35, (Int128)35, (Int128)1 };
            yield return new object[] { (Int128)36, (Int128)35, (Int128)1 };
            yield return new object[] { (UInt128)44, (UInt128)45, (UInt128)1 };
            yield return new object[] { (UInt128)45, (UInt128)45, (UInt128)1 };
            yield return new object[] { (UInt128)46, (UInt128)45, (UInt128)1 };
#endif
        }

        [TestCaseSource(nameof(GetLinearToleranceData))]
        public void LinearTolerance<TActual, TExpected, TTolerance>(TActual actual, TExpected expected, TTolerance tolerance)
            where TActual : notnull, IComparable<TExpected>
            where TExpected : notnull
            where TTolerance : notnull
        {
            Assert.That(actual, Is.GreaterThanOrEqualTo(expected).Within(tolerance));
        }

        private static IEnumerable<object[]> GetLinearToleranceData_FailureData()
        {
            yield return new object[] { 4.9, 5.0, 0.05 };
            yield return new object[] { 190, 200, 5 };
            yield return new object[] { ConstantDateTime, ConstantDateTime.AddSeconds(2), TimeSpan.FromSeconds(1) };

            yield return new object[] { (nint)3, (nint)5, (nint)1 };
            yield return new object[] { (nuint)13, (nuint)15, (nuint)1 };
#if !NETFRAMEWORK
            yield return new object[] { (Half)23, (Half)25, (Half)1 };
            yield return new object[] { (Int128)33, (Int128)35, (Int128)1 };
            yield return new object[] { (UInt128)43, (UInt128)45, (UInt128)1 };
#endif
        }

        [TestCaseSource(nameof(GetLinearToleranceData_FailureData))]
        public void LinearTolerance_Failure<TActual, TExpected, TTolerance>(TActual actual, TExpected expected, TTolerance tolerance)
            where TActual : notnull, IComparable<TExpected>
            where TExpected : notnull
            where TTolerance : notnull
        {
            var ex = Assert.Throws<AssertionException>(
                () => Assert.That(actual, Is.GreaterThanOrEqualTo(expected).Within(tolerance)),
                "Assertion should have failed");

            Assert.That(ex?.Message, Contains.Substring("Expected: greater than or equal to " + MsgUtils.FormatValue(expected)));
        }

        private static IEnumerable<object[]> GetPercentToleranceData()
        {
            yield return new object[] { 6.0, 5.0, 1 };
            yield return new object[] { 5.05, 5.0, 1 }; // upper range bound
            yield return new object[] { 5.0001, 5.0, 1 };
            yield return new object[] { 4.9999, 5.0, 1 };
            yield return new object[] { 4.9501, 5.0, 1 }; // lower range bound + .01
            yield return new object[] { 4.95, 5.0, 1 }; // lower range bound
            yield return new object[] { 210, 200, 2.5 };
            yield return new object[] { 205, 200, 2.5 }; // upper range bound
            yield return new object[] { 202, 200, 2.5 };
            yield return new object[] { 198, 200, 2.5 };
            yield return new object[] { 196, 200, 2.5 }; // lower range bound + 1
            yield return new object[] { 195, 200, 2.5 }; // lower range bound
        }

        [TestCaseSource(nameof(GetPercentToleranceData))]
        public void PercentTolerance<TActual, TExpected, TTolerance>(TActual actual, TExpected expected, TTolerance tolerance)
            where TActual : notnull, IComparable<TExpected>
            where TExpected : notnull
            where TTolerance : notnull
        {
            Assert.That(actual, Is.GreaterThanOrEqualTo(expected).Within(tolerance).Percent);
        }

        private static IEnumerable<object[]> GetPercentTolerance_FailureData()
        {
            yield return new object[] { 4.9, 5.0, 1 };
            yield return new object[] { 190, 200, 2.5 };
        }

        [TestCaseSource(nameof(GetPercentTolerance_FailureData))]
        public void PercentTolerance_Failure<TActual, TExpected, TTolerance>(TActual actual, TExpected expected, TTolerance tolerance)
            where TActual : notnull, IComparable<TExpected>
            where TExpected : notnull
            where TTolerance : notnull
        {
            var ex = Assert.Throws<AssertionException>(
                () => Assert.That(actual, Is.GreaterThanOrEqualTo(expected).Within(tolerance).Percent),
                "Assertion should have failed");

            Assert.That(ex?.Message, Contains.Substring("Expected: greater than or equal to " + MsgUtils.FormatValue(expected) + " within " + MsgUtils.FormatValue(tolerance) + " percent"));
        }
    }
}
