// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;

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
            public static readonly NoMessagePumpStrategy Instance = new();
            private NoMessagePumpStrategy()
            {
            }

            public override void WaitForCompletion(AwaitAdapter awaiter)
            {
                awaiter.BlockUntilCompleted();
            }
        }

        private sealed class WindowsFormsMessagePumpStrategy : MessagePumpStrategy
        {
            private static WindowsFormsMessagePumpStrategy? _instance;

            private readonly Action _applicationRun;
            private readonly Action _applicationExit;

            private WindowsFormsMessagePumpStrategy(Action applicationRun, Action applicationExit)
            {
                _applicationRun = applicationRun;
                _applicationExit = applicationExit;
            }

            public static MessagePumpStrategy? GetIfApplicable()
            {
                if (!IsApplicable(SynchronizationContext.Current))
                    return null;

                if (_instance is null)
                {
                    var applicationType = SynchronizationContext.Current.GetType().Assembly.GetType("System.Windows.Forms.Application", throwOnError: true)!;

                    var applicationRun = (Action)applicationType
                        .GetMethod("Run", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null)!
                        .CreateDelegate(typeof(Action));

                    var applicationExit = (Action)applicationType
                        .GetMethod("Exit", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null)!
                        .CreateDelegate(typeof(Action));

                    _instance = new WindowsFormsMessagePumpStrategy(applicationRun, applicationExit);
                }

                return _instance;
            }

            private static bool IsApplicable([NotNullWhen(true)] SynchronizationContext? context)
            {
                return context?.GetType().FullName == "System.Windows.Forms.WindowsFormsSynchronizationContext";
            }

            public override void WaitForCompletion(AwaitAdapter awaiter)
            {
                var context = SynchronizationContext.Current;

                if (!IsApplicable(context))
                    throw new InvalidOperationException("This strategy must only be used from a WindowsFormsSynchronizationContext.");

                if (awaiter.IsCompleted)
                    return;

                // Wait for a post rather than scheduling the continuation now. If there has been a race condition
                // and it completed after the IsCompleted check, it will wait until the application runs *before*
                // shutting it down. Otherwise Application.Exit is a no-op and we would then proceed to do
                // Application.Run and never return.
                context.Post(
                    _ => ContinueOnSameSynchronizationContext(awaiter, _applicationExit),
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
            private static WpfMessagePumpStrategy? _instance;

            private readonly MethodInfo _dispatcherPushFrame;
            private readonly MethodInfo _dispatcherFrameSetContinueProperty;
            private readonly Type _dispatcherFrameType;

            private WpfMessagePumpStrategy(
                MethodInfo dispatcherPushFrame,
                MethodInfo dispatcherFrameSetContinueProperty,
                Type dispatcherFrameType)
            {
                _dispatcherPushFrame = dispatcherPushFrame;
                _dispatcherFrameSetContinueProperty = dispatcherFrameSetContinueProperty;
                _dispatcherFrameType = dispatcherFrameType;
            }

            public static MessagePumpStrategy? GetIfApplicable()
            {
                SynchronizationContext? context = SynchronizationContext.Current;

                if (!IsApplicable(context))
                    return null;

                if (_instance is null)
                {
                    var assemblyType = context.GetType().Assembly;
                    var dispatcherType = assemblyType.GetType("System.Windows.Threading.Dispatcher", throwOnError: true)!;
                    var dispatcherFrameType = assemblyType.GetType("System.Windows.Threading.DispatcherFrame", throwOnError: true)!;

                    var dispatcherPushFrame = dispatcherType
                        .GetMethod("PushFrame", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, new[] { dispatcherFrameType }, null)!;

                    var dispatcherSetFrameContinue = dispatcherFrameType
                        .GetProperty("Continue")?
                        .GetSetMethod()!;

                    _instance = new WpfMessagePumpStrategy(
                        dispatcherPushFrame,
                        dispatcherSetFrameContinue,
                        dispatcherFrameType);
                }

                return _instance;
            }

            private static bool IsApplicable([NotNullWhen(true)] SynchronizationContext? context)
            {
                return context?.GetType().FullName == "System.Windows.Threading.DispatcherSynchronizationContext";
            }

            public override void WaitForCompletion(AwaitAdapter awaiter)
            {
                var context = SynchronizationContext.Current;

                if (!IsApplicable(context))
                    throw new InvalidOperationException("This strategy must only be used from a DispatcherSynchronizationContext.");

                if (awaiter.IsCompleted)
                    return;

                // We are going to start a new frame which ensures the dispatcher is running our continuation.
                // As soon as the continuation is finished we set the frame to not continue - this will force the call
                // to PushFrame to return
                // If there was already a frame running before it will still run after we return
                // If the continuation was run and finished by the previous frame our call to PushFrame is more or less a no-op
                var frame = Activator.CreateInstance(_dispatcherFrameType, true);

                context.Post(
                    _ => ContinueOnSameSynchronizationContext(
                        awaiter,
                        () => _dispatcherFrameSetContinueProperty.Invoke(frame, new object[] { false })),
                    state: awaiter);

                _dispatcherPushFrame.Invoke(null, new[] { frame });
            }
        }

        private sealed class SingleThreadedTestMessagePumpStrategy : MessagePumpStrategy
        {
            public static readonly SingleThreadedTestMessagePumpStrategy Instance = new();
            private SingleThreadedTestMessagePumpStrategy()
            {
            }

            public override void WaitForCompletion(AwaitAdapter awaiter)
            {
                var context = SynchronizationContext.Current as SingleThreadedTestSynchronizationContext
                    ?? throw new InvalidOperationException("This strategy must only be used from a SingleThreadedTestSynchronizationContext.");

                if (awaiter.IsCompleted)
                    return;

                // Wait for a post rather than scheduling the continuation now. If there has been a race condition
                // and it completed after the IsCompleted check, it will wait until the message loop runs *before*
                // shutting it down. Otherwise context.ShutDown will throw.
                context.Post(
                    _ => ContinueOnSameSynchronizationContext(awaiter, context.ShutDown),
                    state: awaiter);

                context.Run();
            }
        }

        private static void ContinueOnSameSynchronizationContext(AwaitAdapter awaiter, Action continuation)
        {
            if (awaiter is null)
                throw new ArgumentNullException(nameof(awaiter));
            if (continuation is null)
                throw new ArgumentNullException(nameof(continuation));

            var context = SynchronizationContext.Current;

            awaiter.OnCompleted(() =>
            {
                if (context is null || SynchronizationContext.Current == context)
                    continuation.Invoke();
                else
                    context.Post(_ => continuation.Invoke(), state: continuation);
            });
        }
    }
}
