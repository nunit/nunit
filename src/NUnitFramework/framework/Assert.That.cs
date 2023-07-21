// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Constraints;
#if NET6_0
using System.Runtime.CompilerServices;
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

        #region Boolean

        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="message">The message to display if the condition is false</param>
        public static void That(bool condition, string message="", [CallerArgumentExpression("condition")] string? actualExpression = null)
        {
            That(condition, Is.True, message, actualExpression, "Is.True");
        }

        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        //public static void That(bool condition, [CallerArgumentExpression("condition")] string? actualExpression = null)
        //{
        //    That(condition, Is.True, string.Empty, actualExpression, "Is.True");
        //}

        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">The evaluated condition</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void That(bool condition, Func<string?> getExceptionMessage,
            [CallerArgumentExpression("condition")] string? actualExpression = null)
        {
            That(condition, Is.True, getExceptionMessage, actualExpression, "Is.True");
        }

        #endregion

        #region Lambda returning Boolean

        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        /// <param name="message">The message to display if the condition is false</param>
        public static void That(Func<bool> condition, string message="",
            [CallerArgumentExpression("condition")] string? actualExpression = null)
        {
            That(condition.Invoke(), Is.True, message, actualExpression, "Is.True");
        }

        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        //public static void That(Func<bool> condition,
        //    [CallerArgumentExpression("condition")] string? actualExpression = null)
        //{
        //    That(condition.Invoke(), Is.True, string.Empty, actualExpression, "Is.True");
        //}

        /// <summary>
        /// Asserts that a condition is true. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="condition">A lambda that returns a Boolean</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void That(Func<bool> condition, Func<string?> getExceptionMessage,
            [CallerArgumentExpression("condition")] string? actualExpression = null)
        {
            That(condition.Invoke(), Is.True, getExceptionMessage, actualExpression, "Is.True");
        }

        #endregion

        #region ActualValueDelegate

        /// <summary>
        /// Apply a constraint to a delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
        /// <param name="expr">A Constraint expression to be applied</param>
        //public static void That<TActual>(
        //    ActualValueDelegate<TActual> del,
        //    IResolveConstraint expr,
        //    [CallerArgumentExpression("del")] string? actualExpression = null,
        //    [CallerArgumentExpression("expr")] string? constraintExpression = null)
        //{
        //    That(del, expr.Resolve(), string.Empty, actualExpression, constraintExpression);
        //}

        /// <summary>
        /// Apply a constraint to a delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
        /// <param name="expr">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void That<TActual>(
            ActualValueDelegate<TActual> del,
            IResolveConstraint expr,
            string message="",
            [CallerArgumentExpression("del")] string? actualExpression = null,
            [CallerArgumentExpression("expr")] string? constraintExpression = null)
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
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void That<TActual>(
            ActualValueDelegate<TActual> del,
            IResolveConstraint expr,
            Func<string?> getExceptionMessage,
            [CallerArgumentExpression("del")] string? actualExpression = null,
            [CallerArgumentExpression("expr")] string? constraintExpression = null)
        {
            var constraint = expr.Resolve();

            IncrementAssertCount();
            var result = constraint.ApplyTo(del);
            if (!result.IsSuccess)
                ReportFailure(result, getExceptionMessage(), actualExpression, constraintExpression);
        }

        #endregion

        #region TestDelegate

        /// <summary>
        /// Apply a constraint to a delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="code">A TestDelegate to be executed</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        //public static void That(
        //    TestDelegate code,
        //    IResolveConstraint constraint,
        //    [CallerArgumentExpression("code")] string? actualExpression = null,
        //    [CallerArgumentExpression("constraint")] string? constraintExpression = null)
        //{
        //    That(code, constraint, string.Empty, actualExpression, constraintExpression);
        //}

        /// <summary>
        /// Apply a constraint to a delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="code">A TestDelegate to be executed</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void That(
            TestDelegate code, 
            IResolveConstraint constraint, 
            string message = "",
            [CallerArgumentExpression("code")] string? actualExpression = null,
            [CallerArgumentExpression("constraint")] string? constraintExpression = null)
        {
            That((object)code, constraint, message, actualExpression, constraintExpression);
        }

        /// <summary>
        /// Apply a constraint to a delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <param name="code">A TestDelegate to be executed</param>
        /// <param name="constraint">A Constraint expression to be applied</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void That(TestDelegate code, IResolveConstraint constraint, Func<string?> getExceptionMessage,
            [CallerArgumentExpression("code")] string? actualExpression = null,
            [CallerArgumentExpression("constraint")] string? constraintExpression = null)
        {
            That((object)code, constraint, getExceptionMessage, actualExpression, constraintExpression);
        }

        #endregion

        #endregion

        #region Assert.That<TActual>

        /// <summary>
        /// Apply a constraint to an actual value. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="expression">A Constraint expression to be applied</param>
        //public static void That<TActual>(
        //    TActual actual, 
        //    IResolveConstraint expression,
        //    [CallerArgumentExpression("actual")] string? actualExpression = null,
        //    [CallerArgumentExpression("expression")] string? constraintExpression = null)
        //{
        //    That(actual, expression, string.Empty, actualExpression, constraintExpression);
        //}

        /// <summary>
        /// Apply a constraint to an actual value. Returns without throwing an exception when inside a multiple assert
        /// block.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="actual">The actual value to test</param>
        /// <param name="expression">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void That<TActual>(
            TActual actual,
            IResolveConstraint expression,
            string message="",
            [CallerArgumentExpression("actual")] string? actualExpression = null,
            [CallerArgumentExpression("expression")] string? constraintExpression = null)
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
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void That<TActual>(
            TActual actual,
            IResolveConstraint expression,
            Func<string?> getExceptionMessage,
            [CallerArgumentExpression("actual")] string? actualExpression = null,
            [CallerArgumentExpression("expression")] string? constraintExpression = null)
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
        /// <param name="actual">The actual value to test</param>
        /// <param name="expression">A Constraint expression to be applied</param>
        public static void ByVal(object? actual, IResolveConstraint expression,
            [CallerArgumentExpression("actual")] string? actualExpression = null,
            [CallerArgumentExpression("expression")] string? constraintExpression = null)
        {
            That(actual, expression, string.Empty, actualExpression, constraintExpression);
        }

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
        public static void ByVal(object? actual, IResolveConstraint expression, string? message,
            [CallerArgumentExpression("actual")] string? actualExpression = null,
            [CallerArgumentExpression("expression")] string? constraintExpression = null)
        {
            That(actual, expression, message, actualExpression, constraintExpression);
        }

        #endregion
    }
}
