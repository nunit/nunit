// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Tests
{
    public class SlowTests
    {
        public const int SINGLE_TEST_DELAY = 1000;

        public class AAA
        {
            [Test]
            public void Test1() { SlowTests.Delay(); }
            [Test]
            public void Test2() { SlowTests.Delay(); }
            [Test]
            public void Test3() { SlowTests.Delay(); }
        }

        public class BBB
        {
            [Test]
            public void Test1() { SlowTests.Delay(); }
            [Test]
            public void Test2() { SlowTests.Delay(); }
            [Test]
            public void Test3() { SlowTests.Delay(); }
        }

        public class CCC
        {
            [Test]
            public void Test1() { SlowTests.Delay(); }
            [Test]
            public void Test2() { SlowTests.Delay(); }
            [Test]
            public void Test3() { SlowTests.Delay(); }
        }

        private static void Delay()
        {
            System.Threading.Thread.Sleep(SINGLE_TEST_DELAY);
        }
    }
}
