// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
#if NET6_0
using System.Runtime.CompilerServices;
#endif
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
#pragma warning disable CS1573

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
        public static void That<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr, string message = "",
            [CallerArgumentExpression("del")] string? actualExpression = null,
            [CallerArgumentExpression("expr")] string? constraintExpression = null)
        {
            CheckMultipleAssertLevel();

            var constraint = expr.Resolve();
            var result = constraint.ApplyTo(del);

            if (!result.IsSuccess)
                ReportFailure(result, message, actualExpression, constraintExpression);
        }

        private static void ReportFailure(ConstraintResult result, string? message, string? actualExpression, string? constraintExpression)
        {
            var msg = !string.IsNullOrEmpty(actualExpression) && !string.IsNullOrEmpty(constraintExpression)
                ? (!string.IsNullOrEmpty(message)
                    ? $"{message}\nAssume.That({actualExpression}, {constraintExpression})"
                    : $"Assume.That({actualExpression}, {constraintExpression})")
                : message;
            MessageWriter writer = new TextMessageWriter(msg);
            result.WriteMessageTo(writer);
            throw new InconclusiveException(writer.ToString());
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
            [CallerArgumentExpression("del")] string? actualExpression = null,
            [CallerArgumentExpression("expr")] string? constraintExpression = null)
        {
            CheckMultipleAssertLevel();

            var constraint = expr.Resolve();

            var result = constraint.ApplyTo(del);
            if (!result.IsSuccess)
            {
                ReportFailure(result, getExceptionMessage(), actualExpression, constraintExpression);
                throw new InconclusiveException(getExceptionMessage());
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
        public static void That(
            [DoesNotReturnIf(false)] bool condition,
            string message = "",
            [CallerArgumentExpression("condition")] string? actualExpression = null
            )
        {
            That(condition, Is.True, message, actualExpression, "Is.True");
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false, the method throws
        /// an <see cref="InconclusiveException"/>.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void That([DoesNotReturnIf(false)] bool condition, Func<string?> getExceptionMessage, 
            [CallerArgumentExpression("condition")] string? actualExpression = null
           )
        {
            That(condition, Is.True, getExceptionMessage,actualExpression,"Is.True");
        }

        #endregion

        #region Lambda returning Boolean

        /// <summary>
        /// Asserts that a condition is true. If the condition is false, the method throws
        /// an <see cref="InconclusiveException"/>.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        /// <param name="message">The message to display if the condition is false</param>
        public static void That(Func<bool> condition, string message = "",
            [CallerArgumentExpression("condition")] string? actualExpression = null)
        {
            That(condition.Invoke(), Is.True, message,actualExpression,"Is.True");
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false, the method throws
        /// an <see cref="InconclusiveException"/>.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void That(Func<bool> condition, Func<string?> getExceptionMessage,
            [CallerArgumentExpression("condition")] string? actualExpression = null)
        {
            That(condition.Invoke(), Is.True, getExceptionMessage,actualExpression,"Is.True");
        }

        #endregion

        #region TestDelegate

        /// <summary>
        /// Asserts that the code represented by a delegate throws an exception
        /// that satisfies the constraint provided.
        /// </summary>
        /// <param name="code">A TestDelegate to be executed</param>
        /// <param name="constraint">A ThrowsConstraint used in the test</param>
        public static void That(TestDelegate code, IResolveConstraint constraint, string message = "",
            [CallerArgumentExpression("code")] string? actualExpression = null,
            [CallerArgumentExpression("constraint")] string? constraintExpression = null)
        {
            That((object)code, constraint, message, actualExpression, constraintExpression);
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
        public static void That<TActual>(
            TActual actual,
            IResolveConstraint expression,
            string message = "",
            [CallerArgumentExpression("actual")] string? actualExpression = null,
            [CallerArgumentExpression("expression")] string? constraintExpression = null)
        {
            CheckMultipleAssertLevel();

            var constraint = expression.Resolve();

            var result = constraint.ApplyTo(actual);
            if (!result.IsSuccess)
            {
                ReportFailure(result, message, actualExpression, constraintExpression);
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
            Func<string?> getExceptionMessage, [CallerArgumentExpression("actual")] string? actualExpression = null,
            [CallerArgumentExpression("expression")] string? constraintExpression = null)
        {
            CheckMultipleAssertLevel();

            var constraint = expression.Resolve();

            var result = constraint.ApplyTo(actual);
            if (!result.IsSuccess)
            {
                ReportFailure(result,getExceptionMessage(),actualExpression,constraintExpression);
            }
        }

        #endregion

        #region Helper Methods

        private static void CheckMultipleAssertLevel()
        {
            if (TestExecutionContext.CurrentContext.MultipleAssertLevel > 0)
                throw new Exception("Assume.That may not be used in a multiple assertion block.");
        }

        #endregion
    }
}
