// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

using NUnit.Framework.Constraints;

#pragma warning disable NUnit2041 // Incompatible types for comparison constraint

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
        private static readonly object[] SuccessData = [
            6, 5.001,
            (nint)6,
            (nuint)6,
#if !NETFRAMEWORK
            (Half)6,
            (Int128)6,
            (UInt128)6,
#endif
        ];
        private static readonly object[] FailureData = [
            new object[] { 4, "4" },
            new object[] { 5, "5" },
            new object[] { (nint)4, "4" },
            new object[] { (nint)5, "5" },
            new object[] { (nuint)4, "4" },
            new object[] { (nuint)5, "5" },
#if !NETFRAMEWORK
            new object[] { (Half)4, "4" },
            new object[] { (Half)5, "5" },
            new object[] { (Int128)4, "4" },
            new object[] { (Int128)5, "5" },
            new object[] { (UInt128)4, "4" },
            new object[] { (UInt128)5, "5" },
#endif
        ];
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

        private static IEnumerable<object[]> GetLinearToleranceData_BoundsData()
        {
            yield return new object[] { byte.MinValue, byte.MinValue, 1 };
            yield return new object[] { byte.MaxValue, byte.MaxValue, 1 };
            yield return new object[] { ushort.MinValue, ushort.MinValue, 1 };
            yield return new object[] { ushort.MaxValue, ushort.MaxValue, 1 };
            yield return new object[] { short.MinValue, short.MinValue, 1 };
            yield return new object[] { short.MaxValue, short.MaxValue, 1 };
            yield return new object[] { uint.MinValue, uint.MinValue, 1 };
            yield return new object[] { uint.MaxValue, uint.MaxValue, 1 };
            yield return new object[] { int.MinValue, int.MinValue, 1 };
            yield return new object[] { int.MaxValue, int.MaxValue, 1 };

#if !NETFRAMEWORK
            yield return new object[] { ulong.MinValue, ulong.MinValue, 1 };
            yield return new object[] { ulong.MaxValue, ulong.MaxValue, 1 };
            yield return new object[] { long.MinValue, long.MinValue, 1 };
            yield return new object[] { long.MaxValue, long.MaxValue, 1 };

            yield return new object[] { nint.MinValue, nint.MinValue, 1 };
            yield return new object[] { nint.MaxValue, nint.MaxValue, 1 };
            yield return new object[] { nuint.MinValue, nuint.MinValue, 1 };
            yield return new object[] { nuint.MaxValue, nuint.MaxValue, 1 };
#endif

#if !NETFRAMEWORK
            yield return new object[] { Half.MinValue, Half.MinValue, (Half)1 };
            yield return new object[] { Half.MaxValue, Half.MaxValue, (Half)1 };

            //yield return new object[] { UInt128.MinValue, UInt128.MinValue, (UInt128)1 };
            yield return new object[] { UInt128.MaxValue, UInt128.MaxValue, (UInt128)1 };
            //yield return new object[] { Int128.MinValue, Int128.MinValue, (Int128)1 };
            yield return new object[] { Int128.MaxValue, Int128.MaxValue, (Int128)1 };
#endif
        }

        [TestCaseSource(nameof(GetLinearToleranceData))]
        [TestCaseSource(nameof(GetLinearToleranceData_BoundsData))]
        public void LinearTolerance<TActual, TExpected, TTolerance>(TActual actual, TExpected expected, TTolerance tolerance)
            where TActual : notnull
            where TExpected : notnull
            where TTolerance : notnull
        {
            Assert.That(actual, Is.GreaterThan(expected).Within(tolerance));
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
        public void LinearTolerance_Failure<TActual, TExpected, TTolerance>(TActual actual, TExpected expected, TTolerance tolerance)
            where TActual : notnull
            where TExpected : notnull
            where TTolerance : notnull
        {
            var ex = Assert.Throws<AssertionException>(
                () => Assert.That(actual, Is.GreaterThan(expected).Within(tolerance)),
                "Assertion should have failed");

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

            yield return new object[] { (nint)991, (nint)1000, 1 };
            yield return new object[] { (nint)1010, (nint)1000, 1 };
            yield return new object[] { (nuint)991, (nuint)1000, 1 };
            yield return new object[] { (nuint)1010, (nuint)1000, 1 };
#if !NETFRAMEWORK
            yield return new object[] { (Half)991, (Half)1000, 1 };
            yield return new object[] { (Half)1010, (Half)1000, 1 };
            yield return new object[] { (Int128)991, (Int128)1000, 1 };
            yield return new object[] { (Int128)1010, (Int128)1000, 1 };
            yield return new object[] { (UInt128)991, (UInt128)1000, 1 };
            yield return new object[] { (UInt128)1010, (UInt128)1000, 1 };
#endif
        }

        private static IEnumerable<object[]> GetPercentToleranceData_BoundsData()
        {
            yield return new object[] { byte.MinValue + 1, byte.MinValue + 1, 1 };
            yield return new object[] { byte.MaxValue, byte.MaxValue, 1 };
            yield return new object[] { ushort.MinValue + 1, ushort.MinValue + 1, 1 };
            yield return new object[] { ushort.MaxValue, ushort.MaxValue, 1 };
            yield return new object[] { short.MinValue, short.MinValue, 1 };
            yield return new object[] { short.MaxValue, short.MaxValue, 1 };
            yield return new object[] { uint.MinValue + 1, uint.MinValue + 1, 1 };
            yield return new object[] { uint.MaxValue, uint.MaxValue, 1 };

            yield return new object[] { int.MinValue, int.MinValue, 1 };
            yield return new object[] { int.MaxValue, int.MaxValue, 1 };

            yield return new object[] { ulong.MinValue + 1, ulong.MinValue + 1, 1 };
            yield return new object[] { ulong.MaxValue, ulong.MaxValue, 1 };
            yield return new object[] { long.MinValue, long.MinValue, 1 };
            yield return new object[] { long.MaxValue, long.MaxValue, 1 };

            yield return new object[] { nint.MinValue, nint.MinValue, 1 };
            yield return new object[] { nint.MaxValue, nint.MaxValue, 1 };
            yield return new object[] { nuint.MinValue + 1, nuint.MinValue + 1, 1 };
            yield return new object[] { nuint.MaxValue, nuint.MaxValue, 1 };

#if !NETFRAMEWORK
            yield return new object[] { Half.MinValue, Half.MinValue, (Half)1 };
            yield return new object[] { Half.MaxValue, Half.MaxValue, (Half)1 };

            yield return new object[] { UInt128.MinValue + 1, UInt128.MinValue + 1, (UInt128)1 };
            yield return new object[] { Int128.MinValue, Int128.MinValue, (Int128)1 };
#endif
        }

        [TestCaseSource(nameof(GetPercentToleranceData))]
        [TestCaseSource(nameof(GetPercentToleranceData_BoundsData))]
        public void PercentTolerance<TActual, TExpected, TTolerance>(TActual actual, TExpected expected, TTolerance tolerance)
            where TActual : notnull
            where TExpected : notnull
            where TTolerance : notnull
        {
            Assert.That(actual, Is.GreaterThan(expected).Within(tolerance).Percent);
        }

        private static IEnumerable<object[]> GetPercentTolerance_FailureData()
        {
            yield return new object[] { 4.95, 5.0, 1 }; // lower range bound
            yield return new object[] { 4.9, 5.0, 1 };
            yield return new object[] { 195, 200, 2.5 }; // lower range bound
            yield return new object[] { 190, 200, 2.5 };

            yield return new object[] { (nint)990, (nint)1000, 1 };
            yield return new object[] { (nuint)990, (nuint)1000, 1 };
#if !NETFRAMEWORK
            yield return new object[] { (Half)990, (Half)1000, 1 };
            yield return new object[] { (Int128)990, (Int128)1000, 1 };
            yield return new object[] { (UInt128)990, (UInt128)1000, 1 };
#endif
        }

        [TestCaseSource(nameof(GetPercentTolerance_FailureData))]
        public void PercentTolerance_Failure<TActual, TExpected, TTolerance>(TActual actual, TExpected expected, TTolerance tolerance)
            where TActual : notnull
            where TExpected : notnull
            where TTolerance : notnull
        {
            var ex = Assert.Throws<AssertionException>(
                () => Assert.That(actual, Is.GreaterThan(expected).Within(tolerance).Percent),
                "Assertion should have failed");

            Assert.That(ex?.Message, Contains.Substring("Expected: greater than " + MsgUtils.FormatValue(expected) + " within " + MsgUtils.FormatValue(tolerance) + " percent"));
        }
    }
}

#pragma warning restore NUnit2041 // Incompatible types for comparison constraint
