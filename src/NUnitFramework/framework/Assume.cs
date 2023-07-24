// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Provides static methods to express the assumptions
    /// that must be met for a test to give a meaningful
    /// result. If an assumption is not met, the test
    /// should produce an inconclusive result.
    /// </summary>
    public abstract class Assume
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
            throw new InvalidOperationException("Assume.Equals should not be used. Use Assume.That instead.");
        }

        /// <summary>
        /// DO NOT USE!
        /// The ReferenceEquals method throws an InvalidOperationException. This is done
        /// to make sure there is no mistake by calling this function.
        /// </summary>
        /// <param name="a">The left object.</param>
        /// <param name="b">The right object.</param>
        public static new void ReferenceEquals(object a, object b)
        {
            throw new InvalidOperationException("Assume.ReferenceEquals should not be used. Use Assume.That instead.");
        }

        #endregion

        #region Assume.That

        #region ActualValueDelegate

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and throwing an InconclusiveException on failure.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
        /// <param name="expr">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void That<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr,
            NUnitString message = default,
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
            [CallerArgumentExpression(nameof(del))] string actualExpression = "",
            [CallerArgumentExpression(nameof(expr))] string constraintExpression = "")
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
            CheckMultipleAssertLevel();

            var constraint = expr.Resolve();
            var result = constraint.ApplyTo(del);

            if (!result.IsSuccess)
                ReportInconclusive(result, message.ToString(), actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and throwing an InconclusiveException on failure.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
        /// <param name="expr">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void That<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr,
            FormattableString message,
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
            [CallerArgumentExpression(nameof(del))] string actualExpression = "",
            [CallerArgumentExpression(nameof(expr))] string constraintExpression = "")
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
            CheckMultipleAssertLevel();

            var constraint = expr.Resolve();
            var result = constraint.ApplyTo(del);

            if (!result.IsSuccess)
                ReportInconclusive(result, message.ToString(), actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and throwing an InconclusiveException on failure.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
        /// <param name="expr">A Constraint expression to be applied</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void That<TActual>(
            ActualValueDelegate<TActual> del,
            IResolveConstraint expr,
            Func<string?> getExceptionMessage,
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
            [CallerArgumentExpression(nameof(del))] string actualExpression = "",
            [CallerArgumentExpression(nameof(expr))] string constraintExpression = "")
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
            CheckMultipleAssertLevel();

            var constraint = expr.Resolve();

            var result = constraint.ApplyTo(del);
            if (!result.IsSuccess)
            {
                ReportInconclusive(result, getExceptionMessage(), actualExpression, constraintExpression);
            }
        }

        #endregion

        #region Boolean

        /// <summary>
        /// Asserts that a condition is true. If the condition is false, the method throws
        /// an <see cref="InconclusiveException"/>.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display if the condition is false</param>
        public static void That([DoesNotReturnIf(false)] bool condition,
            NUnitString message = default,
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
            [CallerArgumentExpression(nameof(condition))] string actualExpression = "")
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
            That(condition, Is.True, message, actualExpression, Assert.IsTrueExpression);
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false, the method throws
        /// an <see cref="InconclusiveException"/>.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display if the condition is false</param>
        public static void That([DoesNotReturnIf(false)] bool condition,
            FormattableString message,
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
            [CallerArgumentExpression(nameof(condition))] string actualExpression = "")
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
            That(condition, Is.True, message, actualExpression, Assert.IsTrueExpression);
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false, the method throws
        /// an <see cref="InconclusiveException"/>.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void That([DoesNotReturnIf(false)] bool condition,
            Func<string?> getExceptionMessage,
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
            [CallerArgumentExpression(nameof(condition))] string actualExpression = "")
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
            That(condition, Is.True, getExceptionMessage, actualExpression, Assert.IsTrueExpression);
        }

        #endregion

        #region Lambda returning Boolean

        /// <summary>
        /// Asserts that a condition is true. If the condition is false, the method throws
        /// an <see cref="InconclusiveException"/>.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        /// <param name="message">The message to display if the condition is false</param>
        public static void That(Func<bool> condition,
            NUnitString message = default,
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
            [CallerArgumentExpression(nameof(condition))] string actualExpression = "")
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
            That(condition.Invoke(), Is.True, message, actualExpression, Assert.IsTrueExpression);
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false, the method throws
        /// an <see cref="InconclusiveException"/>.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        /// <param name="message">The message to display if the condition is false</param>
        public static void That(Func<bool> condition,
            FormattableString message,
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
            [CallerArgumentExpression(nameof(condition))] string actualExpression = "")
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
            That(condition.Invoke(), Is.True, message, actualExpression, Assert.IsTrueExpression);
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false, the method throws
        /// an <see cref="InconclusiveException"/>.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void That(Func<bool> condition,
            Func<string?> getExceptionMessage,
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
            [CallerArgumentExpression(nameof(condition))] string actualExpression = "")
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
            That(condition.Invoke(), Is.True, getExceptionMessage, actualExpression, Assert.IsTrueExpression);
        }

        #endregion

        #endregion

        #region Assume.That<TActual>

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and throwing an InconclusiveException on failure.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="expression">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void That<TActual>(TActual actual, IResolveConstraint expression,
            NUnitString message = default,
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
            [CallerArgumentExpression(nameof(actual))] string actualExpression = "",
            [CallerArgumentExpression(nameof(expression))] string constraintExpression = "")
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
            CheckMultipleAssertLevel();

            var constraint = expression.Resolve();

            var result = constraint.ApplyTo(actual);
            if (!result.IsSuccess)
            {
                ReportInconclusive(result, message.ToString(), actualExpression, constraintExpression);
            }
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and throwing an InconclusiveException on failure.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="expression">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void That<TActual>(TActual actual, IResolveConstraint expression,
            FormattableString message,
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
            [CallerArgumentExpression(nameof(actual))] string actualExpression = "",
            [CallerArgumentExpression(nameof(expression))] string constraintExpression = "")
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
            CheckMultipleAssertLevel();

            var constraint = expression.Resolve();

            var result = constraint.ApplyTo(actual);
            if (!result.IsSuccess)
            {
                ReportInconclusive(result, message.ToString(), actualExpression, constraintExpression);
            }
        }

        /// <summary>
        /// Apply a constraint to an actual value, succeeding if the constraint
        /// is satisfied and throwing an InconclusiveException on failure.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="expression">A Constraint expression to be applied</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void That<TActual>(
            TActual actual,
            IResolveConstraint expression,
            Func<string?> getExceptionMessage,
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
            [CallerArgumentExpression(nameof(actual))] string actualExpression = "",
            [CallerArgumentExpression(nameof(expression))] string constraintExpression = "")
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
            CheckMultipleAssertLevel();

            var constraint = expression.Resolve();

            var result = constraint.ApplyTo(actual);
            if (!result.IsSuccess)
            {
                ReportInconclusive(result, getExceptionMessage(), actualExpression, constraintExpression);
            }
        }

        #endregion

        #region Helper Methods

        private static void CheckMultipleAssertLevel()
        {
            if (TestExecutionContext.CurrentContext.MultipleAssertLevel > 0)
                throw new Exception("Assume.That may not be used in a multiple assertion block.");
        }

        private static void ReportInconclusive(ConstraintResult result, string? message, string actualExpression, string constraintExpression)
        {
            MessageWriter writer = new TextMessageWriter(Assert.ExtendedMessage(message, actualExpression, constraintExpression));
            result.WriteMessageTo(writer);
            throw new InconclusiveException(writer.ToString());
        }

        #endregion
    }
}
