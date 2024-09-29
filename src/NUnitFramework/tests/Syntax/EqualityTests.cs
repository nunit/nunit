// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;

namespace NUnit.Framework.Tests.Syntax
{
    public class EqualToTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<equal 999>";
            StaticSyntax = Is.EqualTo(999);
            BuilderSyntax = Builder().EqualTo(999);
        }
    }

    public class EqualToTest_IgnoreCase : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = @"<equal ""X"">";
            StaticSyntax = Is.EqualTo("X").IgnoreCase;
            BuilderSyntax = Builder().EqualTo("X").IgnoreCase;
        }
    }

    public class EqualToTest_WithinTolerance : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<equal 0.7>";
            StaticSyntax = Is.EqualTo(0.7).Within(.005);
            BuilderSyntax = Builder().EqualTo(0.7).Within(.005);
        }
    }

    public class EqualityTests
    {
        [Test]
        public void SimpleEqualityTests()
        {
            int[] i3 = new[] { 1, 2, 3 };
            double[] d3 = new[] { 1.0, 2.0, 3.0 };
            int[] iunequal = new[] { 1, 3, 2 };

            Assert.That(2 + 2, Is.EqualTo(4));
            Assert.That(2 + 2 == 4);
            Assert.That(i3, Is.EqualTo(d3));
            Assert.That(2 + 2, Is.Not.EqualTo(5));
            Assert.That(i3, Is.Not.EqualTo(iunequal));
            List<string> list = new List<string>
            {
                "foo",
                "bar"
            };
            Assert.That(list, Is.EqualTo(new[] { "foo", "bar" }));
        }

        [Test]
        public void EqualityTestsWithTolerance()
        {
            Assert.Multiple(() =>
            {
                Assert.That(4.99d, Is.EqualTo(5.0d).Within(0.05d));
                Assert.That(4.0d, Is.Not.EqualTo(5.0d).Within(0.5d));
                Assert.That(4.99f, Is.EqualTo(5.0f).Within(0.05f));
                Assert.That(4.99m, Is.EqualTo(5.0m).Within(0.05m));
                Assert.That(3999999999u, Is.EqualTo(4000000000u).Within(5u));
                Assert.That(499, Is.EqualTo(500).Within(5));
                Assert.That(4999999999L, Is.EqualTo(5000000000L).Within(5L));
                Assert.That(5999999999ul, Is.EqualTo(6000000000ul).Within(5ul));
            });
        }

        [Test]
        public void EqualityTestsWithTolerance_MixedFloatAndDouble()
        {
            Assert.Multiple(() =>
            {
                // Bug Fix 1743844
                Assert.That(2.20492d, Is.EqualTo(2.2d).Within(0.01f),
                    "Double actual, Double expected, Single tolerance");
                Assert.That(2.20492d, Is.EqualTo(2.2f).Within(0.01d),
                    "Double actual, Single expected, Double tolerance");
                Assert.That(2.20492d, Is.EqualTo(2.2f).Within(0.01f),
                    "Double actual, Single expected, Single tolerance");
                Assert.That(2.20492f, Is.EqualTo(2.2f).Within(0.01d),
                    "Single actual, Single expected, Double tolerance");
                Assert.That(2.20492f, Is.EqualTo(2.2d).Within(0.01d),
                    "Single actual, Double expected, Double tolerance");
                Assert.That(2.20492f, Is.EqualTo(2.2d).Within(0.01f),
                    "Single actual, Double expected, Single tolerance");
            });
        }

        [Test]
        public void EqualityTestsWithTolerance_MixingTypesGenerally()
        {
            Assert.Multiple(() =>
            {
                // Extending tolerance to all numeric types
                Assert.That(202d, Is.EqualTo(200d).Within(2),
                    "Double actual, Double expected, int tolerance");
                Assert.That(4.87m, Is.EqualTo(5).Within(.25),
                    "Decimal actual, int expected, Double tolerance");
                Assert.That(4.87m, Is.EqualTo(5ul).Within(1),
                    "Decimal actual, ulong expected, int tolerance");
                Assert.That(487, Is.EqualTo(500).Within(25),
                    "int actual, int expected, int tolerance");
                Assert.That(487u, Is.EqualTo(500).Within(25),
                    "uint actual, int expected, int tolerance");
                Assert.That(487L, Is.EqualTo(500).Within(25),
                    "long actual, int expected, int tolerance");
                Assert.That(487ul, Is.EqualTo(500).Within(25),
                    "ulong actual, int expected, int tolerance");
            });
        }

        [Test, DefaultFloatingPointTolerance(0.05)]
        public void EqualityTestsUsingDefaultFloatingPointTolerance()
        {
            Assert.Multiple(() =>
            {
                Assert.That(4.99d, Is.EqualTo(5.0d));
                Assert.That(4.0d, Is.Not.EqualTo(5.0d));
                Assert.That(4.99f, Is.EqualTo(5.0f));
            });
        }
    }
}
