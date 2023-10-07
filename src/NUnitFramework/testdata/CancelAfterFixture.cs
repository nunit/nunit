// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading.Tasks;
using NUnit.Framework;

namespace NUnit.TestData
{
    [TestFixture]
    public class CancelAfterFixture
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

        [Test, CancelAfter(50)]
        public async Task InfiniteLoopWith50msCancelAfter()
        {
            while (true)
            {
                await Task.Delay(100, TestContext.CurrentContext.CancellationToken);
            }
        }
    }

    [TestFixture]
    public sealed class CancelAfterFixtureWithTimeoutInSetUp : CancelAfterFixture
    {
        [SetUp]
        public async Task SetUp2()
        {
            await Task.Delay(1000, TestContext.CurrentContext.CancellationToken).ConfigureAwait(false);
        }

        [Test, CancelAfter(50)]
        public void Test()
        {
        }
    }

    [TestFixture]
    public sealed class CancelAfterFixtureWithTimeoutInTearDown : CancelAfterFixture
    {
        [TearDown]
        public async Task TearDown2()
        {
            await Task.Delay(1000, TestContext.CurrentContext.CancellationToken).ConfigureAwait(false);
        }

        [Test, CancelAfter(50)]
        public void Test() { }
    }

    [TestFixture, CancelAfter(50)]
    public sealed class CancelAfterFixtureWithCancelAfterOnFixture
    {
        [Test]
        public void Test1() { }

        [Test]
        public async Task Test2ExceedsTimeout()
        {
            await Task.Delay(1000, TestContext.CurrentContext.CancellationToken).ConfigureAwait(false);
        }

        [Test]
        public void Test3() { }
    }
}
