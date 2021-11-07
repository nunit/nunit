// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt
using System;
using NUnit.Framework;

namespace NUnit.TestData.TheoryFixture
{
    [TestFixture]
    public class TheoryFixture
    {
#pragma warning disable 414
        [Datapoint]
        private int i0 = 0;
        [Datapoint]
        static int i1 = 1;
#pragma warning restore 414
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
            [Values(0, 5)]decimal d)
        {
        }

        [Theory]
        public void TestWithAllDataSuppliedByAttributes(
            [Values(1.0, 2.0)]double d1,
            [Values(3.0, 4.0)]double d2)
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
