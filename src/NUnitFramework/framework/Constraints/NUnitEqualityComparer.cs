// ***********************************************************************
// Copyright (c) 2009 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        private bool _isFrozen;

        /// <summary>
        /// If true, all string comparisons will ignore case
        /// </summary>
        private bool _caseInsensitive;

        /// <summary>
        /// If true, arrays will be treated as collections, allowing
        /// those of different dimensions to be compared
        /// </summary>
        private bool _compareAsCollection;

        private bool _withSameOffset;

        /// <summary>
        /// Comparison objects used in comparisons for some constraints.
        /// </summary>
        private IList<EqualityAdapter> _externalComparers = new List<EqualityAdapter>();

        /// <summary>
        /// List of points at which a failure occurred.
        /// </summary>
        private List<FailurePoint> failurePoints;

        /// <summary>
        /// List of comparers used to compare pairs of objects.
        /// </summary>
        private List<ChainComparer> _comparers;

        #endregion

        #region Properties

        /// <summary>
        /// Returns the default NUnitEqualityComparer
        /// </summary>
        [Obsolete("Deprecated. Use the default constructor instead.")]
        public static NUnitEqualityComparer Default
        {
            get { return new NUnitEqualityComparer(); }
        }
        /// <summary>
        /// Gets and sets a flag indicating whether case should
        /// be ignored in determining equality.
        /// </summary>
        public bool IgnoreCase
        {
            get { return _caseInsensitive; }
            set { SetProperty(ref _caseInsensitive, value); }
        }

        /// <summary>
        /// Gets and sets a flag indicating that arrays should be
        /// compared as collections, without regard to their shape.
        /// </summary>
        public bool CompareAsCollection
        {
            get { return _compareAsCollection; }
            set { SetProperty(ref _compareAsCollection, value); }
        }

        /// <summary>
        /// Gets the list of external comparers to be used to
        /// test for equality. They are applied to members of
        /// collections, in place of NUnit's own logic.
        /// </summary>
        public IList<EqualityAdapter> ExternalComparers
        {
            get { return _externalComparers; }
        }

        // TODO: Define some sort of FailurePoint struct or otherwise
        // eliminate the type-unsafeness of the current approach

        /// <summary>
        /// Gets the list of failure points for the last Match performed.
        /// The list consists of objects to be interpreted by the caller.
        /// This generally means that the caller may only make use of
        /// objects it has placed on the list at a particular depth.
        /// </summary>
        public IList<FailurePoint> FailurePoints
        {
            get { return failurePoints; }
        }

        /// <summary>
        /// Flags the comparer to include <see cref="DateTimeOffset.Offset"/>
        /// property in comparison of two <see cref="DateTimeOffset"/> values.
        /// </summary>
        /// <remarks>
        /// Using this modifier does not allow to use the <see cref="Tolerance"/>
        /// modifier.
        /// </remarks>
        public bool WithSameOffset
        {
            get { return _withSameOffset; }
            set { SetProperty(ref _withSameOffset, value); }
        }

        private IList<ChainComparer> Comparers
        {
            get
            {
                if (_comparers == null)
                {
                    _comparers = new List<ChainComparer>
                    {
                        new ArraysComparer(this),
                        new DictionariesComparer(this),
                        new DictionaryEntriesComparer(this),
                        new KeyValuePairsComparer(this),
                        new StringsComparer(_caseInsensitive),
                        new StreamsComparer(this),
                        new CharsComparer(_caseInsensitive),
                        new DirectoriesComparer(),
                        new NumericsComparer(),
                        new DateTimesComparer(),
                        new DateTimeOffsetsComparer(_withSameOffset),
                        new TimeSpansComparer(),
                        new EquatablesComparer(),
                        new TupleComparer(this),
                        new ValueTupleComparer(this),
                        new EnumerablesComparer(this)
                    };
                    Freeze();
                }

                return _comparers;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Compares two objects for equality within a tolerance.
        /// </summary>
        public bool AreEqual(object x, object y, ref Tolerance tolerance, bool topLevelComparison = true)
        {
            this.failurePoints = new List<FailurePoint>();

            if (x == null && y == null)
                return true;

            if (x == null || y == null)
                return false;

            if (object.ReferenceEquals(x, y))
                return true;

            EqualityAdapter externalComparer = GetExternalComparer(x, y);
            if (externalComparer != null)
                return externalComparer.Equals(x, y);

            foreach (ChainComparer comparer in SelectComparers(topLevelComparison))
            {
                bool? result = comparer.Equals(x, y, ref tolerance);
                if (result.HasValue)
                    return result.Value;
            }

            return x.Equals(y);
        }

        public int GetHashCode(object obj, bool topLevelComparison = true)
        {
            if (obj == null) return 0;

            foreach (EqualityAdapter adapter in _externalComparers)
            {
                if (adapter.CanCompare(obj))
                {
                    return adapter.GetHashCode(obj);
                }
            }

            foreach (var comparer in SelectComparers(topLevelComparison))
            {
                if (comparer.CanCompare(obj))
                {
                    return comparer.GetHashCode(obj);
                }
            }

            return obj.GetHashCode();
        }

        #endregion

        #region Helper Methods

        private EqualityAdapter GetExternalComparer(object x, object y)
        {
            foreach (EqualityAdapter adapter in _externalComparers)
                if (adapter.CanCompare(x, y))
                    return adapter;

            return null;
        }

        private IEnumerable<ChainComparer> SelectComparers(bool topLevelComparison)
        {
            return topLevelComparison && _compareAsCollection
                ? SelectCollectionComparers()
                : Comparers;
        }

        private IEnumerable<ChainComparer> SelectCollectionComparers()
        {
            return Comparers.Where(c => !(c is ArraysComparer) && !(c is EquatablesComparer));
        }

        private void SetProperty<T>(ref T field, T value)
        {
            if (_isFrozen)
            {
                throw new InvalidOperationException("Comparer is read-only after first comparison operation.");
            }
            field = value;
        }

        private void Freeze()
        {
            _externalComparers = new ReadOnlyCollection<EqualityAdapter>(_externalComparers);
            _isFrozen = true;
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
