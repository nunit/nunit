// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NUnit.Tests
{
    public class SlowTests
    {
        public const int SINGLE_TEST_DELAY = 1000;
        public const int SINGLE_TEST_TIMEOUT = SINGLE_TEST_DELAY * 3;

        public class NonCancelableTests
        {
            [Test]
            public void Test1()
            {
                Delay();
            }
            [Test]
            public void Test2()
            {
                Delay();
            }
            [Test]
            public void Test3()
            {
                Delay();
            }

            private static void Delay()
            {
                Thread.Sleep(SINGLE_TEST_DELAY);
            }
        }

        public class GlobalCancelableTests
        {
            [Test]
            public async Task Test1(CancellationToken cancellationToken)
            {
                await Task.Delay(SINGLE_TEST_DELAY, cancellationToken);
            }

            [Test]
            public async Task Test2()
            {
                await Task.Delay(SINGLE_TEST_DELAY, TestContext.CurrentContext.CancellationToken);
            }

            [Test]
            public async Task Test3()
            {
                await Task.Delay(SINGLE_TEST_TIMEOUT + SINGLE_TEST_DELAY, TestContext.CurrentContext.CancellationToken);
            }
        }

        [CancelAfter(SINGLE_TEST_TIMEOUT)]
        public class CancelAfterTests
        {
            [Test]
            public async Task Test1(CancellationToken cancellationToken)
            {
                await Task.Delay(SINGLE_TEST_DELAY, cancellationToken);
            }

            [Test]
            public async Task Test2()
            {
                await Task.Delay(SINGLE_TEST_DELAY, TestContext.CurrentContext.CancellationToken);
            }

            [Test]
            public async Task Test3()
            {
                await Task.Delay(SINGLE_TEST_TIMEOUT + SINGLE_TEST_DELAY, TestContext.CurrentContext.CancellationToken);
            }
        }
    }
}
