// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

namespace NUnit.TestData.TestFixtureTests
{
    /// <summary>
    /// Classes used for testing NUnit
    /// </summary>
    public class BaseClassWithOneTimeSetUp
    {
        [OneTimeSetUp]
        public void BaseOneTimeSetUp()
        {
        }
    }

    [TestFixture]
    public class TestFixtureWithOneTimeSetUpTearDown : BaseClassWithOneTimeSetUp
    {
        [OneTimeSetUp]
        public void MyOneTimeSetUp()
        {
        }

        [OneTimeTearDown]
        public void MyOneTimeTearDown()
        {
        }

        [SetUp]
        public void MySetUp()
        {
        }

        [TearDown]
        public void MyTearDown()
        {
        }

        [Test]
        public void MyTest1()
        {
        }

        [Test]
        public void MyTest2()
        {
        }
    }
}
