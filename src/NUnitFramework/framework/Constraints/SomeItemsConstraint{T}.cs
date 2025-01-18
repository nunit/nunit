// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq.Expressions;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// SomeItemsConstraint applies another constraint to each
    /// item in a collection, succeeding if any of them succeeds.
    /// </summary>
    public class SomeItemsConstraint<T> : SomeItemsConstraint
    {
        private readonly EqualConstraint<T> _equalConstraint;

        /// <summary>
        /// Construct a SomeItemsConstraint on top of an existing constraint
        /// </summary>
        /// <param name="itemConstraint"></param>
        public SomeItemsConstraint(EqualConstraint<T> itemConstraint)
            : base(itemConstraint)
        {
            _equalConstraint = itemConstraint;
        }

        /// <summary>
        /// Enables comparing a subset of instance properties.
        /// </summary>
        /// <remarks>
        /// This allows comparing classes that don't implement <see cref="IEquatable{T}"/>
        /// without having to compare each property separately in own code.
        /// </remarks>
        /// <param name="propertyNamesToExclude">List of properties to exclude from comparison.</param>
        public SomeItemsConstraint UsingPropertiesComparerExcluding(params Expression<Func<T, object?>>[] propertyNamesToExclude)
        {
            _equalConstraint.UsingPropertiesComparerExcluding(propertyNamesToExclude);
            return this;
        }

        /// <summary>
        /// Enables comparing a subset of instance properties.
        /// </summary>
        /// <remarks>
        /// This allows comparing classes that don't implement <see cref="IEquatable{T}"/>
        /// without having to compare each property separately in own code.
        /// </remarks>
        /// <param name="propertyNamesToUse">List of properties to compare.</param>
        public SomeItemsConstraint UsingPropertiesComparerUsingOnly(params Expression<Func<T, object?>>[] propertyNamesToUse)
        {
            _equalConstraint.UsingPropertiesComparerUsingOnly(propertyNamesToUse);
            return this;
        }
    }
}
