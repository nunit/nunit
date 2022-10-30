// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

// Enable these tests if you need to debug unhandled exceptions
#if false

namespace NUnit.Framework.Tests
{
    [TestFixture, Explicit("These tests fail by design")]
    public class UnhandledExceptionTests
    {
        #region Normal
        [NUnit.Framework.Test]
        public void Normal()
        {
            testDummy("Normal", false);
        }

        private static void testDummy(string dummyName, bool shouldPass)
        {
            Type fixtureType = typeof(NUnit.TestData.UnhandledExceptionData.UnhandledExceptions);
            ITestResult result = TestBuilder.RunTestCase(fixtureType, dummyName);
            if (shouldPass)
                Assert.IsTrue(result.ResultState == ResultState.Success, "{0} test should have passed", dummyName);
            else
            {
                Assert.IsTrue(result.ResultState == ResultState.Failure, "{0} test should have failed", dummyName);
                Assert.AreEqual("System.ApplicationException : Test exception", result.Message);
            }
        }
        #endregion Normal

        #region Threaded
        //[NUnit.Framework.Test]
        //public void Threaded()
        //{
        //    // TODO: Make this fail
        //    testDummy("Threaded", true);
        //}
        #endregion Threaded

        #region ThreadedAndWait
        [NUnit.Framework.Test]
        public void ThreadedAndWait()
        {
            // TODO: Make this fail
            testDummy("ThreadedAndWait", true);
        }
        #endregion ThreadedAndWait

        #region ThreadedAndForget
        [NUnit.Framework.Test]
        public void ThreadedAndForget()
        {
            // TODO: Make this fail
            testDummy("ThreadedAndForget", true);
        }
        #endregion ThreadedAndForget

        #region ThreadedAssert
        [NUnit.Framework.Test]
        public void ThreadedAssert()
        {
            testDummy("ThreadedAssert", true);
        }
        #endregion
    }
}
#endif
