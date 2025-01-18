// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// EqualConstraint is able to compare an actual value with the
    /// expected value provided in its constructor. Two objects are
    /// considered equal if both are null, or if both have the same
    /// value. NUnit has special semantics for some object types.
    /// </summary>
    public class EqualConstraint<T> : EqualConstraint
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="EqualConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        public EqualConstraint(object? expected)
            : base(expected)
        {
        }
        #endregion

        #region Constraint Modifiers
        /// <summary>
        /// Enables comparing a subset of instance properties.
        /// </summary>
        /// <remarks>
        /// This allows comparing classes that don't implement <see cref="IEquatable{T}"/>
        /// without having to compare each property separately in own code.
        /// </remarks>
        /// <param name="propertyNamesToUse">List of properties to compare.</param>
        public EqualConstraint UsingPropertiesComparerUsingOnly(params Expression<Func<T, object?>>[] propertyNamesToUse)
        {
            Comparer.CompareProperties = true;
            Comparer.PropertyNamesToExclude = null;
            Comparer.PropertyNamesToUse = new HashSet<string>(propertyNamesToUse.Select(GetNameFromExpression));
            return this;
        }

        /// <summary>
        /// Enables comparing a subset of instance properties.
        /// </summary>
        /// <remarks>
        /// This allows comparing classes that don't implement <see cref="IEquatable{T}"/>
        /// without having to compare each property separately in own code.
        /// </remarks>
        /// <param name="propertyNamesToExclude">List of property names to exclude from comparison.</param>
        public EqualConstraint UsingPropertiesComparerExcluding(params Expression<Func<T, object?>>[] propertyNamesToExclude)
        {
            Comparer.CompareProperties = true;
            Comparer.PropertyNamesToExclude = new HashSet<string>(propertyNamesToExclude.Select(GetNameFromExpression));
            Comparer.PropertyNamesToUse = null;
            return this;
        }

        private static string GetNameFromExpression(Expression<Func<T, object?>> expression)
        {
            Expression body = expression.Body;

            // We only expect a single member access, but it might include in implicit cast to object.
            if (body is UnaryExpression unaryExpresion &&
                unaryExpresion.NodeType == ExpressionType.Convert &&
                unaryExpresion.Type == typeof(object))
            {
                body = unaryExpresion.Operand;
            }

            if (body is MemberExpression memberExpression)
                return memberExpression.Member.Name;

            throw new ArgumentException("Expression must be a member expression", nameof(expression));
        }

        #endregion

    }
}
