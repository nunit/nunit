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

using System;
using System.Reflection;
using System.Security;
using System.Threading;

#if NET45
using System.Windows.Forms;
using System.Windows.Threading;
#endif

namespace NUnit.Framework.Internal
{
    internal abstract class MessagePumpStrategy
    {
        public abstract void WaitForCompletion(AwaitAdapter awaiter);

        public static MessagePumpStrategy FromCurrentSynchronizationContext()
        {
            var context = SynchronizationContext.Current;

            if (context is SingleThreadedTestSynchronizationContext)
                return SingleThreadedTestMessagePumpStrategy.Instance;

            return WindowsFormsMessagePumpStrategy.GetIfApplicable()
                ?? WpfMessagePumpStrategy.GetIfApplicable()
                ?? NoMessagePumpStrategy.Instance;
        }

        private sealed class NoMessagePumpStrategy : MessagePumpStrategy
        {
            public static readonly NoMessagePumpStrategy Instance = new NoMessagePumpStrategy();
            private NoMessagePumpStrategy() { }

            public override void WaitForCompletion(AwaitAdapter awaiter)
            {
                awaiter.BlockUntilCompleted();
            }
        }

        private sealed class WindowsFormsMessagePumpStrategy : MessagePumpStrategy
        {
            private static WindowsFormsMessagePumpStrategy _instance;

            private readonly Action _applicationRun;
            private readonly Action _applicationExit;

            private WindowsFormsMessagePumpStrategy(Action applicationRun, Action applicationExit)
            {
                _applicationRun = applicationRun;
                _applicationExit = applicationExit;
            }

            public static MessagePumpStrategy GetIfApplicable()
            {
                if (!IsApplicable(SynchronizationContext.Current)) return null;

                if (_instance is null)
                {
                    var applicationType = SynchronizationContext.Current.GetType().Assembly.GetType("System.Windows.Forms.Application", throwOnError: true);

                    var applicationRun = (Action)applicationType
                        .GetMethod("Run", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null)
                        .CreateDelegate(typeof(Action));

                    var applicationExit = (Action)applicationType
                        .GetMethod("Exit", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null)
                        .CreateDelegate(typeof(Action));

                    _instance = new WindowsFormsMessagePumpStrategy(applicationRun, applicationExit);
                }

                return _instance;
            }

            private static bool IsApplicable(SynchronizationContext context)
            {
                return context?.GetType().FullName == "System.Windows.Forms.WindowsFormsSynchronizationContext";
            }

            [SecuritySafeCritical]
            public override void WaitForCompletion(AwaitAdapter awaiter)
            {
                var context = SynchronizationContext.Current;

                if (!IsApplicable(context))
                    throw new InvalidOperationException("This strategy must only be used from a WindowsFormsSynchronizationContext.");

                if (awaiter.IsCompleted) return;

                // Wait for a post rather than scheduling the continuation now. If there has been a race condition
                // and it completed after the IsCompleted check, it will wait until the application runs *before*
                // shutting it down. Otherwise Application.Exit is a no-op and we would then proceed to do
                // Application.Run and never return.
                context.Post(
                    state => ContinueOnSameSynchronizationContext((AwaitAdapter)state, _applicationExit),
                    state: awaiter);

                try
                {
                    _applicationRun.Invoke();
                }
                finally
                {
                    SynchronizationContext.SetSynchronizationContext(context);
                }
            }
        }

        private sealed class WpfMessagePumpStrategy : MessagePumpStrategy
        {
            private static WpfMessagePumpStrategy _instance;

            private readonly Action _dispatcherRun;
            private readonly Action _dispatcherExitAllFrames;

            private WpfMessagePumpStrategy(Action dispatcherRun, Action dispatcherExitAllFrames)
            {
                _dispatcherRun = dispatcherRun;
                _dispatcherExitAllFrames = dispatcherExitAllFrames;
            }

            public static MessagePumpStrategy GetIfApplicable()
            {
                if (!IsApplicable(SynchronizationContext.Current)) return null;

                if (_instance is null)
                {
                    var dispatcherType = SynchronizationContext.Current.GetType().Assembly.GetType("System.Windows.Threading.Dispatcher", throwOnError: true);

                    var dispatcherRun = (Action)dispatcherType
                        .GetMethod("Run", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null)
                        .CreateDelegate(typeof(Action));

                    var dispatcherExitAllFrames = (Action)dispatcherType
                        .GetMethod("ExitAllFrames", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null)
                        .CreateDelegate(typeof(Action));

                    _instance = new WpfMessagePumpStrategy(dispatcherRun, dispatcherExitAllFrames);
                }

                return _instance;
            }

            private static bool IsApplicable(SynchronizationContext context)
            {
                return context?.GetType().FullName == "System.Windows.Threading.DispatcherSynchronizationContext";
            }

            public override void WaitForCompletion(AwaitAdapter awaiter)
            {
                var context = SynchronizationContext.Current;

                if (!IsApplicable(context))
                    throw new InvalidOperationException("This strategy must only be used from a DispatcherSynchronizationContext.");

                if (awaiter.IsCompleted) return;

                // Wait for a post rather than scheduling the continuation now. If there has been a race condition
                // and it completed after the IsCompleted check, it will wait until the application runs *before*
                // shutting it down. Otherwise Dispatcher.ExitAllFrames is a no-op and we would then proceed to do
                // Dispatcher.Run and never return.
                context.Post(
                    state => ContinueOnSameSynchronizationContext((AwaitAdapter)state, _dispatcherExitAllFrames),
                    state: awaiter);

                _dispatcherRun.Invoke();
            }
        }

        private sealed class SingleThreadedTestMessagePumpStrategy : MessagePumpStrategy
        {
            public static readonly SingleThreadedTestMessagePumpStrategy Instance = new SingleThreadedTestMessagePumpStrategy();
            private SingleThreadedTestMessagePumpStrategy() { }

            public override void WaitForCompletion(AwaitAdapter awaiter)
            {
                var context = SynchronizationContext.Current as SingleThreadedTestSynchronizationContext;

                if (context == null)
                    throw new InvalidOperationException("This strategy must only be used from a SingleThreadedTestSynchronizationContext.");

                if (awaiter.IsCompleted) return;

                // Wait for a post rather than scheduling the continuation now. If there has been a race condition
                // and it completed after the IsCompleted check, it will wait until the message loop runs *before*
                // shutting it down. Otherwise context.ShutDown will throw.
                context.Post(
                    state => ContinueOnSameSynchronizationContext((AwaitAdapter)state, context.ShutDown),
                    state: awaiter);

                context.Run();
            }
        }

        private static void ContinueOnSameSynchronizationContext(AwaitAdapter awaiter, Action continuation)
        {
            if (awaiter == null) throw new ArgumentNullException(nameof(awaiter));
            if (continuation == null) throw new ArgumentNullException(nameof(continuation));

            var context = SynchronizationContext.Current;

            awaiter.OnCompleted(() =>
            {
                if (SynchronizationContext.Current == context)
                    continuation.Invoke();
                else
                    context.Post(state => ((Action)state).Invoke(), state: continuation);
            });
        }
    }
}
