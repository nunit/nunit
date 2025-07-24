// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

namespace NUnit.TestData
{
    [TestFixture]
    public sealed class ExplicitTests
    {
        public int NumberOfTestsExecuted { get; private set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            NumberOfTestsExecuted = 0;
        }

        [Test]
        public void NormalTest()
        {
            NumberOfTestsExecuted++;
            Assert.That(NumberOfTestsExecuted, Is.GreaterThan(0));
        }

        [Test]
        [Explicit]
        public void ExplicitTest()
        {
            NumberOfTestsExecuted++;
            Assert.That(NumberOfTestsExecuted, Is.GreaterThan(0));
        }
    }
}
