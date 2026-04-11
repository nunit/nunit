// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Runtime.CompilerServices;
using NUnit.Framework.Constraints;

// Disabled because of the CallerArgumentExpression attributes which are only for the compiler.
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)

namespace NUnit.Framework
{
    /// <summary>
    /// Provides static methods to express conditions
    /// that must be met for the test to succeed. If
    /// any test fails, a warning is issued.
    /// </summary>
    public abstract partial class Warn
    {
        #region Warn.Unless

        #region Func<TActual>

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and issuing a warning on failure.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">A Func returning the value to be tested</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void Unless<TActual>(Func<TActual> del, IConstraint<TActual> constraint,
            NUnitString message = default,
            [CallerArgumentExpression(nameof(del))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
        {
            IncrementAssertCount();
            var result = constraint.ApplyTo(del);

            if (!result.IsSuccess)
                IssueWarning(result, nameof(Unless), message.ToString(), actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and issuing a warning on failure.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">A Func returning the value to be tested</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void Unless<TActual>(Func<TActual> del, IConstraint<TActual> constraint,
            FormattableString message,
            [CallerArgumentExpression(nameof(del))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
        {
            IncrementAssertCount();
            var result = constraint.ApplyTo(del);

            if (!result.IsSuccess)
                IssueWarning(result, nameof(Unless), message.ToString(), actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and issuing a warning on failure.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">A Func returning the value to be tested</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void Unless<TActual>(
            Func<TActual> del,
            IConstraint<TActual> constraint,
            Func<string> getExceptionMessage,
            [CallerArgumentExpression(nameof(del))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
        {
            IncrementAssertCount();
            var result = constraint.ApplyTo(del);

            if (!result.IsSuccess)
                IssueWarning(result, nameof(Unless), getExceptionMessage(), actualExpression, constraintExpression);
        }

        #endregion

        #region Generic

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and issuing a warning on failure.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void Unless<TActual>(TActual actual, IConstraint<TActual> constraint,
            NUnitString message = default,
            [CallerArgumentExpression(nameof(actual))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
        {
            Unless(actual, constraint, () => message.ToString(), actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and issuing a warning on failure.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void Unless<TActual>(TActual actual, IConstraint<TActual> constraint,
            FormattableString message,
            [CallerArgumentExpression(nameof(actual))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
        {
            Unless(actual, constraint, () => message.ToString(), actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and issuing a warning on failure.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void Unless<TActual>(
            TActual actual,
            IConstraint<TActual> constraint,
            Func<string> getExceptionMessage,
            [CallerArgumentExpression(nameof(actual))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
        {
            IncrementAssertCount();
            var result = constraint.ApplyTo(actual);

            if (!result.IsSuccess)
                IssueWarning(result, nameof(Unless), getExceptionMessage(), actualExpression, constraintExpression);
        }

        #endregion

        #endregion

        #region Warn.If

        #region Func<TActual>

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// fails and issuing a warning on success.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">A Func returning the value to be tested</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void If<TActual>(Func<TActual> del, IConstraint<TActual> constraint,
            NUnitString message = default,
            [CallerArgumentExpression(nameof(del))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
        {
            IncrementAssertCount();
            var result = constraint.ApplyTo(del);

            if (result.IsSuccess)
                IssueWarning(result, nameof(If), message.ToString(), actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// fails and issuing a warning on success.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">A Func returning the value to be tested</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void If<TActual>(Func<TActual> del, IConstraint<TActual> constraint,
            FormattableString message,
            [CallerArgumentExpression(nameof(del))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
        {
            IncrementAssertCount();
            var result = constraint.ApplyTo(del);

            if (result.IsSuccess)
                IssueWarning(result, nameof(If), message.ToString(), actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// fails and issuing a warning on failure.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">A Func returning the value to be tested</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void If<TActual>(
            Func<TActual> del,
            IConstraint<TActual> constraint,
            Func<string> getExceptionMessage,
            [CallerArgumentExpression(nameof(del))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
        {
            IncrementAssertCount();
            var result = constraint.ApplyTo(del);

            if (result.IsSuccess)
                IssueWarning(result, nameof(If), getExceptionMessage(), actualExpression, constraintExpression);
        }

        #endregion

        #region Generic

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// fails and issuing a warning if it succeeds.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void If<TActual>(TActual actual, IConstraint<TActual> constraint,
            NUnitString message = default,
            [CallerArgumentExpression(nameof(actual))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
        {
            If(actual, constraint, () => message.ToString(), actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// fails and issuing a warning if it succeeds.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void If<TActual>(TActual actual, IConstraint<TActual> constraint,
            FormattableString message,
            [CallerArgumentExpression(nameof(actual))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
        {
            If(actual, constraint, () => message.ToString(), actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and issuing a warning on failure.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void If<TActual>(
            TActual actual,
            IConstraint<TActual> constraint,
            Func<string> getExceptionMessage,
            [CallerArgumentExpression(nameof(actual))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
        {
            IncrementAssertCount();
            var result = constraint.ApplyTo(actual);

            if (result.IsSuccess)
                IssueWarning(result, nameof(If), getExceptionMessage(), actualExpression, constraintExpression);
        }

        #endregion

        #endregion
    }
}
