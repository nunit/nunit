// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading;
using NUnit.Framework;

namespace NUnit.TestData
{
    [SingleThreaded]
    public class SingleThreadedFixture_TestWithRequiresThread
    {
        [Test, RequiresThread]
        public void TestWithRequiresThread()
        {
        }
    }

    [SingleThreaded]
    public class SingleThreadedFixture_TestWithDifferentApartment
    {
        [Test, Apartment(ApartmentState.STA)]
        public void TestWithDifferentApartment()
        {
        }
    }

    [SingleThreaded]
    public class SingleThreadedFixture_TestWithRequiresThreadAndDifferentApartment
    {
        [Test, RequiresThread, Apartment(ApartmentState.STA)]
        public void TestWithRequiresThreadAndDifferentApartment()
        {
        }
    }

#if THREAD_ABORT
    [SingleThreaded]
    public class SingleThreadedFixture_TestWithTimeout
    {
        [Test, Timeout(100)]
        public void TestWithTimeout()
        {
        }
    }

    [SingleThreaded]
    public class SingleThreadedFixture_TestWithTimeoutAndRequiresThread
    {
        [Test, Timeout(100), RequiresThread]
        public void TestWithTimeoutAndRequiresThread()
        {
        }
    }

    [SingleThreaded]
    public class SingleThreadedFixture_TestWithTimeoutAndDifferentApartment
    {
        [Test, Timeout(100), Apartment(ApartmentState.STA)]
        public void TestWithTimeoutAndDifferentApartment()
        {
        }
    }

    [SingleThreaded]
    public class SingleThreadedFixture_TestWithTimeoutRequiresThreadAndDifferentApartment
    {
        [Test, Timeout(100), RequiresThread, Apartment(ApartmentState.STA)]
        public void TestWithTimeoutRequiresThreadAndDifferentApartment()
        {
        }
    }
#endif
}
