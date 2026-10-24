// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

using NUnit.Framework.Constraints;

#pragma warning disable NUnit2041 // Incompatible types for comparison constraint

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class LessThanOrEqualConstraintTests : ComparisonConstraintTestBase
    {
        protected override Constraint TheConstraint { get; } = new LessThanOrEqualConstraint(5);

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "less than or equal to 5";
            StringRepresentation = "<lessthanorequal 5>";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = [
            4, 5,
            (nint)4, (nint)5,
            (nuint)4, (nuint)5,
#if !NETFRAMEWORK
            (Half)4, (Half)5,
            (Int128)4, (Int128)5,
            (UInt128)4, (UInt128)5,
#endif
        ];
        private static readonly object[] FailureData = [
            new object[] { 6, "6" },
            new object[] { (nint)6, "6" },
            new object[] { (nuint)6, "6" },
#if !NETFRAMEWORK
            new object[] { (Half)6, "6" },
            new object[] { (Int128)6, "6" },
            new object[] { (UInt128)6, "6" },
#endif
        ];
#pragma warning restore IDE0052 // Remove unread private members

        [Test]
        public void CanCompareIComparables()
        {
            ClassWithIComparable expected = new ClassWithIComparable(42);
            ClassWithIComparable actual = new ClassWithIComparable(0);
            Assert.That(actual, Is.LessThanOrEqualTo(expected));
        }

        [Test]
        public void CanCompareIComparablesOfT()
        {
            ClassWithIComparableOfT expected = new ClassWithIComparableOfT(42);
            ClassWithIComparableOfT actual = new ClassWithIComparableOfT(0);
            Assert.That(actual, Is.LessThanOrEqualTo(expected));
        }

        [Test]
        public void CanCompareIComparablesOfInt()
        {
            int expected = 42;
            ClassWithIComparableOfT actual = new ClassWithIComparableOfT(0);
            Assert.That(actual, Is.LessThanOrEqualTo(expected));
        }

        private static readonly DateTime ConstantDateTime = new(2024, 1, 1, 1, 1, 1);

        private static IEnumerable<object[]> GetLinearToleranceData()
        {
            yield return new object[] { 4.0, 5.0, 0.05 };
            yield return new object[] { 4.95, 5.0, 0.05 }; // lower range bound
            yield return new object[] { 4.9501, 5.0, 0.05 }; // lower range bound + .01
            yield return new object[] { 4.9999, 5.0, 0.05 };
            yield return new object[] { 5.0001, 5.0, 0.05 };
            yield return new object[] { 5.05, 5.0, 0.05 }; // upper range bound
            yield return new object[] { 190, 200, 5 };
            yield return new object[] { 195, 200, 5 }; // lower range bound
            yield return new object[] { 196, 200, 5 }; // lower range bound + 1
            yield return new object[] { 198, 200, 5 };
            yield return new object[] { 202, 200, 5 };
            yield return new object[] { 205, 200, 5 }; // upper range bound
            yield return new object[] { ConstantDateTime, ConstantDateTime.AddSeconds(-1), TimeSpan.FromSeconds(2) };
            yield return new object[] { ConstantDateTime, ConstantDateTime.AddSeconds(-2), TimeSpan.FromSeconds(2) };

            yield return new object[] { (nint)6, (nint)5, (nint)1 };
            yield return new object[] { (nint)5, (nint)5, (nint)1 };
            yield return new object[] { (nint)4, (nint)5, (nint)1 };
            yield return new object[] { (nuint)16, (nuint)15, (nuint)1 };
            yield return new object[] { (nuint)15, (nuint)15, (nuint)1 };
            yield return new object[] { (nuint)14, (nuint)15, (nuint)1 };
#if !NETFRAMEWORK
            yield return new object[] { (Half)26, (Half)25, (Half)1 };
            yield return new object[] { (Half)25, (Half)25, (Half)1 };
            yield return new object[] { (Half)24, (Half)25, (Half)1 };
            yield return new object[] { (Int128)36, (Int128)35, (Int128)1 };
            yield return new object[] { (Int128)35, (Int128)35, (Int128)1 };
            yield return new object[] { (Int128)34, (Int128)35, (Int128)1 };
            yield return new object[] { (UInt128)46, (UInt128)45, (UInt128)1 };
            yield return new object[] { (UInt128)45, (UInt128)45, (UInt128)1 };
            yield return new object[] { (UInt128)44, (UInt128)45, (UInt128)1 };
#endif
        }

        private static IEnumerable<object[]> GetLinearToleranceData_BoundsData()
        {
            yield return new object[] { byte.MinValue + 1, byte.MinValue, 1 };
            yield return new object[] { byte.MaxValue, byte.MaxValue - 1, 1 };
            yield return new object[] { ushort.MinValue + 1, ushort.MinValue, 1 };
            yield return new object[] { ushort.MaxValue, ushort.MaxValue - 1, 1 };
            yield return new object[] { short.MinValue + 1, short.MinValue, 1 };
            yield return new object[] { short.MaxValue, short.MaxValue - 1, 1 };
            yield return new object[] { uint.MinValue + 1, uint.MinValue, 1 };
            yield return new object[] { uint.MaxValue, uint.MaxValue - 1, 1 };
            yield return new object[] { int.MinValue + 1, int.MinValue, 1 };
            yield return new object[] { int.MaxValue, int.MaxValue - 1, 1 };
            //yield return new object[] { ulong.MinValue + 1, ulong.MinValue, 1 };
            yield return new object[] { ulong.MaxValue, ulong.MaxValue - 1, 1 };
            //yield return new object[] { long.MinValue + 1, long.MinValue, 1 };
            yield return new object[] { long.MaxValue, long.MaxValue - 1, 1 };

            //yield return new object[] { nint.MinValue + 1, nint.MinValue, 1 };
            yield return new object[] { nint.MaxValue, nint.MaxValue - 1, 1 };
            //yield return new object[] { nuint.MinValue + 1, nuint.MinValue, 1 };
            yield return new object[] { nuint.MaxValue, nuint.MaxValue - 1, 1 };
#if !NETFRAMEWORK
            yield return new object[] { Half.MinValue + Half.CreateChecked(1), Half.MinValue, (Half)1 };
            yield return new object[] { Half.MaxValue, Half.MaxValue - Half.CreateChecked(1), (Half)1 };

            yield return new object[] { UInt128.MinValue + 1, UInt128.MinValue, (UInt128)1 };
            yield return new object[] { UInt128.MaxValue, UInt128.MaxValue - 1, (UInt128)1 };
            yield return new object[] { Int128.MinValue + 1, Int128.MinValue, (Int128)1 };
            yield return new object[] { Int128.MaxValue, Int128.MaxValue - 1, (Int128)1 };
#endif
        }

        [TestCaseSource(nameof(GetLinearToleranceData))]
        [TestCaseSource(nameof(GetLinearToleranceData_BoundsData))]
        public void LinearTolerance<TActual, TExpected, TTolerance>(TActual actual, TExpected expected, TTolerance tolerance)
            where TActual : notnull
            where TExpected : notnull
            where TTolerance : notnull
        {
            Assert.That(actual, Is.LessThanOrEqualTo(expected).Within(tolerance));
        }

        private static IEnumerable<object[]> GetLinearToleranceData_FailureData()
        {
            yield return new object[] { 6.0, 5.0, 0.05 };
            yield return new object[] { 210, 200, 5 };
            yield return new object[] { ConstantDateTime, ConstantDateTime.AddSeconds(-2), TimeSpan.FromSeconds(1) };

            yield return new object[] { (nint)7, (nint)5, (nint)1 };
            yield return new object[] { (nuint)17, (nuint)15, (nuint)1 };
#if !NETFRAMEWORK
            yield return new object[] { (Half)27, (Half)25, (Half)1 };
            yield return new object[] { (Int128)37, (Int128)35, (Int128)1 };
            yield return new object[] { (UInt128)47, (UInt128)45, (UInt128)1 };
#endif
        }

        [TestCaseSource(nameof(GetLinearToleranceData_FailureData))]
        public void LinearTolerance_Failure<TActual, TExpected, TTolerance>(TActual actual, TExpected expected, TTolerance tolerance)
            where TActual : notnull
            where TExpected : notnull
            where TTolerance : notnull
        {
            var ex = Assert.Throws<AssertionException>(
                () => Assert.That(actual, Is.LessThanOrEqualTo(expected).Within(tolerance)),
                "Assertion should have failed");

            Assert.That(ex?.Message, Contains.Substring("Expected: less than or equal to " + MsgUtils.FormatValue(expected)));
        }

        private static IEnumerable<object[]> GetPercentToleranceData()
        {
            yield return new object[] { 4.0, 5.0, 1 };
            yield return new object[] { 4.95, 5.0, 1 }; // lower range bound
            yield return new object[] { 4.9501, 5.0, 1 }; // lower range bound + .01
            yield return new object[] { 4.9999, 5.0, 1 };
            yield return new object[] { 5.0001, 5.0, 1 };
            yield return new object[] { 5.05, 5.0, 1 }; // upper range bound
            yield return new object[] { 190, 200, 2.5 };
            yield return new object[] { 195, 200, 2.5 }; // lower range bound
            yield return new object[] { 196, 200, 2.5 }; // lower range bound + 1
            yield return new object[] { 198, 200, 2.5 };
            yield return new object[] { 202, 200, 2.5 };
            yield return new object[] { 205, 200, 2.5 }; // upper range bound

            yield return new object[] { (nint)1010, (nint)1000, 1 };
            yield return new object[] { (nint)980, (nint)1000, 1 };
            yield return new object[] { (nuint)1010, (nuint)1000, 1 };
            yield return new object[] { (nuint)980, (nuint)1000, 1 };
#if !NETFRAMEWORK
            yield return new object[] { (Half)1010, (Half)1000, 1 };
            yield return new object[] { (Half)980, (Half)1000, 1 };
            yield return new object[] { (Int128)1010, (Int128)1000, 1 };
            yield return new object[] { (Int128)980, (Int128)1000, 1 };
            yield return new object[] { (UInt128)1010, (UInt128)1000, 1 };
            yield return new object[] { (UInt128)980, (UInt128)1000, 1 };
#endif
        }

        [TestCaseSource(nameof(GetPercentToleranceData))]
        public void PercentTolerance<TActual, TExpected, TTolerance>(TActual actual, TExpected expected, TTolerance tolerance)
            where TActual : notnull
            where TExpected : notnull
            where TTolerance : notnull
        {
            Assert.That(actual, Is.LessThanOrEqualTo(expected).Within(tolerance).Percent);
        }

        private static IEnumerable<object[]> GetPercentTolerance_FailureData()
        {
            yield return new object[] { 6.0, 5.0, 1 };
            yield return new object[] { 210, 200, 2.5 };

            yield return new object[] { (nint)1020, (nint)1000, 1 };
            yield return new object[] { (nuint)1020, (nuint)1000, 1 };
#if !NETFRAMEWORK
            yield return new object[] { (Half)1020, (Half)1000, 1 };
            yield return new object[] { (Int128)1020, (Int128)1000, 1 };
            yield return new object[] { (UInt128)1020, (UInt128)1000, 1 };
#endif
        }

        [TestCaseSource(nameof(GetPercentTolerance_FailureData))]
        public void PercentTolerance_Failure<TActual, TExpected, TTolerance>(TActual actual, TExpected expected, TTolerance tolerance)
            where TActual : notnull
            where TExpected : notnull
            where TTolerance : notnull
        {
            var ex = Assert.Throws<AssertionException>(
                () => Assert.That(actual, Is.LessThanOrEqualTo(expected).Within(tolerance).Percent),
                "Assertion should have failed");

            Assert.That(ex?.Message, Contains.Substring("Expected: less than or equal to " + MsgUtils.FormatValue(expected) + " within " + MsgUtils.FormatValue(tolerance) + " percent"));
        }
    }
}

#pragma warning restore NUnit2041 // Incompatible types for comparison constraint
