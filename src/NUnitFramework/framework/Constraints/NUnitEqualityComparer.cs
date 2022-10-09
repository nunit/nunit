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
        private delegate bool? EqualMethod(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer);

        /// <summary>
        /// List of comparers used to compare pairs of objects.
        /// </summary>
        private static readonly EqualMethod[] _comparers = 
        {
            ArraysComparer.Equal,
            DictionariesComparer.Equal,
            DictionaryEntriesComparer.Equal,
            KeyValuePairsComparer.Equal,
            StringsComparer.Equal,
            StreamsComparer.Equal,
            CharsComparer.Equal,
            DirectoriesComparer.Equal,
            NumericsComparer.Equal,
            DateTimeOffsetsComparer.Equal,
            TimeSpanToleranceComparer.Equal,
            TupleComparer.Equal,
            ValueTupleComparer.Equal,
            StructuralComparer.Equal,
            EquatablesComparer.Equal,
            EnumerablesComparer.Equal,
        };

        /// <summary>
        /// If true, all string comparisons will ignore case
        /// </summary>
        private bool _caseInsensitive;

        /// <summary>
        /// If true, arrays will be treated as collections, allowing
        /// those of different dimensions to be compared
        /// </summary>
        private bool _compareAsCollection;

        /// <summary>
        /// Comparison objects used in comparisons for some constraints.
        /// </summary>
        private List<EqualityAdapter> _externalComparers;

        /// <summary>
        /// List of points at which a failure occurred.
        /// </summary>
        private List<FailurePoint> _failurePoints;

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
        /// Gets the list of failure points for the last Match performed.
        /// The list consists of objects to be interpreted by the caller.
        /// This generally means that the caller may only make use of
        /// objects it has placed on the list at a particular depth.
        /// </summary>
        public IList<FailurePoint> FailurePoints => _failurePoints;

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
        public bool AreEqual(object x, object y, ref Tolerance tolerance)
        {
            return AreEqual(x, y, ref tolerance, new ComparisonState(true));
        }

        internal bool AreEqual(object x, object y, ref Tolerance tolerance, ComparisonState state)
        {
            this._failurePoints = new List<FailurePoint>();

            if (x == null && y == null)
                return true;

            if (x == null || y == null)
                return false;

            if (object.ReferenceEquals(x, y))
                return true;

            if (state.DidCompare(x, y))
                return false;

            EqualityAdapter externalComparer = GetExternalComparer(x, y);
            if (externalComparer != null)
                return externalComparer.AreEqual(x, y);

            foreach (EqualMethod equalMethod in _comparers)
            {
                bool? result = equalMethod(x, y, ref tolerance, state, this);
                if (result.HasValue)
                    return result.Value;
            }

            return x.Equals(y);
        }

        #endregion

        #region Helper Methods

        private EqualityAdapter GetExternalComparer(object x, object y)
        {
            if (_externalComparers != null)
            {
                foreach (EqualityAdapter adapter in _externalComparers)
                    if (adapter.CanCompare(x, y))
                        return adapter;
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
            /// The expected value
            /// </summary>
            public object ExpectedValue;

            /// <summary>
            /// The actual value
            /// </summary>
            public object ActualValue;

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
