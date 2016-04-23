#if !PORTABLE
using System.Threading;
using NUnit.Framework;

namespace NUnit.TestData
{
    [SingleThreaded]
    public class SingleThreadedFixture_TestWithTimeout
    {
        [Test, Timeout(100)]
        public void TestWithTimeout() { }
    }

#if !SILVERLIGHT
    [SingleThreaded]
    public class SingleThreadedFixture_TestWithRequiresThread
    {
        [Test, RequiresThread]
        public void TestWithRequiresThread() { }
    }

#if !NETCF
    [SingleThreaded]
    public class SingleThreadedFixture_TestWithDifferentApartment
    { 
        [Test, Apartment(ApartmentState.STA)]
        public void TestWithDifferentApartment() { }
    }
#endif
#endif
}
#endif
