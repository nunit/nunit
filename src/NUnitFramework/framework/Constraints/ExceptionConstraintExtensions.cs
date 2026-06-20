// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
    /// <summary>
    /// Provides extension methods for creating constraint expressions targeting Exception-related
    /// constraints types in tests.
    /// </summary>
    public static class ExceptionConstraintExtensions
    {
        extension<T>(ExactTypeConstraint<T> value)
            where T : ArgumentException
        {
            /// <summary>
            /// Gets a constraint expression for the ParamName property.
            /// </summary>
            public ResolvableConstraintExpression ParamName => value.With.Property(nameof(ArgumentException.ParamName));
        }

        extension<T>(InstanceOfTypeConstraint<T> value)
            where T : ArgumentException
        {
            /// <summary>
            /// Gets a constraint expression for the ParamName property.
            /// </summary>
            public ResolvableConstraintExpression ParamName => value.With.Property(nameof(ArgumentException.ParamName));
        }
    }
}
