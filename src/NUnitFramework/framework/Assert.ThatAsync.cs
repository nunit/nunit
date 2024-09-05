// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
        /// <param name="message">The message that will be displayed on failure</param>
        /// <returns>Awaitable.</returns>
        public static async Task ThatAsync(AsyncTestDelegate code, IResolveConstraint constraint,
            NUnitString message = default,
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
            [CallerArgumentExpression(nameof(code))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
            var resolvedConstraint = constraint.Resolve();

            IncrementAssertCount();
            var result = await resolvedConstraint.ApplyToAsync(async () =>
            {
                await code();
                return (object?)null;
            });

            if (!result.IsSuccess)
                ReportFailure(result, message.ToString(), actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to an async delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="code">An AsyncTestDelegate to be executed</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <returns>Awaitable.</returns>
        public static async Task ThatAsync(AsyncTestDelegate code, IResolveConstraint constraint,
            FormattableString message,
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
            [CallerArgumentExpression(nameof(code))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
            var resolvedConstraint = constraint.Resolve();

            IncrementAssertCount();
            var result = await resolvedConstraint.ApplyToAsync(async () =>
            {
                await code();
                return (object?)null;
            });

            if (!result.IsSuccess)
                ReportFailure(result, message.ToString(), actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to an async delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="code">An async method to be executed</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <returns>Awaitable.</returns>
        public static async Task ThatAsync<T>(Func<Task<T>> code, IResolveConstraint constraint,
            NUnitString message = default,
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
            [CallerArgumentExpression(nameof(code))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
            var resolvedConstraint = constraint.Resolve();

            IncrementAssertCount();
            var result = await resolvedConstraint.ApplyToAsync(code);

            if (!result.IsSuccess)
                ReportFailure(result, message.ToString(), actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to an async delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="code">An async method to be executed</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <returns>Awaitable.</returns>
        public static async Task ThatAsync<T>(Func<Task<T>> code, IResolveConstraint constraint,
            FormattableString message,
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
            [CallerArgumentExpression(nameof(code))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
            var resolvedConstraint = constraint.Resolve();

            IncrementAssertCount();
            var result = await resolvedConstraint.ApplyToAsync(code);

            if (!result.IsSuccess)
                ReportFailure(result, message.ToString(), actualExpression, constraintExpression);
        }

        #endregion
    }
}
