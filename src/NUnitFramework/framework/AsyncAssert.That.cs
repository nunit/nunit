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
using System.Threading.Tasks;
using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
    public static partial class AsyncAssert
    {
        /// <summary>
        /// Applies a constraint to an async delegate, succeeding if the constraint is satisfied and throwing an assertion exception on failure.
        /// </summary>
        /// <param name="code">The async delegate to execute.</param>
        /// <param name="expression">The constraint expression to apply to the delegate.</param>
        public static Task That(AsyncTestDelegate code, IResolveConstraint expression)
        {
           return That(code, expression, null, null);
        }

        /// <summary>
        /// Applies a constraint to an async delegate, succeeding if the constraint is satisfied and throwing an assertion exception on failure.
        /// </summary>
        /// <param name="code">The async delegate to execute.</param>
        /// <param name="expression">The constraint expression to apply to the delegate.</param>
        /// <param name="message">The message that will be displayed on failure.</param>
        /// <param name="args">Arguments to be used in formatting the message.</param>
#if NET40
        // Approximate TPL implementation since the types needed for the async keyword are not declared.
        public static Task That(AsyncTestDelegate code, IResolveConstraint expression, string message, params object[] args)
        {
            var constraint = ResolveAsyncConstraint(expression);

            AssertThatHelper.Start();
            return constraint.ApplyToAsync(code).ContinueWith(task =>
            {
                task.ThrowAwaitExceptionOnFailure(); // May be user code
                AssertThatHelper.End(task.Result, message, args);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
#else
        public static async Task That(AsyncTestDelegate code, IResolveConstraint expression, string message, params object[] args)
        {
            var constraint = ResolveAsyncConstraint(expression);

            AssertThatHelper.Start();
            var result = await constraint.ApplyToAsync(code).ConfigureAwait(true);
            AssertThatHelper.End(result, message, args);
        }
#endif

        /// <summary>
        /// Applies a constraint to an async delegate, succeeding if the constraint is satisfied and throwing an assertion exception on failure.
        /// </summary>
        /// <param name="code">The async delegate toexecute.</param>
        /// <param name="expression">The constraint expression to apply to the delegate.</param>
        /// <param name="getExceptionMessage">Builds the failure message if needed.</param>
#if NET40
        // Approximate TPL implementation since the types needed for the async keyword are not declared.
        public static Task That(AsyncTestDelegate code, IResolveConstraint expression, Func<string> getExceptionMessage)
        {
            var constraint = ResolveAsyncConstraint(expression);

            AssertThatHelper.Start();
            return constraint.ApplyToAsync(code).ContinueWith(task =>
            {
                task.ThrowAwaitExceptionOnFailure(); // May be user code
                AssertThatHelper.End(task.Result, getExceptionMessage);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
#else
        public static async Task That(AsyncTestDelegate code, IResolveConstraint expression, Func<string> getExceptionMessage)
        {
            var constraint = ResolveAsyncConstraint(expression);

            AssertThatHelper.Start();
            var result = await constraint.ApplyToAsync(code).ConfigureAwait(true);
            AssertThatHelper.End(result, getExceptionMessage);
        }
#endif

        /// <summary>
        /// Applies a constraint to an async delegate or its return value, succeeding if the constraint is satisfied and throwing an assertion exception on failure.
        /// </summary>
        /// <typeparam name="TActual">The type of the asynchronous return value.</typeparam>
        /// <param name="del">The async delegate execute.</param>
        /// <param name="expression">The constraint expression to apply to the delegate or its return value.</param>
        public static Task That<TActual>(ActualValueDelegate<Task<TActual>> del, IResolveConstraint expression)
        {
            return That(del, expression.Resolve(), null, null);
        }

        /// <summary>
        /// Applies a constraint to an async delegate or its return value, succeeding if the constraint is satisfied and throwing an assertion exception on failure.
        /// </summary>
        /// <typeparam name="TActual">The type of the asynchronous return value.</typeparam>
        /// <param name="del">The async delegate execute.</param>
        /// <param name="expression">The constraint expression to apply to the delegate or its return value.</param>
        /// <param name="message">The message that will be displayed on failure.</param>
        /// <param name="args">Arguments to be used in formatting the message.</param>
#if NET40
        // Approximate TPL implementation since the types needed for the async keyword are not declared.
        public static Task That<TActual>(ActualValueDelegate<Task<TActual>> del, IResolveConstraint expression, string message, params object[] args)
        {
            var constraint = ResolveAsyncConstraint(expression);
            
            AssertThatHelper.Start();
            return constraint.ApplyToAsync(del).ContinueWith(task =>
            {
                task.ThrowAwaitExceptionOnFailure(); // May be user code
                AssertThatHelper.End(task.Result, message, args);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
#else
        public static async Task That<TActual>(ActualValueDelegate<Task<TActual>> del, IResolveConstraint expression, string message, params object[] args)
        {
            var constraint = ResolveAsyncConstraint(expression);

            AssertThatHelper.Start();
            var result = await constraint.ApplyToAsync(del).ConfigureAwait(true);
            AssertThatHelper.End(result, message, args);
        }
#endif

        /// <summary>
        /// Applies a constraint to an async delegate or its return value, succeeding if the constraint is satisfied and throwing an assertion exception on failure.
        /// </summary>
        /// <typeparam name="TActual">The type of the asynchronous return value.</typeparam>
        /// <param name="del">The async delegate execute.</param>
        /// <param name="expression">The constraint expression to apply to the delegate or its return value.</param>
        /// <param name="getExceptionMessage">Builds the failure message if needed.</param>
#if NET40
        // Approximate TPL implementation since the types needed for the async keyword are not declared.
        public static Task That<TActual>(ActualValueDelegate<Task<TActual>> del, IResolveConstraint expression, Func<string> getExceptionMessage)
        {
            var constraint = ResolveAsyncConstraint(expression);

            AssertThatHelper.Start();
            return constraint.ApplyToAsync(del).ContinueWith(task =>
            {
                task.ThrowAwaitExceptionOnFailure(); // May be user code
                AssertThatHelper.End(task.Result, getExceptionMessage);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
#else
        public static async Task That<TActual>(ActualValueDelegate<Task<TActual>> del, IResolveConstraint expression, Func<string> getExceptionMessage)
        {
            var constraint = ResolveAsyncConstraint(expression);
            
            AssertThatHelper.Start();
            var result = await constraint.ApplyToAsync(del).ConfigureAwait(true);
            AssertThatHelper.End(result, getExceptionMessage);
        }
#endif

        private static IAsyncConstraint ResolveAsyncConstraint(IResolveConstraint expression)
        {
            var constraint = expression.Resolve() as IAsyncConstraint;
            Guard.ArgumentValid(constraint != null, $"The resolved constraint must implement {nameof(IAsyncConstraint)}.", nameof(expression));
            return constraint;
        }
    }
}
#endif
