// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.NativeAotTests
{
    [TestFixture]
    [SingleThreaded]
    [NonParallelizable]
    public sealed class AsyncTests
    {
        private int _threadId;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _threadId = Environment.CurrentManagedThreadId;
            await Task.Yield();
            Assert.That(Environment.CurrentManagedThreadId, Is.EqualTo(_threadId));
        }

        [SetUp]
        public async Task SetUp()
        {
            Assert.That(Environment.CurrentManagedThreadId, Is.EqualTo(_threadId));
            await Task.Yield();
            Assert.That(Environment.CurrentManagedThreadId, Is.EqualTo(_threadId));
        }

        [TearDown]
        public async Task TearDown()
        {
            await Task.Yield();
            Assert.That(Environment.CurrentManagedThreadId, Is.EqualTo(_threadId));
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await Task.Yield();
            Assert.That(Environment.CurrentManagedThreadId, Is.EqualTo(_threadId));
        }

        [Test]
        public void Synchronous()
        {
            Assert.That(2 + 2, Is.EqualTo(4));
        }

        [Test]
        public async Task Asynchronous()
        {
            await Task.Yield();
            Assert.That(Environment.CurrentManagedThreadId, Is.EqualTo(_threadId));
        }

        [Test]
        public Task CompletedTask()
        {
            return Task.CompletedTask;
        }

        [TestCase(2, ExpectedResult = 4)]
        public async Task<int> ValueTypeResult(int input)
        {
            await Task.Yield();
            return input * 2;
        }

        [TestCase(ExpectedResult = "result")]
        public async Task<string> ReferenceTypeResult()
        {
            await Task.Yield();
            return "result";
        }

        [TestCase(ExpectedResult = null)]
        public async Task<object?> NullResult()
        {
            await Task.Yield();
            return null;
        }

        [TestCaseSource(nameof(StructResultData))]
        public async Task<Result> StructTaskResult()
        {
            await Task.Yield();
            return new Result(42);
        }

        [TestCase(ExpectedResult = 42)]
        public Task<int> DerivedTaskWithHiddenGetAwaiter()
        {
            var task = new HiddenAwaiterTask();
            task.RunSynchronously();
            return task;
        }

        [Test]
        public async ValueTask AsynchronousValueTask()
        {
            await Task.Yield();
            Assert.That(Environment.CurrentManagedThreadId, Is.EqualTo(_threadId));
        }

        [TestCase(2, ExpectedResult = 4)]
        public async ValueTask<int> ValueTaskValueTypeResult(int input)
        {
            await Task.Yield();
            return input * 2;
        }

        [TestCase(ExpectedResult = "result")]
        public async ValueTask<string> ValueTaskReferenceTypeResult()
        {
            await Task.Yield();
            return "result";
        }

        [TestCase(ExpectedResult = null)]
        public async ValueTask<object?> ValueTaskNullResult()
        {
            await Task.Yield();
            return null;
        }

        [TestCaseSource(nameof(StructResultData))]
        public async ValueTask<Result> StructValueTaskResult()
        {
            await Task.Yield();
            return new Result(42);
        }

        [Test]
        public void TaskExceptionIsNotWrapped()
        {
            var exception = new InvalidOperationException("Expected exception");
            Func<Task<int>> code = async () =>
            {
                await Task.Yield();
                throw exception;
            };

            Assert.That(code, Throws.Exception.SameAs(exception));
        }

        [Test]
        public void TaskCancellationIsNotWrapped()
        {
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();
            Func<Task<int>> code = () => Task.FromCanceled<int>(cancellation.Token);
            var constraint = new ThrowsConstraint(Is.TypeOf<TaskCanceledException>());

            Assert.That(code, constraint);
            var exception = (TaskCanceledException)constraint.ActualException!;
            Assert.That(exception.CancellationToken, Is.EqualTo(cancellation.Token));
        }

        [Test]
        public void ValueTaskExceptionIsNotWrapped()
        {
            var exception = new InvalidOperationException("Expected exception");
            Func<ValueTask<int>> code = async () =>
            {
                await Task.Yield();
                throw exception;
            };

            Assert.That(code, Throws.Exception.SameAs(exception));
        }

        [Test]
        public void ValueTaskCancellationIsNotWrapped()
        {
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();
            Func<ValueTask<int>> code = () => new ValueTask<int>(Task.FromCanceled<int>(cancellation.Token));
            var constraint = new ThrowsConstraint(Is.TypeOf<TaskCanceledException>());

            Assert.That(code, constraint);
            var exception = (TaskCanceledException)constraint.ActualException!;
            Assert.That(exception.CancellationToken, Is.EqualTo(cancellation.Token));
        }

        [TestCase(1, 2)]
        [TestCase(3, 4)]
        public void Parameterized(int input, int expected)
        {
            Assert.That(input + 1, Is.EqualTo(expected));
        }

        [TestCaseSource(nameof(Data))]
        public void TestCaseSource(int input, int expected)
        {
            Assert.That(input * 2, Is.EqualTo(expected));
        }

        [TestCase(42)]
        public void GenericValueType<T>(T input)
        {
            Assert.That(input, Is.EqualTo(42));
        }

        private static object[] Data => new object[] { new object[] { 2, 4 } };

        private static TestCaseData[] StructResultData => new[] { new TestCaseData().Returns(new Result(42)) };

        public readonly record struct Result(int Value);

        private sealed class HiddenAwaiterTask : Task<int>
        {
            public HiddenAwaiterTask() : base(() => 42)
            {
            }

            public new TaskAwaiter<int> GetAwaiter()
            {
                throw new InvalidOperationException("NUnit must await Task<T>, not a hidden awaiter.");
            }
        }
    }
}
