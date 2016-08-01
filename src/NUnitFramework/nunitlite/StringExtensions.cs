// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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

#if NETCF
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace System
{
    public static class StringExtensions
    {
        public static string ToUpperInvariant(this string s)
        {
            return s.ToUpper(CultureInfo.InvariantCulture);
        }

        public static String[] Split(this string s, char[] separator, StringSplitOptions options)
        {
            return s.Split(separator, Int32.MaxValue, options);
        }

        public static String[] Split(this string s, char[] separator, int count, StringSplitOptions options)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "Count cannot be less than zero.");
            if ((options != StringSplitOptions.None) && (options != StringSplitOptions.RemoveEmptyEntries))
                throw new ArgumentException("Illegal enum value: " + options + ".");

            if (s.Length == 0 && (options & StringSplitOptions.RemoveEmptyEntries) != 0)
                return EmptyArray<string>.Value;

            if (count <= 1)
            {
                return count == 0 ?
                    EmptyArray<string>.Value :
                    new String[1] {
                        s
                    };
            }

            return s.SplitByCharacters(separator, count, options != 0);
        }

        public static String[] Split(this string s, string[] separator, StringSplitOptions options)
        {
            return s.Split(separator, Int32.MaxValue, options);
        }

        public static String[] Split(this string s, string[] separator, int count, StringSplitOptions options)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "Count cannot be less than zero.");
            if ((options != StringSplitOptions.None) && (options != StringSplitOptions.RemoveEmptyEntries))
                throw new ArgumentException("Illegal enum value: " + options + ".");

            if (count <= 1)
            {
                return count == 0 ?
                    EmptyArray<string>.Value :
                    new String[1] {
                        s
                    };
            }

            bool removeEmpty = (options & StringSplitOptions.RemoveEmptyEntries) != 0;

            if (separator == null || separator.Length == 0)
                return s.SplitByCharacters(null, count, removeEmpty);

            if (s.Length == 0 && removeEmpty)
                return EmptyArray<string>.Value;

            List<String> arr = new List<String>();

            int pos = 0;
            int matchCount = 0;
            while (pos < s.Length)
            {
                int matchIndex = -1;
                int matchPos = Int32.MaxValue;

                // Find the first position where any of the separators matches
                for (int i = 0; i < separator.Length; ++i)
                {
                    string sep = separator[i];
                    if (sep == null || sep.Length == 0)
                        continue;

                    int match = s.IndexOfOrdinalUnchecked(sep, pos, s.Length - pos);
                    if (match >= 0 && match < matchPos)
                    {
                        matchIndex = i;
                        matchPos = match;
                    }
                }

                if (matchIndex == -1)
                    break;

                if (!(matchPos == pos && removeEmpty))
                {
                    if (arr.Count == count - 1)
                        break;
                    arr.Add(s.Substring(pos, matchPos - pos));
                }

                pos = matchPos + separator[matchIndex].Length;

                matchCount++;
            }

            if (matchCount == 0)
                return new String[] { s };

            // string contained only separators
            if (removeEmpty && matchCount != 0 && pos == s.Length && arr.Count == 0)
                return EmptyArray<string>.Value;

            if (!(removeEmpty && pos == s.Length))
                arr.Add(s.Substring(pos));

            return arr.ToArray();
        }

        static readonly char[] WhiteChars = {
            (char) 0x9, (char) 0xA, (char) 0xB, (char) 0xC, (char) 0xD,
            (char) 0x85, (char) 0x1680, (char) 0x2028, (char) 0x2029,
            (char) 0x20, (char) 0xA0, (char) 0x2000, (char) 0x2001, (char) 0x2002, (char) 0x2003, (char) 0x2004,
            (char) 0x2005, (char) 0x2006, (char) 0x2007, (char) 0x2008, (char) 0x2009, (char) 0x200A, (char) 0x200B,
            (char) 0x3000, (char) 0xFEFF
        };

        unsafe static string[] SplitByCharacters(this string s, char[] sep, int count, bool removeEmpty)
        {
            if (sep == null || sep.Length == 0)
                sep = WhiteChars;

            int[] split_points = null;
            int total_points = 0;
            --count;

            if (sep == null || sep.Length == 0)
            {
                fixed(char* src = s)
                {
                    char* src_ptr = src;
                    int len = s.Length;

                    while (len > 0)
                    {
                        if (char.IsWhiteSpace(*src_ptr++))
                        {
                            if (split_points == null)
                                split_points = new int[8];
                            else if (split_points.Length == total_points)
                                Array.Resize(ref split_points, split_points.Length * 2);

                            split_points[total_points++] = s.Length - len;
                            if (total_points == count && !removeEmpty)
                                break;
                        }
                        --len;
                    }
                }
            }
            else
            {
                fixed(char* src = s)
                {
                    fixed(char* sep_src = sep)
                    {
                        char* src_ptr = src;
                        char* sep_ptr_end = sep_src + sep.Length;
                        int len = s.Length;
                        while (len > 0)
                        {
                            char* sep_ptr = sep_src;
                            do
                            {
                                if (*sep_ptr++ == *src_ptr)
                                {
                                    if (split_points == null)
                                        split_points = new int[8];
                                    else if (split_points.Length == total_points)
                                        Array.Resize(ref split_points, split_points.Length * 2);

                                    split_points[total_points++] = s.Length - len;
                                    if (total_points == count && !removeEmpty)
                                        len = 0;

                                    break;
                                }
                            } while (sep_ptr != sep_ptr_end);

                            ++src_ptr;
                            --len;
                        }
                    }
                }
            }

            if (total_points == 0)
                return new string[] { s };

            var res = new string[Math.Min(total_points, count) + 1];
            int prev_index = 0;
            int i = 0;
            if (!removeEmpty)
            {
                for (; i < total_points; ++i)
                {
                    var start = split_points[i];
                    res[i] = s.SubstringUnchecked(prev_index, start - prev_index);
                    prev_index = start + 1;
                }

                res[i] = s.SubstringUnchecked(prev_index, s.Length - prev_index);
            }
            else
            {
                int used = 0;
                int length;
                for (; i < total_points; ++i)
                {
                    var start = split_points[i];
                    length = start - prev_index;
                    if (length != 0)
                    {
                        if (used == count)
                            break;

                        res[used++] = s.SubstringUnchecked(prev_index, length);
                    }

                    prev_index = start + 1;
                }

                length = s.Length - prev_index;
                if (length != 0)
                    res[used++] = s.SubstringUnchecked(prev_index, length);

                if (used != res.Length)
                    Array.Resize(ref res, used);
            }

            return res;
        }

        internal static String SubstringUnchecked(this string s, int startIndex, int length)
        {
            return s.Substring(startIndex, length);
            /*
            if (length == 0)
                return String.Empty;

            string tmp = InternalAllocateStr (length);
            fixed (char* dest = tmp, src = s)
                {
                CharCopy (dest, src + startIndex, length);
                }
            return tmp;
            */
        }

        internal static unsafe int IndexOfOrdinalUnchecked(this string s, string value)
        {
            return s.IndexOfOrdinalUnchecked(value, 0, s.Length);
        }

        internal static unsafe int IndexOfOrdinalUnchecked(this string s, string value, int startIndex, int count)
        {
            int valueLen = value.Length;
            if (count < valueLen)
                return -1;

            if (valueLen <= 1)
            {
                if (valueLen == 1)
                    return s.IndexOfUnchecked(value[0], startIndex, count);
                return startIndex;
            }

            fixed(char* thisptr = s, valueptr = value)
            {
                char* ap = thisptr + startIndex;
                char* thisEnd = ap + count - valueLen + 1;
                while (ap != thisEnd)
                {
                    if (*ap == *valueptr)
                    {
                        for (int i = 1; i < valueLen; i++)
                        {
                            if (ap[i] != valueptr[i])
                                goto NextVal;
                        }
                        return (int)(ap - thisptr);
                    }
NextVal:
                    ap++;
                }
            }
            return -1;
        }

        internal static unsafe int IndexOfUnchecked(this string s, char value, int startIndex, int count)
        {
            // It helps JIT compiler to optimize comparison
            int value_32 = (int)value;

            fixed(char* start = s)
            {
                char* ptr = start + startIndex;
                char* end_ptr = ptr + (count >> 3 << 3);

                while (ptr != end_ptr)
                {
                    if (*ptr == value_32)
                        return (int)(ptr - start);
                    if (ptr[1] == value_32)
                        return (int)(ptr - start + 1);
                    if (ptr[2] == value_32)
                        return (int)(ptr - start + 2);
                    if (ptr[3] == value_32)
                        return (int)(ptr - start + 3);
                    if (ptr[4] == value_32)
                        return (int)(ptr - start + 4);
                    if (ptr[5] == value_32)
                        return (int)(ptr - start + 5);
                    if (ptr[6] == value_32)
                        return (int)(ptr - start + 6);
                    if (ptr[7] == value_32)
                        return (int)(ptr - start + 7);

                    ptr += 8;
                }

                end_ptr += count & 0x07;
                while (ptr != end_ptr)
                {
                    if (*ptr == value_32)
                        return (int)(ptr - start);

                    ptr++;
                }
                return -1;
            }
        }

    }
}

namespace System
{
    public static class EmptyArray<T>
    {
        public static readonly T[] Value = new T[0];
    }
}
#endif
