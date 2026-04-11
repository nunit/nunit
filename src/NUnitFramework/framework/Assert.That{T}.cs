// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Runtime.CompilerServices;
using NUnit.Framework.Constraints;

// Disabled because of the CallerArgumentExpression attributes which are only for the compiler.
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)

namespace NUnit.Framework
{
    /// <summary>
    /// The Assert class contains a collection of static methods that
    /// implement the most common assertions used in NUnit.
    /// </summary>
    public abstract partial class Assert
    {
        #region Func<TActual>

        /// <summary>
        /// Apply a constraint to a delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">A Func returning the value to be tested</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void That<TActual>(Func<TActual> del, IConstraint<TActual> constraint,
            NUnitString message = default,
            [CallerArgumentExpression(nameof(del))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
        {
            IncrementAssertCount();
            var result = constraint.ApplyTo(del);
            if (!result.IsSuccess)
                ReportFailure(result, message.ToString(), actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to a delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">A Func returning the value to be tested</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void That<TActual>(Func<TActual> del, IConstraint<TActual> constraint,
            FormattableString message,
            [CallerArgumentExpression(nameof(del))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
        {
            IncrementAssertCount();
            var result = constraint.ApplyTo(del);
            if (!result.IsSuccess)
                ReportFailure(result, message.ToString(), actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to a delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">A Func returning the value to be tested</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void That<TActual>(
            Func<TActual> del,
            IConstraint<TActual> constraint,
            Func<string> getExceptionMessage,
            [CallerArgumentExpression(nameof(del))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
        {
            IncrementAssertCount();
            var result = constraint.ApplyTo(del);
            if (!result.IsSuccess)
                ReportFailure(result, getExceptionMessage(), actualExpression, constraintExpression);
        }

        #endregion

        #region Assert.That<TActual>

        /// <summary>
        /// Apply a constraint to an actual value. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void That<TActual>(TActual actual, IConstraint<TActual> constraint,
            NUnitString message = default,
            [CallerArgumentExpression(nameof(actual))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
        {
            IncrementAssertCount();
            var result = constraint.ApplyTo(actual);
            if (!result.IsSuccess)
                ReportFailure(result, message.ToString(), actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to an actual value. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void That<TActual>(TActual actual, IConstraint<TActual> constraint,
            FormattableString message,
            [CallerArgumentExpression(nameof(actual))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
        {
            IncrementAssertCount();
            var result = constraint.ApplyTo(actual);
            if (!result.IsSuccess)
                ReportFailure(result, message.ToString(), actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to an actual value. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void That<TActual>(
            TActual actual,
            IConstraint<TActual> constraint,
            Func<string> getExceptionMessage,
            [CallerArgumentExpression(nameof(actual))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
        {
            IncrementAssertCount();
            var result = constraint.ApplyTo(actual);
            if (!result.IsSuccess)
                ReportFailure(result, getExceptionMessage(), actualExpression, constraintExpression);
        }

        #endregion
    }
}
