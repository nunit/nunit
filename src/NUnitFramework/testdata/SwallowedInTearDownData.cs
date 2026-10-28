// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

namespace NUnit.TestData.SwallowedInTearDownData
{
    [TestFixture]
    public class FailedTestSwallowsPassInTearDown
    {
        [Test]
        public void Test()
        {
            Assert.Fail("Test failed");
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                Assert.Pass("Swallowed");
            }
            catch (SuccessException)
            {
            }
        }
    }

    [TestFixture]
    public class PassedTestSwallowsInconclusiveInTearDown
    {
        [Test]
        public void Test()
        {
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                Assert.Inconclusive("Swallowed");
            }
            catch (InconclusiveException)
            {
            }
        }
    }
}
