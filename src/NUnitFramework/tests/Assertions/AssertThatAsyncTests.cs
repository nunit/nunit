// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Assertions
{
    [TestFixture]
    public class AssertThatAsyncTests
    {
        [Test]
        public async Task AssertionPasses_CompletedTask_ThrowsNothing()
        {
            await Assert.ThatAsync(() => Task.CompletedTask, Throws.Nothing);
        }

        [Test]
        public async Task AssertionPasses_CanceledTask_ThrowsCanceled()
        {
            var cancel = new CancellationTokenSource();
            cancel.Cancel();

            await Assert.ThatAsync(() => Task.FromCanceled(cancel.Token),
                Throws.InstanceOf<TaskCanceledException>()
                      .With.Property(nameof(TaskCanceledException.CancellationToken)).EqualTo(cancel.Token));
        }

        [Test]
        public async Task AssertionPasses_FaultedTask_ThrowsMatchingException()
        {
            await Assert.ThatAsync(() => Task.FromException(new InvalidOperationException()), Throws.InvalidOperationException);
        }

        [Test]
        public async Task AssertionPasses_CompletedTask_ThrowsNothingWithMessage()
        {
            await Assert.ThatAsync(() => Task.CompletedTask, Throws.Nothing, "Success");
        }

        [Test]
        public async Task AssertionPasses_CanceledTask_ThrowsCanceledWithMessage()
        {
            var cancel = new CancellationTokenSource();
            cancel.Cancel();

            await Assert.ThatAsync(() => Task.FromCanceled(cancel.Token),
                Throws.InstanceOf<TaskCanceledException>()
                      .With.Property(nameof(TaskCanceledException.CancellationToken)).EqualTo(cancel.Token),
                "Cancelled");
        }

        [Test]
        public async Task AssertionPasses_FaultedTask_ThrowsMatchingExceptionWithMessage()
        {
            await Assert.ThatAsync(() => Task.FromException(new InvalidOperationException()), Throws.InvalidOperationException, "Faulted");
        }

        [Test]
        public async Task Failure_CompletedTask_ThrowsException()
        {
            await AssertAssertionFailsAsync(async () => await Assert.ThatAsync(() => Task.CompletedTask, Throws.InvalidOperationException));
        }

        [Test]
        public async Task Failure_CanceledTask_ThrowsNothing()
        {
            var cancel = new CancellationTokenSource();
            cancel.Cancel();

            await AssertAssertionFailsAsync(async () => await Assert.ThatAsync(() => Task.FromCanceled(cancel.Token), Throws.Nothing));
        }

        [Test]
        public async Task Failure_FaultedTask_ThrowsNothing()
        {
            await AssertAssertionFailsAsync(async () => await Assert.ThatAsync(() => Task.FromException(new InvalidOperationException()), Throws.Nothing));
        }

        [Test]
        public async Task AssertionPasses_CompletedTaskWithResult_ThrowsNothing()
        {
            await Assert.ThatAsync(() => Task.FromResult(42), Throws.Nothing);
        }

        [Test]
        public async Task AssertionPasses_CompletedTaskWithResult_EqualsResult()
        {
            await Assert.ThatAsync(() => Task.FromResult(42), Is.EqualTo(42));
        }

        [Test]
        public async Task AssertionPasses_CanceledTaskWithResult_ThrowsCanceled()
        {
            var cancel = new CancellationTokenSource();
            cancel.Cancel();

            await Assert.ThatAsync(() => Task.FromCanceled<int>(cancel.Token),
                Throws.InstanceOf<TaskCanceledException>()
                      .With.Property(nameof(TaskCanceledException.CancellationToken)).EqualTo(cancel.Token));
        }

        [Test]
        public async Task AssertionPasses_FaultedTaskWithResult_ThrowsMatchingException()
        {
            await Assert.ThatAsync(() => Task.FromException<int>(new InvalidOperationException()), Throws.InvalidOperationException);
        }

        private static async Task AssertAssertionFailsAsync(Func<Task> assertion)
        {
            await Assert.ThatAsync(
                async () =>
                {
                    using (new TestExecutionContext.IsolatedContext())
                    {
                        await assertion();
                    }
                },
                Throws.InstanceOf<AssertionException>());
        }
    }
}
