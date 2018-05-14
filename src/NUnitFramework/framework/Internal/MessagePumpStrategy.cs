// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
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

#if ASYNC
using System;
using System.Security;
using System.Threading;

#if NET40 || NET45
using System.Windows.Forms;
using System.Windows.Threading;
#endif

namespace NUnit.Framework.Internal
{
    internal abstract class MessagePumpStrategy
    {
        public abstract void WaitForCompletion(AwaitAdapter awaitable);

        public static MessagePumpStrategy FromCurrentSynchronizationContext()
        {
            var context = SynchronizationContext.Current;

            if (context is SingleThreadedTestSynchronizationContext)
                return SingleThreadedTestMessagePumpStrategy.Instance;

#if NET40 || NET45
            if (context is WindowsFormsSynchronizationContext)
                return WindowsFormsMessagePumpStrategy.Instance;

            if (context is DispatcherSynchronizationContext)
                return WpfMessagePumpStrategy.Instance;
#endif

            return NoMessagePumpStrategy.Instance;
        }

        private sealed class NoMessagePumpStrategy : MessagePumpStrategy
        {
            public static readonly NoMessagePumpStrategy Instance = new NoMessagePumpStrategy();
            private NoMessagePumpStrategy() { }

            public override void WaitForCompletion(AwaitAdapter awaitable)
            {
                awaitable.BlockUntilCompleted();
            }
        }

#if NET40 || NET45
        private sealed class WindowsFormsMessagePumpStrategy : MessagePumpStrategy
        {
            public static readonly WindowsFormsMessagePumpStrategy Instance = new WindowsFormsMessagePumpStrategy();
            private WindowsFormsMessagePumpStrategy() { }

            [SecuritySafeCritical]
            public override void WaitForCompletion(AwaitAdapter awaitable)
            {
                var context = SynchronizationContext.Current;

                if (!(context is WindowsFormsSynchronizationContext))
                    throw new InvalidOperationException("This strategy must only be used from a WindowsFormsSynchronizationContext.");

                if (awaitable.IsCompleted) return;

                // Wait for a post rather than scheduling the continuation now. If there has been a race condition
                // and it completed after the IsCompleted check, it will wait until the application runs *before*
                // shutting it down. Otherwise Application.Exit is a no-op and we would then proceed to do
                // Application.Run and never return.
                context.Post(
                    state => ContinueOnSameSynchronizationContext((AwaitAdapter)state, Application.Exit),
                    state: awaitable);

                try
                {
                    Application.Run();
                }
                finally
                {
                    SynchronizationContext.SetSynchronizationContext(context);
                }
            }
        }

        private sealed class WpfMessagePumpStrategy : MessagePumpStrategy
        {
            public static readonly WpfMessagePumpStrategy Instance = new WpfMessagePumpStrategy();
            private WpfMessagePumpStrategy() { }

            public override void WaitForCompletion(AwaitAdapter awaitable)
            {
                var context = SynchronizationContext.Current;

                if (!(context is DispatcherSynchronizationContext))
                    throw new InvalidOperationException("This strategy must only be used from a DispatcherSynchronizationContext.");

                if (awaitable.IsCompleted) return;

                // Wait for a post rather than scheduling the continuation now. If there has been a race condition
                // and it completed after the IsCompleted check, it will wait until the application runs *before*
                // shutting it down. Otherwise Dispatcher.ExitAllFrames is a no-op and we would then proceed to do
                // Dispatcher.Run and never return.
                context.Post(
                    state => ContinueOnSameSynchronizationContext((AwaitAdapter)state, Dispatcher.ExitAllFrames),
                    state: awaitable);

                Dispatcher.Run();
            }
        }
#endif

        private sealed class SingleThreadedTestMessagePumpStrategy : MessagePumpStrategy
        {
            public static readonly SingleThreadedTestMessagePumpStrategy Instance = new SingleThreadedTestMessagePumpStrategy();
            private SingleThreadedTestMessagePumpStrategy() { }

            public override void WaitForCompletion(AwaitAdapter awaitable)
            {
                var context = SynchronizationContext.Current as SingleThreadedTestSynchronizationContext;

                if (context == null)
                    throw new InvalidOperationException("This strategy must only be used from a SingleThreadedTestSynchronizationContext.");

                if (awaitable.IsCompleted) return;

                // Wait for a post rather than scheduling the continuation now. If there has been a race condition
                // and it completed after the IsCompleted check, it will wait until the message loop runs *before*
                // shutting it down. Otherwise context.ShutDown will throw.
                context.Post(
                    state => ContinueOnSameSynchronizationContext((AwaitAdapter)state, context.ShutDown),
                    state: awaitable);

                context.Run();
            }
        }

        private static void ContinueOnSameSynchronizationContext(AwaitAdapter adapter, Action continuation)
        {
            if (adapter == null) throw new ArgumentNullException(nameof(adapter));
            if (continuation == null) throw new ArgumentNullException(nameof(continuation));

            var context = SynchronizationContext.Current;

            adapter.OnCompleted(() =>
            {
                if (SynchronizationContext.Current == context)
                    continuation.Invoke();
                else
                    context.Post(state => ((Action)state).Invoke(), state: continuation);
            });
        }
    }
}
#endif
