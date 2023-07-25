// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Runtime.CompilerServices;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

// Disabled because of the CallerArgumentExpression attributes which are only for the compiler.
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)

#if NET6_0_OR_GREATER
// Disabled because of InterpolatedStringHandlerArgument attributes.
#pragma warning disable IDE0060 // Remove unused parameter
#endif

namespace NUnit.Framework
{
    /// <summary>
    /// The Assert class contains a collection of static methods that
    /// implement the most common assertions used in NUnit.
    /// </summary>
    public abstract partial class Assert
    {
        #region Assert.That

        internal const string IsTrueExpression = "Is.True";

        #region Boolean

#if NET6_0_OR_GREATER
        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display if the condition is false</param>
        /// <param name="actualExpression"></param>
        public static void That(bool condition,
            string? message = default,
            [CallerArgumentExpression(nameof(condition))] string actualExpression = "")
        {
            That(condition, Is.True, message, actualExpression, IsTrueExpression);
        }

        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="assertingHandler">The message to display if the condition is false</param>
        public static void That(bool condition,
            [InterpolatedStringHandlerArgument(nameof(condition))] AssertingInterpolatedStringHandler<bool> assertingHandler,
            [CallerArgumentExpression(nameof(condition))] string actualExpression = "")
        {
            IncrementAssertCount();
            var result = assertingHandler.Result;
            if (!result.IsSuccess)
                ReportFailure(result, assertingHandler.ToString(), actualExpression, IsTrueExpression);
        }
#else
        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display if the condition is false</param>
        /// <param name="actualExpression"></param>
        public static void That(bool condition,
            NUnitString message = default,
            [CallerArgumentExpression(nameof(condition))] string actualExpression = "")
        {
            That(condition, Is.True, message, actualExpression, IsTrueExpression);
        }

        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display if the condition is false</param>
        public static void That(bool condition,
            FormattableString message,
            [CallerArgumentExpression(nameof(condition))] string actualExpression = "")
        {
            That(condition, Is.True, message, actualExpression, IsTrueExpression);
        }
#endif

        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void That(bool condition,
            Func<string?> getExceptionMessage,
            [CallerArgumentExpression(nameof(condition))] string actualExpression = "")
        {
            That(condition, Is.True, getExceptionMessage, actualExpression, IsTrueExpression);
        }

        #endregion

        #region Lambda returning Boolean

#if NET6_0_OR_GREATER
        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        /// <param name="message">The message to display if the condition is false</param>
        public static void That(Func<bool> condition,
            string? message = default,
            [CallerArgumentExpression(nameof(condition))] string actualExpression = "")
        {
            That(condition.Invoke(), Is.True, message, actualExpression, IsTrueExpression);
        }

        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        /// <param name="assertingHandler">The message to display if the condition is false</param>
        public static void That(Func<bool> condition,
            [InterpolatedStringHandlerArgument(nameof(condition))] AssertingInterpolatedStringHandler<bool> assertingHandler,
            [CallerArgumentExpression(nameof(condition))] string actualExpression = "")
        {
            IncrementAssertCount();
            var result = assertingHandler.Result;
            if (!result.IsSuccess)
                ReportFailure(result, assertingHandler.ToString(), actualExpression, IsTrueExpression);
        }
#else
        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        /// <param name="message">The message to display if the condition is false</param>
        public static void That(Func<bool> condition,
            NUnitString message = default,
            [CallerArgumentExpression(nameof(condition))] string actualExpression = "")
        {
            That(condition.Invoke(), Is.True, message, actualExpression, IsTrueExpression);
        }

        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        /// <param name="message">The message to display if the condition is false</param>
        public static void That(Func<bool> condition,
            FormattableString message,
            [CallerArgumentExpression(nameof(condition))] string actualExpression = "")
        {
            That(condition.Invoke(), Is.True, message, actualExpression, IsTrueExpression);
        }
#endif

        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void That(Func<bool> condition,
            Func<string?> getExceptionMessage,
            [CallerArgumentExpression(nameof(condition))] string actualExpression = "")
        {
            That(condition.Invoke(), Is.True, getExceptionMessage, actualExpression, IsTrueExpression);
        }

        #endregion

        #region ActualValueDelegate

#if NET6_0_OR_GREATER
        /// <summary>
        /// Apply a constraint to a delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
        /// <param name="expr">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void That<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr,
            string? message = default,
            [CallerArgumentExpression(nameof(del))] string actualExpression = "",
            [CallerArgumentExpression(nameof(expr))] string constraintExpression = "")
        {
            var constraint = expr.Resolve();

            IncrementAssertCount();
            var result = constraint.ApplyTo(del);
            if (!result.IsSuccess)
                ReportFailure(result, message, actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to a delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
        /// <param name="expr">A Constraint expression to be applied</param>
        /// <param name="assertingHandler">The message that will be displayed on failure</param>
        public static void That<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr,
            [InterpolatedStringHandlerArgument(nameof(del), nameof(expr))] AssertingInterpolatedStringHandler<TActual> assertingHandler,
            [CallerArgumentExpression(nameof(del))] string actualExpression = "",
            [CallerArgumentExpression(nameof(expr))] string constraintExpression = "")
        {
            IncrementAssertCount();
            var result = assertingHandler.Result;
            if (!result.IsSuccess)
                ReportFailure(result, assertingHandler.ToString(), actualExpression, constraintExpression);
        }
#else
        /// <summary>
        /// Apply a constraint to a delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
        /// <param name="expr">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void That<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr,
            NUnitString message = default,
            [CallerArgumentExpression(nameof(del))] string actualExpression = "",
            [CallerArgumentExpression(nameof(expr))] string constraintExpression = "")
        {
            var constraint = expr.Resolve();

            IncrementAssertCount();
            var result = constraint.ApplyTo(del);
            if (!result.IsSuccess)
                ReportFailure(result, message.ToString(), actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to a delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
        /// <param name="expr">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void That<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr,
            FormattableString message,
            [CallerArgumentExpression(nameof(del))] string actualExpression = "",
            [CallerArgumentExpression(nameof(expr))] string constraintExpression = "")
        {
            var constraint = expr.Resolve();

            IncrementAssertCount();
            var result = constraint.ApplyTo(del);
            if (!result.IsSuccess)
                ReportFailure(result, message.ToString(), actualExpression, constraintExpression);
        }
#endif

        /// <summary>
        /// Apply a constraint to a delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
        /// <param name="expr">A Constraint expression to be applied</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void That<TActual>(
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
                ReportFailure(result, getExceptionMessage(), actualExpression, constraintExpression);
        }

        #endregion

        #region TestDelegate

#if NET6_0_OR_GREATER
        /// <summary>
        /// Apply a constraint to a delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="code">A TestDelegate to be executed</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void That(TestDelegate code, IResolveConstraint constraint,
            string? message = default,
            [CallerArgumentExpression(nameof(code))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
        {
            That((object)code, constraint, message, actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to a delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="code">A TestDelegate to be executed</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="assertingHandler">The message that will be displayed on failure</param>
        public static void That(TestDelegate code, IResolveConstraint constraint,
            [InterpolatedStringHandlerArgument(nameof(code), nameof(constraint))] AssertingInterpolatedStringHandler<object> assertingHandler,
            [CallerArgumentExpression(nameof(code))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
        {
            IncrementAssertCount();
            var result = assertingHandler.Result;
            if (!result.IsSuccess)
                ReportFailure(result, assertingHandler.ToString(), actualExpression, constraintExpression);
        }
#else
        /// <summary>
        /// Apply a constraint to a delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="code">A TestDelegate to be executed</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void That(TestDelegate code, IResolveConstraint constraint,
            NUnitString message = default,
            [CallerArgumentExpression(nameof(code))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
        {
            That((object)code, constraint, message, actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to a delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="code">A TestDelegate to be executed</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void That(TestDelegate code, IResolveConstraint constraint,
            FormattableString message,
            [CallerArgumentExpression(nameof(code))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
        {
            That((object)code, constraint, message, actualExpression, constraintExpression);
        }
#endif

        /// <summary>
        /// Apply a constraint to a delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="code">A TestDelegate to be executed</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void That(TestDelegate code, IResolveConstraint constraint,
            Func<string?> getExceptionMessage,
            [CallerArgumentExpression(nameof(code))] string actualExpression = "",
            [CallerArgumentExpression(nameof(constraint))] string constraintExpression = "")
        {
            That((object)code, constraint, getExceptionMessage, actualExpression, constraintExpression);
        }

        #endregion

        #endregion

        #region Assert.That<TActual>

#if NET6_0_OR_GREATER
        /// <summary>
        /// Apply a constraint to an actual value. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="expression">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void That<TActual>(TActual actual, IResolveConstraint expression,
            string? message = default,
            [CallerArgumentExpression(nameof(actual))] string actualExpression = "",
            [CallerArgumentExpression(nameof(expression))] string constraintExpression = "")
        {
            var constraint = expression.Resolve();

            IncrementAssertCount();
            var result = constraint.ApplyTo(actual);
            if (!result.IsSuccess)
                ReportFailure(result, message, actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to an actual value. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="expression">A Constraint expression to be applied</param>
        /// <param name="assertingHandler">The message that will be displayed on failure</param>
        public static void That<TActual>(TActual actual, IResolveConstraint expression,
            [InterpolatedStringHandlerArgument(nameof(actual), nameof(expression))] AssertingInterpolatedStringHandler<TActual> assertingHandler,
            [CallerArgumentExpression(nameof(actual))] string actualExpression = "",
            [CallerArgumentExpression(nameof(expression))] string constraintExpression = "")
        {
            IncrementAssertCount();
            var result = assertingHandler.Result;
            if (!result.IsSuccess)
                ReportFailure(result, assertingHandler.ToString(), actualExpression, constraintExpression);
        }
#else
        /// <summary>
        /// Apply a constraint to an actual value. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="expression">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void That<TActual>(TActual actual, IResolveConstraint expression,
            NUnitString message = default,
            [CallerArgumentExpression(nameof(actual))] string actualExpression = "",
            [CallerArgumentExpression(nameof(expression))] string constraintExpression = "")
        {
            var constraint = expression.Resolve();

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
        /// <param name="expression">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void That<TActual>(TActual actual, IResolveConstraint expression,
            FormattableString message,
            [CallerArgumentExpression(nameof(actual))] string actualExpression = "",
            [CallerArgumentExpression(nameof(expression))] string constraintExpression = "")
        {
            var constraint = expression.Resolve();

            IncrementAssertCount();
            var result = constraint.ApplyTo(actual);
            if (!result.IsSuccess)
                ReportFailure(result, message.ToString(), actualExpression, constraintExpression);
        }
#endif

        /// <summary>
        /// Apply a constraint to an actual value. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="expression">A Constraint expression to be applied</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void That<TActual>(
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
                ReportFailure(result, getExceptionMessage(), actualExpression, constraintExpression);
        }

#endregion

        #region Assert.ByVal

        /// <summary>
        /// Apply a constraint to an actual value. Returns without throwing an exception when inside a multiple assert
        /// block. Used as a synonym for That in rare cases where a private setter causes a Visual Basic compilation
        /// error.
        /// </summary>
        /// <remarks>
        /// This method is provided for use by VB developers needing to test the value of properties with private
        /// setters.
        /// </remarks>
        /// <param name="actual">The actual value to test</param>
        /// <param name="expression">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void ByVal(object? actual, IResolveConstraint expression,
            string message = "",
            [CallerArgumentExpression(nameof(actual))] string actualExpression = "",
            [CallerArgumentExpression(nameof(expression))] string constraintExpression = "")
        {
            That(actual, expression, message, actualExpression, constraintExpression);
        }

        #endregion

        #region Helper Method

        private static void ReportFailure(ConstraintResult result, string? message, string actualExpression, string constraintExpression)
        {
            MessageWriter writer = new TextMessageWriter(
                ExtendedMessage($"{nameof(Assert)}.{nameof(Assert.That)}",
                message, actualExpression, constraintExpression));
            result.WriteMessageTo(writer);

            ReportFailure(writer.ToString());
        }

        #endregion
    }
}
