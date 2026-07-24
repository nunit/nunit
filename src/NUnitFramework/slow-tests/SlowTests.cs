// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Diagnostics;
using System.Threading;
using NUnit.Framework;

namespace NUnit.Tests
{
    public class SlowTests
    {
        public const int SINGLE_TEST_DELAY = 1000;

        [Category("Interruptable")]
        public class AAA
        {
            [Test]
            public void Test1()
            {
                SlowTests.Delay();
            }
            [Test]
            public void Test2()
            {
                SlowTests.Delay();
            }
            [Test]
            public void Test3()
            {
                SlowTests.Delay();
            }
        }

        [Category("Interruptable")]
        public class BBB
        {
            [Test]
            public void Test1()
            {
                SlowTests.Delay();
            }
            [Test]
            public void Test2()
            {
                SlowTests.Delay();
            }
            [Test]
            public void Test3()
            {
                SlowTests.Delay();
            }
        }

        [Category("Hanging")]
        public class CCC
        {
            [Test]
            public void Test1()
            {
                SlowTests.Hanging();
            }
            [Test]
            public void Test2()
            {
                SlowTests.Hanging();
            }
            [Test]
            public void Test3()
            {
                SlowTests.Hanging();
            }
        }

        [Category("HangingOnOwnThread")]
        public class DDD
        {
            [Test]
            [RequiresThread]
            public void Test1()
            {
                SlowTests.Hanging();
            }
            [Test]
            [RequiresThread]
            public void Test2()
            {
                SlowTests.Hanging();
            }
            [Test]
            [RequiresThread]
            public void Test3()
            {
                SlowTests.Hanging();
            }
        }

        private static void Delay()
        {
            Thread.Sleep(SINGLE_TEST_DELAY);
        }

        private static void Hanging()
        {
            const int numberOfRepeats = 10;

            for (int i = 0; i < numberOfRepeats || Debugger.IsAttached; i++)
            {
                Delay();
            }
        }
    }
}
