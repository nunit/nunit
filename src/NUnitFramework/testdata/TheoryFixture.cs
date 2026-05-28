// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

namespace NUnit.TestData.TheoryFixture
{
    /// <summary>
    /// Issue #4426: Fixture with non-static DatapointSource and constructor that throws.
    /// The Theory cannot be expanded into parameterized test cases because the non-static
    /// DatapointSource field cannot be read without an instance (constructor throws = no instance).
    /// The Theory is discovered but marked NotRunnable.
    /// </summary>
    [TestFixture]
    public class TheoryWithNonStaticDatapointSourceAndThrowingConstructor
    {
        public TheoryWithNonStaticDatapointSourceAndThrowingConstructor()
        {
            throw new System.InvalidOperationException("Constructor throws");
        }

#pragma warning disable IDE0052 // Remove unused private members
        [DatapointSource]
        private readonly int[] _intSource = [0, 1, 6, 8, 50];
#pragma warning restore IDE0052 // Remove unused private members

        [Theory]
        public void TheoryMethod(int value)
        {
            Assert.That(value, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public void RegularTestMethod()
        {
            Assert.Pass();
        }
    }

    /// <summary>
    /// Issue #4426: Fixture with static DatapointSource and constructor that throws.
    /// Tests should be discovered and report an error (this works correctly).
    /// </summary>
    [TestFixture]
    public class TheoryWithStaticDatapointSourceAndThrowingConstructor
    {
        public TheoryWithStaticDatapointSourceAndThrowingConstructor()
        {
            throw new System.InvalidOperationException("Constructor throws");
        }

#pragma warning disable IDE0052 // Remove unused private members
#pragma warning disable IDE1006 // Naming rule violation
        [DatapointSource]
        private static readonly int[] IntSource = [0, 1, 6, 8, 50];
#pragma warning restore IDE1006 // Naming rule violation
#pragma warning restore IDE0052 // Remove unused private members

        [Theory]
        public void TheoryMethod(int value)
        {
            Assert.That(value, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public void RegularTestMethod()
        {
            Assert.Pass();
        }
    }

    [TestFixture]
    public class TheoryFixture
    {
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CS0414 // The field is assigned but its value is never used
        [Datapoint]
        private readonly int _i0 = 0;
        [Datapoint]
        private static readonly int I1 = 1;
#pragma warning restore CS0414 // The field is assigned but its value is never used
#pragma warning restore IDE0051 // Remove unused private members
        [Datapoint]
        public int I100 = 100;

        [Theory]
        public void TheoryWithNoArguments()
        {
        }

        [Theory]
        public void TheoryWithArgumentsButNoDatapoints(decimal x, decimal y)
        {
        }

        [Theory]
        public void TestWithUnsupportedNullableTypeArgumentWithNoDataPoints(decimal? x)
        {
        }

        [Theory]
        public void TheoryWithArgumentsAndDatapoints(int x, int y)
        {
        }

        [TestCase(5, 10)]
        [TestCase(3, 12)]
        public void TestWithArguments(int x, int y)
        {
        }

        [Theory]
        public void TestWithBooleanArguments(bool a, bool b)
        {
        }

        [Theory]
        public void TestWithNullableBooleanArguments(bool? a, bool? b)
        {
        }

        [Theory]
        public void TestWithEnumAsArgument(System.AttributeTargets targets)
        {
        }

        [Theory]
        public void TestWithNullableEnumAsArgument(System.AttributeTargets? targets)
        {
        }

        [Theory]
        public void TestWithBothDatapointAndAttributeData(
            int i,
            [Values(0, 5)] decimal d)
        {
        }

        [Theory]
        public void TestWithAllDataSuppliedByAttributes(
            [Values(1.0, 2.0)] double d1,
            [Values(3.0, 4.0)] double d2)
        {
        }

        [Theory]
        public void TestWithAllBadValues(
            [Values(-12.0, -4.0, -9.0)] double d)
        {
            Assume.That(d > 0);
            Assert.Pass();
        }

        public class NestedWhileSearchingInDeclaringType
        {
            [Theory(true)]
            public void WithNoArguments()
            {
            }

            [Theory(true)]
            public void WithArgumentsButNoDatapoints(decimal x, decimal y)
            {
            }

            [Theory(true)]
            public void WithUnsupportedNullableTypeArgumentWithNoDataPoints(decimal? x)
            {
            }

            [Theory(true)]
            public void WithArgumentsAndDatapoints(int x, int y)
            {
            }

            [Theory(true)]
            public void WithBooleanArguments(bool a, bool b)
            {
            }

            [Theory(true)]
            public void WithNullableBooleanArguments(bool? a, bool? b)
            {
            }

            [Theory(true)]
            public void WithEnumAsArgument(System.AttributeTargets targets)
            {
            }

            [Theory(true)]
            public void WithNullableEnumAsArgument(System.AttributeTargets? targets)
            {
            }

            [Theory(true)]
            public void WithBothDatapointAndAttributeData(
                int i,
                [Values(0, 5)] decimal d)
            {
            }

            [Theory(true)]
            public void WithAllDataSuppliedByAttributes(
                [Values(1.0, 2.0)] double d1,
                [Values(3.0, 4.0)] double d2)
            {
            }

            [Theory(true)]
            public void WithAllBadValues(
                [Values(-12.0, -4.0, -9.0)] double d)
            {
                Assume.That(d > 0);
                Assert.Pass();
            }
        }
    }
}
