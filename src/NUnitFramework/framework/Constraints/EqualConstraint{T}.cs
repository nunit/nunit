// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

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
        /// <param name="configure">Function to configure the <see cref="PropertiesComparerConfiguration"/></param>
        public EqualConstraint UsingPropertiesComparer(Func<PropertiesComparerConfiguration<T>, PropertiesComparerConfiguration<T>> configure)
        {
            Comparer.CompareProperties = true;
            Comparer.ComparePropertiesConfiguration = configure(new PropertiesComparerConfiguration<T>());
            return this;
        }

        #endregion

    }
}
