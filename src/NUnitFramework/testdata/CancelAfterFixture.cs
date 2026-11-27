// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

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
        public void Test()
        {
        }
    }

    [TestFixture, CancelAfter(50)]
    public sealed class CancelAfterFixtureWithCancelAfterOnFixture
    {
        [Test]
        public void Test1()
        {
        }

        [Test]
        public async Task Test2ExceedsTimeout()
        {
            await Task.Delay(1000, TestContext.CurrentContext.CancellationToken).ConfigureAwait(false);
        }

        [Test]
        public void Test3()
        {
        }
    }

    [CancelAfter(50)]
    public abstract class CancelAfterBaseFixtureWithCancelAfterOnFixture
    {
        [Test]
        public void Test1()
        {
        }

        [Test]
        public void Test3()
        {
        }
    }

    [TestFixture]
    public sealed class CancelAfterFixtureWithInheritedCancelAfterOnFixture : CancelAfterBaseFixtureWithCancelAfterOnFixture
    {
        [Test]
        public async Task Test2ExceedsTimeout()
        {
            await Task.Delay(1000, TestContext.CurrentContext.CancellationToken).ConfigureAwait(false);
        }
    }

    [TestFixture, CancelAfter(50)]
    public sealed class CancelAfterFixtureWithTimeoutInOneTimeSetUp
    {
        public bool TestWasRun;
        public bool OneTimeTearDownWasRun;
        public bool OneTimeTearDownTokenWasCancelled;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            await Task.Delay(1000, TestContext.CurrentContext.CancellationToken).ConfigureAwait(false);
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

    [TestFixture, CancelAfter(500)]
    public sealed class CancelAfterFixtureWithOneTimeSetUpAndTearDownWithinCancelAfter
    {
        public CancellationToken OneTimeSetUpToken;
        public CancellationToken TestToken;
        public CancellationToken OneTimeTearDownToken;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            OneTimeSetUpToken = TestContext.CurrentContext.CancellationToken;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            OneTimeTearDownToken = TestContext.CurrentContext.CancellationToken;
        }

        [Test]
        public void Test()
        {
            TestToken = TestContext.CurrentContext.CancellationToken;
        }
    }

    public abstract class CancelAfterBaseFixtureWithOneTimeSetUp
    {
        public CancellationToken BaseOneTimeSetUpToken;

        [OneTimeSetUp]
        public void BaseOneTimeSetUp()
        {
            BaseOneTimeSetUpToken = TestContext.CurrentContext.CancellationToken;
        }
    }

    [TestFixture, CancelAfter(500)]
    public sealed class CancelAfterDerivedFixtureWithOneTimeSetUp : CancelAfterBaseFixtureWithOneTimeSetUp
    {
        public CancellationToken DerivedOneTimeSetUpToken;

        [OneTimeSetUp]
        public void DerivedOneTimeSetUp()
        {
            DerivedOneTimeSetUpToken = TestContext.CurrentContext.CancellationToken;
        }

        [Test]
        public void Test()
        {
        }
    }

    [TestFixture]
    public sealed class CancelAfterFixtureWithInheritedCancelAfterAndTimeoutInOneTimeSetUp : CancelAfterBaseFixtureWithCancelAfterOnFixture
    {
        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            await Task.Delay(1000, TestContext.CurrentContext.CancellationToken).ConfigureAwait(false);
        }
    }

    [TestFixture, CancelAfter(50)]
    public sealed class CancelAfterFixtureWithTimeoutInConstructor
    {
        public CancelAfterFixtureWithTimeoutInConstructor()
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

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CancellableAfterSuiteActionAttribute : Attribute, ITestAction
    {
        public ActionTargets Targets => ActionTargets.Suite;

        public void BeforeTest(ITest test)
        {
        }

        public void AfterTest(ITest test)
        {
            CancellationToken cancellationToken = TestContext.CurrentContext.CancellationToken;
            cancellationToken.WaitHandle.WaitOne(1000);
            cancellationToken.ThrowIfCancellationRequested();
        }
    }

    [TestFixture, CancelAfter(50), CancellableAfterSuiteAction]
    public sealed class CancelAfterFixtureWithTimeoutInAfterSuiteAction
    {
        [Test]
        public void Test()
        {
        }
    }

    [TestFixture, CancelAfter(50)]
    public sealed class CancelAfterFixtureWithTimeoutInOneTimeTearDown
    {
        public bool TestWasRun;

        [OneTimeTearDown]
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
}
