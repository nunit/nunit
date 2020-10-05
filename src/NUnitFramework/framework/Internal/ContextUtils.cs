// ***********************************************************************
// Copyright (c) 2020 Charlie Poole, Rob Prouse
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
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace NUnit.Framework.Internal
{
    internal static class ContextUtils
    {
        public static void DoIsolated(Action action)
        {
            DoIsolated(state => ((Action)state).Invoke(), state: action);
        }

        public static T DoIsolated<T>(Func<T> func)
        {
            var returnValue = default(T);
            DoIsolated(_ => returnValue = func.Invoke(), state: null);
            return returnValue;
        }

        [SecuritySafeCritical]
        public static void DoIsolated(ContextCallback callback, object state)
        {
            var previousState = SandboxedThreadState.Capture();
            try
            {
                var executionContext = ExecutionContext.Capture()
                    ?? throw new InvalidOperationException("Execution context flow must not be suppressed.");

                using ((object)executionContext as IDisposable)
                {
                    ExecutionContext.Run(executionContext, callback, state);
                }
            }
            finally
            {
                previousState.Restore();
            }
        }

        public static async Task DoIsolatedAsync(Func<Task> callback)
        {
            var previousState = SandboxedThreadState.Capture();
            try
            {
                var executionContext = ExecutionContext.Capture()
                    ?? throw new InvalidOperationException("Execution context flow must not be suppressed.");

                using ((object)executionContext as IDisposable)
                {
                    TaskScheduler ts = null;
                    var sc = new SynchronizationContext();

                    ExecutionContext.Run(executionContext, _ =>
                    {
                        var old = SynchronizationContext.Current;
                        SynchronizationContext.SetSynchronizationContext(sc);
                        try
                        {
                            ts = TaskScheduler.FromCurrentSynchronizationContext();
                        }
                        finally
                        {
                            SynchronizationContext.SetSynchronizationContext(old);
                        }
                    }, null);

                    await Task.Factory.StartNew(callback, default, TaskCreationOptions.None, ts).Unwrap();
                }
            }
            finally
            {
                previousState.Restore();
            }
        }
    }
}
