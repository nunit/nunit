// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System;
using NUnit.Framework.Constraints;
using System.Runtime.CompilerServices;

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
        public static Task ThatAsync(AsyncTestDelegate code, IResolveConstraint constraint,
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
            [CallerArgumentExpression(nameof(code))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
            return ThatAsync(code, constraint, string.Empty, actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to an async delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="code">An AsyncTestDelegate to be executed</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <returns>Awaitable.</returns>
        public static async Task ThatAsync(AsyncTestDelegate code, IResolveConstraint constraint, NUnitString message,
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
            [CallerArgumentExpression(nameof(code))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
            try
            {
                await code();
                Assert.That(() => { }, constraint, message, actualExpression, constraintExpression);
            }
            catch (Exception ex)
            {
                var edi = ExceptionDispatchInfo.Capture(ex);
                Assert.That(() => edi.Throw(), constraint, message, actualExpression, constraintExpression);
            }
        }

        /// <summary>
        /// Apply a constraint to an async delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="code">An AsyncTestDelegate to be executed</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <returns>Awaitable.</returns>
        public static async Task ThatAsync(AsyncTestDelegate code, IResolveConstraint constraint, FormattableString message,
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
            [CallerArgumentExpression(nameof(code))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
            try
            {
                await code();
                Assert.That(() => { }, constraint, message, actualExpression, constraintExpression);
            }
            catch (Exception ex)
            {
                var edi = ExceptionDispatchInfo.Capture(ex);
                Assert.That(() => edi.Throw(), constraint, message, actualExpression, constraintExpression);
            }
        }

        /// <summary>
        /// Apply a constraint to an async delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="code">An async method to be executed</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <returns>Awaitable.</returns>
        public static Task ThatAsync<T>(Func<Task<T>> code, IResolveConstraint constraint,
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
            [CallerArgumentExpression(nameof(code))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
            return ThatAsync(code, constraint, string.Empty, actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to an async delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="code">An async method to be executed</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <returns>Awaitable.</returns>
        public static async Task ThatAsync<T>(Func<Task<T>> code, IResolveConstraint constraint, NUnitString message,
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
            [CallerArgumentExpression(nameof(code))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
            try
            {
                var result = await code();
                Assert.That(() => result, constraint, message, actualExpression, constraintExpression);
            }
            catch (Exception ex)
            {
                var edi = ExceptionDispatchInfo.Capture(ex);
                Assert.That(() => edi.Throw(), constraint, message, actualExpression, constraintExpression);
            }
        }

        /// <summary>
        /// Apply a constraint to an async delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="code">An async method to be executed</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <returns>Awaitable.</returns>
        public static async Task ThatAsync<T>(Func<Task<T>> code, IResolveConstraint constraint, FormattableString message,
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
            [CallerArgumentExpression(nameof(code))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
            try
            {
                var result = await code();
                Assert.That(() => result, constraint, message, actualExpression, constraintExpression);
            }
            catch (Exception ex)
            {
                var edi = ExceptionDispatchInfo.Capture(ex);
                Assert.That(() => edi.Throw(), constraint, message, actualExpression, constraintExpression);
            }
        }

        #endregion
    }
}
