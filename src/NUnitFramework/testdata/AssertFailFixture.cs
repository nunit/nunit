// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

namespace NUnit.TestData
{
    [TestFixture]
    public class AssertFailFixture
    {
        [Test]
        public void CallAssertFail()
        {
            Assert.Fail();
        }

        [Test]
        public void CallAssertFailWithMessage()
        {
            Assert.Fail("MESSAGE");
        }

        [Test]
        public void HandleAssertionException()
        {
            try
            {
                Assert.Fail("Custom message");
            }
            catch
            {
                // Eat the exception
            }
        }
    }
}
