// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// CollectionItemsEqualConstraint is the abstract base class for all
    /// collection constraints that apply some notion of item equality
    /// as a part of their operation.
    /// </summary>
    public abstract class CollectionItemsEqualConstraint : CollectionConstraint
    {
        /// <summary>
        /// The NUnitEqualityComparer in use for this constraint
        /// </summary>
        private readonly NUnitEqualityComparer _comparer = new();

        /// <summary>
        /// Construct an empty CollectionConstraint
        /// </summary>
        protected CollectionItemsEqualConstraint()
        {
        }

        /// <summary>
        /// Construct a CollectionConstraint
        /// </summary>
        /// <param name="arg"></param>
        protected CollectionItemsEqualConstraint(object? arg) : base(arg)
        {
        }

        #region Protected Properties

        /// <summary>
        /// Get a flag indicating whether the user requested us to ignore case.
        /// </summary>
        protected bool IgnoringCase => _comparer.IgnoreCase;

        /// <summary>
        /// Get a flag indicating whether any external comparers are in use.
        /// </summary>
        protected bool UsingExternalComparer => _comparer.ExternalComparers.Count > 0;

        #endregion

        #region Modifiers

        /// <summary>
        /// Flag the constraint to ignore case and return self.
        /// </summary>
        public CollectionItemsEqualConstraint IgnoreCase
        {
            get
            {
                _comparer.IgnoreCase = true;
                return this;
            }
        }

        /// <summary>
        /// Flag the constraint to use the supplied IComparer object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        public CollectionItemsEqualConstraint Using(IComparer comparer)
        {
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied IComparer object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        public CollectionItemsEqualConstraint Using<T>(IComparer<T> comparer)
        {
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied Comparison object.
        /// </summary>
        /// <param name="comparison">The Comparison object to use.</param>
        public CollectionItemsEqualConstraint Using<T>(Comparison<T> comparison)
        {
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparison));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied IEqualityComparer object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        public CollectionItemsEqualConstraint Using(IEqualityComparer comparer)
        {
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied IEqualityComparer object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        public CollectionItemsEqualConstraint Using<T>(IEqualityComparer<T> comparer)
        {
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied boolean-returning delegate.
        /// </summary>
        /// <param name="comparer">The supplied boolean-returning delegate to use.</param>
        public CollectionItemsEqualConstraint Using<T>(Func<T, T, bool> comparer)
        {
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        internal CollectionItemsEqualConstraint Using(EqualityAdapter adapter)
        {
            _comparer.ExternalComparers.Add(adapter);
            return this;
        }

        /// <summary>
        /// Enables comparing of instance properties.
        /// </summary>
        /// <remarks>
        /// This allows comparing classes that don't implement <see cref="IEquatable{T}"/>
        /// without having to compare each property separately in own code.
        /// </remarks>
        public CollectionItemsEqualConstraint UsingPropertiesComparer()
        {
            _comparer.CompareProperties = true;
            return this;
        }

        #endregion

        /// <summary>
        /// Compares two collection members for equality
        /// </summary>
        protected bool ItemsEqual(object? x, object? y)
        {
            Tolerance tolerance = Tolerance.Default;
            return _comparer.AreEqual(x, y, ref tolerance);
        }

        /// <summary>
        /// Return a new CollectionTally for use in making tests
        /// </summary>
        /// <param name="c">The collection to be included in the tally</param>
        protected CollectionTally Tally(IEnumerable c)
        {
            return new CollectionTally(_comparer, c);
        }
    }
}
