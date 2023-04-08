// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

namespace NUnit.TestData.AssertIgnoreData
{
    [TestFixture]
    public class IgnoredTestCaseFixture
    {
        [Test]
        public void CallsIgnore()
        {
            Assert.Ignore("Ignore me");
        }
    }

    [TestFixture]
    public class IgnoredTestSuiteFixture
    {
        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            Assert.Ignore("Ignore this fixture");
        }

        [Test]
        public void ATest()
        {
        }

        [Test]
        public void AnotherTest()
        {
        }
    }

    [TestFixture]
    public class IgnoreInSetUpFixture
    {
        [SetUp]
        public void SetUp()
        {
            Assert.Ignore( "Ignore this test" );
        }

        [Test]
        public void Test1()
        {
        }

        [Test]
        public void Test2()
        {
        }
    }
}
