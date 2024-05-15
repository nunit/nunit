// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
        private readonly object? _expectedValue;
        private readonly Tolerance _tolerance;
        private readonly bool _caseInsensitive;
        private readonly bool _ignoringWhiteSpace;
        private readonly bool _comparingProperties;
        private readonly bool _clipStrings;
        private readonly IList<NUnitEqualityComparer.FailurePoint>? _failurePoints;

        #region Message Strings
        private static readonly string StringsDiffer_1 =
            "String lengths are both {0}. Strings differ at index {1}.";
        private static readonly string StringsDiffer_2 =
            "Expected string length {0} but was {1}. Strings differ at index {2}.";
        private static readonly string StreamsDiffer_1 =
            "Stream lengths are both {0}. Streams differ at offset {1}.";
        private static readonly string StreamsDiffer_2 =
            "Expected Stream length {0} but was {1}."; // Streams differ at offset {2}.";
        private static readonly string UnSeekableStreamsDiffer =
            "Streams differ at offset {0}.";
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
        public EqualConstraintResult(EqualConstraint constraint, object? actual, bool hasSucceeded)
            : base(constraint, actual, hasSucceeded)
        {
            _expectedValue = constraint.Arguments[0];
            _tolerance = constraint.Tolerance;
            _caseInsensitive = constraint.CaseInsensitive;
            _ignoringWhiteSpace = constraint.IgnoringWhiteSpace;
            _comparingProperties = constraint.ComparingProperties;
            _clipStrings = constraint.ClipStrings;
            _failurePoints = constraint.HasFailurePoints ? constraint.FailurePoints : null;
        }

        /// <summary>
        /// Write a failure message. Overridden to provide custom
        /// failure messages for EqualConstraint.
        /// </summary>
        /// <param name="writer">The MessageWriter to write to</param>
        public override void WriteMessageTo(MessageWriter writer)
        {
            DisplayDifferences(writer, _expectedValue, ActualValue, 0);
        }

        private void DisplayDifferences(MessageWriter writer, object? expected, object? actual, int depth)
        {
            if (expected is string expectedString && actual is string actualString)
                DisplayStringDifferences(writer, expectedString, actualString);
            else if (expected is ICollection expectedCollection && actual is ICollection actualCollection)
                DisplayCollectionDifferences(writer, expectedCollection, actualCollection, depth);
            else if (expected is IEnumerable expectedEnumerable && actual is IEnumerable actualEnumerable)
                DisplayEnumerableDifferences(writer, expectedEnumerable, actualEnumerable, depth);
            else if (expected is Stream expectedStream && actual is Stream actualStream)
                DisplayStreamDifferences(writer, expectedStream, actualStream, depth);
            else if (_comparingProperties && IsPropertyFailurePoint(depth))
                DisplayPropertyDifferences(writer, depth);
            else if (_tolerance is not null)
                writer.DisplayDifferences(expected, actual, _tolerance);
            else
                writer.DisplayDifferences(expected, actual);
        }

        #region DisplayStringDifferences
        private void DisplayStringDifferences(MessageWriter writer, string expected, string actual)
        {
            (int mismatchExpected, int mismatchActual) = MsgUtils.FindMismatchPosition(expected, actual, _caseInsensitive, _ignoringWhiteSpace);

            if (expected.Length == actual.Length)
                writer.WriteMessageLine(StringsDiffer_1, expected.Length, mismatchExpected);
            else
                writer.WriteMessageLine(StringsDiffer_2, expected.Length, actual.Length, mismatchExpected);

            writer.DisplayStringDifferences(expected, actual, mismatchExpected, mismatchActual, _caseInsensitive, _ignoringWhiteSpace, _clipStrings);
        }
        #endregion

        #region DisplayStreamDifferences
        private void DisplayStreamDifferences(MessageWriter writer, Stream expected, Stream actual, int depth)
        {
            long offset = _failurePoints is not null && _failurePoints.Count > depth ? _failurePoints[depth].Position : 0;

            if (expected.CanSeek && actual.CanSeek)
            {
                if (expected.Length == actual.Length)
                {
                    writer.WriteMessageLine(StreamsDiffer_1, expected.Length, offset);
                }
                else
                {
                    writer.WriteMessageLine(StreamsDiffer_2, expected.Length, actual.Length);
                }
            }
            else
            {
                writer.WriteMessageLine(UnSeekableStreamsDiffer, offset);
            }
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

            if (_failurePoints is not null && _failurePoints.Count > depth)
            {
                NUnitEqualityComparer.FailurePoint failurePoint = _failurePoints[depth];

                DisplayFailurePoint(writer, expected, actual, failurePoint, depth);

                if (failurePoint.ExpectedHasData && failurePoint.ActualHasData)
                {
                    DisplayCollectionDifferenceWithFailurePoint(writer, expected, actual, failurePoint, depth);
                }
                else if (failurePoint.ActualHasData)
                {
                    writer.Write("  Extra:    ");
                    writer.WriteCollectionElements(actual.Skip(failurePoint.Position), 0, 3);
                }
                else
                {
                    writer.Write("  Missing:  ");
                    writer.WriteCollectionElements(expected.Skip(failurePoint.Position), 0, 3);
                }
            }
        }

        /// <summary>
        /// Display the failure information for two collections with failure point
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="failurePoint"></param>
        /// <param name="depth"></param>
        private void DisplayCollectionDifferenceWithFailurePoint(MessageWriter writer, ICollection expected, ICollection actual, NUnitEqualityComparer.FailurePoint failurePoint, int depth)
        {
            if (failurePoint.ExpectedValue is string expectedString && failurePoint.ActualValue is string actualString)
            {
                (int mismatchExpected, int _) = MsgUtils.FindMismatchPosition(expectedString, actualString, _caseInsensitive, _ignoringWhiteSpace);

                if (expectedString.Length == actualString.Length)
                    writer.WriteMessageLine(StringsDiffer_1, expectedString.Length, mismatchExpected);
                else
                    writer.WriteMessageLine(StringsDiffer_2, expectedString.Length, actualString.Length, mismatchExpected);
                writer.WriteLine($"  Expected: {MsgUtils.FormatCollection(expected)}");
                writer.WriteLine($"  But was:  {MsgUtils.FormatCollection(actual)}");
                writer.WriteLine($"  First non-matching item at index [{failurePoint.Position}]: \"{failurePoint.ExpectedValue}\"");
                return;
            }
            else
            {
                DisplayDifferences(
                       writer,
                       failurePoint.ExpectedValue,
                       failurePoint.ActualValue,
                       ++depth);
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
            if (expected is ICollection expectedCollection && !(expected is Array))
                sExpected += $" with {expectedCollection.Count} elements";

            string sActual = MsgUtils.GetTypeRepresentation(actual);
            if (actual is ICollection actualCollection && !(actual is Array))
                sActual += $" with {actualCollection.Count} elements";

            if (sExpected == sActual)
                writer.WriteMessageLine(indent, CollectionType_1, sExpected);
            else
                writer.WriteMessageLine(indent, CollectionType_2, sExpected, sActual);
        }

        /// <summary>
        /// Displays a single line showing the point in the expected and actual
        /// arrays at which the comparison failed. If the arrays have different
        /// structures or dimensions, both values are shown.
        /// </summary>
        /// <param name="writer">The MessageWriter on which to display</param>
        /// <param name="expected">The expected array</param>
        /// <param name="actual">The actual array</param>
        /// <param name="failurePoint">Index of the failure point in the underlying collections</param>
        /// <param name="indent">The indentation level for the message line</param>
        private void DisplayFailurePoint(MessageWriter writer, IEnumerable expected, IEnumerable actual, NUnitEqualityComparer.FailurePoint failurePoint, int indent)
        {
            Array? expectedArray = expected as Array;
            Array? actualArray = actual as Array;

            int expectedRank = expectedArray?.Rank ?? 1;
            int actualRank = actualArray?.Rank ?? 1;

            bool useOneIndex = expectedRank == actualRank;

            if (expectedArray is not null && actualArray is not null)
            {
                for (int r = 1; r < expectedRank && useOneIndex; r++)
                {
                    if (expectedArray.GetLength(r) != actualArray.GetLength(r))
                        useOneIndex = false;
                }
            }

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

            if (_failurePoints is not null && _failurePoints.Count > depth)
            {
                NUnitEqualityComparer.FailurePoint failurePoint = _failurePoints[depth];

                DisplayFailurePoint(writer, expected, actual, failurePoint, depth);

                if (failurePoint.ExpectedHasData && failurePoint.ActualHasData)
                {
                    DisplayDifferences(
                        writer,
                        failurePoint.ExpectedValue,
                        failurePoint.ActualValue,
                        ++depth);
                }
                else if (failurePoint.ActualHasData)
                {
                    writer.Write($"  Extra:    < {MsgUtils.FormatValue(failurePoint.ActualValue)}, ... >");
                }
                else
                {
                    writer.Write($"  Missing:  < {MsgUtils.FormatValue(failurePoint.ExpectedValue)}, ... >");
                }
            }
        }

        #endregion

        #region DisplayPropertyDifferences

        private void DisplayPropertyDifferences(MessageWriter writer, int depth)
        {
            if (_failurePoints is not null && _failurePoints.Count > depth)
            {
                NUnitEqualityComparer.FailurePoint failurePoint = _failurePoints[depth];

                writer.WriteMessageLine($"Values differ at property {failurePoint.PropertyName}");
                DisplayDifferences(writer, failurePoint.ExpectedValue, failurePoint.ActualValue, ++depth);
            }
        }

        private bool IsPropertyFailurePoint(int depth)
        {
            return _failurePoints is not null &&
                   _failurePoints.Count > depth &&
                   _failurePoints[depth].PropertyName is not null;
        }

        #endregion
    }
}
