// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Custom value formatter function
    /// </summary>
    /// <param name="val">The value</param>
    /// <returns></returns>
    public delegate string ValueFormatter(object val);

    /// <summary>
    /// Custom value formatter factory function
    /// </summary>
    /// <param name="next">The next formatter function</param>
    /// <returns>ValueFormatter</returns>
    /// <remarks>If the given formatter is unable to handle a certain format, it must call the next formatter in the chain</remarks>
    public delegate ValueFormatter ValueFormatterFactory(ValueFormatter next);

    /// <summary>
    /// Static methods used in creating messages
    /// </summary>
    internal static class MsgUtils
    {
        /// <summary>
        /// Default amount of items used by <see cref="FormatCollection"/> method.
        /// </summary>
        internal const int DefaultMaxItems = 10;

        /// <summary>
        /// Static string used when strings are clipped
        /// </summary>
        private const string ELLIPSIS = "...";

        /// <summary>
        /// Formatting strings used for expected and actual values
        /// </summary>
        private static readonly string Fmt_Null = "null";

        private static readonly string Fmt_EmptyString = "<string.Empty>";
        private static readonly string Fmt_EmptyCollection = "<empty>";
        private static readonly string Fmt_String = "\"{0}\"";
        private static readonly string Fmt_Char = "'{0}'";
        private static readonly string Fmt_DateTime = "yyyy-MM-dd HH:mm:ss.FFFFFFF";
        private static readonly string Fmt_DateTimeOffset = "yyyy-MM-dd HH:mm:ss.FFFFFFFzzz";
        private static readonly string Fmt_ValueType = "{0}";
        private static readonly string Fmt_Default = "<{0}>";
        private static readonly string Fmt_ExceptionThrown = "<! {0} !>";

        /// <summary>
        /// Current head of chain of value formatters. Public for testing.
        /// </summary>
        public static ValueFormatter DefaultValueFormatter { get; set; }

        static MsgUtils()
        {
            // Initialize formatter to default for values of indeterminate type.
            DefaultValueFormatter = FormatValueWithoutThrowing;

            AddFormatter(next => val => TryFormatTuple(val, TypeHelper.IsTuple, GetValueFromTuple) ?? next(val));

            AddFormatter(next => val => val is ValueType ? string.Format(Fmt_ValueType, val) : next(val));

            AddFormatter(next => val => TryFormatKeyValuePair(val) ?? next(val));

            AddFormatter(next => val => TryFormatTuple(val, TypeHelper.IsValueTuple, GetValueFromValueTuple) ?? next(val));

            AddFormatter(next => val => val is DateTime value ? FormatDateTime(value) : next(val));

            AddFormatter(next => val => val is DateTimeOffset value ? FormatDateTimeOffset(value) : next(val));

            AddFormatter(next => val => val is decimal value ? FormatDecimal(value) : next(val));

            AddFormatter(next => val => val is float value ? FormatFloat(value) : next(val));

            AddFormatter(next => val => val is double value ? FormatDouble(value) : next(val));

            AddFormatter(next => val => val is char ? string.Format(Fmt_Char, val) : next(val));

            AddFormatter(next => val => val is IEnumerable value ? FormatCollection(value) : next(val));

            AddFormatter(next => val => val is string value ? FormatString(value) : next(val));

            AddFormatter(next => val => val is DictionaryEntry de ? FormatKeyValuePair(de.Key, de.Value) : next(val));

            AddFormatter(next => val => val is Array valArray ? FormatArray(valArray) : next(val));
        }

        /// <summary>
        /// Try to format the properties of an object as a string.
        /// </summary>
        /// <param name="val">The object to format.</param>
        /// <returns>Formatted string for all properties.</returns>
        public static string FormatValueProperties(object? val)
        {
            if (val is null)
                return "null";

            Type valueType = val.GetType();

            // If the type is a low level type, call our default formatter.
            if (valueType.IsPrimitive || valueType == typeof(string) || valueType == typeof(decimal))
                return FormatValue(val);

            // If the type implements its own ToString() method, call that.
            MethodInfo? toStringMethod = valueType.GetMethod(nameof(ToString), Type.EmptyTypes);
            if (toStringMethod?.DeclaringType == valueType)
                return FormatValueWithoutThrowing(val);

            PropertyInfo[] properties = valueType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            if (properties.Length == 0 || properties.Any(p => p.GetIndexParameters().Length > 0))
            {
                // We can't print if there are no properties.
                // We also can't deal with indexer properties as we don't know the range of valid values.
                return FormatValue(val);
            }

            var sb = new StringBuilder();
            sb.Append(valueType.Name);
            sb.Append(" { ");

            bool firstProperty = true;
            foreach (var property in properties)
            {
                if (!firstProperty)
                    sb.Append(", ");
                else
                    firstProperty = false;

                sb.Append(property.Name);
                sb.Append(" = ");
                sb.Append(FormatValue(property.GetValue(val, null)));
            }
            sb.Append(" }");
            return sb.ToString();
        }

#if NETFRAMEWORK
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
#endif
        private static string FormatValueWithoutThrowing(object? val)
        {
            string? asString;
            try
            {
                asString = val?.ToString();
            }
            catch (Exception ex)
            {
                return string.Format(Fmt_ExceptionThrown, $"{ex.GetType().Name} was thrown by {val!.GetType().Name}.ToString()");
            }

            return string.Format(Fmt_Default, asString);
        }

        /// <summary>
        /// Add a formatter to the chain of responsibility.
        /// </summary>
        /// <param name="formatterFactory"></param>
        public static void AddFormatter(ValueFormatterFactory formatterFactory)
        {
            DefaultValueFormatter = formatterFactory(DefaultValueFormatter);
        }

        /// <summary>
        /// Formats text to represent a generalized value.
        /// </summary>
        /// <param name="val">The value</param>
        /// <returns>The formatted text</returns>
        public static string FormatValue(object? val)
        {
            if (val is null)
                return Fmt_Null;

            var context = TestExecutionContext.CurrentContext;

            if (context is not null)
                return context.CurrentValueFormatter(val);
            else
                return DefaultValueFormatter(val);
        }

        /// <summary>
        /// Formats text for a collection value,
        /// starting at a particular point, to a max length
        /// </summary>
        /// <param name="collection">The collection containing elements to write.</param>
        /// <param name="start">The starting point of the elements to write</param>
        /// <param name="max">The maximum number of elements to write</param>
        public static string FormatCollection(IEnumerable collection, long start = 0, int max = DefaultMaxItems)
        {
            int count = 0;
            int index = 0;
            StringBuilder sb = new StringBuilder();

            sb.Append("< ");

            if (start > 0)
                sb.Append("...");

            foreach (object? obj in collection)
            {
                if (index++ >= start)
                {
                    if (++count > max)
                        break;
                    if (count > 1)
                        sb.Append(", ");
                    sb.Append(FormatValue(obj));
                }
            }

            if (count == 0)
                return Fmt_EmptyCollection;

            if (count > max)
                sb.Append("...");

            sb.Append(" >");

            return sb.ToString();
        }

        private static string FormatArray(Array array)
        {
            if (array.Length == 0)
                return Fmt_EmptyCollection;

            int rank = array.Rank;
            int[] products = new int[rank];

            for (int product = 1, r = rank; --r >= 0;)
                products[r] = product *= array.GetLength(r);

            int count = 0;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (object obj in array)
            {
                if (count > 0)
                    sb.Append(", ");

                bool startSegment = false;
                for (int r = 0; r < rank; r++)
                {
                    startSegment = startSegment || count % products[r] == 0;
                    if (startSegment)
                        sb.Append("< ");
                }

                sb.Append(FormatValue(obj));

                ++count;

                bool nextSegment = false;
                for (int r = 0; r < rank; r++)
                {
                    nextSegment = nextSegment || count % products[r] == 0;
                    if (nextSegment)
                        sb.Append(" >");
                }
            }

            return sb.ToString();
        }

        private static string? TryFormatKeyValuePair(object? value)
        {
            if (value is null)
                return null;

            Type valueType = value.GetType();
            if (!valueType.IsGenericType)
                return null;

            Type baseValueType = valueType.GetGenericTypeDefinition();
            if (baseValueType != typeof(KeyValuePair<,>))
                return null;

            object? k = valueType.GetProperty("Key")?.GetValue(value, null);
            object? v = valueType.GetProperty("Value")?.GetValue(value, null);

            return FormatKeyValuePair(k, v);
        }

        private static string FormatKeyValuePair(object? key, object? value)
        {
            return $"[{FormatValue(key)}, {FormatValue(value)}]";
        }

        private static object? GetValueFromTuple(Type type, string propertyName, object obj)
        {
            return type.GetProperty(propertyName)?.GetValue(obj, null);
        }

        private static object? GetValueFromValueTuple(Type type, string propertyName, object obj)
        {
            return type.GetField(propertyName)?.GetValue(obj);
        }

        private static string? TryFormatTuple(object? value, Func<Type, bool> isTuple, Func<Type, string, object, object?> getValue)
        {
            if (value is null)
                return null;

            Type valueType = value.GetType();
            return !isTuple(valueType) ? null : FormatTuple(value, true, getValue);
        }

        private static string FormatTuple(object value, bool printParentheses, Func<Type, string, object, object?> getValue)
        {
            Type valueType = value.GetType();
            int numberOfGenericArgs = valueType.GetGenericArguments().Length;

            StringBuilder sb = new StringBuilder();
            if (printParentheses)
                sb.Append("(");

            for (int i = 0; i < numberOfGenericArgs; i++)
            {
                if (i > 0)
                    sb.Append(", ");

                bool notLastElement = i < 7;
                string propertyName = notLastElement ? "Item" + (i + 1) : "Rest";
                object? itemValue = getValue(valueType, propertyName, value);
                string formattedValue = notLastElement ? FormatValue(itemValue) : FormatTuple(itemValue!, false, getValue);
                sb.Append(formattedValue);
            }
            if (printParentheses)
                sb.Append(")");

            return sb.ToString();
        }

        private static string FormatString(string s)
        {
            return s == string.Empty
                ? Fmt_EmptyString
                : string.Format(Fmt_String, s);
        }

        private static string FormatDouble(double d)
        {
            if (double.IsNaN(d) || double.IsInfinity(d))
            {
                return d.ToString();
            }
            else
            {
                string s = d.ToString("G17", CultureInfo.InvariantCulture);

                if (s.IndexOf('.') > 0)
                    return s + "d";
                else
                    return s + ".0d";
            }
        }

        private static string FormatFloat(float f)
        {
            if (float.IsNaN(f) || float.IsInfinity(f))
            {
                return f.ToString();
            }
            else
            {
                string s = f.ToString("G9", CultureInfo.InvariantCulture);

                if (s.IndexOf('.') > 0)
                    return s + "f";
                else
                    return s + ".0f";
            }
        }

        private static string FormatDecimal(decimal d)
        {
            return d.ToString("G29", CultureInfo.InvariantCulture) + "m";
        }

        private static string FormatDateTime(DateTime dt)
        {
            return dt.ToString(Fmt_DateTime, CultureInfo.InvariantCulture);
        }

        private static string FormatDateTimeOffset(DateTimeOffset dto)
        {
            return dto.ToString(Fmt_DateTimeOffset, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns the representation of a type as used in NUnitLite.
        /// This is the same as Type.ToString() except for arrays,
        /// which are displayed with their declared sizes.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetTypeRepresentation(object obj)
        {
            if (!(obj is Array array))
                return $"<{obj.GetType()}>";

            StringBuilder sb = new StringBuilder();
            Type elementType = array.GetType();
            int nest = 0;
            while (elementType.IsArray)
            {
                elementType = elementType.GetElementType()!;
                ++nest;
            }
            sb.Append(elementType);
            sb.Append('[');
            for (int r = 0; r < array.Rank; r++)
            {
                if (r > 0)
                    sb.Append(',');
                sb.Append(array.GetLength(r));
            }
            sb.Append(']');

            while (--nest > 0)
                sb.Append("[]");

            return $"<{sb}>";
        }

        /// <summary>
        /// Converts any control characters in a string
        /// to their escaped representation.
        /// </summary>
        /// <param name="s">The string to be converted</param>
        /// <returns>The converted string</returns>
        [return: NotNullIfNotNull("s")]
        public static string? EscapeControlChars(string? s)
        {
            int index = 0;
            return EscapeControlChars(s, ref index);
        }

        /// <summary>
        /// Converts any control characters in a string
        /// to their escaped representation.
        /// </summary>
        /// <param name="s">The string to be converted</param>
        /// <param name="index">The index in the array of a specific spot, which needs to be updated when expanding.</param>
        /// <returns>The converted string</returns>
        [return: NotNullIfNotNull("s")]
        public static string? EscapeControlChars(string? s, ref int index)
        {
            if (s is null)
                return null;

            int originalIndex = index;
            const int headRoom = 42;
            StringBuilder sb = new(s.Length + headRoom);

            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                string? escaped = EscapeControlChars(c);
                if (escaped is null)
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append(escaped);
                    if (originalIndex > i)
                    {
                        index += escaped.Length - 1;
                    }
                }
            }

            return sb.ToString();
        }

        private static string? EscapeControlChars(char c)
        {
            switch (c)
            {
                //case '\'':
                //    return "\\\'";
                //case '\"':
                //    return ("\\\"");
                //    break;
                case '\\':
                    return "\\\\";
                case '\0':
                    return "\\0";
                case '\a':
                    return "\\a";
                case '\b':
                    return "\\b";
                case '\f':
                    return "\\f";
                case '\n':
                    return "\\n";
                case '\r':
                    return "\\r";
                case '\t':
                    return "\\t";
                case '\v':
                    return "\\v";

                case '\x0085':
                case '\x2028':
                case '\x2029':
                    return $"\\x{(int)c:X4}";

                default:
                    return null;
            }
        }

        /// <summary>
        /// Converts any null characters in a string
        /// to their escaped representation.
        /// </summary>
        /// <param name="s">The string to be converted</param>
        /// <returns>The converted string</returns>
        [return: NotNullIfNotNull("s")]
        public static string? EscapeNullCharacters(string? s)
        {
            if (s is not null)
            {
                const int headRoom = 42;
                StringBuilder sb = new(s.Length + headRoom);

                foreach (char c in s)
                {
                    switch (c)
                    {
                        case '\0':
                            sb.Append("\\0");
                            break;

                        default:
                            sb.Append(c);
                            break;
                    }
                }

                s = sb.ToString();
            }

            return s;
        }

        /// <summary>
        /// Return the a string representation for a set of indices into an array
        /// </summary>
        /// <param name="indices">Array of indices for which a string is needed</param>
        public static string GetArrayIndicesAsString(int[] indices)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            for (int r = 0; r < indices.Length; r++)
            {
                if (r > 0)
                    sb.Append(',');
                sb.Append(indices[r].ToString());
            }
            sb.Append(']');
            return sb.ToString();
        }

        /// <summary>
        /// Get an array of indices representing the point in a collection or
        /// array corresponding to a single int index into the collection.
        /// </summary>
        /// <param name="collection">The collection to which the indices apply</param>
        /// <param name="index">Index in the collection</param>
        /// <returns>Array of indices</returns>
        public static int[] GetArrayIndicesFromCollectionIndex(IEnumerable collection, long index)
        {
            Array? array = collection as Array;

            int rank = array?.Rank ?? 1;
            int[] result = new int[rank];

            for (int r = rank; --r > 0;)
            {
                int l = array!.GetLength(r);
                result[r] = (int)index % l;
                index /= l;
            }

            result[0] = (int)index;
            return result;
        }

        /// <summary>
        /// Clip a string to a given length, starting at a particular offset, returning the clipped
        /// string with ellipses representing the removed parts
        /// </summary>
        /// <param name="s">The string to be clipped</param>
        /// <param name="clipLength">The length of the clipped string</param>
        /// <param name="clipStart">The point at which to start clipping</param>
        /// <returns>The clipped string</returns>
        public static string ClipString(string s, int clipLength, int clipStart)
        {
            StringBuilder sb = new StringBuilder(s.Length + 2 * ELLIPSIS.Length);

            if (clipStart > 0)
                sb.Append(ELLIPSIS);

            int remainingLength = s.Length - clipStart;
            int count = Math.Min(remainingLength, clipLength);
            sb.Append(s, clipStart, count);

            if (remainingLength > clipLength)
                sb.Append(ELLIPSIS);

            return sb.ToString();
        }

        /// <summary>
        /// Clips the <paramref name="s"/> string if it exceeds <paramref name="maxDisplayLength"/>.
        /// </summary>
        /// <remarks>
        /// The string ensures that the content around <paramref name="mismatchLocation"/> stays visible
        /// by either clipping from the front or the back or both. The clipped part is replaced with "...".
        /// </remarks>
        /// <param name="s">The string to clip</param>
        /// <param name="length">The assumed length of the string (needed if called for a pair)</param>
        /// <param name="maxDisplayLength">The maximum length of the display message.</param>
        /// <param name="mismatchLocation">The location in <paramref name="s"/> that needs to stay visible.</param>
        /// <returns>Clip string with a maximum length of <paramref name="maxDisplayLength"/>.</returns>
        public static string ClipWhenNeeded(string s, int length, int maxDisplayLength, ref int mismatchLocation)
        {
            if (length <= maxDisplayLength)
            {
                // No need to clip
                return s;
            }

            // We need to clip at least one side.
            maxDisplayLength -= ELLIPSIS.Length;

            const int minimumJoiningMatchingCharacters = 5;

            int clipStart;

            if (mismatchLocation + minimumJoiningMatchingCharacters < maxDisplayLength)
            {
                // Clip the tail
                clipStart = 0;
            }
            else if (length - mismatchLocation + minimumJoiningMatchingCharacters < maxDisplayLength)
            {
                // Show the tail
                clipStart = length - maxDisplayLength;
            }
            else
            {
                // We need to clip both sides.
                maxDisplayLength -= ELLIPSIS.Length;

                // Centre the clip around the mismatchLocation
                clipStart = mismatchLocation - maxDisplayLength / 2;
            }

            if (clipStart > 0)
            {
                // If clipping off the front, adjust the location
                // and correct for the ... added to the front.
                mismatchLocation -= clipStart - ELLIPSIS.Length;
            }

            return ClipString(s, maxDisplayLength, clipStart);
        }

        /// <summary>
        /// Clip the expected and actual strings in a coordinated fashion,
        /// so that they may be displayed together.
        /// </summary>
        /// <remarks>
        /// The values of <paramref name="mismatchExpected"/> and <paramref name="mismatchActual"/>
        /// are assumed to be the same. If <paramref name="expected"/> and <paramref name="actual"/>
        /// are not linked, then call <see cref="ClipWhenNeeded"/> individually.
        /// </remarks>
        /// <param name="expected">The expected string to clip</param>
        /// <param name="actual">The actual string to clip</param>
        /// <param name="maxDisplayLength">The maximum length of the display message.</param>
        /// <param name="mismatchExpected">The location in <paramref name="expected"/> that needs to stay visible.</param>
        /// <param name="mismatchActual">The location in <paramref name="actual"/> that needs to stay visible.</param>
        public static void ClipExpectedAndActual(ref string expected, ref string actual, int maxDisplayLength, ref int mismatchExpected, ref int mismatchActual)
        {
            if (mismatchExpected != mismatchActual)
            {
                throw new ArgumentException($"The values for {nameof(mismatchExpected)} and {nameof(mismatchActual)} should be the same.");
            }

            // Clip based upon longest length
            int longestLength = Math.Max(expected.Length, actual.Length);
            if (longestLength <= maxDisplayLength)
                return;

            expected = ClipWhenNeeded(expected, longestLength, maxDisplayLength, ref mismatchExpected);
            actual = ClipWhenNeeded(actual, longestLength, maxDisplayLength, ref mismatchActual);
        }

        /// <summary>
        /// Finds the position two strings start to differ.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The actual string</param>
        /// <param name="ignoreCase">Boolean indicating whether case should be ignored</param>
        /// <param name="ignoreWhiteSpace">Boolean indicating whether white space should be ignored</param>
        /// <param name="ignoreLineEndingFormat">Boolean indicating whether line ending format should be ignored</param>
        /// <returns>(-1,-1) if no mismatch found, or the indices (expected, actual) where mismatches found.</returns>
        public static (int, int) FindMismatchPosition(string expected, string actual, bool ignoreCase, bool ignoreWhiteSpace, bool ignoreLineEndingFormat)
        {
            string s1 = ignoreCase ? expected.ToLower() : expected;
            string s2 = ignoreCase ? actual.ToLower() : actual;
            int i1 = 0;
            int i2 = 0;

            while (true)
            {
                if (ignoreWhiteSpace)
                {
                    // Find next non-white space character in both s1 and s2.
                    i1 = FindNonWhiteSpace(s1, i1);
                    i2 = FindNonWhiteSpace(s2, i2);
                }

                if (i1 < s1.Length && i2 < s2.Length)
                {
                    char c1 = s1[i1];
                    char c2 = s2[i2];

                    if (ignoreLineEndingFormat)
                    {
                        bool isLineBreak1 = TryConsumeLineBreak(s1, i1, out int nextI1);
                        bool isLineBreak2 = TryConsumeLineBreak(s2, i2, out int nextI2);

                        if (isLineBreak1 && isLineBreak2)
                        {
                            i1 = nextI1;
                            i2 = nextI2;
                            continue;
                        }

                        if (isLineBreak1 || isLineBreak2)
                        {
                            return (i1, i2);
                        }
                    }

                    if (c1 != c2)
                        return (i1, i2);

                    i1++;
                    i2++;
                }
                else
                {
                    break;
                }
            }

            //
            // Strings have same content up to the length of the shorter string.
            // Mismatch occurs because string lengths are different, so show
            // that they start differing where the shortest string ends
            //
            if (i1 < s1.Length || i2 < s2.Length)
                return (i1, i2);

            //
            // Same strings : We shouldn't get here
            //
            return (-1, -1);
        }

        private static int FindNonWhiteSpace(string s, int i)
        {
            while (i < s.Length && char.IsWhiteSpace(s[i]))
                i++;

            return i;
        }

        private static bool TryConsumeLineBreak(string s, int index, out int nextIndex)
        {
            if (index >= s.Length)
            {
                nextIndex = index;
                return false;
            }

            char current = s[index];
            if (current == '\r')
            {
                if (index + 1 < s.Length && s[index + 1] == '\n')
                {
                    nextIndex = index + 2;
                    return true;
                }

                nextIndex = index + 1;
                return true;
            }

            if (current == '\n')
            {
                nextIndex = index + 1;
                return true;
            }

            nextIndex = index;
            return false;
        }
    }
}
