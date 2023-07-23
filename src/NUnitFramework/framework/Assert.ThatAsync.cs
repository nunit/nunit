// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System;
using NUnit.Framework.Constraints;
using System.Runtime.CompilerServices;
#pragma warning disable CS1573

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
        public static async Task ThatAsync(
            AsyncTestDelegate code,
            IResolveConstraint constraint,
            string message = "",
            [CallerArgumentExpression("code")] string? actualExpression = null,
            [CallerArgumentExpression("constraint")] string? constraintExpression = null)
        {
            try
            {
                await code();
                That(() => { }, constraint, message, actualExpression, constraintExpression);
            }
            catch (Exception ex)
            {
                var edi = ExceptionDispatchInfo.Capture(ex);
                That(() => edi.Throw(), constraint, message);
            }
        }

        /// <summary>
        /// Apply a constraint to an async delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="code">An async method to be executed</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <returns>Awaitable.</returns>
        public static async Task ThatAsync<T>(
            Func<Task<T>> code, 
            IResolveConstraint constraint, 
            string message = "",
            [CallerArgumentExpression("code")] string? actualExpression = null,
            [CallerArgumentExpression("constraint")] string? constraintExpression = null)
        {
            try
            {
                var result = await code();
                That(() => result, constraint, message, actualExpression, constraintExpression);
            }
            catch (Exception ex)
            {
                var edi = ExceptionDispatchInfo.Capture(ex);
                That(() => edi.Throw(), constraint, message);
            }
        }

        #endregion
    }
}
