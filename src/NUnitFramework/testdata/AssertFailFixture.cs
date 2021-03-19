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
        public void CallAssertFailWithMessageAndArgs()
        {
            Assert.Fail("MESSAGE: {0}+{1}={2}", 2, 2, 4);
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
