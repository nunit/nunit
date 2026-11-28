// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NUnit.TestData
{
    public abstract class SetUpTearDownWithCancellationTokenBaseFixture
    {
        public CancellationToken OneTimeSetUpParameter;
        public CancellationToken OneTimeSetUpContextToken;
        public CancellationToken SetUpParameter;
        public CancellationToken SetUpContextToken;
        public CancellationToken TestContextToken;
        public CancellationToken TearDownParameter;
        public CancellationToken TearDownContextToken;
        public CancellationToken OneTimeTearDownParameter;
        public CancellationToken OneTimeTearDownContextToken;

        [OneTimeSetUp]
        public void OneTimeSetUp(CancellationToken cancellationToken)
        {
            OneTimeSetUpParameter = cancellationToken;
            OneTimeSetUpContextToken = TestContext.CurrentContext.CancellationToken;
        }

        [SetUp]
        public void SetUp(CancellationToken cancellationToken)
        {
            SetUpParameter = cancellationToken;
            SetUpContextToken = TestContext.CurrentContext.CancellationToken;
        }

        [Test]
        public void Test()
        {
            TestContextToken = TestContext.CurrentContext.CancellationToken;
        }

        [TearDown]
        public async Task TearDown(CancellationToken cancellationToken)
        {
            await Task.Yield();
            TearDownParameter = cancellationToken;
            TearDownContextToken = TestContext.CurrentContext.CancellationToken;
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown(CancellationToken cancellationToken)
        {
            await Task.Yield();
            OneTimeTearDownParameter = cancellationToken;
            OneTimeTearDownContextToken = TestContext.CurrentContext.CancellationToken;
        }
    }

    [TestFixture]
    public sealed class SetUpTearDownWithCancellationTokenFixture : SetUpTearDownWithCancellationTokenBaseFixture
    {
    }

    [TestFixture, CancelAfter(500)]
    public sealed class SetUpTearDownWithCancellationTokenAndCancelAfterFixture : SetUpTearDownWithCancellationTokenBaseFixture
    {
    }

    [TestFixture, CancelAfter(50)]
    public sealed class OneTimeSetUpWithCancellationTokenExceedingCancelAfterFixture
    {
        [OneTimeSetUp]
        public async Task OneTimeSetUp(CancellationToken cancellationToken)
        {
            await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
        }

        [Test]
        public void Test()
        {
        }
    }

    [TestFixture]
    public sealed class SetUpWithCancellationTokenExceedingCancelAfterFixture
    {
        [SetUp]
        public async Task SetUp(CancellationToken cancellationToken)
        {
            await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
        }

        [Test, CancelAfter(50)]
        public void Test()
        {
        }
    }

    [TestFixture]
    public sealed class SetUpWithNonCancellationTokenParameterFixture
    {
        [SetUp]
        public void SetUp(int value)
        {
        }

        [Test]
        public void Test()
        {
        }
    }

    [TestFixture]
    public sealed class OneTimeTearDownWithTwoCancellationTokenParametersFixture
    {
        [OneTimeTearDown]
        public void OneTimeTearDown(CancellationToken first, CancellationToken second)
        {
        }

        [Test]
        public void Test()
        {
        }
    }
}
