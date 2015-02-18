// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// The EqualConstraintResult class is tailored for formatting
    /// and displaying the result of an EqualConstraint. 
    /// </summary>
    public class EqualConstraintResult : ConstraintResult
    {
        private object expectedValue;
        private Tolerance tolerance;
        private bool caseInsensitive;
        private bool clipStrings;
        private IList<NUnitEqualityComparer.FailurePoint> failurePoints;

        #region Message Strings
        private static readonly string StringsDiffer_1 =
            "String lengths are both {0}. Strings differ at index {1}.";
        private static readonly string StringsDiffer_2 =
            "Expected string length {0} but was {1}. Strings differ at index {2}.";
        private static readonly string StreamsDiffer_1 =
            "Stream lengths are both {0}. Streams differ at offset {1}.";
        private static readonly string StreamsDiffer_2 =
            "Expected Stream length {0} but was {1}.";// Streams differ at offset {2}.";
        private static readonly string CollectionType_1 =
            "Expected and actual are both {0}";
        private static readonly string CollectionType_2 =
            "Expected is {0}, actual is {1}";
        private static readonly string ValuesDiffer_1 =
            "Values differ at index {0}";
        private static readonly string ValuesDiffer_2 =
            "Values differ at expected index {0}, actual index {1}";
        #endregion

        /// <summary>
        /// Construct an EqualConstraintResult
        /// </summary>
        public EqualConstraintResult(EqualConstraint constraint, object actual, bool hasSucceeded)
            : base(constraint, actual, hasSucceeded) 
        {
            this.expectedValue = constraint.Arguments[0];
            this.tolerance = constraint.Tolerance;
            this.caseInsensitive = constraint.CaseInsensitive;
            this.clipStrings = constraint.ClipStrings;
            this.failurePoints = constraint.FailurePoints;
        }

        /// <summary>
        /// Write a failure message. Overridden to provide custom 
        /// failure messages for EqualConstraint.
        /// </summary>
        /// <param name="writer">The MessageWriter to write to</param>
        public override void WriteMessageTo(MessageWriter writer)
        {
            DisplayDifferences(writer, expectedValue, ActualValue, 0);
        }

        private void DisplayDifferences(MessageWriter writer, object expected, object actual, int depth)
        {
            if (expected is string && actual is string)
                DisplayStringDifferences(writer, (string)expected, (string)actual);
            else if (expected is ICollection && actual is ICollection)
                DisplayCollectionDifferences(writer, (ICollection)expected, (ICollection)actual, depth);
            else if (expected is IEnumerable && actual is IEnumerable)
                DisplayEnumerableDifferences(writer, (IEnumerable)expected, (IEnumerable)actual, depth);
            else if (expected is Stream && actual is Stream)
                DisplayStreamDifferences(writer, (Stream)expected, (Stream)actual, depth);
            else if (tolerance != null)
                writer.DisplayDifferences(expected, actual, tolerance);
            else
                writer.DisplayDifferences(expected, actual);
        }

        #region DisplayStringDifferences
        private void DisplayStringDifferences(MessageWriter writer, string expected, string actual)
        {
            int mismatch = MsgUtils.FindMismatchPosition(expected, actual, 0, caseInsensitive);

            if (expected.Length == actual.Length)
                writer.WriteMessageLine(StringsDiffer_1, expected.Length, mismatch);
            else
                writer.WriteMessageLine(StringsDiffer_2, expected.Length, actual.Length, mismatch);

            writer.DisplayStringDifferences(expected, actual, mismatch, caseInsensitive, clipStrings);
        }
        #endregion

        #region DisplayStreamDifferences
        private void DisplayStreamDifferences(MessageWriter writer, Stream expected, Stream actual, int depth)
        {
            if (expected.Length == actual.Length)
            {
                long offset = failurePoints[depth].Position;
                writer.WriteMessageLine(StreamsDiffer_1, expected.Length, offset);
            }
            else
                writer.WriteMessageLine(StreamsDiffer_2, expected.Length, actual.Length);
        }
        #endregion

        #region DisplayCollectionDifferences
        /// <summary>
        /// Display the failure information for two collections that did not match.
        /// </summary>
        /// <param name="writer">The MessageWriter on which to display</param>
        /// <param name="expected">The expected collection.</param>
        /// <param name="actual">The actual collection</param>
        /// <param name="depth">The depth of this failure in a set of nested collections</param>
        private void DisplayCollectionDifferences(MessageWriter writer, ICollection expected, ICollection actual, int depth)
        {
            DisplayTypesAndSizes(writer, expected, actual, depth);

            if (failurePoints.Count > depth)
            {
                NUnitEqualityComparer.FailurePoint failurePoint = failurePoints[depth];

                DisplayFailurePoint(writer, expected, actual, failurePoint, depth);

                if (failurePoint.ExpectedHasData && failurePoint.ActualHasData)
                    DisplayDifferences(
                        writer,
                        failurePoint.ExpectedValue,
                        failurePoint.ActualValue,
                        ++depth);
                else if (failurePoint.ActualHasData)
                {
                    writer.Write("  Extra:    ");
                    writer.WriteCollectionElements(actual, failurePoint.Position, 3);
                }
                else
                {
                    writer.Write("  Missing:  ");
                    writer.WriteCollectionElements(expected, failurePoint.Position, 3);
                }
            }
        }

        /// <summary>
        /// Displays a single line showing the types and sizes of the expected
        /// and actual collections or arrays. If both are identical, the value is 
        /// only shown once.
        /// </summary>
        /// <param name="writer">The MessageWriter on which to display</param>
        /// <param name="expected">The expected collection or array</param>
        /// <param name="actual">The actual collection or array</param>
        /// <param name="indent">The indentation level for the message line</param>
        private void DisplayTypesAndSizes(MessageWriter writer, IEnumerable expected, IEnumerable actual, int indent)
        {
            string sExpected = MsgUtils.GetTypeRepresentation(expected);
            if (expected is ICollection && !(expected is Array))
                sExpected += string.Format(" with {0} elements", ((ICollection)expected).Count);

            string sActual = MsgUtils.GetTypeRepresentation(actual);
            if (actual is ICollection && !(actual is Array))
                sActual += string.Format(" with {0} elements", ((ICollection)actual).Count);

            if (sExpected == sActual)
                writer.WriteMessageLine(indent, CollectionType_1, sExpected);
            else
                writer.WriteMessageLine(indent, CollectionType_2, sExpected, sActual);
        }

        /// <summary>
        /// Displays a single line showing the point in the expected and actual
        /// arrays at which the comparison failed. If the arrays have different
        /// structures or dimensions, both _values are shown.
        /// </summary>
        /// <param name="writer">The MessageWriter on which to display</param>
        /// <param name="expected">The expected array</param>
        /// <param name="actual">The actual array</param>
        /// <param name="failurePoint">Index of the failure point in the underlying collections</param>
        /// <param name="indent">The indentation level for the message line</param>
        private void DisplayFailurePoint(MessageWriter writer, IEnumerable expected, IEnumerable actual, NUnitEqualityComparer.FailurePoint failurePoint, int indent)
        {
            Array expectedArray = expected as Array;
            Array actualArray = actual as Array;

            int expectedRank = expectedArray != null ? expectedArray.Rank : 1;
            int actualRank = actualArray != null ? actualArray.Rank : 1;

            bool useOneIndex = expectedRank == actualRank;

            if (expectedArray != null && actualArray != null)
                for (int r = 1; r < expectedRank && useOneIndex; r++)
                    if (expectedArray.GetLength(r) != actualArray.GetLength(r))
                        useOneIndex = false;

            int[] expectedIndices = MsgUtils.GetArrayIndicesFromCollectionIndex(expected, failurePoint.Position);
            if (useOneIndex)
            {
                writer.WriteMessageLine(indent, ValuesDiffer_1, MsgUtils.GetArrayIndicesAsString(expectedIndices));
            }
            else
            {
                int[] actualIndices = MsgUtils.GetArrayIndicesFromCollectionIndex(actual, failurePoint.Position);
                writer.WriteMessageLine(indent, ValuesDiffer_2,
                    MsgUtils.GetArrayIndicesAsString(expectedIndices), MsgUtils.GetArrayIndicesAsString(actualIndices));
            }
        }

        private static object GetValueFromCollection(ICollection collection, int index)
        {
            Array array = collection as Array;

            if (array != null && array.Rank > 1)
                return array.GetValue(MsgUtils.GetArrayIndicesFromCollectionIndex(array, index));

            if (collection is IList)
                return ((IList)collection)[index];

            foreach (object obj in collection)
                if (--index < 0)
                    return obj;

            return null;
        }
        #endregion

        #region DisplayEnumerableDifferences

        /// <summary>
        /// Display the failure information for two IEnumerables that did not match.
        /// </summary>
        /// <param name="writer">The MessageWriter on which to display</param>
        /// <param name="expected">The expected enumeration.</param>
        /// <param name="actual">The actual enumeration</param>
        /// <param name="depth">The depth of this failure in a set of nested collections</param>
        private void DisplayEnumerableDifferences(MessageWriter writer, IEnumerable expected, IEnumerable actual, int depth)
        {
            DisplayTypesAndSizes(writer, expected, actual, depth);

            if (failurePoints.Count > depth)
            {
                NUnitEqualityComparer.FailurePoint failurePoint = failurePoints[depth];

                DisplayFailurePoint(writer, expected, actual, failurePoint, depth);

                if (failurePoint.ExpectedHasData && failurePoint.ActualHasData)
                    DisplayDifferences(
                        writer,
                        failurePoint.ExpectedValue,
                        failurePoint.ActualValue,
                        ++depth);
                //else if (failurePoint.ActualHasData)
                //{
                //    writer.Write("  Extra:    ");
                //    writer.WriteCollectionElements(actual, failurePoint.Position, 3);
                //}
                //else
                //{
                //    writer.Write("  Missing:  ");
                //    writer.WriteCollectionElements(expected, failurePoint.Position, 3);
                //}
            }
        }

        #endregion
    }
}