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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints.Comparers;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// NUnitEqualityComparer encapsulates NUnit's handling of
    /// equality tests between objects.
    /// </summary>
    public class NUnitEqualityComparer
    {
        #region Static and Instance Fields
        /// <summary>
        /// If true, all string comparisons will ignore case
        /// </summary>
        private bool caseInsensitive;

        /// <summary>
        /// If true, arrays will be treated as collections, allowing
        /// those of different dimensions to be compared
        /// </summary>
        private bool compareAsCollection;

        /// <summary>
        /// Comparison objects used in comparisons for some constraints.
        /// </summary>
        private List<EqualityAdapter> externalComparers = new List<EqualityAdapter>();

        /// <summary>
        /// List of points at which a failure occurred.
        /// </summary>
        private List<FailurePoint> failurePoints;

        private readonly EnumerablesComparer _enumerablesComparer;
        private readonly ArraysComparer _arraysComparer;
        private readonly CharsComparer _charsComparer;
        private readonly DateTimeOffsetsComparer _dateTimeOffsetsComparer;
        private readonly DictionariesComparer _dictionariesComparer;
        private readonly DictionaryEntriesComparer _dictionaryEntriesComparer;
        private readonly DirectoriesComparer _directoriesComparer;
        private readonly IEquatablesComparer _IEqualityComparer;
        private readonly KeyValuePairsComparer _keyValuePairsComparer;
        private readonly NumericsComparer _numericsComparer;
        private readonly StreamsComparer _streamsComparer;
        private readonly StringsComparer _stringsComparer;
        private readonly TimeSpanToleranceComparer _timeSpanToleranceComparer;

        #endregion

        internal NUnitEqualityComparer()
        {
            _enumerablesComparer = new EnumerablesComparer(this);
            _arraysComparer = new ArraysComparer(_enumerablesComparer);
            _charsComparer = new CharsComparer(this);
            _dateTimeOffsetsComparer = new DateTimeOffsetsComparer(this);
            _dictionariesComparer = new DictionariesComparer(this);
            _dictionaryEntriesComparer = new DictionaryEntriesComparer(this);
            _directoriesComparer = new DirectoriesComparer();
            _IEqualityComparer = new IEquatablesComparer();
            _keyValuePairsComparer = new KeyValuePairsComparer(this);
            _numericsComparer = new NumericsComparer();
            _streamsComparer = new StreamsComparer(this);
            _stringsComparer = new StringsComparer(this );
            _timeSpanToleranceComparer = new TimeSpanToleranceComparer();
        }

        #region Properties

        /// <summary>
        /// Returns the default NUnitEqualityComparer
        /// </summary>
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
            get { return caseInsensitive; }
            set { caseInsensitive = value; }
        }

        /// <summary>
        /// Gets and sets a flag indicating that arrays should be
        /// compared as collections, without regard to their shape.
        /// </summary>
        public bool CompareAsCollection
        {
            get { return compareAsCollection; }
            set { compareAsCollection = value; }
        }

        /// <summary>
        /// Gets the list of external comparers to be used to
        /// test for equality. They are applied to members of
        /// collections, in place of NUnit's own logic.
        /// </summary>
        public IList<EqualityAdapter> ExternalComparers
        {
            get { return externalComparers; }
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
        public bool WithSameOffset { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Compares two objects for equality within a tolerance.
        /// </summary>
        public bool AreEqual(object x, object y, ref Tolerance tolerance)
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
                return externalComparer.AreEqual(x, y);

            if (x.GetType().IsArray && y.GetType().IsArray && !compareAsCollection)
                return _arraysComparer.Equal((Array)x, (Array)y, ref tolerance);

            if (x is IDictionary && y is IDictionary)
                return _dictionariesComparer.Equal((IDictionary)x, (IDictionary)y, ref tolerance);

            // Issue #70 - EquivalentTo isn't compatible with IgnoreCase for dictionaries
            if (x is DictionaryEntry && y is DictionaryEntry)
                return _dictionaryEntriesComparer.Equal((DictionaryEntry)x, (DictionaryEntry)y, ref tolerance);

            // IDictionary<,> will eventually try to compare it's key value pairs when using CollectionTally
            bool? keyValuePairEqual = _keyValuePairsComparer.Equal(x, y, ref tolerance);
            if (keyValuePairEqual.HasValue)
                return keyValuePairEqual.Value;

            if (x is string && y is string)
                return _stringsComparer.Equal((string)x, (string)y);

            if (x is Stream && y is Stream)
                return _streamsComparer.Equal((Stream)x, (Stream)y);

            if (x is char && y is char)
                return _charsComparer.Equal((char)x, (char)y);

            if (x is DirectoryInfo && y is DirectoryInfo)
                return _directoriesComparer.Equal((DirectoryInfo)x, (DirectoryInfo)y);

            if (Numerics.IsNumericType(x) && Numerics.IsNumericType(y))
                return _numericsComparer.Equal(x, y, ref tolerance);

            if (x is DateTimeOffset && y is DateTimeOffset)
                return _dateTimeOffsetsComparer.Equal((DateTimeOffset)x, (DateTimeOffset)y, ref tolerance);

            if (tolerance != null && tolerance.Amount is TimeSpan)
            {
                bool? result = _timeSpanToleranceComparer.Equal(x, y, (TimeSpan)tolerance.Amount);
                if (result.HasValue)
                    return result.Value;
            }

            if (!compareAsCollection)
            {
                bool? result = _IEqualityComparer.Equal(x, y);
                if (result.HasValue)
                    return result.Value;
            }

            if (x is IEnumerable && y is IEnumerable)
                return _enumerablesComparer.Equal((IEnumerable) x, (IEnumerable) y, ref tolerance);

            return x.Equals(y);
        }

        #endregion

        #region Helper Methods

        private EqualityAdapter GetExternalComparer(object x, object y)
        {
            foreach (EqualityAdapter adapter in externalComparers)
                if (adapter.CanCompare(x, y))
                    return adapter;

            return null;
        }

        #endregion

        #region Nested FailurePoint Class

        /// <summary>
        /// FailurePoint class represents one point of failure
        /// in an equality test.
        /// </summary>
        public class FailurePoint
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