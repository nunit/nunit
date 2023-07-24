// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

// Disabled because of the CallerArgumentExpression attributes which are only for the compiler.
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)

namespace NUnit.Framework
{
    /// <summary>
    /// Provides static methods to express conditions
    /// that must be met for the test to succeed. If
    /// any test fails, a warning is issued.
    /// </summary>
    public abstract class Warn
    {
        #region Equals and ReferenceEquals

        /// <summary>
        /// DO NOT USE!
        /// The Equals method throws an InvalidOperationException. This is done
        /// to make sure there is no mistake by calling this function.
        /// </summary>
        /// <param name="a">The left object.</param>
        /// <param name="b">The right object.</param>
        /// <returns>Not applicable</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static new bool Equals(object a, object b)
        {
            throw new InvalidOperationException("Warn.Equals should not be used.");
        }

        /// <summary>
        /// DO NOT USE!
        /// The ReferenceEquals method throws an InvalidOperationException. This is done
        /// to make sure there is no mistake by calling this function.
        /// </summary>
        /// <param name="a">The left object.</param>
        /// <param name="b">The right object.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static new void ReferenceEquals(object a, object b)
        {
            throw new InvalidOperationException("Warn.ReferenceEquals should not be used.");
        }

        #endregion

        #region Warn.Unless

        #region ActualValueDelegate

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and issuing a warning on failure.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
        /// <param name="expr">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void Unless<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr,
            NUnitString message = default,
            [CallerArgumentExpression(nameof(del))] string actualExpression = "",
            [CallerArgumentExpression(nameof(expr))] string constraintExpression = "")
        {
            var constraint = expr.Resolve();

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
        /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
        /// <param name="expr">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void Unless<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr,
            FormattableString message,
            [CallerArgumentExpression(nameof(del))] string actualExpression = "",
            [CallerArgumentExpression(nameof(expr))] string constraintExpression = "")
        {
            var constraint = expr.Resolve();

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
        /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
        /// <param name="expr">A Constraint expression to be applied</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void Unless<TActual>(
            ActualValueDelegate<TActual> del,
            IResolveConstraint expr,
            Func<string?> getExceptionMessage,
            [CallerArgumentExpression(nameof(del))] string actualExpression = "",
            [CallerArgumentExpression(nameof(expr))] string constraintExpression = "")
        {
            var constraint = expr.Resolve();

            IncrementAssertCount();
            var result = constraint.ApplyTo(del);

            if (!result.IsSuccess)
                IssueWarning(result, nameof(Unless), getExceptionMessage(), actualExpression, constraintExpression);
        }

        #endregion

        #region Boolean

        /// <summary>
        /// Asserts that a condition is true. If the condition is false, a warning is issued.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display if the condition is false</param>
        public static void Unless(bool condition,
            NUnitString message = default,
            [CallerArgumentExpression(nameof(condition))] string actualExpression = "")
        {
            Unless(condition, Is.True, () => message.ToString(), actualExpression, Assert.IsTrueExpression);
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false, a warning is issued.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display if the condition is false</param>
        public static void Unless(bool condition,
            FormattableString message,
            [CallerArgumentExpression(nameof(condition))] string actualExpression = "")
        {
            Unless(condition, Is.True, () => message.ToString(), actualExpression, Assert.IsTrueExpression);
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false, a warning is issued.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void Unless(bool condition,
            Func<string?> getExceptionMessage,
            [CallerArgumentExpression(nameof(condition))] string actualExpression = "")
        {
            Unless(condition, Is.True, getExceptionMessage, actualExpression, Assert.IsTrueExpression);
        }

        #endregion

        #region Lambda returning Boolean

        /// <summary>
        /// Asserts that a condition is true. If the condition is false, a warning is issued.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        /// <param name="message">The message to display if the condition is false</param>
        public static void Unless(Func<bool> condition,
            NUnitString message = default,
            [CallerArgumentExpression(nameof(condition))] string actualExpression = "")
        {
            Unless(condition.Invoke(), Is.True, message, actualExpression, Assert.IsTrueExpression);
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false, a warning is issued.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        /// <param name="message">The message to display if the condition is false</param>
        public static void Unless(Func<bool> condition,
            FormattableString message,
            [CallerArgumentExpression(nameof(condition))] string actualExpression = "")
        {
            Unless(condition.Invoke(), Is.True, message, actualExpression, Assert.IsTrueExpression);
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false, a warning is issued.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void Unless(Func<bool> condition,
            Func<string?> getExceptionMessage,
            [CallerArgumentExpression(nameof(condition))] string actualExpression = "")
        {
            Unless(condition.Invoke(), Is.True, getExceptionMessage, actualExpression, Assert.IsTrueExpression);
        }

        #endregion

        #region Generic

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and issuing a warning on failure.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="expression">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void Unless<TActual>(TActual actual, IResolveConstraint expression,
            NUnitString message = default,
            [CallerArgumentExpression(nameof(actual))] string actualExpression = "",
            [CallerArgumentExpression(nameof(expression))] string constraintExpression = "")
        {
            Unless(actual, expression, () => message.ToString(), actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and issuing a warning on failure.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="expression">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void Unless<TActual>(TActual actual, IResolveConstraint expression,
            FormattableString message,
            [CallerArgumentExpression(nameof(actual))] string actualExpression = "",
            [CallerArgumentExpression(nameof(expression))] string constraintExpression = "")
        {
            Unless(actual, expression, () => message.ToString(), actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and issuing a warning on failure.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="expression">A Constraint expression to be applied</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void Unless<TActual>(
            TActual actual,
            IResolveConstraint expression,
            Func<string?> getExceptionMessage,
            [CallerArgumentExpression(nameof(actual))] string actualExpression = "",
            [CallerArgumentExpression(nameof(expression))] string constraintExpression = "")
        {
            var constraint = expression.Resolve();

            IncrementAssertCount();
            var result = constraint.ApplyTo(actual);

            if (!result.IsSuccess)
                IssueWarning(result, nameof(Unless), getExceptionMessage(), actualExpression, constraintExpression);
        }

        #endregion

        #endregion

        #region Warn.If

        #region ActualValueDelegate

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// fails and issuing a warning on success.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
        /// <param name="expr">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void If<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr,
            NUnitString message = default,
            [CallerArgumentExpression(nameof(del))] string actualExpression = "",
            [CallerArgumentExpression(nameof(expr))] string constraintExpression = "")
        {
            var constraint = new NotConstraint(expr.Resolve());

            IncrementAssertCount();
            var result = constraint.ApplyTo(del);

            if (!result.IsSuccess)
                IssueWarning(result, nameof(If), message.ToString(), actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// fails and issuing a warning on success.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
        /// <param name="expr">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void If<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr,
            FormattableString message,
            [CallerArgumentExpression(nameof(del))] string actualExpression = "",
            [CallerArgumentExpression(nameof(expr))] string constraintExpression = "")
        {
            var constraint = new NotConstraint(expr.Resolve());

            IncrementAssertCount();
            var result = constraint.ApplyTo(del);

            if (!result.IsSuccess)
                IssueWarning(result, nameof(If), message.ToString(), actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// fails and issuing a warning on failure.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
        /// <param name="expr">A Constraint expression to be applied</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void If<TActual>(
            ActualValueDelegate<TActual> del,
            IResolveConstraint expr,
            Func<string?> getExceptionMessage,
            [CallerArgumentExpression(nameof(del))] string actualExpression = "",
            [CallerArgumentExpression(nameof(expr))] string constraintExpression = "")
        {
            var constraint = new NotConstraint(expr.Resolve());

            IncrementAssertCount();
            var result = constraint.ApplyTo(del);

            if (!result.IsSuccess)
                IssueWarning(result, nameof(If), getExceptionMessage(), actualExpression, constraintExpression);
        }

        #endregion

        #region Boolean

        /// <summary>
        /// Asserts that a condition is false. If the condition is true, a warning is issued.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display if the condition is false</param>
        public static void If(bool condition,
            NUnitString message = default,
            [CallerArgumentExpression(nameof(condition))] string actualExpression = "")
        {
            If(condition, Is.True, () => message.ToString(), actualExpression, Assert.IsTrueExpression);
        }

        /// <summary>
        /// Asserts that a condition is false. If the condition is true, a warning is issued.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display if the condition is false</param>
        public static void If(bool condition,
            FormattableString message,
            [CallerArgumentExpression(nameof(condition))] string actualExpression = "")
        {
            If(condition, Is.True, () => message.ToString(), actualExpression, Assert.IsTrueExpression);
        }

        /// <summary>
        /// Asserts that a condition is false. If the condition is true, a warning is issued.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void If(bool condition,
            Func<string?> getExceptionMessage,
            [CallerArgumentExpression(nameof(condition))] string actualExpression = "")
        {
            If(condition, Is.True, getExceptionMessage, actualExpression, Assert.IsTrueExpression);
        }

        #endregion

        #region Lambda returning Boolean

        /// <summary>
        /// Asserts that a condition is false. If the condition is true a warning is issued.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        /// <param name="message">The message to display if the condition is true</param>
        public static void If(Func<bool> condition,
            NUnitString message = default,
            [CallerArgumentExpression(nameof(condition))] string actualExpression = "")
        {
            If(condition.Invoke(), Is.True, () => message.ToString(), actualExpression, Assert.IsTrueExpression);
        }

        /// <summary>
        /// Asserts that a condition is false. If the condition is true a warning is issued.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        /// <param name="message">The message to display if the condition is true</param>
        public static void If(Func<bool> condition,
            FormattableString message,
            [CallerArgumentExpression(nameof(condition))] string actualExpression = "")
        {
            If(condition.Invoke(), Is.True, () => message.ToString(), actualExpression, Assert.IsTrueExpression);
        }

        /// <summary>
        /// Asserts that a condition is false. If the condition is true a warning is issued.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void If(Func<bool> condition,
            Func<string?> getExceptionMessage,
            [CallerArgumentExpression(nameof(condition))] string actualExpression = "")
        {
            If(condition.Invoke(), Is.True, getExceptionMessage, actualExpression, Assert.IsTrueExpression);
        }

        #endregion

        #region Generic

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// fails and issuing a warning if it succeeds.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="expression">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void If<TActual>(TActual actual, IResolveConstraint expression,
            NUnitString message = default,
            [CallerArgumentExpression(nameof(actual))] string actualExpression = "",
            [CallerArgumentExpression(nameof(expression))] string constraintExpression = "")
        {
            If(actual, expression, () => message.ToString(), actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// fails and issuing a warning if it succeeds.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="expression">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void If<TActual>(TActual actual, IResolveConstraint expression,
            FormattableString message,
            [CallerArgumentExpression(nameof(actual))] string actualExpression = "",
            [CallerArgumentExpression(nameof(expression))] string constraintExpression = "")
        {
            If(actual, expression, () => message.ToString(), actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and issuing a warning on failure.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="expression">A Constraint expression to be applied</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void If<TActual>(
            TActual actual,
            IResolveConstraint expression,
            Func<string?> getExceptionMessage,
            [CallerArgumentExpression(nameof(actual))] string actualExpression = "",
            [CallerArgumentExpression(nameof(expression))] string constraintExpression = "")
        {
            var constraint = new NotConstraint(expression.Resolve());

            IncrementAssertCount();
            var result = constraint.ApplyTo(actual);

            if (!result.IsSuccess)
                IssueWarning(result, nameof(If), getExceptionMessage(), actualExpression, constraintExpression);
        }

        #endregion

        #endregion

        #region Helper Methods

        private static void IncrementAssertCount()
        {
            TestExecutionContext.CurrentContext.IncrementAssertCount();
        }

        private static void IssueWarning(ConstraintResult result, string method, string? message, string actualExpression, string constraintExpression)
        {
            MessageWriter writer = new TextMessageWriter(
                Assert.ExtendedMessage($"{nameof(Warn)}.{method}", message, actualExpression, constraintExpression));
            result.WriteMessageTo(writer);
            Assert.Warn(writer.ToString());
        }

        #endregion
    }
}
