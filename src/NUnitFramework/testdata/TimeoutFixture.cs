// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if THREAD_ABORT
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
        private const int TIME_OUT_TIME = 100;
        private const int NOT_TIMEOUTED_TIME = 10;
        private const int TIMEOUTED_TIME = 500;

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
