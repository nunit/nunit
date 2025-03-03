// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

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
        /// <param name="configure">Function to configure the <see cref="PropertiesComparerConfiguration"/></param>
        public SomeItemsConstraint UsingPropertiesComparer(Func<PropertiesComparerConfiguration<T>, PropertiesComparerConfiguration<T>> configure)
        {
            _equalConstraint.UsingPropertiesComparer(configure);
            return this;
        }
    }
}
