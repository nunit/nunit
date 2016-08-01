// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

// Enable these tests if you need to debug unhandled exceptions
#if !PORTABLE && false
using System;
using System.Collections;
using System.Text;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestUtilities;

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
