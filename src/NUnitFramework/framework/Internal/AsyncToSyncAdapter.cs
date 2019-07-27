// ***********************************************************************
// Copyright (c) 2013–2018 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;

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

        public static object Await(Func<object> invoke, IAsyncCompletionStrategy waitStrategy = null)
        {
            Guard.ArgumentNotNull(invoke, nameof(invoke));

            using (InitializeExecutionEnvironment())
            {
                var awaitAdapter = AwaitAdapter.FromAwaitable(invoke.Invoke());

                if (!awaitAdapter.IsCompleted)
                {
                    if (waitStrategy == null)
                        waitStrategy = MessagePumpStrategy.FromCurrentSynchronizationContext();
                    waitStrategy.WaitForCompletion(awaitAdapter);
                }

                return awaitAdapter.GetResult();
            }
        }

        private static IDisposable InitializeExecutionEnvironment()
        {
#if APARTMENT_STATE
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
#endif
            return null;
        }

        [SecuritySafeCritical]
        private static void SetSynchronizationContext(SynchronizationContext syncContext)
        {
            SynchronizationContext.SetSynchronizationContext(syncContext);
        }
    }
}
