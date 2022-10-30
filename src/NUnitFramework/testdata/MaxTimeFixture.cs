// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;

namespace NUnit.TestData
{
    [TestFixture]
    public class MaxTimeFixture
    {
        [Test, MaxTime(1)]
        public void MaxTimeExceeded()
        {
            Thread.Sleep(20);
        }
    }

    [TestFixture]
    public class MaxTimeFixtureWithTestCase
    {
        [TestCase(5), MaxTime(1)]
        public void MaxTimeExceeded(int x)
        {
            Thread.Sleep(20);
        }
    }

    [TestFixture]
    public class MaxTimeFixtureWithFailure
    {
        [Test, MaxTime(1)]
        public void MaxTimeExceeded()
        {
            Thread.Sleep(20);
            Assert.Fail("Intentional Failure");
        }
    }

    [TestFixture]
    public class MaxTimeFixtureWithError
    {
        [Test, MaxTime(1)]
        public void MaxTimeExceeded()
        {
            Thread.Sleep(20);
            throw new Exception("Exception message");
        }
    }
}
