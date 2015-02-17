﻿// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
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
using System.Reflection;

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
        /// List of points at which a failure occured.
        /// </summary>
        private List<FailurePoint> failurePoints;

        private static readonly int BUFFER_SIZE = 4096;
        #endregion

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
        /// objects it has placed on the list at a particular depthy.
        /// </summary>
        public IList<FailurePoint> FailurePoints
        {
            get { return failurePoints; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Compares two strings for equality.
        /// </summary>
        public bool AreEqual(string x, string y, ref Tolerance tolerance)
        {
            return StringsEqual(x, y);
        }

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

            Type xType = x.GetType();
            Type yType = y.GetType();

            Type xGenericTypeDefinition = xType.IsGenericType ? xType.GetGenericTypeDefinition() : null;
            Type yGenericTypeDefinition = yType.IsGenericType ? yType.GetGenericTypeDefinition() : null;

            EqualityAdapter externalComparer = GetExternalComparer(x, y);
            if (externalComparer != null)
                return externalComparer.AreEqual(x, y);

            if (xType.IsArray && yType.IsArray && !compareAsCollection)
                return ArraysEqual((Array)x, (Array)y, ref tolerance);

            if (x is IDictionary && y is IDictionary)
                return DictionariesEqual((IDictionary)x, (IDictionary)y, ref tolerance);
            
            // Issue #70 - EquivalentTo isn't compatible with IgnoreCase for dictionaries
            if (x is DictionaryEntry && y is DictionaryEntry)
                return DictionaryEntriesEqual((DictionaryEntry)x, (DictionaryEntry)y, ref tolerance);

            // IDictionary<,> will eventually try to compare it's key value pairs when using CollectionTally
            if (xGenericTypeDefinition == typeof(KeyValuePair<,>) &&
                yGenericTypeDefinition == typeof(KeyValuePair<,>))
            {
                var keyTolerance = Tolerance.Exact;
                object xKey = xType.GetProperty("Key").GetValue(x, null);
                object yKey = yType.GetProperty("Key").GetValue(y, null);
                object xValue = xType.GetProperty("Value").GetValue(x, null);
                object yValue = yType.GetProperty("Value").GetValue(y, null);

                return AreEqual(xKey, yKey, ref keyTolerance) && AreEqual(xValue, yValue, ref tolerance);
            }

            //if (x is ICollection && y is ICollection)
            //    return CollectionsEqual((ICollection)x, (ICollection)y, ref tolerance);

            if (x is string && y is string)
                return StringsEqual((string)x, (string)y);

            if (x is IEnumerable && y is IEnumerable)
                return EnumerablesEqual((IEnumerable)x, (IEnumerable)y, ref tolerance);

            if (x is Stream && y is Stream)
                return StreamsEqual((Stream)x, (Stream)y);

            if ( x is char && y is char )
                return CharsEqual( (char)x, (char)y );

#if !PORTABLE
            if (x is DirectoryInfo && y is DirectoryInfo)
                return DirectoriesEqual((DirectoryInfo)x, (DirectoryInfo)y);
#endif

            if (Numerics.IsNumericType(x) && Numerics.IsNumericType(y))
                return Numerics.AreEqual(x, y, ref tolerance);

            if (tolerance != null && tolerance.Value is TimeSpan)
            {
                TimeSpan amount = (TimeSpan)tolerance.Value;

                if (x is DateTime && y is DateTime)
                    return ((DateTime)x - (DateTime)y).Duration() <= amount;

#if !NETCF
                if (x is DateTimeOffset && y is DateTimeOffset)
                    return ((DateTimeOffset)x - (DateTimeOffset)y).Duration() <= amount;
#endif

                if (x is TimeSpan && y is TimeSpan)
                    return ((TimeSpan)x - (TimeSpan)y).Duration() <= amount;
            }

            if (FirstImplementsIEquatableOfSecond(xType, yType))
                return InvokeFirstIEquatableEqualsSecond(x, y);
            else if (xType != yType && FirstImplementsIEquatableOfSecond(yType, xType))
                return InvokeFirstIEquatableEqualsSecond(y, x);
            
            return x.Equals(y);
        }

        private static bool FirstImplementsIEquatableOfSecond(Type first, Type second)
        {
            foreach (var xEquatableArgument in GetEquatableGenericArguments(first))
                if (xEquatableArgument.Equals(second))
                    return true;

            return false;
        }

        private static IList<Type> GetEquatableGenericArguments(Type type)
        {
            // NOTE: Original implementation used Array.ConvertAll and
            // Array.FindAll, which don't exist in the compact framework.
            var genericArgs = new List<Type>();

            foreach (Type @interface in type.GetInterfaces())
            {
                if (@interface.IsGenericType && @interface.GetGenericTypeDefinition().Equals(typeof(IEquatable<>)))
                {
                    genericArgs.Add(@interface.GetGenericArguments()[0]);
                }
            }

            return genericArgs;
        }

        private static bool InvokeFirstIEquatableEqualsSecond(object first, object second)
        {
            MethodInfo equals = typeof(IEquatable<>).MakeGenericType(second.GetType()).GetMethod("Equals");

            return (bool)equals.Invoke(first, new object[] { second });
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

        /// <summary>
        /// Helper method to compare two arrays
        /// </summary>
        private bool ArraysEqual(Array x, Array y, ref Tolerance tolerance)
        {
            int rank = x.Rank;

            if (rank != y.Rank)
                return false;

            for (int r = 1; r < rank; r++)
                if (x.GetLength(r) != y.GetLength(r))
                    return false;

            return EnumerablesEqual((IEnumerable)x, (IEnumerable)y, ref tolerance);
        }

        private bool DictionariesEqual(IDictionary x, IDictionary y, ref Tolerance tolerance)
        {
            if (x.Count != y.Count)
                return false;
 
            CollectionTally tally = new CollectionTally(this, x.Keys);
            if (!tally.TryRemove(y.Keys) || tally.Count > 0)
                return false;

            foreach (object key in x.Keys)
                if (!AreEqual(x[key], y[key], ref tolerance))
                    return false;
 
            return true;
        }

        private bool DictionaryEntriesEqual(DictionaryEntry x, DictionaryEntry y, ref Tolerance tolerance)
        {
            var keyTolerance = Tolerance.Exact;
            return AreEqual(x.Key, y.Key, ref keyTolerance) && AreEqual(x.Value, y.Value, ref tolerance);
        }

        private bool CollectionsEqual(ICollection x, ICollection y, ref Tolerance tolerance)
        {
            IEnumerator expectedEnum = x.GetEnumerator();
            IEnumerator actualEnum = y.GetEnumerator();

            int count;
            for (count = 0; ; count++)
            {
                bool expectedHasData = expectedEnum.MoveNext();
                bool actualHasData = actualEnum.MoveNext();

                if (!expectedHasData && !actualHasData)
                    return true;

                if (expectedHasData != actualHasData ||
                    !AreEqual(expectedEnum.Current, actualEnum.Current, ref tolerance))
                {
                    FailurePoint fp = new FailurePoint();
                    fp.Position = count;
                    fp.ExpectedHasData = expectedHasData;
                    if (expectedHasData)
                        fp.ExpectedValue = expectedEnum.Current;
                    fp.ActualHasData = actualHasData;
                    if (actualHasData)
                        fp.ActualValue = actualEnum.Current;
                    failurePoints.Insert(0, fp);
                    return false;
                }
            }
        }

        private bool StringsEqual(string x, string y)
        {
            string s1 = caseInsensitive ? x.ToLower() : x;
            string s2 = caseInsensitive ? y.ToLower() : y;

            return s1.Equals(s2);
        }

        private bool CharsEqual(char x, char y)
        {
            char c1 = caseInsensitive ? Char.ToLower(x) : x;
            char c2 = caseInsensitive ? Char.ToLower(y) : y;

            return c1 == c2;
        }

        private bool EnumerablesEqual(IEnumerable x, IEnumerable y, ref Tolerance tolerance)
        {
            IEnumerator expectedEnum = x.GetEnumerator();
            IEnumerator actualEnum = y.GetEnumerator();

            int count;
            for (count = 0; ; count++)
            {
                bool expectedHasData = expectedEnum.MoveNext();
                bool actualHasData = actualEnum.MoveNext();

                if (!expectedHasData && !actualHasData)
                    return true;

                if (expectedHasData != actualHasData ||
                    !AreEqual(expectedEnum.Current, actualEnum.Current, ref tolerance))
                {
                    FailurePoint fp = new FailurePoint();
                    fp.Position = count;
                    fp.ExpectedHasData = expectedHasData;
                    if (expectedHasData)
                        fp.ExpectedValue = expectedEnum.Current;
                    fp.ActualHasData = actualHasData;
                    if (actualHasData)
                        fp.ActualValue = actualEnum.Current;
                    failurePoints.Insert(0, fp);
                    return false;
                }
            }
        }

#if !PORTABLE
        /// <summary>
        /// Method to compare two DirectoryInfo objects
        /// </summary>
        /// <param name="x">first directory to compare</param>
        /// <param name="y">second directory to compare</param>
        /// <returns>true if equivalent, false if not</returns>
        private static bool DirectoriesEqual(DirectoryInfo x, DirectoryInfo y)
        {
            // Do quick compares first
            if (x.Attributes != y.Attributes ||
                x.CreationTime != y.CreationTime ||
                x.LastAccessTime != y.LastAccessTime)
            {
                return false;
            }

            // TODO: Find a cleaner way to do this
            return new SamePathConstraint(x.FullName).ApplyTo(y.FullName).IsSuccess;
        }
#endif

        private bool StreamsEqual(Stream x, Stream y)
        {
            if (x == y) return true;

            if (!x.CanRead)
                throw new ArgumentException("Stream is not readable", "expected");
            if (!y.CanRead)
                throw new ArgumentException("Stream is not readable", "actual");
            if (!x.CanSeek)
                throw new ArgumentException("Stream is not seekable", "expected");
            if (!y.CanSeek)
                throw new ArgumentException("Stream is not seekable", "actual");

            if (x.Length != y.Length) return false;

            byte[] bufferExpected = new byte[BUFFER_SIZE];
            byte[] bufferActual = new byte[BUFFER_SIZE];

            BinaryReader binaryReaderExpected = new BinaryReader(x);
            BinaryReader binaryReaderActual = new BinaryReader(y);

            long expectedPosition = x.Position;
            long actualPosition = y.Position;

            try
            {
                binaryReaderExpected.BaseStream.Seek(0, SeekOrigin.Begin);
                binaryReaderActual.BaseStream.Seek(0, SeekOrigin.Begin);

                for (long readByte = 0; readByte < x.Length; readByte += BUFFER_SIZE)
                {
                    binaryReaderExpected.Read(bufferExpected, 0, BUFFER_SIZE);
                    binaryReaderActual.Read(bufferActual, 0, BUFFER_SIZE);

                    for (int count = 0; count < BUFFER_SIZE; ++count)
                    {
                        if (bufferExpected[count] != bufferActual[count])
                        {
                            FailurePoint fp = new FailurePoint();
                            fp.Position = readByte + count;
                            fp.ExpectedHasData = true;
                            fp.ExpectedValue = bufferExpected[count];
                            fp.ActualHasData = true;
                            fp.ActualValue = bufferActual[count];
                            failurePoints.Insert(0, fp);
                            return false;
                        }
                    }
                }
            }
            finally
            {
                x.Position = expectedPosition;
                y.Position = actualPosition;
            }

            return true;
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