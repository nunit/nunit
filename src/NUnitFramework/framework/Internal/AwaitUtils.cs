// ***********************************************************************
// Copyright © 2017 Charlie Poole
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
using System.Linq;
using System.Reflection;
using System.Threading;
using NUnit.Compatibility;

namespace NUnit.Framework.Internal
{
    internal static partial class AwaitUtils
    {
        public static bool IsAsyncVoid(MethodInfo method)
        {
            // Can't use GetCustomAttributes(typeof(Type.GetType("System.Runtime.CompilerServices.AsyncStateMachineAttribute"))
            // because the same type is in multiple assemblies side by side in the case of the net40 async polyfill.
            return
                method.ReturnType == typeof(void) &&
                method.GetCustomAttributes(false)
                    .Any(_ => _.GetType().FullName == "System.Runtime.CompilerServices.AsyncStateMachineAttribute");
        }

        public static bool IsAsyncVoid(Delegate @delegate)
        {
#if NETSTANDARD1_6 || PORTABLE
            return IsAsyncVoid(@delegate.GetMethodInfo());
#else
            return IsAsyncVoid(@delegate.Method);
#endif
        }


        /// <summary>
        /// If the delegate was not created by the end user, <see cref="IsAwaitable(MethodInfo)"/> must be used instead.
        /// </summary>
        public static bool IsAwaitable(Delegate endUserDelegate)
        {
            if (endUserDelegate == null) throw new ArgumentNullException(nameof(endUserDelegate));

            // May be different from both @endUserDelegate.Method.ReturnType and invocationResult.GetType().
            // Delegate return type is what we need to care about.
            var awaitableType = GetDelegateReturnType(endUserDelegate.GetType());

            return AwaitableApiAccessors.TryCreate(awaitableType) != null;
        }

        /// <summary>
        /// If a delegate is created by the end user, <see cref="IsAwaitable(Delegate)"/> must be used instead.
        /// </summary>
        public static bool IsAwaitable(MethodInfo methodWithoutEndUserDelegate)
        {
            if (methodWithoutEndUserDelegate == null) throw new ArgumentNullException(nameof(methodWithoutEndUserDelegate));

            // May be different from both @endUserDelegate.Method.ReturnType and invocationResult.GetType().
            // Delegate return type is what we need to care about.
            var awaitableType = methodWithoutEndUserDelegate.ReturnType;

            return AwaitableApiAccessors.TryCreate(awaitableType) != null;
        }

        /// <summary>
        /// If a delegate is created by the end user, <see cref="IsAwaitable(Delegate)"/> must be used instead.
        /// </summary>
        public static Type GetAwaitableResultType(MethodInfo methodWithoutEndUserDelegate)
        {
            if (methodWithoutEndUserDelegate == null) throw new ArgumentNullException(nameof(methodWithoutEndUserDelegate));

            // May be different from both @endUserDelegate.Method.ReturnType and invocationResult.GetType().
            // Delegate return type is what we need to care about.
            var awaitableType = methodWithoutEndUserDelegate.ReturnType;

            var accessors = AwaitableApiAccessors.TryCreate(awaitableType);
            if (accessors == null)
                throw new InvalidOperationException($"{awaitableType} is not awaitable.");

            return accessors.ResultType;
        }



        private static Type GetDelegateReturnType(Type delegateType)
        {
            var invokeMethod = delegateType.GetMethod("Invoke");
            if (invokeMethod == null) throw new ArgumentException($"{delegateType} is not a delegate type.", nameof(delegateType));
            return invokeMethod.ReturnType;
        }


        /// <summary>
        /// If the delegate is awaitable, blocks until the awaitable result is available, then returns the awaited result.
        /// If the delegate was not created by the end user, <see cref="TryAwait(MethodInfo, object, out object)"/> must be used instead.
        /// </summary>
        public static bool TryAwait(Delegate endUserDelegate, object invocationResult, out object result)
        {
            if (endUserDelegate == null) throw new ArgumentNullException(nameof(endUserDelegate));

            // May be different from both endUserDelegate.Method.ReturnType and invocationResult.GetType().
            // Delegate return type is what we need to care about.
            var typeToCheckForAwaitability = GetDelegateReturnType(endUserDelegate.GetType());

            return TryBlockingAwait(typeToCheckForAwaitability, invocationResult, out result);
        }

        /// <summary>
        /// Blocks until the awaitable result is available, then returns the awaited result.
        /// If the method's return type is not awaitable, throws <see cref="InvalidOperationException"/>.
        /// If the delegate was not created by the end user, <see cref="Await(MethodInfo, object)"/> must be used instead.
        /// </summary>
        public static object Await(Delegate endUserDelegate, object invocationResult)
        {
            if (endUserDelegate == null) throw new ArgumentNullException(nameof(endUserDelegate));

            // May be different from both endUserDelegate.Method.ReturnType and invocationResult.GetType().
            // Delegate return type is what we need to care about.
            var awaitableType = GetDelegateReturnType(endUserDelegate.GetType());

            object result;
            if (!TryAwait(endUserDelegate, invocationResult, out result))
                throw new InvalidOperationException($"{awaitableType} is not awaitable.");
            return result;
        }

        /// <summary>
        /// If the delegate is awaitable, blocks until the awaitable result is available, then returns the awaited result.
        /// If the delegate was not created by the end user, <see cref="TryAwait(Delegate, object, out object)"/> must be used instead.
        /// </summary>
        public static bool TryAwait(MethodInfo methodWithoutEndUserDelegate, object invocationResult, out object result)
        {
            if (methodWithoutEndUserDelegate == null) throw new ArgumentNullException(nameof(methodWithoutEndUserDelegate));

            // May be different from invocationResult.GetType().
            var typeToCheckForAwaitability = methodWithoutEndUserDelegate.ReturnType;

            return TryBlockingAwait(typeToCheckForAwaitability, invocationResult, out result);
        }

        /// <summary>
        /// Blocks until the awaitable result is available, then returns the awaited result.
        /// If the method's return type is not awaitable, throws <see cref="InvalidOperationException"/>.
        /// If a delegate is created by the end user, <see cref="Await(Delegate, object)"/> must be used instead.
        /// </summary>
        public static object Await(MethodInfo methodWithoutEndUserDelegate, object invocationResult)
        {
            if (methodWithoutEndUserDelegate == null) throw new ArgumentNullException(nameof(methodWithoutEndUserDelegate));

            // May be different from invocationResult.GetType().
            var awaitableType = methodWithoutEndUserDelegate.ReturnType;

            object result;
            if (!TryBlockingAwait(awaitableType, invocationResult, out result))
                throw new InvalidOperationException($"{awaitableType} is not awaitable.");
            return result;
        }


        private static bool TryBlockingAwait(Type awaitableType, object invocationResult, out object result)
        {
            // https://github.com/dotnet/csharplang/blob/master/spec/expressions.md#runtime-evaluation-of-await-expressions

            var accessors = AwaitableApiAccessors.TryCreate(awaitableType);
            if (accessors == null)
            {
                result = null;
                return false;
            }

            var awaiter = accessors.GetAwaiter(invocationResult);
            if (!accessors.IsCompleted(awaiter))
            {
                using (var mres = new ManualResetEventSlim())
                {
                    accessors.OnCompleted(awaiter, mres.Set);
                    mres.Wait();
                }
            }

            result = accessors.GetResult(awaiter);
            return true;
        }
    }
}
#endif