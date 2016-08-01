// ***********************************************************************
// Copyright (c) 2012 Charlie Poole
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
using System.Text;
using System.Collections;
using System.Globalization;
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
        /// Static string used when strings are clipped
        /// </summary>
        private const string ELLIPSIS = "...";

        /// <summary>
        /// Formatting strings used for expected and actual _values
        /// </summary>
        private static readonly string Fmt_Null = "null";
        private static readonly string Fmt_EmptyString = "<string.Empty>";
        private static readonly string Fmt_EmptyCollection = "<empty>";

        private static readonly string Fmt_String = "\"{0}\"";
        private static readonly string Fmt_Char = "'{0}'";
        private static readonly string Fmt_DateTime = "yyyy-MM-dd HH:mm:ss.fff";
#if !NETCF
		private static readonly string Fmt_DateTimeOffset = "yyyy-MM-dd HH:mm:ss.fffzzz";
#endif
		private static readonly string Fmt_ValueType = "{0}";
        private static readonly string Fmt_Default = "<{0}>";

        /// <summary>
        /// Current head of chain of value formatters. Public for testing.
        /// </summary>
        public static ValueFormatter DefaultValueFormatter { get; set; }

        static MsgUtils()
        {
            // Initialize formatter to default for values of indeterminate type.
            DefaultValueFormatter = val => string.Format(Fmt_Default, val);

            AddFormatter(next => val => val is ValueType ? string.Format(Fmt_ValueType, val) : next(val));

            AddFormatter(next => val => val is DateTime ? FormatDateTime((DateTime)val) : next(val));

#if !NETCF
			AddFormatter(next => val => val is DateTimeOffset ? FormatDateTimeOffset ((DateTimeOffset)val) : next (val));
#endif

			AddFormatter(next => val => val is decimal ? FormatDecimal((decimal)val) : next(val));

            AddFormatter(next => val => val is float ? FormatFloat((float)val) : next(val));

            AddFormatter(next => val => val is double ? FormatDouble((double)val) : next(val));

            AddFormatter(next => val => val is char ? string.Format(Fmt_Char, val) : next(val));

            AddFormatter(next => val => val is IEnumerable ? FormatCollection((IEnumerable)val, 0, 10) : next(val));

            AddFormatter(next => val => val is string ? FormatString((string)val) : next(val));

            AddFormatter(next => val => val.GetType().IsArray ? FormatArray((Array)val) : next(val));

#if NETCF
            AddFormatter(next => val =>
            {
                var vi = val as System.Reflection.MethodInfo;
                return (vi != null && vi.IsGenericMethodDefinition)
                        ? string.Format(Fmt_Default, vi.Name + "<>") 
                        : next(val);
            });
#endif
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
        public static string FormatValue(object val)
        {
            if (val == null)
                return Fmt_Null;

            var context = TestExecutionContext.CurrentContext;

            if (context != null)
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
        public static string FormatCollection(IEnumerable collection, long start, int max)
        {
            int count = 0;
            int index = 0;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            foreach (object obj in collection)
            {
                if (index++ >= start)
                {
                    if (++count > max)
                        break;
                    sb.Append(count == 1 ? "< " : ", ");
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

            for (int product = 1, r = rank; --r >= 0; )
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
                    if (startSegment) sb.Append("< ");
                }

                sb.Append(FormatValue(obj));

                ++count;

                bool nextSegment = false;
                for (int r = 0; r < rank; r++)
                {
                    nextSegment = nextSegment || count % products[r] == 0;
                    if (nextSegment) sb.Append(" >");
                }
            }

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
                return d.ToString();
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
                return f.ToString();
            else
            {
                string s = f.ToString("G9", CultureInfo.InvariantCulture);

                if (s.IndexOf('.') > 0)
                    return s + "f";
                else
                    return s + ".0f";
            }
        }

        private static string FormatDecimal(Decimal d)
        {
            return d.ToString("G29", CultureInfo.InvariantCulture) + "m";
        }

        private static string FormatDateTime(DateTime dt)
        {
            return dt.ToString(Fmt_DateTime, CultureInfo.InvariantCulture);
        }

#if !NETCF
		private static string FormatDateTimeOffset(DateTimeOffset dto)
        {
            return dto.ToString(Fmt_DateTimeOffset, CultureInfo.InvariantCulture);
        }
#endif

		/// <summary>
		/// Returns the representation of a type as used in NUnitLite.
		/// This is the same as Type.ToString() except for arrays,
		/// which are displayed with their declared sizes.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string GetTypeRepresentation(object obj)
        {
            Array array = obj as Array;
            if (array == null)
                return string.Format("<{0}>", obj.GetType());

            StringBuilder sb = new StringBuilder();
            Type elementType = array.GetType();
            int nest = 0;
            while (elementType.IsArray)
            {
                elementType = elementType.GetElementType();
                ++nest;
            }
            sb.Append(elementType.ToString());
            sb.Append('[');
            for (int r = 0; r < array.Rank; r++)
            {
                if (r > 0) sb.Append(',');
                sb.Append(array.GetLength(r));
            }
            sb.Append(']');

            while (--nest > 0)
                sb.Append("[]");

            return string.Format("<{0}>", sb.ToString());
        }
        /// <summary>
        /// Converts any control characters in a string 
        /// to their escaped representation.
        /// </summary>
        /// <param name="s">The string to be converted</param>
        /// <returns>The converted string</returns>
        public static string EscapeControlChars(string s)
        {
            if (s != null)
            {
                StringBuilder sb = new StringBuilder();

                foreach (char c in s)
                {
                    switch (c)
                    {
                        //case '\'':
                        //    sb.Append("\\\'");
                        //    break;
                        //case '\"':
                        //    sb.Append("\\\"");
                        //    break;
                        case '\\':
                            sb.Append("\\\\");
                            break;
                        case '\0':
                            sb.Append("\\0");
                            break;
                        case '\a':
                            sb.Append("\\a");
                            break;
                        case '\b':
                            sb.Append("\\b");
                            break;
                        case '\f':
                            sb.Append("\\f");
                            break;
                        case '\n':
                            sb.Append("\\n");
                            break;
                        case '\r':
                            sb.Append("\\r");
                            break;
                        case '\t':
                            sb.Append("\\t");
                            break;
                        case '\v':
                            sb.Append("\\v");
                            break;

                        case '\x0085':
                        case '\x2028':
                        case '\x2029':
                            sb.Append(string.Format("\\x{0:X4}", (int)c));
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
        /// Converts any null characters in a string 
        /// to their escaped representation.
        /// </summary>
        /// <param name="s">The string to be converted</param>
        /// <returns>The converted string</returns>
        public static string EscapeNullCharacters(string s)
        {
            if(s != null)
            {
                StringBuilder sb = new StringBuilder();

                foreach(char c in s)
                {
                    switch(c)
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
                if (r > 0) sb.Append(',');
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
            Array array = collection as Array;

            int rank = array == null ? 1 : array.Rank;
            int[] result = new int[rank];

            for (int r = rank; --r > 0; )
            {
                int l = array.GetLength(r);
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
        /// <param name="maxStringLength">The maximum permitted length of the result string</param>
        /// <param name="clipStart">The point at which to start clipping</param>
        /// <returns>The clipped string</returns>
        public static string ClipString(string s, int maxStringLength, int clipStart)
        {
            int clipLength = maxStringLength;
            StringBuilder sb = new StringBuilder();

            if (clipStart > 0)
            {
                clipLength -= ELLIPSIS.Length;
                sb.Append(ELLIPSIS);
            }

            if (s.Length - clipStart > clipLength)
            {
                clipLength -= ELLIPSIS.Length;
                sb.Append(s.Substring(clipStart, clipLength));
                sb.Append(ELLIPSIS);
            }
            else if (clipStart > 0)
                sb.Append(s.Substring(clipStart));
            else
                sb.Append(s);

            return sb.ToString();
        }

        /// <summary>
        /// Clip the expected and actual strings in a coordinated fashion, 
        /// so that they may be displayed together.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="maxDisplayLength"></param>
        /// <param name="mismatch"></param>
        public static void ClipExpectedAndActual(ref string expected, ref string actual, int maxDisplayLength, int mismatch)
        {
            // Case 1: Both strings fit on line
            int maxStringLength = Math.Max(expected.Length, actual.Length);
            if (maxStringLength <= maxDisplayLength)
                return;

            // Case 2: Assume that the tail of each string fits on line
            int clipLength = maxDisplayLength - ELLIPSIS.Length;
            int clipStart = maxStringLength - clipLength;

            // Case 3: If it doesn't, center the mismatch position
            if (clipStart > mismatch)
                clipStart = Math.Max(0, mismatch - clipLength / 2);

            expected = ClipString(expected, maxDisplayLength, clipStart);
            actual = ClipString(actual, maxDisplayLength, clipStart);
        }

        /// <summary>
        /// Shows the position two strings start to differ.  Comparison 
        /// starts at the start index.
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <param name="actual">The actual string</param>
        /// <param name="istart">The index in the strings at which comparison should start</param>
        /// <param name="ignoreCase">Boolean indicating whether case should be ignored</param>
        /// <returns>-1 if no mismatch found, or the index where mismatch found</returns>
        static public int FindMismatchPosition(string expected, string actual, int istart, bool ignoreCase)
        {
            int length = Math.Min(expected.Length, actual.Length);

            string s1 = ignoreCase ? expected.ToLower() : expected;
            string s2 = ignoreCase ? actual.ToLower() : actual;

            for (int i = istart; i < length; i++)
            {
                if (s1[i] != s2[i])
                    return i;
            }

            //
            // Strings have same content up to the length of the shorter string.
            // Mismatch occurs because string lengths are different, so show
            // that they start differing where the shortest string ends
            //
            if (expected.Length != actual.Length)
                return length;

            //
            // Same strings : We shouldn't get here
            //
            return -1;
        }
    }
}