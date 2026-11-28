// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading.Tasks;
using NUnit.Framework;

namespace NUnit.TestData.CancelAfterSetUpFixture
{
    [SetUpFixture, CancelAfter(50)]
    public sealed class SetUpFixtureWithTimeoutInOneTimeSetUp
    {
        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            await Task.Delay(1000, TestContext.CurrentContext.CancellationToken).ConfigureAwait(false);
        }
    }

    [TestFixture]
    public sealed class FixtureInSetUpFixtureNamespace
    {
        [Test]
        public void Test()
        {
        }
    }
}
