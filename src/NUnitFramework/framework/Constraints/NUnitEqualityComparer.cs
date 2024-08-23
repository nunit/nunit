// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints.Comparers;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// NUnitEqualityComparer encapsulates NUnit's handling of
    /// equality tests between objects.
    /// </summary>
    public sealed class NUnitEqualityComparer
    {
        #region Static and Instance Fields

        /// <summary>
        /// Method for comparing two objects with a tolerance.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <param name="tolerance">The tolerance to use when comparing the objects.</param>
        /// <param name="state">The evaluation state of the comparison.</param>
        /// <param name="equalityComparer">The <see cref="NUnitEqualityComparer"/> for parameters.</param>
        /// <returns>
        ///     <see langword="null"/> if the objects cannot be compared using the method.
        ///     Otherwise the result of the comparison is returned.
        /// </returns>
        private delegate EqualMethodResult EqualMethod(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer);

        /// <summary>
        /// List of comparers used to compare pairs of objects.
        /// </summary>
        private static readonly EqualMethod[] Comparers =
        {
            ArraysComparer.Equal,
            DictionariesComparer.Equal,
            DictionaryEntriesComparer.Equal,
            KeyValuePairsComparer.Equal,
            StringsComparer.Equal,
            StreamsComparer.Equal,
            CharsComparer.Equal,
            EnumComparer.Equal,
            DirectoriesComparer.Equal,
            NumericsComparer.Equal,
            DateTimeOffsetsComparer.Equal,
            TimeSpanToleranceComparer.Equal,
            TupleComparer.Equal,
            ValueTupleComparer.Equal,
            StructuralComparer.Equal,
            EquatablesComparer.Equal,
            EnumerablesComparer.Equal,
            EqualsComparer.Equal,
        };

        /// <summary>
        /// If true, all string comparisons will ignore case
        /// </summary>
        private bool _caseInsensitive;

        /// <summary>
        /// If true, all string comparisons will ignore white space differences
        /// </summary>
        private bool _ignoreWhiteSpace;

        /// <summary>
        /// If true, arrays will be treated as collections, allowing
        /// those of different dimensions to be compared
        /// </summary>
        private bool _compareAsCollection;

        /// <summary>
        /// If true, when a class does not implement <see cref="IEquatable{T}"/>
        /// it will be compared property by property.
        /// </summary>
        private bool _compareProperties;

        /// <summary>
        /// Comparison objects used in comparisons for some constraints.
        /// </summary>
        private List<EqualityAdapter>? _externalComparers;

        /// <summary>
        /// List of points at which a failure occurred.
        /// </summary>
        private List<FailurePoint>? _failurePoints;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="NUnitEqualityComparer"/> class.
        /// </summary>
        public NUnitEqualityComparer()
        {
        }

        #region Properties

        /// <summary>
        /// Gets and sets a flag indicating whether case should
        /// be ignored in determining equality.
        /// </summary>
        public bool IgnoreCase
        {
            get => _caseInsensitive;
            set => _caseInsensitive = value;
        }

        /// <summary>
        /// Gets and sets a flag indicating whether white space should
        /// be ignored in determining equality.
        /// </summary>
        public bool IgnoreWhiteSpace
        {
            get => _ignoreWhiteSpace;
            set => _ignoreWhiteSpace = value;
        }

        /// <summary>
        /// Gets and sets a flag indicating whether an instance properties
        /// should be compared when determining equality.
        /// </summary>
        public bool CompareProperties
        {
            get => _compareProperties;
            set => _compareProperties = value;
        }

        /// <summary>
        /// Gets and sets a flag indicating that arrays should be
        /// compared as collections, without regard to their shape.
        /// </summary>
        public bool CompareAsCollection
        {
            get => _compareAsCollection;
            set => _compareAsCollection = value;
        }

        /// <summary>
        /// Gets the list of external comparers to be used to
        /// test for equality. They are applied to members of
        /// collections, in place of NUnit's own logic.
        /// </summary>
        public IList<EqualityAdapter> ExternalComparers => _externalComparers ??= new();

        /// <summary>
        /// Gets a value indicating whether there is any additional Failure Information.
        /// </summary>
        public bool HasFailurePoints => _failurePoints is not null && _failurePoints.Count > 0;

        /// <summary>
        /// Gets the list of failure points for the last Match performed.
        /// The list consists of objects to be interpreted by the caller.
        /// This generally means that the caller may only make use of
        /// objects it has placed on the list at a particular depth.
        /// </summary>
        public IList<FailurePoint> FailurePoints => _failurePoints ??= new();

        /// <summary>
        /// Flags the comparer to include <see cref="DateTimeOffset.Offset"/>
        /// property in comparison of two <see cref="DateTimeOffset"/> values.
        /// </summary>
        /// <remarks>
        /// Using this modifier does not allow to use the <see cref="Tolerance"/>
        /// modifier.
        /// </remarks>
        public bool WithSameOffset { get; set; }
        #endregion

        #region Public Methods

        /// <summary>
        /// Compares two objects for equality within a tolerance.
        /// </summary>
        public bool AreEqual(object? x, object? y, ref Tolerance tolerance)
        {
            EqualMethodResult result = AreEqual(x, y, ref tolerance, new ComparisonState(true));

            return ProcessEqualResult(x, y, result);
        }

        internal bool AreEqual<T>(T? x, T? y, ref Tolerance tolerance)
        {
            EqualMethodResult result = AreEqual(x, y, ref tolerance, new ComparisonState(true));

            return ProcessEqualResult(x, y, result);
        }

        internal static bool ProcessEqualResult<T1, T2>(T1? x, T2? y, EqualMethodResult result)
        {
            switch (result)
            {
                case EqualMethodResult.TypesNotSupported:
                    throw new NotSupportedException($"No comparer found for instances of type '{GetType(x)}' and '{GetType(y)}'");
                case EqualMethodResult.ToleranceNotSupported:
                    throw new NotSupportedException($"Specified Tolerance not supported for instances of type '{GetType(x)}' and '{GetType(y)}'");
                case EqualMethodResult.ComparedEqual:
                    return true;
                case EqualMethodResult.ComparisonPending:
                case EqualMethodResult.ComparedNotEqual:
                default:
                    return false;
            }

            static string GetType(object? x) => x?.GetType().FullName ?? "null";
        }

        internal EqualMethodResult AreEqual<T>(T? x, T? y, ref Tolerance tolerance, ComparisonState state)
        {
            if (x is null && y is null)
                return EqualMethodResult.ComparedEqual;

            if (x is null || y is null)
                return EqualMethodResult.ComparedNotEqual;

            if (object.ReferenceEquals(x, y))
                return EqualMethodResult.ComparedEqual;

            if (state.DidCompare(x, y))
                return EqualMethodResult.ComparisonPending;

            EqualityAdapter? externalComparer = GetExternalComparer(x, y);

            if (externalComparer is not null)
            {
                try
                {
                    return externalComparer.AreEqual(x, y, ref tolerance) ?
                        EqualMethodResult.ComparedEqual : EqualMethodResult.ComparedNotEqual;
                }
                catch (InvalidOperationException)
                {
                    return EqualMethodResult.ToleranceNotSupported;
                }
            }

            foreach (EqualMethod equalMethod in Comparers)
            {
                EqualMethodResult result = equalMethod(x, y, ref tolerance, state, this);
                if (result != EqualMethodResult.TypesNotSupported)
                    return result;
            }

            if (_compareProperties)
            {
                return PropertiesComparer.Equal(x, y, ref tolerance, state, this);
            }

            if (tolerance.HasVariance)
                return EqualMethodResult.ToleranceNotSupported;

            return x.Equals(y) ?
                EqualMethodResult.ComparedEqual : EqualMethodResult.ComparedNotEqual;
        }

        #endregion

        #region Helper Methods

        private EqualityAdapter? GetExternalComparer(object x, object y)
        {
            if (_externalComparers is not null)
            {
                foreach (EqualityAdapter adapter in _externalComparers)
                {
                    if (adapter.CanCompare(x, y))
                        return adapter;
                }
            }

            return null;
        }

        #endregion

        #region Nested FailurePoint Class

        /// <summary>
        /// FailurePoint class represents one point of failure
        /// in an equality test.
        /// </summary>
        public sealed class FailurePoint
        {
            /// <summary>
            /// The location of the failure
            /// </summary>
            public long Position;

            /// <summary>
            /// The name of the property.
            /// </summary>
            public string? PropertyName;

            /// <summary>
            /// The expected value
            /// </summary>
            public object? ExpectedValue;

            /// <summary>
            /// The actual value
            /// </summary>
            public object? ActualValue;

            /// <summary>
            /// Indicates whether the expected value is valid
            /// </summary>
            public bool ExpectedHasData;

            /// <summary>
            /// Indicates whether the actual value is valid
            /// </summary>
            public bool ActualHasData;
        }

        #endregion
    }
}
