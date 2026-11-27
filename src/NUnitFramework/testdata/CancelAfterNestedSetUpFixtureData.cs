// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading;
using NUnit.Framework;

namespace NUnit.TestData.CancelAfterNestedSetUpFixture
{
    [SetUpFixture, CancelAfter(500)]
    public sealed class SetUpFixtureWithCancelAfter
    {
    }

    [TestFixture, CancelAfter(50)]
    public sealed class FixtureWithOwnCancelAfter
    {
        [OneTimeSetUp]
        public void OneTimeSetUp() => WaitForCancellation();

        [Test]
        public void Test()
        {
        }

        internal static void WaitForCancellation()
        {
            CancellationToken cancellationToken = TestContext.CurrentContext.CancellationToken;
            cancellationToken.WaitHandle.WaitOne(5000);
            cancellationToken.ThrowIfCancellationRequested();
        }
    }

    [TestFixture]
    public sealed class FixtureWithEnclosingCancelAfter
    {
        [OneTimeSetUp]
        public void OneTimeSetUp() => FixtureWithOwnCancelAfter.WaitForCancellation();

        [Test]
        public void Test()
        {
        }
    }
}
