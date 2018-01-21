// ***********************************************************************
// Copyright (c) 2008 Charlie Poole, Rob Prouse
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

#if THREAD_ABORT
using System;
using System.Runtime.InteropServices;
using System.Threading;
using NUnit.Framework;

namespace NUnit.TestData
{
    [TestFixture]
    public class TimeoutFixture
    {
        public bool TearDownWasRun;

        [SetUp]
        public void SetUp()
        {
            TearDownWasRun = false;
        }

        [TearDown]
        public void TearDown()
        {
            TearDownWasRun = true;
        }

        [Test, Timeout(50)]
        public void InfiniteLoopWith50msTimeout()
        {
            while (true) { }
        }


        [Test, Timeout(500)]
        public void TimeoutWithMessagePumpShouldAbort()
        {
            // Simulate System.Windows.Forms.Application.Run or .ShowDialog,
            // or System.Windows.Threading.Dispatcher.PushFrame,
            // which block on native calls to WaitMessage or MsgWaitForMultipleObjectsEx.
            while (true)
                WaitMessage();
        }

        [DllImport("user32.dll")]
        private static extern bool WaitMessage();
    }

    public class TimeoutFixtureWithTimeoutInSetUp : TimeoutFixture
    {
        [SetUp]
        public bool SetUp2()
        {
            while (true) { }
        }

        [Test, Timeout(50)]
        public void Test1() { }
    }

    public class TimeoutFixtureWithTimeoutInTearDown : TimeoutFixture
    {
        [TearDown]
        public void TearDown2()
        {
            while (true) { }
        }

        [Test, Timeout(50)]
        public void Test1() { }
    }

    [TestFixture, Timeout(50)]
    public class TimeoutFixtureWithTimeoutOnFixture
    {
        [Test]
        public void Test1() { }
        [Test]
        public void Test2WithInfiniteLoop()
        {
            while (true) { }
        }
        [Test]
        public void Test3() { }
    }

    public class TimeoutTestCaseFixture
    {
        const int TIME_OUT_TIME = 100;
        const int NOT_TIMEOUTED_TIME = 10;
        const int TIMEOUTED_TIME = 500;

        [Test]
        [Timeout(TIME_OUT_TIME)]
        public void TestTimeOutNotElapsed()
        {
            TestTimeOutTestCase(NOT_TIMEOUTED_TIME);
        }

        [Test]
        [Timeout(TIME_OUT_TIME)]
        public void TestTimeOutElapsed()
        {
            TestTimeOutTestCase(TIMEOUTED_TIME);
        }

        [Test]
        [Timeout(TIME_OUT_TIME)]
        [TestCase(NOT_TIMEOUTED_TIME)]
        [TestCase(TIMEOUTED_TIME)]
        public void TestTimeOutTestCase(int delay)
        {
            Thread.Sleep(delay);
        }
    }
}
#endif
