using System;
using System.Runtime.CompilerServices;
using NUnit.Framework;

#if ASYNC
using System.Threading.Tasks;
#endif

namespace NUnit.TestData
{
    public sealed class AwaitableReturnTypeFixture
    {
        private readonly AsyncWorkload _workload;

        public AwaitableReturnTypeFixture(AsyncWorkload workload)
        {
            _workload = workload;
        }

#if ASYNC
#pragma warning disable 1998
        public Task ReturnsTask()
#pragma warning restore 1998
        {
            _workload.BeforeReturningAwaitable();
            _workload.BeforeReturningAwaiter();

            var source = new TaskCompletionSource<object>();

            var complete = new Action(() =>
            {
                try
                {
                    _workload.GetResult();
                    source.SetResult(null);
                }
                catch (Exception ex)
                {
                    source.SetException(ex);
                }
            });

            if (_workload.IsCompleted)
                complete.Invoke();
            else
                _workload.OnCompleted(complete);

            return source.Task;
        }

        public CustomTask ReturnsCustomTask()
        {
            _workload.BeforeReturningAwaitable();
            _workload.BeforeReturningAwaiter();

            var task = new CustomTask(() => _workload.GetResult());

            if (_workload.IsCompleted)
                task.Start();
            else
                _workload.OnCompleted(task.Start);

            return task;
        }

        public sealed class CustomTask : Task
        {
            public CustomTask(Action action) : base(action)
            {
            }
        }
#endif

        public CustomAwaitable ReturnsCustomAwaitable()
        {
            _workload.BeforeReturningAwaitable();
            return new CustomAwaitable(_workload);
        }

        public struct CustomAwaitable
        {
            private readonly AsyncWorkload _workload;

            public CustomAwaitable(AsyncWorkload workload)
            {
                _workload = workload;
            }

            public CustomAwaiter GetAwaiter()
            {
                _workload.BeforeReturningAwaiter();
                return new CustomAwaiter(_workload);
            }

            public struct CustomAwaiter : ICriticalNotifyCompletion
            {
                private readonly AsyncWorkload _workload;

                public CustomAwaiter(AsyncWorkload workload)
                {
                    _workload = workload;
                }

                public bool IsCompleted => _workload.IsCompleted;

                public void OnCompleted(Action continuation)
                {
                    Assert.Fail("This method should not be used because UnsafeOnCompleted is available.");
                }

                public void UnsafeOnCompleted(Action continuation) => _workload.OnCompleted(continuation);

                public void GetResult() => _workload.GetResult();
            }
        }

        public CustomAwaitableWithImplicitOnCompleted ReturnsCustomAwaitableWithImplicitOnCompleted()
        {
            _workload.BeforeReturningAwaitable();
            return new CustomAwaitableWithImplicitOnCompleted(_workload);
        }

        public struct CustomAwaitableWithImplicitOnCompleted
        {
            private readonly AsyncWorkload _workload;

            public CustomAwaitableWithImplicitOnCompleted(AsyncWorkload workload)
            {
                _workload = workload;
            }

            public CustomAwaiterWithImplicitOnCompleted GetAwaiter()
            {
                _workload.BeforeReturningAwaiter();
                return new CustomAwaiterWithImplicitOnCompleted(_workload);
            }

            public struct CustomAwaiterWithImplicitOnCompleted : INotifyCompletion
            {
                private readonly AsyncWorkload _workload;

                public CustomAwaiterWithImplicitOnCompleted(AsyncWorkload workload)
                {
                    _workload = workload;
                }

                public bool IsCompleted => _workload.IsCompleted;

                void INotifyCompletion.OnCompleted(Action continuation) => _workload.OnCompleted(continuation);

                public void GetResult() => _workload.GetResult();
            }
        }

        public CustomAwaitableWithImplicitUnsafeOnCompleted ReturnsCustomAwaitableWithImplicitUnsafeOnCompleted()
        {
            _workload.BeforeReturningAwaitable();
            return new CustomAwaitableWithImplicitUnsafeOnCompleted(_workload);
        }

        public struct CustomAwaitableWithImplicitUnsafeOnCompleted
        {
            private readonly AsyncWorkload _workload;

            public CustomAwaitableWithImplicitUnsafeOnCompleted(AsyncWorkload workload)
            {
                _workload = workload;
            }

            public CustomAwaiterWithImplicitUnsafeOnCompleted GetAwaiter()
            {
                _workload.BeforeReturningAwaiter();
                return new CustomAwaiterWithImplicitUnsafeOnCompleted(_workload);
            }

            public struct CustomAwaiterWithImplicitUnsafeOnCompleted : ICriticalNotifyCompletion
            {
                private readonly AsyncWorkload _workload;

                public CustomAwaiterWithImplicitUnsafeOnCompleted(AsyncWorkload workload)
                {
                    _workload = workload;
                }

                public bool IsCompleted => _workload.IsCompleted;

                public void OnCompleted(Action continuation)
                {
                    Assert.Fail("This method should not be used because UnsafeOnCompleted is available.");
                }

                void ICriticalNotifyCompletion.UnsafeOnCompleted(Action continuation) => _workload.OnCompleted(continuation);

                public void GetResult() => _workload.GetResult();
            }
        }
    }
}
