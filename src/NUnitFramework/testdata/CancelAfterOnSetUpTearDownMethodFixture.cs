// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NUnit.TestData
{
    [TestFixture]
    public sealed class CancelAfterOnSetUpMethodFixture
    {
        public bool TestWasRun;
        public bool TearDownWasRun;

        [SetUp, CancelAfter(50)]
        public async Task SetUp(CancellationToken cancellationToken)
        {
            await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
        }

        [TearDown]
        public void TearDown()
        {
            TearDownWasRun = true;
        }

        [Test]
        public void Test()
        {
            TestWasRun = true;
        }
    }

    [TestFixture]
    public sealed class CancelAfterOnTearDownMethodFixture
    {
        public bool TestWasRun;

        [TearDown, CancelAfter(50)]
        public async Task TearDown()
        {
            await Task.Delay(1000, TestContext.CurrentContext.CancellationToken).ConfigureAwait(false);
        }

        [Test]
        public void Test()
        {
            TestWasRun = true;
        }
    }

    [TestFixture]
    public sealed class CancelAfterOnOneTimeSetUpMethodFixture
    {
        public bool TestWasRun;
        public bool OneTimeTearDownWasRun;
        public bool OneTimeTearDownTokenWasCancelled;

        [OneTimeSetUp, CancelAfter(50)]
        public async Task OneTimeSetUp(CancellationToken cancellationToken)
        {
            await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            OneTimeTearDownWasRun = true;
            OneTimeTearDownTokenWasCancelled = TestContext.CurrentContext.CancellationToken.IsCancellationRequested;
        }

        [Test]
        public void Test()
        {
            TestWasRun = true;
        }
    }

    [TestFixture]
    public sealed class CancelAfterOnOneTimeTearDownMethodFixture
    {
        public bool TestWasRun;

        [OneTimeTearDown, CancelAfter(50)]
        public async Task OneTimeTearDown()
        {
            await Task.Delay(1000, TestContext.CurrentContext.CancellationToken).ConfigureAwait(false);
        }

        [Test]
        public void Test()
        {
            TestWasRun = true;
        }
    }

    [TestFixture]
    public sealed class CancelAfterOnSetUpMethodsWithinLimitFixture
    {
        public CancellationToken OneTimeSetUpParameterToken;
        public CancellationToken OneTimeSetUpContextToken;
        public CancellationToken SetUpToken;
        public CancellationToken TestToken;

        [OneTimeSetUp, CancelAfter(500)]
        public void OneTimeSetUp(CancellationToken cancellationToken)
        {
            OneTimeSetUpParameterToken = cancellationToken;
            OneTimeSetUpContextToken = TestContext.CurrentContext.CancellationToken;
        }

        [SetUp, CancelAfter(500)]
        public void SetUp()
        {
            SetUpToken = TestContext.CurrentContext.CancellationToken;
        }

        [Test]
        public void Test()
        {
            TestToken = TestContext.CurrentContext.CancellationToken;
        }
    }

    [TestFixture]
    public sealed class CancelAfterOnSetUpMethodDoesNotApplyToTestFixture
    {
        [SetUp, CancelAfter(50)]
        public void SetUp()
        {
        }

        [Test]
        public void Test()
        {
            CancellationToken cancellationToken = TestContext.CurrentContext.CancellationToken;
            cancellationToken.WaitHandle.WaitOne(300);
            cancellationToken.ThrowIfCancellationRequested();
        }
    }

    public abstract class CancelAfterBaseFixtureWithShortCancelAfterOnOneTimeSetUpMethod
    {
        [OneTimeSetUp, CancelAfter(50)]
        public void BaseOneTimeSetUp()
        {
        }
    }

    [TestFixture]
    public sealed class CancelAfterOnBaseOneTimeSetUpMethodDoesNotApplyToDerivedFixture : CancelAfterBaseFixtureWithShortCancelAfterOnOneTimeSetUpMethod
    {
        [OneTimeSetUp]
        public void DerivedOneTimeSetUp()
        {
            CancellationToken cancellationToken = TestContext.CurrentContext.CancellationToken;
            cancellationToken.WaitHandle.WaitOne(300);
            cancellationToken.ThrowIfCancellationRequested();
        }

        [Test]
        public void Test()
        {
        }
    }

    public abstract class CancelAfterBaseFixtureWithLongCancelAfterOnOneTimeSetUpMethod
    {
        [OneTimeSetUp, CancelAfter(5000)]
        public void BaseOneTimeSetUp()
        {
        }
    }

    [TestFixture, CancelAfter(100)]
    public sealed class CancelAfterOnFixtureStillAppliesAfterOneTimeSetUpMethodWithCancelAfter : CancelAfterBaseFixtureWithLongCancelAfterOnOneTimeSetUpMethod
    {
        [OneTimeSetUp]
        public void DerivedOneTimeSetUp()
        {
            CancellationToken cancellationToken = TestContext.CurrentContext.CancellationToken;
            cancellationToken.WaitHandle.WaitOne(1000);
            cancellationToken.ThrowIfCancellationRequested();
        }

        [Test]
        public void Test()
        {
        }
    }

    [TestFixture, CancelAfter(5000)]
    public sealed class CancelAfterOnOneTimeSetUpMethodShorterThanOnFixture
    {
        [OneTimeSetUp, CancelAfter(50)]
        public void OneTimeSetUp()
        {
            CancellationToken cancellationToken = TestContext.CurrentContext.CancellationToken;
            cancellationToken.WaitHandle.WaitOne(1000);
            cancellationToken.ThrowIfCancellationRequested();
        }

        [Test]
        public void Test()
        {
        }
    }

    [TestFixture]
    public sealed class CancelAfterOnSetUpMethodThatIsSlowToStopFixture
    {
        [SetUp, CancelAfter(100)]
        public void SetUp()
        {
            CancellationToken cancellationToken = TestContext.CurrentContext.CancellationToken;
            cancellationToken.WaitHandle.WaitOne(1000);

            // Keep running after the timeout of the test, until the timeout of this method has expired as well.
            Thread.Sleep(200);
            cancellationToken.ThrowIfCancellationRequested();
        }

        [Test, CancelAfter(50)]
        public void Test()
        {
        }
    }

    [TestFixture, CancelAfter(50)]
    public sealed class CancelAfterOnOneTimeSetUpMethodLongerThanOnFixture
    {
        [OneTimeSetUp, CancelAfter(5000)]
        public void OneTimeSetUp()
        {
            CancellationToken cancellationToken = TestContext.CurrentContext.CancellationToken;
            cancellationToken.WaitHandle.WaitOne(1000);
            cancellationToken.ThrowIfCancellationRequested();
        }

        [Test]
        public void Test()
        {
        }
    }
}
