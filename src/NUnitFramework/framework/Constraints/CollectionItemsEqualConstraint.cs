// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using NUnit.Framework.Internal;

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
        /// The generic helper method used to compare two collections of the same underlying type. This is used to avoid boxing of value types when comparing collections of value types.
        /// </summary>
        private static readonly MethodInfo TallyResultCoreMethod = typeof(CollectionItemsEqualConstraint).GetMethod(nameof(TallyResultCore), BindingFlags.NonPublic | BindingFlags.Static)!;

        /// <summary>
        /// The NUnitEqualityComparer in use for this constraint
        /// </summary>
        private protected readonly NUnitEqualityComparer _comparer = new();

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
        /// Get a flag indicating whether the user requested that case be ignored.
        /// </summary>
        protected bool IgnoringCase => _comparer.IgnoreCase;

        /// <summary>
        /// Get a flag indicating whether the user requested that white space be ignored.
        /// </summary>
        protected bool IgnoringWhiteSpace => _comparer.IgnoreWhiteSpace;

        /// <summary>
        /// Get a flag indicating whether the user requested that line ending format be ignored.
        /// </summary>
        protected bool IgnoringLineEndingFormat => _comparer.IgnoreLineEndingFormat;

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
        /// Flag the constraint to ignore white space and return self.
        /// </summary>
        public CollectionItemsEqualConstraint IgnoreWhiteSpace
        {
            get
            {
                _comparer.IgnoreWhiteSpace = true;
                return this;
            }
        }

        /// <summary>
        /// Flag the constraint to ignore line ending format (\r vs. \n vs. \r\n) and return self.
        /// </summary>
        public CollectionItemsEqualConstraint IgnoreLineEndingFormat
        {
            get
            {
                _comparer.IgnoreLineEndingFormat = true;
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

        /// <summary>
        /// Enables comparing of instance properties.
        /// </summary>
        /// <remarks>
        /// This allows comparing classes that don't implement <see cref="IEquatable{T}"/>
        /// without having to compare each property separately in own code.
        /// </remarks>
        public CollectionItemsEqualConstraint UsingPropertiesComparer(Func<PropertiesComparerConfigurationUntyped, PropertiesComparerConfigurationUntyped> configure)
        {
            _comparer.CompareProperties = true;
            _comparer.ComparePropertiesConfiguration = configure(new PropertiesComparerConfigurationUntyped());
            return this;
        }

        #endregion

        /// <summary>
        /// Enables comparing of instance properties.
        /// </summary>
        /// <remarks>
        /// This allows comparing classes that don't implement <see cref="IEquatable{T}"/>
        /// without having to compare each property separately in own code.
        /// </remarks>
        public CollectionItemsEqualConstraint UsingPropertiesComparer<T>(Func<PropertiesComparerConfiguration<T>, PropertiesComparerConfiguration<T>> configure)
        {
            _comparer.CompareProperties = true;
            _comparer.ComparePropertiesConfiguration = configure(new PropertiesComparerConfiguration<T>());
            return this;
        }

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
        [Obsolete("This method will be removed in a future release.")]
        protected CollectionTally Tally(IEnumerable c)
        {
            return new CollectionTally(_comparer, c);
        }

        private protected static CollectionTally.CollectionTallyResult TallyResult(IEnumerable expected, IEnumerable actual, NUnitEqualityComparer comparer)
        {
            if (expected is IEnumerable<int> expectedInts && actual is IEnumerable<int> actualInts)
            {
                return TallyResultCore(expectedInts, actualInts, comparer);
            }
            else if (expected is IEnumerable<string> expectedStrings && actual is IEnumerable<string> actualStrings)
            {
                return TallyResultCore(expectedStrings, actualStrings, comparer);
            }
            else if (expected is IEnumerable<double> expectedDoubles && actual is IEnumerable<double> actualDoubles)
            {
                return TallyResultCore(expectedDoubles, actualDoubles, comparer);
            }
            else if (expected is IEnumerable<byte> expectedBytes && actual is IEnumerable<byte> actualBytes)
            {
                return TallyResultCore(expectedBytes, actualBytes, comparer);
            }
            else if (expected is StringCollection expectedStringCollection && actual is StringCollection actualStringCollection)
            {
                return TallyResultCore(ToStringList(expectedStringCollection), ToStringList(actualStringCollection), comparer);
            }
            else
            {
                var underlyingType = expected.GetType().FindPrimaryEnumerableInterfaceGenericTypeArgument();
                if (underlyingType is not null && underlyingType == actual.GetType().FindPrimaryEnumerableInterfaceGenericTypeArgument())
                {
                    var method = TallyResultCoreMethod.MakeGenericMethod(underlyingType);
                    return (CollectionTally.CollectionTallyResult)method.Invoke(null, [expected, actual, comparer])!;
                }

                // Fallback to object-based approach for non-generic collections or generic collections of different underlying type.
                var tally = new CollectionTally<object>(expected, comparer);
                tally.TryRemove(actual);
                return CollectionTally.CollectionTallyResult.FromGenericResult(tally.Result);
            }

            static IEnumerable<string?> ToStringList(IEnumerable l)
            {
                foreach (var item in l)
                    yield return item?.ToString();
            }
        }

        private protected static CollectionTally.CollectionTallyResult TallyResultCore<T>(IEnumerable<T> expectedItems, IEnumerable<T> actualItems, NUnitEqualityComparer comparer)
        {
            var tally = new CollectionTally<T>(expectedItems, comparer);

            tally.TryRemove(actualItems);
            return CollectionTally.CollectionTallyResult.FromGenericResult(tally.Result);
        }
    }
}
