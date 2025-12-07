// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Reflection;
using System.Threading;

namespace NUnit.Framework.Internal
{
    internal static class AsyncToSyncAdapter
    {
        private static readonly Type? AsyncStateMachineAttributeType = Type.GetType("System.Runtime.CompilerServices.AsyncStateMachineAttribute, System.Runtime", false);

        public static bool IsAsyncOperation(MethodInfo method)
        {
            return AwaitAdapter.IsAwaitable(method.ReturnType)
                || (AsyncStateMachineAttributeType is not null && method.IsDefined(AsyncStateMachineAttributeType, false));
        }

        public static bool IsAsyncOperation(Delegate @delegate)
        {
            return IsAsyncOperation(@delegate.GetMethodInfo());
        }

        public static object? Await(TestExecutionContext? context, Func<object?> invoke)
            => Await<object?>(context, invoke);

        public static TResult? Await<TResult>(TestExecutionContext? context, Func<object?> invoke)
        {
            Guard.ArgumentNotNull(invoke, nameof(invoke));

            using (InitializeExecutionEnvironment(context))
            {
                var awaiter = AwaitAdapter.FromAwaitable(invoke.Invoke());

                if (!awaiter.IsCompleted)
                {
                    var waitStrategy = MessagePumpStrategy.FromCurrentSynchronizationContext();
                    waitStrategy.WaitForCompletion(awaiter);
                }

                return (TResult?)awaiter.GetResult();
            }
        }
        private static IDisposable? InitializeExecutionEnvironment(TestExecutionContext? executionContext)
        {
            if (executionContext?.IsSingleThreaded == true ||
                Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                var context = SynchronizationContext.Current;
                if (context is null || context.GetType() == typeof(SynchronizationContext) ||
                    context.GetType() == typeof(SafeSynchronizationContext))
                {
                    var singleThreadedContext = new SingleThreadedTestSynchronizationContext(
                        shutdownTimeout: TimeSpan.FromSeconds(10));

                    SetSynchronizationContext(singleThreadedContext);

                    return On.Dispose(() =>
                    {
                        SetSynchronizationContext(context);
                        singleThreadedContext.Dispose();
                    });
                }
            }

            return null;
        }

        private static void SetSynchronizationContext(SynchronizationContext? syncContext)
        {
            SynchronizationContext.SetSynchronizationContext(syncContext);
        }
    }
}
