// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture]
    public class CombinatorialTests
    {
        [Test]
        public void SingleArgument(
            [Values(1.3, 1.7, 1.5)] double x)
        {
            Assert.That(x > 1.0 && x < 2.0);
        }

        [Test, Combinatorial]
        public void TwoArguments_Combinatorial(
            [Values(1, 2, 3)] int x,
            [Values(10, 20)] int y)
        {
            Assert.That(x > 0 && x < 4 && y % 10 == 0);
        }

        [Test, Sequential]
        public void TwoArguments_Sequential(
            [Values(1, 2, 3)] int x,
            [Values(10, 20)] int y)
        {
            Assert.That(x > 0 && x < 4 && y % 10 == 0);
        }

        [Test, Combinatorial]
        public void ThreeArguments_Combinatorial(
            [Values(1, 2, 3)] int x,
            [Values(10, 20)] int y,
            [Values("Charlie", "Joe", "Frank")] string name)
        {
            Assert.Multiple(() =>
            {
                Assert.That(x > 0 && x < 4 && y % 10 == 0);
                Assert.That(name, Has.Length.GreaterThanOrEqualTo(2));
            });
        }

        [Test, Sequential]
        public void ThreeArguments_Sequential(
            [Values(1, 2, 3)] int x,
            [Values(10, 20)] int y,
            [Values("Charlie", "Joe", "Frank")] string name)
        {
            Assert.Multiple(() =>
            {
                Assert.That(x > 0 && x < 4 && y % 10 == 0);
                Assert.That(name, Has.Length.GreaterThanOrEqualTo(2));
            });
        }

        [Test]
        public void RangeTest(
            [Range(0.2, 0.6, 0.2)] double a,
            [Range(10, 20, 5)] int b)
        {
            Assert.Multiple(() =>
            {
                Assert.That(a, Is.InRange(0.2, 0.6));
                Assert.That(b, Is.InRange(10, 20));
            });
        }

        [Test, Sequential]
        public void RandomTest(
            [Random(32, 212, 5)] int x,
            [Random(5)] double y,
            [Random(5)] AttributeTargets z)
        {
            Assert.Multiple(() =>
            {
                Assert.That(x, Is.InRange(32, 212));
                Assert.That(y, Is.InRange(0.0, 1.0));
                Assert.That(z, Is.TypeOf<AttributeTargets>());
            });
        }

        [Test, Sequential]
        public void RandomArgsAreIndependent(
            [Random(1)] double x,
            [Random(1)] double y)
        {
            Assert.That(y, Is.Not.EqualTo(x));
        }

        [Test, Combinatorial]
        public void CombinatorialIsIgnoredIfThereAreNoArguments()
        {
        }
    }
}
