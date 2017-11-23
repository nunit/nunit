// ***********************************************************************
// Copyright (c) 2009 Charlie Poole, Rob Prouse
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

#if !NETCOREAPP1_1
using System.Threading;

namespace NUnit.Framework.Attributes
{
    public class ThreadingTests
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
}
#endif
