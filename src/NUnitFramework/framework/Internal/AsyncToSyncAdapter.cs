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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading;
using NUnit.Compatibility;

namespace NUnit.Framework.Internal
{
    internal static class AsyncToSyncAdapter
    {
        public static bool IsAsyncOperation(MethodInfo method)
        {
            return IsTaskType(method.ReturnType) ||
                   method.GetCustomAttributes(false).Any(attr => attr.GetType().FullName == "System.Runtime.CompilerServices.AsyncStateMachineAttribute");
        }

        public static bool IsAsyncOperation(Delegate @delegate)
        {
            return IsAsyncOperation(@delegate.GetMethodInfo());
        }

        private static bool IsTaskType(Type type)
        {
            for (; type != null; type = type.GetTypeInfo().BaseType)
            {
                if (type.GetTypeInfo().IsGenericType
                    ? type.GetGenericTypeDefinition().FullName == "System.Threading.Tasks.Task`1"
                    : type.FullName == "System.Threading.Tasks.Task")
                {
                    return true;
                }
            }

            return false;
        }

#if ASYNC
        private const string TaskWaitMethod = "Wait";
        private const string TaskResultProperty = "Result";
        private const string VoidTaskResultType = "VoidTaskResult";
        private const string SystemAggregateException = "System.AggregateException";
        private const string InnerExceptionsProperty = "InnerExceptions";
        private const BindingFlags TaskResultPropertyBindingFlags = BindingFlags.Instance | BindingFlags.Public;

        public static object Await(Func<object> invoke)
        {
            Guard.ArgumentNotNull(invoke, nameof(invoke));

            object invocationResult;
            using (InitializeExecutionEnvironment())
            {
                invocationResult = invoke.Invoke();
                if (invocationResult == null || !IsTaskType(invocationResult.GetType()))
                    throw new InvalidOperationException("The delegate did not return a Task."); // General awaitable support coming soon.

                var awaitAdapter = AwaitAdapter.FromAwaitable(invocationResult);

                if (!awaitAdapter.IsCompleted)
                {
                    var waitStrategy = MessagePumpStrategy.FromCurrentSynchronizationContext();
                    waitStrategy.WaitForCompletion(awaitAdapter);
                }
            }

            // Future: instead of Wait(), use GetAwaiter() to check awaiter.IsCompleted above
            // and use awaiter.OnCompleted/awaiter.GetResult below.
            // (Implement a ReflectionAwaitAdapter)
            try
            {
                invocationResult.GetType().GetMethod(TaskWaitMethod, new Type[0]).Invoke(invocationResult, null);
            }
            catch (TargetInvocationException e)
            {
                IList<Exception> innerExceptions = GetAllExceptions(e.InnerException);
                ExceptionHelper.Rethrow(innerExceptions[0]);
            }
            var genericArguments = invocationResult.GetType().GetGenericArguments();
            if (genericArguments.Length == 1 && genericArguments[0].Name == VoidTaskResultType)
            {
                return null;
            }

            PropertyInfo taskResultProperty = invocationResult.GetType().GetProperty(TaskResultProperty, TaskResultPropertyBindingFlags);

            return taskResultProperty != null ? taskResultProperty.GetValue(invocationResult, null) : invocationResult;
        }

        private static IDisposable InitializeExecutionEnvironment()
        {
#if APARTMENT_STATE
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                var context = SynchronizationContext.Current;
                if (context == null || context.GetType() == typeof(SynchronizationContext))
                {
                    var singleThreadedContext = new SingleThreadedTestSynchronizationContext();
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

        private static IList<Exception> GetAllExceptions(Exception exception)
        {
            if (SystemAggregateException.Equals(exception.GetType().FullName))
                return (IList<Exception>)exception.GetType().GetProperty(InnerExceptionsProperty).GetValue(exception, null);

            return new Exception[] { exception };
        }
#endif
    }
}
