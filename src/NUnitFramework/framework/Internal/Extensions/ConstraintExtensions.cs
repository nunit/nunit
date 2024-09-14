// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Internal.Extensions
{
    internal static class ConstraintExtensions
    {
        public static Task<ConstraintResult> ApplyToAsync<TActual>(this IConstraint constraint, Func<Task<TActual>> delTask)
        {
            if (constraint is not IAsyncConstraint asyncConstraint)
            {
                throw new NotSupportedException($"Constraint {constraint?.GetType().Name} does not support async execution.");
            }

            return asyncConstraint.ApplyToAsync(delTask);
        }
    }
}
