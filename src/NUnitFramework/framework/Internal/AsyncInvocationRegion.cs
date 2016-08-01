// ***********************************************************************
// Copyright (c) 2013 Charlie Poole
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

#if NET_4_0 || NET_4_5 || PORTABLE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using NUnit.Compatibility;

#if NET_4_5 || PORTABLE
using System.Runtime.ExceptionServices;
#endif

namespace NUnit.Framework.Internal
{
    internal abstract class AsyncInvocationRegion : IDisposable
    {
        private const string TaskTypeName = "System.Threading.Tasks.Task";
        private const string AsyncAttributeTypeName = "System.Runtime.CompilerServices.AsyncStateMachineAttribute";

        private AsyncInvocationRegion()
        {
        }

        public static AsyncInvocationRegion Create(Delegate @delegate)
        {
#if PORTABLE
            return Create(@delegate.GetMethodInfo());
#else
            return Create(@delegate.Method);
#endif
        }

        public static AsyncInvocationRegion Create(MethodInfo method)
        {
            if (!IsAsyncOperation(method))
                throw new ArgumentException(@"Either asynchronous support is not available or an attempt 
at wrapping a non-async method invocation in an async region was done");

            if (method.ReturnType == typeof(void))
                throw new ArgumentException("'async void' methods are not supported, please use 'async Task' instead");

            return new AsyncTaskInvocationRegion();
        }

        public static bool IsAsyncOperation(MethodInfo method)
        {
            var name = method.ReturnType.FullName;
            if (name == null) return false;
            return name.StartsWith(TaskTypeName) ||
                   method.GetCustomAttributes(false).Any(attr => AsyncAttributeTypeName == attr.GetType().FullName);
        }

        public static bool IsAsyncOperation(Delegate @delegate)
        {
#if PORTABLE
            return IsAsyncOperation(@delegate.GetMethodInfo());
#else
            return IsAsyncOperation(@delegate.Method);
#endif
        }

        /// <summary>
        /// Waits for pending asynchronous operations to complete, if appropriate,
        /// and returns a proper result of the invocation by unwrapping task results
        /// </summary>
        /// <param name="invocationResult">The raw result of the method invocation</param>
        /// <returns>The unwrapped result, if necessary</returns>
        public abstract object WaitForPendingOperationsToComplete(object invocationResult);

        public virtual void Dispose()
        { }

        private class AsyncTaskInvocationRegion : AsyncInvocationRegion
        {
            private const string TaskWaitMethod = "Wait";
            private const string TaskResultProperty = "Result";
            private const string VoidTaskResultType = "VoidTaskResult";
            private const string SystemAggregateException = "System.AggregateException";
            private const string InnerExceptionsProperty = "InnerExceptions";
            private const BindingFlags TaskResultPropertyBindingFlags = BindingFlags.Instance | BindingFlags.Public;

            public override object WaitForPendingOperationsToComplete(object invocationResult)
            {
                try
                {
                    invocationResult.GetType().GetMethod(TaskWaitMethod, new Type[0]).Invoke(invocationResult, null);
                }
                catch (TargetInvocationException e)
                {
                    IList<Exception> innerExceptions = GetAllExceptions(e.InnerException);
                    ExceptionHelper.Rethrow(innerExceptions[0]);
                }
                var args = invocationResult.GetType().GetGenericArguments();
                if (args != null && args.Length == 1 && args[0].Name == VoidTaskResultType)
                {
                    return null;
                }

                PropertyInfo taskResultProperty = invocationResult.GetType().GetProperty(TaskResultProperty, TaskResultPropertyBindingFlags);

                return taskResultProperty != null ? taskResultProperty.GetValue(invocationResult, null) : invocationResult;
            }

            private static IList<Exception> GetAllExceptions(Exception exception)
            {
                if (SystemAggregateException.Equals(exception.GetType().FullName))
                    return (IList<Exception>)exception.GetType().GetProperty(InnerExceptionsProperty).GetValue(exception, null);

                return new Exception[] { exception };
            }
        }
    }
}
#endif