// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NUnit.TestData
{
    [TestFixture]
    [Parallelizable(ParallelScope.Children)]
    public sealed class StopOnErrorFixture
    {
        public bool OneTimeTearDownExecuted { get; private set; }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            OneTimeTearDownExecuted = true;
        }

        [Test]
        public async Task AlwaysFails(CancellationToken cancellationToken)
        {
            await Task.Delay(10, cancellationToken);
            Assert.Fail();
        }

        [Test]
        [CancelAfter(100)]
        public async Task AlwaysTimesout(CancellationToken cancellationToken)
        {
            await Task.Delay(1000, cancellationToken);
            Assert.Pass();
        }

        [Test]
        public async Task TestThatMightBeSkipped(CancellationToken cancellationToken)
        {
            await Task.Delay(10, cancellationToken);
            Assert.Pass();
        }
    }
}
