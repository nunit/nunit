// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using System.Threading;

namespace NUnit.Framework.Internal
{
    internal static class AsyncToSyncAdapter
    {
        public static bool IsAsyncOperation(MethodInfo method)
        {
            return AwaitAdapter.IsAwaitable(method.ReturnType)
                || method.GetCustomAttributes(false).Any(attr => attr.GetType().FullName == "System.Runtime.CompilerServices.AsyncStateMachineAttribute");
        }

        public static bool IsAsyncOperation(Delegate @delegate)
        {
            return IsAsyncOperation(@delegate.GetMethodInfo());
        }

        public static object Await(Func<object> invoke)
        {
            Guard.ArgumentNotNull(invoke, nameof(invoke));

            using (InitializeExecutionEnvironment())
            {
                var awaiter = AwaitAdapter.FromAwaitable(invoke.Invoke());

                if (!awaiter.IsCompleted)
                {
                    var waitStrategy = MessagePumpStrategy.FromCurrentSynchronizationContext();
                    waitStrategy.WaitForCompletion(awaiter);
                }

                return awaiter.GetResult();
            }
        }

        private static IDisposable InitializeExecutionEnvironment()
        {
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                var context = SynchronizationContext.Current;
                if (context == null || context.GetType() == typeof(SynchronizationContext))
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

        [SecuritySafeCritical]
        private static void SetSynchronizationContext(SynchronizationContext syncContext)
        {
            SynchronizationContext.SetSynchronizationContext(syncContext);
        }
    }
}
