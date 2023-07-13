// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

// Enable these tests if you need to debug unhandled exceptions
#if false
using System;
using System.Collections;
using System.Text;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests
{
    [TestFixture, Explicit("These tests fail by design")]
    public class UnhandledExceptionTests
    {
        #region Normal
        [NUnit.Framework.Test]
        public void Normal()
        {
            TestDummy("Normal", false);
        }

        private static void TestDummy(string dummyName, bool shouldPass)
        {
            Type fixtureType = typeof(NUnit.TestData.UnhandledExceptionData.UnhandledExceptions);
            ITestResult result = TestBuilder.RunTestCase(fixtureType, dummyName);
            if (shouldPass)
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Success), "{0} test should have passed", dummyName);
            else
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure), "{0} test should have failed", dummyName);
                Assert.That(result.Message, Is.EqualTo("System.ApplicationException : Test exception"));
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
            TestDummy("ThreadedAndWait", true);
        }
        #endregion ThreadedAndWait

        #region ThreadedAndForget
        [NUnit.Framework.Test]
        public void ThreadedAndForget()
        {
            // TODO: Make this fail
            TestDummy("ThreadedAndForget", true);
        }
        #endregion ThreadedAndForget

        #region ThreadedAssert
        [NUnit.Framework.Test]
        public void ThreadedAssert()
        {
            TestDummy("ThreadedAssert", true);
        }
        #endregion
    }
}
#endif
