// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System;
using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
    public abstract partial class Assert
    {
        #region Assert.ThatAsync

        /// <summary>
        /// Apply a constraint to an async delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="code">An AsyncTestDelegate to be executed</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <returns>Awaitable.</returns>
        public static Task ThatAsync(AsyncTestDelegate code, IResolveConstraint constraint)
        {
            return ThatAsync(code, constraint, null, null);
        }

        /// <summary>
        /// Apply a constraint to an async delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="code">An AsyncTestDelegate to be executed</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        /// <returns>Awaitable.</returns>
        public static async Task ThatAsync(AsyncTestDelegate code, IResolveConstraint constraint, string? message, params object?[]? args)
        {
            try
            {
                await code();
                Assert.That(() => { }, constraint, message, args);
            }
            catch (Exception ex)
            {
                var edi = ExceptionDispatchInfo.Capture(ex);
                Assert.That(() => edi.Throw(), constraint, message, args);
            }
        }

        /// <summary>
        /// Apply a constraint to an async delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="code">An async method to be executed</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <returns>Awaitable.</returns>
        public static Task ThatAsync<T>(Func<Task<T>> code, IResolveConstraint constraint)
        {
            return ThatAsync(code, constraint, null, null);
        }

        /// <summary>
        /// Apply a constraint to an async delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="code">An async method to be executed</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        /// <returns>Awaitable.</returns>
        public static async Task ThatAsync<T>(Func<Task<T>> code, IResolveConstraint constraint, string? message, params object?[]? args)
        {
            try
            {
                var result = await code();
                Assert.That(() => result, constraint, message, args);
            }
            catch (Exception ex)
            {
                var edi = ExceptionDispatchInfo.Capture(ex);
                Assert.That(() => edi.Throw(), constraint, message, args);
            }
        }

        #endregion
    }
}
