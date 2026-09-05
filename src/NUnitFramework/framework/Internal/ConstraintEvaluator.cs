// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Resolves a constraint, applies it, and dispatches the result to a failure handler.
    /// Shared by <see cref="NUnit.Framework.Assert"/> and <see cref="NUnit.Framework.Warn"/>,
    /// which differ only in what they do when the constraint is not satisfied.
    /// </summary>
    /// <remarks>
    /// The actual value/delegate and message are passed through as plain generic parameters rather
    /// than captured in a delegate, so a successful assertion allocates nothing beyond what the
    /// caller already provided.
    /// </remarks>
    internal static class ConstraintEvaluator
    {
        public static void Evaluate<TActual, TMessage>(
            Func<TActual> code,
            IResolveConstraint expr,
            TMessage message,
            string actualExpression,
            string constraintExpression,
            Action<ConstraintResult, string, string, string> onFailure)
        {
            var constraint = expr.Resolve();

            TestExecutionContext.CurrentContext.IncrementAssertCount();
            var result = constraint.ApplyTo(code);
            if (!result.IsSuccess)
                onFailure(result, message!.ToString()!, actualExpression, constraintExpression);
        }

        public static void Evaluate<TActual>(
            Func<TActual> code,
            IResolveConstraint expr,
            Func<string> getExceptionMessage,
            string actualExpression,
            string constraintExpression,
            Action<ConstraintResult, string, string, string> onFailure)
        {
            var constraint = expr.Resolve();

            TestExecutionContext.CurrentContext.IncrementAssertCount();
            var result = constraint.ApplyTo(code);
            if (!result.IsSuccess)
                onFailure(result, getExceptionMessage(), actualExpression, constraintExpression);
        }

        public static void Evaluate<TActual, TMessage>(
            TActual actual,
            IResolveConstraint expr,
            TMessage message,
            string actualExpression,
            string constraintExpression,
            Action<ConstraintResult, string, string, string> onFailure)
        {
            var constraint = expr.Resolve();

            TestExecutionContext.CurrentContext.IncrementAssertCount();
            var result = constraint.ApplyTo(actual);
            if (!result.IsSuccess)
                onFailure(result, message!.ToString()!, actualExpression, constraintExpression);
        }

        public static void Evaluate<TActual>(
            TActual actual,
            IResolveConstraint expr,
            Func<string> getExceptionMessage,
            string actualExpression,
            string constraintExpression,
            Action<ConstraintResult, string, string, string> onFailure)
        {
            var constraint = expr.Resolve();

            TestExecutionContext.CurrentContext.IncrementAssertCount();
            var result = constraint.ApplyTo(actual);
            if (!result.IsSuccess)
                onFailure(result, getExceptionMessage(), actualExpression, constraintExpression);
        }
    }
}
