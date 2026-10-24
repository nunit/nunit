// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

using NUnit.Framework.Constraints;

#pragma warning disable NUnit2041 // Incompatible types for comparison constraint

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

        private static IEnumerable<object[]> GetLinearToleranceData_BoundsData()
        {
            yield return new object[] { byte.MinValue, byte.MinValue + 1, 1 };
            yield return new object[] { byte.MaxValue - 1, byte.MaxValue, 1 };
            yield return new object[] { ushort.MinValue, ushort.MinValue + 1, 1 };
            yield return new object[] { ushort.MaxValue - 1, ushort.MaxValue, 1 };
            yield return new object[] { short.MinValue, short.MinValue + 1, 1 };
            yield return new object[] { short.MaxValue - 1, short.MaxValue, 1 };
            yield return new object[] { uint.MinValue, uint.MinValue + 1, 1 };
            yield return new object[] { uint.MaxValue - 1, uint.MaxValue, 1 };
            yield return new object[] { int.MinValue, int.MinValue + 1, 1 };
            yield return new object[] { int.MaxValue - 1, int.MaxValue, 1 };
            yield return new object[] { ulong.MinValue, ulong.MinValue + 1, 1 };
            //yield return new object[] { ulong.MaxValue - 1, ulong.MaxValue, 1 };
            yield return new object[] { long.MinValue, long.MinValue + 1, 1 };
            //yield return new object[] { long.MaxValue - 1, long.MaxValue, 1 };

            yield return new object[] { nint.MinValue, nint.MinValue + 1, 1 };
            //yield return new object[] { nint.MaxValue - 1, nint.MaxValue, 1 };
            yield return new object[] { nuint.MinValue, nuint.MinValue + 1, 1 };
            //yield return new object[] { nuint.MaxValue - 1, nuint.MaxValue, 1 };
#if !NETFRAMEWORK
            yield return new object[] { (Half)(-65472), (Half)(-65500), (Half)1 };
            yield return new object[] { (Half)65500, (Half)65472, (Half)1 };

            yield return new object[] { UInt128.MinValue, UInt128.MinValue + 1, (UInt128)1 };
            yield return new object[] { UInt128.MaxValue - 1, UInt128.MaxValue, (UInt128)1 };
            yield return new object[] { Int128.MinValue, Int128.MinValue + 1, (Int128)1 };
            yield return new object[] { Int128.MaxValue - 1, Int128.MaxValue, (Int128)1 };
#endif
        }

        [TestCaseSource(nameof(GetLinearToleranceData))]
        [TestCaseSource(nameof(GetLinearToleranceData_BoundsData))]
        public void LinearTolerance<TActual, TExpected, TTolerance>(TActual actual, TExpected expected, TTolerance tolerance)
            where TActual : notnull
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
            where TActual : notnull
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

            yield return new object[] { (nint)990, (nint)1000, 1 };
            yield return new object[] { (nint)1020, (nint)1000, 1 };
            yield return new object[] { (nuint)990, (nuint)1000, 1 };
            yield return new object[] { (nuint)1020, (nuint)1000, 1 };
#if !NETFRAMEWORK
            yield return new object[] { (Half)990, (Half)1000, 1 };
            yield return new object[] { (Half)1020, (Half)1000, 1 };
            yield return new object[] { (Int128)990, (Int128)1000, 1 };
            yield return new object[] { (Int128)1020, (Int128)1000, 1 };
            yield return new object[] { (UInt128)990, (UInt128)1000, 1 };
            yield return new object[] { (UInt128)1020, (UInt128)1000, 1 };
#endif
        }

        [TestCaseSource(nameof(GetPercentToleranceData))]
        public void PercentTolerance<TActual, TExpected, TTolerance>(TActual actual, TExpected expected, TTolerance tolerance)
            where TActual : notnull
            where TExpected : notnull
            where TTolerance : notnull
        {
            Assert.That(actual, Is.GreaterThanOrEqualTo(expected).Within(tolerance).Percent);
        }

        private static IEnumerable<object[]> GetPercentTolerance_FailureData()
        {
            yield return new object[] { 4.9, 5.0, 1 };
            yield return new object[] { 190, 200, 2.5 };

            yield return new object[] { (nint)980, (nint)1000, 1 };
            yield return new object[] { (nuint)980, (nuint)1000, 1 };
#if !NETFRAMEWORK
            yield return new object[] { (Half)980, (Half)1000, 1 };
            yield return new object[] { (Int128)980, (Int128)1000, 1 };
            yield return new object[] { (UInt128)980, (UInt128)1000, 1 };
#endif
        }

        [TestCaseSource(nameof(GetPercentTolerance_FailureData))]
        public void PercentTolerance_Failure<TActual, TExpected, TTolerance>(TActual actual, TExpected expected, TTolerance tolerance)
            where TActual : notnull
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

#pragma warning restore NUnit2041 // Incompatible types for comparison constraint
