// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Runtime.CompilerServices;

namespace NUnit.TestData
{
    public sealed class AwaitableReturnTypeFixture
    {
        private readonly AsyncWorkload _workload;

        public AwaitableReturnTypeFixture(AsyncWorkload workload)
        {
            _workload = workload;
        }

        #region Void result

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

        #endregion

        #region Non-void result

#pragma warning disable 1998
        [Test(ExpectedResult = 42)]
        public Task<object> ReturnsNonVoidResultTask()
#pragma warning restore 1998
        {
            _workload.BeforeReturningAwaitable();
            _workload.BeforeReturningAwaiter();

            var source = new TaskCompletionSource<object>();

            var complete = new Action(() =>
            {
                try
                {
                    source.SetResult(_workload.GetResult());
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

        [Test(ExpectedResult = 42)]
        public NonVoidResultCustomTask ReturnsNonVoidResultCustomTask()
        {
            _workload.BeforeReturningAwaitable();
            _workload.BeforeReturningAwaiter();

            var task = new NonVoidResultCustomTask(_workload.GetResult);

            if (_workload.IsCompleted)
                task.Start();
            else
                _workload.OnCompleted(task.Start);

            return task;
        }

        public sealed class NonVoidResultCustomTask : Task<object>
        {
            public NonVoidResultCustomTask(Func<object> function) : base(function)
            {
            }
        }

        [Test(ExpectedResult = 42)]
        public NonVoidResultCustomAwaitable ReturnsNonVoidResultCustomAwaitable()
        {
            _workload.BeforeReturningAwaitable();
            return new NonVoidResultCustomAwaitable(_workload);
        }

        public struct NonVoidResultCustomAwaitable
        {
            private readonly AsyncWorkload _workload;

            public NonVoidResultCustomAwaitable(AsyncWorkload workload)
            {
                _workload = workload;
            }

            public NonVoidResultCustomAwaiter GetAwaiter()
            {
                _workload.BeforeReturningAwaiter();
                return new NonVoidResultCustomAwaiter(_workload);
            }

            public struct NonVoidResultCustomAwaiter : ICriticalNotifyCompletion
            {
                private readonly AsyncWorkload _workload;

                public NonVoidResultCustomAwaiter(AsyncWorkload workload)
                {
                    _workload = workload;
                }

                public bool IsCompleted => _workload.IsCompleted;

                public void OnCompleted(Action continuation)
                {
                    Assert.Fail("This method should not be used because UnsafeOnCompleted is available.");
                }

                public void UnsafeOnCompleted(Action continuation) => _workload.OnCompleted(continuation);

                public object GetResult() => _workload.GetResult();
            }
        }

        [Test(ExpectedResult = 42)]
        public NonVoidResultCustomAwaitableWithImplicitOnCompleted ReturnsNonVoidResultCustomAwaitableWithImplicitOnCompleted()
        {
            _workload.BeforeReturningAwaitable();
            return new NonVoidResultCustomAwaitableWithImplicitOnCompleted(_workload);
        }

        public struct NonVoidResultCustomAwaitableWithImplicitOnCompleted
        {
            private readonly AsyncWorkload _workload;

            public NonVoidResultCustomAwaitableWithImplicitOnCompleted(AsyncWorkload workload)
            {
                _workload = workload;
            }

            public NonVoidResultCustomAwaiterWithImplicitOnCompleted GetAwaiter()
            {
                _workload.BeforeReturningAwaiter();
                return new NonVoidResultCustomAwaiterWithImplicitOnCompleted(_workload);
            }

            public struct NonVoidResultCustomAwaiterWithImplicitOnCompleted : INotifyCompletion
            {
                private readonly AsyncWorkload _workload;

                public NonVoidResultCustomAwaiterWithImplicitOnCompleted(AsyncWorkload workload)
                {
                    _workload = workload;
                }

                public bool IsCompleted => _workload.IsCompleted;

                void INotifyCompletion.OnCompleted(Action continuation) => _workload.OnCompleted(continuation);

                public object GetResult() => _workload.GetResult();
            }
        }

        [Test(ExpectedResult = 42)]
        public NonVoidResultCustomAwaitableWithImplicitUnsafeOnCompleted ReturnsNonVoidResultCustomAwaitableWithImplicitUnsafeOnCompleted()
        {
            _workload.BeforeReturningAwaitable();
            return new NonVoidResultCustomAwaitableWithImplicitUnsafeOnCompleted(_workload);
        }

        public struct NonVoidResultCustomAwaitableWithImplicitUnsafeOnCompleted
        {
            private readonly AsyncWorkload _workload;

            public NonVoidResultCustomAwaitableWithImplicitUnsafeOnCompleted(AsyncWorkload workload)
            {
                _workload = workload;
            }

            public NonVoidResultCustomAwaiterWithImplicitUnsafeOnCompleted GetAwaiter()
            {
                _workload.BeforeReturningAwaiter();
                return new NonVoidResultCustomAwaiterWithImplicitUnsafeOnCompleted(_workload);
            }

            public struct NonVoidResultCustomAwaiterWithImplicitUnsafeOnCompleted : ICriticalNotifyCompletion
            {
                private readonly AsyncWorkload _workload;

                public NonVoidResultCustomAwaiterWithImplicitUnsafeOnCompleted(AsyncWorkload workload)
                {
                    _workload = workload;
                }

                public bool IsCompleted => _workload.IsCompleted;

                public void OnCompleted(Action continuation)
                {
                    Assert.Fail("This method should not be used because UnsafeOnCompleted is available.");
                }

                void ICriticalNotifyCompletion.UnsafeOnCompleted(Action continuation) => _workload.OnCompleted(continuation);

                public object GetResult() => _workload.GetResult();
            }
        }

        #endregion
    }
}
