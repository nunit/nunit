// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.ComponentModel;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

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
        public static void Unless<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr)
        {
            Unless(del, expr.Resolve(), string.Empty);
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and issuing a warning on failure.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
        /// <param name="expr">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void Unless<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr, NUnitString message)
        {
            var constraint = expr.Resolve();

            IncrementAssertCount();
            var result = constraint.ApplyTo(del);

            if (!result.IsSuccess)
                IssueWarning(result, message.ToString());
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and issuing a warning on failure.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
        /// <param name="expr">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void Unless<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr, FormattableString message)
        {
            var constraint = expr.Resolve();

            IncrementAssertCount();
            var result = constraint.ApplyTo(del);

            if (!result.IsSuccess)
                IssueWarning(result, message.ToString());
        }

        private static void IssueWarning(ConstraintResult result, string? message)
        {
            MessageWriter writer = new TextMessageWriter(message);
            result.WriteMessageTo(writer);
            Assert.Warn(writer.ToString());
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
            Func<string?> getExceptionMessage)
        {
            var constraint = expr.Resolve();

            IncrementAssertCount();
            var result = constraint.ApplyTo(del);

            if (!result.IsSuccess)
                IssueWarning(result, getExceptionMessage());
        }

        #endregion

        #region Boolean

        /// <summary>
        /// Asserts that a condition is true. If the condition is false, a warning is issued.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display if the condition is false</param>
        public static void Unless(bool condition, NUnitString message)
        {
            Unless(condition, Is.True, () => message.ToString());
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false, a warning is issued.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display if the condition is false</param>
        public static void Unless(bool condition, FormattableString message)
        {
            Unless(condition, Is.True, () => message.ToString());
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false, a warning is issued.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        public static void Unless(bool condition)
        {
            Unless(condition, Is.True, () => string.Empty);
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false, a warning is issued.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void Unless(bool condition, Func<string?> getExceptionMessage)
        {
            Unless(condition, Is.True, getExceptionMessage);
        }

        #endregion

        #region Lambda returning Boolean

        /// <summary>
        /// Asserts that a condition is true. If the condition is false, a warning is issued.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        /// <param name="message">The message to display if the condition is false</param>
        public static void Unless(Func<bool> condition, NUnitString message)
        {
            Unless(condition.Invoke(), Is.True, message);
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false, a warning is issued.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        /// <param name="message">The message to display if the condition is false</param>
        public static void Unless(Func<bool> condition, FormattableString message)
        {
            Unless(condition.Invoke(), Is.True, message);
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false, a warning is issued.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        public static void Unless(Func<bool> condition)
        {
            Unless(condition.Invoke(), Is.True, string.Empty);
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false, a warning is issued.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void Unless(Func<bool> condition, Func<string?> getExceptionMessage)
        {
            Unless(condition.Invoke(), Is.True, getExceptionMessage);
        }

        #endregion

        #region TestDelegate

        /// <summary>
        /// Asserts that the code represented by a delegate throws an exception
        /// that satisfies the constraint provided.
        /// </summary>
        /// <param name="code">A TestDelegate to be executed</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        public static void Unless(TestDelegate code, IResolveConstraint constraint)
        {
            Unless((object)code, constraint);
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
        public static void Unless<TActual>(TActual actual, IResolveConstraint expression)
        {
            Unless(actual, expression, string.Empty);
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and issuing a warning on failure.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="expression">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void Unless<TActual>(TActual actual, IResolveConstraint expression, NUnitString message)
        {
            Unless(actual, expression, () => message.ToString());
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and issuing a warning on failure.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="expression">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void Unless<TActual>(TActual actual, IResolveConstraint expression, FormattableString message)
        {
            Unless(actual, expression, () => message.ToString());
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
            Func<string?> getExceptionMessage)
        {
            var constraint = expression.Resolve();

            IncrementAssertCount();
            var result = constraint.ApplyTo(actual);

            if (!result.IsSuccess)
                IssueWarning(result, getExceptionMessage());
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
        public static void If<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr)
        {
            If(del, expr.Resolve(), string.Empty);
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// fails and issuing a warning on success.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
        /// <param name="expr">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void If<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr, NUnitString message)
        {
            var constraint = new NotConstraint(expr.Resolve());

            IncrementAssertCount();
            var result = constraint.ApplyTo(del);

            if (!result.IsSuccess)
                IssueWarning(result, message.ToString());
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// fails and issuing a warning on success.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
        /// <param name="expr">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void If<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr, FormattableString message)
        {
            var constraint = new NotConstraint(expr.Resolve());

            IncrementAssertCount();
            var result = constraint.ApplyTo(del);

            if (!result.IsSuccess)
                IssueWarning(result, message.ToString());
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
            Func<string?> getExceptionMessage)
        {
            var constraint = new NotConstraint(expr.Resolve());

            IncrementAssertCount();
            var result = constraint.ApplyTo(del);

            if (!result.IsSuccess)
                IssueWarning(result, getExceptionMessage());
        }

        #endregion

        #region Boolean

        /// <summary>
        /// Asserts that a condition is false. If the condition is true, a warning is issued.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display if the condition is false</param>
        public static void If(bool condition, NUnitString message)
        {
            If(condition, Is.True, () => message.ToString());
        }

        /// <summary>
        /// Asserts that a condition is false. If the condition is true, a warning is issued.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display if the condition is false</param>
        public static void If(bool condition, FormattableString message)
        {
            If(condition, Is.True, () => message.ToString());
        }

        /// <summary>
        /// Asserts that a condition is false. If the condition is true, a warning is issued.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        public static void If(bool condition)
        {
            If(condition, Is.True, () => string.Empty);
        }

        /// <summary>
        /// Asserts that a condition is false. If the condition is true, a warning is issued.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void If(bool condition, Func<string?> getExceptionMessage)
        {
            If(condition, Is.True, getExceptionMessage);
        }

        #endregion

        #region Lambda returning Boolean

        /// <summary>
        /// Asserts that a condition is false. If the condition is true a warning is issued.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        /// <param name="message">The message to display if the condition is true</param>
        public static void If(Func<bool> condition, NUnitString message)
        {
            If(condition.Invoke(), Is.True, () => message.ToString());
        }

        /// <summary>
        /// Asserts that a condition is false. If the condition is true a warning is issued.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        /// <param name="message">The message to display if the condition is true</param>
        public static void If(Func<bool> condition, FormattableString message)
        {
            If(condition.Invoke(), Is.True, () => message.ToString());
        }

        /// <summary>
        /// Asserts that a condition is false. If the condition is true a warning is issued.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        public static void If(Func<bool> condition)
        {
            If(condition.Invoke(), Is.True, () => string.Empty);
        }

        /// <summary>
        /// Asserts that a condition is false. If the condition is true a warning is issued.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void If(Func<bool> condition, Func<string?> getExceptionMessage)
        {
            If(condition.Invoke(), Is.True, getExceptionMessage);
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
        public static void If<TActual>(TActual actual, IResolveConstraint expression)
        {
            If(actual, expression, string.Empty);
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// fails and issuing a warning if it succeeds.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="expression">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void If<TActual>(TActual actual, IResolveConstraint expression, NUnitString message)
        {
            If(actual, expression, () => message.ToString());
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// fails and issuing a warning if it succeeds.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="expression">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void If<TActual>(TActual actual, IResolveConstraint expression, FormattableString message)
        {
            If(actual, expression, () => message.ToString());
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
            Func<string?> getExceptionMessage)
        {
            var constraint = new NotConstraint(expression.Resolve());

            IncrementAssertCount();
            var result = constraint.ApplyTo(actual);

            if (!result.IsSuccess)
                IssueWarning(result, getExceptionMessage());
        }

        #endregion

        #endregion

        #region Helper Methods

        private static void IncrementAssertCount()
        {
            TestExecutionContext.CurrentContext.IncrementAssertCount();
        }

        #endregion
    }
}
