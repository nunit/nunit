// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;
using System.ComponentModel;
using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
    /// <summary>
    /// Provides static methods to express conditions
    /// that must be met for the test to succeed. If
    /// any test fails, a warning is issued.
    /// </summary>
    public class Warn
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
            Warn.Unless(del, expr.Resolve(), null, null);
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and issuing a warning on failure.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
        /// <param name="expr">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void Unless<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr, string? message, params object?[]? args)
        {
            var constraint = expr.Resolve();

            IncrementAssertCount();
            var result = constraint.ApplyTo(del);

            if (!result.IsSuccess)
                IssueWarning(result, message, args);
        }

        private static void IssueWarning(ConstraintResult result, string? message, object?[]? args)
        {
            MessageWriter writer = new TextMessageWriter(message, args);
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
                IssueWarning(result, getExceptionMessage(), null);
        }

        #endregion

        #region Boolean

        /// <summary>
        /// Asserts that a condition is true. If the condition is false, a warning is issued.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display if the condition is false</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void Unless(bool condition, string? message, params object?[]? args)
        {
            Warn.Unless(condition, Is.True, message, args);
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false, a warning is issued.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        public static void Unless(bool condition)
        {
            Warn.Unless(condition, Is.True, null, null);
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false, a warning is issued.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void Unless(bool condition, Func<string?> getExceptionMessage)
        {
            Warn.Unless(condition, Is.True, getExceptionMessage);
        }

        #endregion

        #region Lambda returning Boolean

        /// <summary>
        /// Asserts that a condition is true. If the condition is false, a warning is issued.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        /// <param name="message">The message to display if the condition is false</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void Unless(Func<bool> condition, string? message, params object?[]? args)
        {
            Warn.Unless(condition.Invoke(), Is.True, message, args);
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false, a warning is issued.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        public static void Unless(Func<bool> condition)
        {
            Warn.Unless(condition.Invoke(), Is.True, null, null);
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false, a warning is issued.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void Unless(Func<bool> condition, Func<string?> getExceptionMessage)
        {
            Warn.Unless(condition.Invoke(), Is.True, getExceptionMessage);
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
            Warn.Unless((object)code, constraint);
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
            Warn.Unless(actual, expression, null, null);
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and issuing a warning on failure.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="expression">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void Unless<TActual>(TActual actual, IResolveConstraint expression, string? message, params object?[]? args)
        {
            var constraint = expression.Resolve();

            IncrementAssertCount();
            var result = constraint.ApplyTo(actual);

            if (!result.IsSuccess)
                IssueWarning(result, message, args);
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
                IssueWarning(result, getExceptionMessage(), null);
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
            Warn.If(del, expr.Resolve(), null, null);
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// fails and issuing a warning on success.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
        /// <param name="expr">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void If<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr, string? message, params object?[]? args)
        {
            var constraint = new NotConstraint(expr.Resolve());

            IncrementAssertCount();
            var result = constraint.ApplyTo(del);

            if (!result.IsSuccess)
                IssueWarning(result, message, args);
        }

        //private static void IssueWarning(ConstraintResult result, string? message, object?[]? args)
        //{
        //    MessageWriter writer = new TextMessageWriter(message, args);
        //    result.WriteMessageTo(writer);
        //    Assert.Warn(writer.ToString());
        //}

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
                IssueWarning(result, getExceptionMessage(), null);
        }

        #endregion

        #region Boolean

        /// <summary>
        /// Asserts that a condition is false. If the condition is true, a warning is issued.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display if the condition is false</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void If(bool condition, string? message, params object?[]? args)
        {
            Warn.If(condition, Is.True, message, args);
        }

        /// <summary>
        /// Asserts that a condition is false. If the condition is true, a warning is issued.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        public static void If(bool condition)
        {
            Warn.If(condition, Is.True, null, null);
        }

        /// <summary>
        /// Asserts that a condition is false. If the condition is true, a warning is issued.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void If(bool condition, Func<string?> getExceptionMessage)
        {
            Warn.If(condition, Is.True, getExceptionMessage);
        }

        #endregion

        #region Lambda returning Boolean

        /// <summary>
        /// Asserts that a condition is false. If the condition is true a warning is issued.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        /// <param name="message">The message to display if the condition is true</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void If(Func<bool> condition, string? message, params object?[]? args)
        {
            Warn.If(condition.Invoke(), Is.True, message, args);
        }

        /// <summary>
        /// Asserts that a condition is false. If the condition is true a warning is issued.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        public static void If(Func<bool> condition)
        {
            Warn.If(condition.Invoke(), Is.True, null, null);
        }

        /// <summary>
        /// Asserts that a condition is false. If the condition is true a warning is issued.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void If(Func<bool> condition, Func<string?> getExceptionMessage)
        {
            Warn.If(condition.Invoke(), Is.True, getExceptionMessage);
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
            Warn.If(actual, expression, null, null);
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// fails and issuing a warning if it succeeds.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="expression">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void If<TActual>(TActual actual, IResolveConstraint expression, string? message, params object?[]? args)
        {
            var constraint = new NotConstraint(expression.Resolve());

            IncrementAssertCount();
            var result = constraint.ApplyTo(actual);

            if (!result.IsSuccess)
                IssueWarning(result, message, args);
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
                IssueWarning(result, getExceptionMessage(), null);
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
