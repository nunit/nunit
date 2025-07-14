// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using NUnit.Framework.Constraints;

// Disabled because of the CallerArgumentExpression attributes which are only for the compiler.
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)

namespace NUnit.Framework
{
    public abstract partial class Assert
    {
        /// <summary>Default retry interval for retrying an assertion.</summary>
        /// <remarks>Default is <c>TimeSpan.FromMilliseconds(300)</c>.</remarks>
        public static TimeSpan RetryDefaultInterval { get; set; } = TimeSpan.FromMilliseconds(300);

        /// <summary>Default timeout for retrying an assertion.</summary>
        /// <remarks>Default is <c>TimeSpan.FromMinutes(5)</c>.</remarks>
        public static TimeSpan RetryDefaultTimeout { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Allow to print the expected vs actual values when assert fails before the final retry. Default is <c>false</c>.
        /// </summary>
        public static bool PrintIntermediateAssertFailures { get; set; } = false;

        /// <summary>
        /// Retry the <paramref name="constraint" /> against the value provided by <paramref name="valueProvider" />
        /// for a duration of up to <see cref="RetryDefaultTimeout" /> with a <see cref="RetryDefaultInterval" /> retry interval.
        /// </summary>
        /// <param name="valueProvider">A TestDelegate to be executed</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        public static void Retry<TValue>(
            Func<TValue> valueProvider,
            IResolveConstraint constraint,
            [CallerArgumentExpression(nameof(valueProvider))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
        {
            Retry(valueProvider, constraint, RetryDefaultTimeout, RetryDefaultInterval, default(NUnitString), actualExpression, constraintExpression);
        }

        /// <summary>
        /// Retry the <paramref name="constraint" /> against the value provided by <paramref name="valueProvider" />
        /// for a duration of up to <paramref name="timeout" /> with a <see cref="RetryDefaultInterval" /> retry interval.
        /// </summary>
        /// <param name="valueProvider">A TestDelegate to be executed</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="timeout">The maximum time to wait for the assertion to succeed</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void Retry<TValue>(
           Func<TValue> valueProvider,
           IResolveConstraint constraint,
           TimeSpan timeout,
           NUnitString message = default,
           [CallerArgumentExpression(nameof(valueProvider))] string actualExpression = "",
           [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
        {
            Retry(valueProvider, constraint, timeout, RetryDefaultInterval, message, actualExpression, constraintExpression);
        }

        /// <summary>
        /// Retry the <paramref name="constraint" /> against the value provided by <paramref name="valueProvider" />
        /// for a duration of up to <paramref name="timeout" /> with a <paramref name="interval" /> retry interval.
        /// </summary>
        /// <param name="valueProvider">A TestDelegate to be executed</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="timeout">The maximum time to wait for the assertion to succeed</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void Retry<TValue>(
            Func<TValue> valueProvider,
            IResolveConstraint constraint,
            TimeSpan timeout,
            TimeSpan interval,
            NUnitString message,
            [CallerArgumentExpression(nameof(valueProvider))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
        {
            AssertRetryInternal(valueProvider, constraint, timeout, interval, message, actualExpression, constraintExpression);
        }

        private static void AssertRetryInternal<TValueActual>(
            Func<TValueActual> valueProvider,
            IResolveConstraint expression,
            TimeSpan timeout,
            TimeSpan interval,
            NUnitString message,
            string actualExpression,
            string constraintExpression)
        {
            if (timeout <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(timeout), $"Negative of zero timeout not allowed, current value: {timeout}");

            if (interval <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(interval), $"Negative of zero interval not allowed, current value: {interval}");

            IncrementAssertCount();

            var constraint = expression.Resolve();

            ConstraintResult result = null;
            TValueActual actual = default;

            Stopwatch timer = Stopwatch.StartNew();
            while (timer.Elapsed < timeout)
            {
                actual = valueProvider();
                result = constraint.ApplyTo(actual);
                if (result.IsSuccess)
                {
                    return;
                }

                if (PrintIntermediateAssertFailures)
                {
                    TestContext.Out.WriteLine(ComposeFailureMessage(result, message.ToString(), actualExpression, constraintExpression, nameof(Assert.Retry)));
                }

                Thread.Sleep(interval);
            }

            ReportFailure(result!, message.ToString(), actualExpression, constraintExpression, nameof(Assert.Retry));
        }
    }
}
