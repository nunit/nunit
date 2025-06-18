// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading;

namespace NUnit.Framework.Tests.Attributes
{
    public abstract class ThreadingTests
    {
        protected Thread ParentThread { get; private set; }
        protected Thread SetupThread { get; private set; }
        protected ApartmentState ParentThreadApartment { get; private set; }

        [OneTimeSetUp]
        public void GetParentThreadInfo()
        {
            ParentThread = Thread.CurrentThread;
            ParentThreadApartment = GetApartmentState(ParentThread);

            TestContext.AddFormatter<Thread>((t) => "Thread #" + ((Thread)t).ManagedThreadId);
        }

        [SetUp]
        public void GetSetUpThreadInfo()
        {
            SetupThread = Thread.CurrentThread;
        }

        [Test]
        public void TestDefaultsToRunningEverythingOnSameThread()
        {
            Assert.That(Thread.CurrentThread, Is.EqualTo(ParentThread));
            Assert.That(Thread.CurrentThread, Is.EqualTo(SetupThread));
        }

        [TestCase(5)]
        public void TestCaseDefaultsToRunningEverythingOnSameThread(int x)
        {
            Assert.That(Thread.CurrentThread, Is.EqualTo(ParentThread));
            Assert.That(Thread.CurrentThread, Is.EqualTo(SetupThread));
        }

        protected static ApartmentState GetApartmentState(Thread thread)
        {
            return thread.GetApartmentState();
        }
    }

    [TestFixture]
    public class DefaultThreadingTests : ThreadingTests
    {
        // This class is used to ensure that the default threading behavior is applied
        // when no specific attributes are set on the test fixture or tests.
    }
}
