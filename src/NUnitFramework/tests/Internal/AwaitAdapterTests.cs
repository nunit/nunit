// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Internal
{
    [TestFixture]
    public class AwaitAdapterTests
    {
        [TestCase(typeof(Task), typeof(void))]
        [TestCase(typeof(Task<int>), typeof(int))]
        [TestCase(typeof(Task<string>), typeof(string))]
        [TestCase(typeof(ValueTask), typeof(void))]
        [TestCase(typeof(ValueTask<int>), typeof(int))]
        [TestCase(typeof(ValueTask<string>), typeof(string))]
        public void TaskTypesAreAwaitableWithTheSameResultTypeAsThePatternBasedLookup(Type awaitableType, Type expectedResultType)
        {
            Assert.Multiple(() =>
            {
                Assert.That(AwaitAdapter.IsAwaitable(awaitableType), Is.True);
                Assert.That(AwaitAdapter.GetResultType(awaitableType), Is.EqualTo(expectedResultType));
                Assert.That(CSharpPatternBasedAwaitAdapter.GetResultType(awaitableType), Is.EqualTo(expectedResultType));
            });
        }

        [TestCase(typeof(CustomAwaitable), typeof(int))]
        [TestCase(typeof(CriticalCustomAwaitable), typeof(string))]
        public void CustomAwaitablesAreStillRecognized(Type awaitableType, Type expectedResultType)
        {
            Assert.Multiple(() =>
            {
                Assert.That(AwaitAdapter.IsAwaitable(awaitableType), Is.True);
                Assert.That(AwaitAdapter.GetResultType(awaitableType), Is.EqualTo(expectedResultType));
            });
        }

        [Test]
        public void AwaiterWithoutINotifyCompletionIsNotAwaitable()
        {
            Assert.That(AwaitAdapter.IsAwaitable(typeof(NotifylessAwaitable)), Is.False);
        }

        [Test]
        public void CustomAwaitableIsAwaitedThroughUnsafeOnCompletedWhenAvailable()
        {
            var awaitable = new CriticalCustomAwaitable();
            var adapter = AwaitAdapter.FromAwaitable(awaitable);

            adapter.OnCompleted(() => { });

            Assert.That(awaitable.Awaiter.UnsafeOnCompletedCalled, Is.True);
            Assert.That(awaitable.Awaiter.OnCompletedCalled, Is.False);
        }

        [Test]
        public void TypeDerivedFromTaskIsRecognized()
        {
            Assert.That(AwaitAdapter.GetResultType(typeof(DerivedTask)), Is.EqualTo(typeof(void)));
        }

        private sealed class DerivedTask : Task
        {
            public DerivedTask() : base(() => { })
            {
            }
        }

        private sealed class CustomAwaitable
        {
            public Awaiter GetAwaiter() => new Awaiter();

            public sealed class Awaiter : INotifyCompletion
            {
                public bool IsCompleted => true;
                public int GetResult() => 42;
                public void OnCompleted(Action continuation) => continuation();
            }
        }

        private sealed class CriticalCustomAwaitable
        {
            public CriticalAwaiter Awaiter { get; } = new CriticalAwaiter();

            public CriticalAwaiter GetAwaiter() => Awaiter;

            public sealed class CriticalAwaiter : ICriticalNotifyCompletion
            {
                public bool OnCompletedCalled { get; private set; }
                public bool UnsafeOnCompletedCalled { get; private set; }

                public bool IsCompleted => true;
                public string GetResult() => "result";
                public void OnCompleted(Action continuation) => OnCompletedCalled = true;
                public void UnsafeOnCompleted(Action continuation) => UnsafeOnCompletedCalled = true;
            }
        }

        private sealed class NotifylessAwaitable
        {
            public Awaiter GetAwaiter() => new Awaiter();

            public sealed class Awaiter
            {
                public bool IsCompleted => true;
                public void GetResult()
                {
                }
                public void OnCompleted(Action continuation) => continuation();
            }
        }
    }
}
