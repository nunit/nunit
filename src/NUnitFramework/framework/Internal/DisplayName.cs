// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
#if !NETFRAMEWORK
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
#endif
using System.Text;

namespace NUnit.Framework.Internal
{
    internal static class DisplayName
    {
        private const string THREE_DOTS = "...";

        public static string GetValueString(object? arg, int stringMax)
        {
            string display = arg is null
                ? "null"
                : Convert.ToString(arg, System.Globalization.CultureInfo.InvariantCulture)!;

            if (arg is Array { Rank: 1 } argArray)
            {
                if (argArray.Length == 0)
                {
                    display = "[]";
                }
                else
                {
                    var builder = new StringBuilder();
                    builder.Append('[');

                    const int maxNumItemsToEnumerate = 5;

                    var numItemsToEnumerate = Math.Min(argArray.Length, maxNumItemsToEnumerate);
                    for (int i = 0; i < numItemsToEnumerate; i++)
                    {
                        if (i > 0)
                            builder.Append(", ");

                        var element = argArray.GetValue(i);

                        if (element is Array { Rank: 1 } childArray)
                        {
                            builder.Append(childArray.GetType().GetElementType()!.Name);
                            builder.Append("[]");
                        }
                        else
                        {
                            var elementDisplayString = GetValueString(element, stringMax);
                            builder.Append(elementDisplayString);
                        }
                    }

                    if (argArray.Length > maxNumItemsToEnumerate)
                        builder.Append($", {THREE_DOTS}");

                    builder.Append(']');
                    display = builder.ToString();
                }
            }
#if !NETFRAMEWORK
            else if (arg is not null && TryFormatNumeric(arg, out var displayValue))
            {
                display = displayValue;
            }
#else
            else if (arg is double dbl)
            {
                if (double.IsNaN(dbl))
                {
                    display = "double.NaN";
                }
                else if (double.IsPositiveInfinity(dbl))
                {
                    display = "double.PositiveInfinity";
                }
                else if (double.IsNegativeInfinity(dbl))
                {
                    display = "double.NegativeInfinity";
                }
                else if (dbl == double.MaxValue)
                {
                    display = "double.MaxValue";
                }
                else if (dbl == double.MinValue)
                {
                    display = "double.MinValue";
                }
                else
                {
                    if (display.IndexOf('.') == -1)
                        display += ".0";
                    display += "d";
                }
            }
            else if (arg is float f)
            {
                if (float.IsNaN(f))
                {
                    display = "float.NaN";
                }
                else if (float.IsPositiveInfinity(f))
                {
                    display = "float.PositiveInfinity";
                }
                else if (float.IsNegativeInfinity(f))
                {
                    display = "float.NegativeInfinity";
                }
                else if (f == float.MaxValue)
                {
                    display = "float.MaxValue";
                }
                else if (f == float.MinValue)
                {
                    display = "float.MinValue";
                }
                else
                {
                    if (display.IndexOf('.') == -1)
                        display += ".0";
                    display += "f";
                }
            }
            else if (arg is decimal dec)
            {
                if (dec == decimal.MinValue)
                    display = "decimal.MinValue";
                else if (dec == decimal.MaxValue)
                    display = "decimal.MaxValue";
                else
                    display += "m";
            }
            else if (arg is long l)
            {
                if (l.Equals(long.MinValue))
                    display = "long.MinValue";
                else if (l.Equals(long.MaxValue))
                    display = "long.MaxValue";
                else
                    display += "L";
            }
            else if (arg is ulong ul)
            {
                if (ul == ulong.MinValue)
                    display = "ulong.MinValue";
                else if (ul == ulong.MaxValue)
                    display = "ulong.MaxValue";
                else
                    display += "UL";
            }
            else if (arg is int i)
            {
                if (i.Equals(int.MaxValue))
                    display = "int.MaxValue";
                else if (i.Equals(int.MinValue))
                    display = "int.MinValue";
            }
            else if (arg is uint ui)
            {
                if (ui.Equals(uint.MaxValue))
                    display = "uint.MaxValue";
                else if (ui.Equals(uint.MinValue))
                    display = "uint.MinValue";
            }
            else if (arg is nint ni)
            {
                if (ni.Equals(nint.MaxValue))
                    display = "nint.MaxValue";
                else if (ni.Equals(nint.MinValue))
                    display = "nint.MinValue";
            }
            else if (arg is nuint nui)
            {
                if (nui.Equals(nuint.MaxValue))
                    display = "nuint.MaxValue";
                else if (nui.Equals(nuint.MinValue))
                    display = "nuint.MinValue";
            }
            else if (arg is short s)
            {
                if (s.Equals(short.MaxValue))
                    display = "short.MaxValue";
                else if (s.Equals(short.MinValue))
                    display = "short.MinValue";
            }
            else if (arg is ushort us)
            {
                if (us.Equals(ushort.MaxValue))
                    display = "ushort.MaxValue";
                else if (us.Equals(ushort.MinValue))
                    display = "ushort.MinValue";
            }
            else if (arg is byte b)
            {
                if (b.Equals(byte.MaxValue))
                    display = "byte.MaxValue";
                else if (b.Equals(byte.MinValue))
                    display = "byte.MinValue";
            }
            else if (arg is sbyte sb)
            {
                if (sb.Equals(sbyte.MaxValue))
                    display = "sbyte.MaxValue";
                else if (sb.Equals(sbyte.MinValue))
                    display = "sbyte.MinValue";
            }
#endif
            else if (arg is string str)
            {
                bool tooLong = stringMax > 0 && str.Length > stringMax;
                int limit = tooLong ? stringMax - THREE_DOTS.Length : 0;

                if (!tooLong && !MayNeedEscape(str))
                {
                    // common case, no need to process with string builder
                    display = "\"" + str + "\"";
                }
                else
                {
                    // cleanup
                    var builder = new StringBuilder();
                    builder.Append('\"');
                    foreach (char c in str)
                    {
                        builder.Append(EscapeCharInString(c));

                        if (tooLong && builder.Length > limit)
                        {
                            builder.Append(THREE_DOTS);
                            break;
                        }
                    }
                    builder.Append('\"');
                    display = builder.ToString();
                }
            }
            else if (arg is char c)
            {
                display = "\'" + EscapeSingleChar(c) + "\'";
            }
            else if (arg is Exception e)
            {
                display = FormatException(e);
            }

            return display;
        }

#if !NETFRAMEWORK
        private static bool TryFormatNumeric(object arg, [NotNullWhen(true)] out string? displayValue)
        {
            switch (arg)
            {
                case float f:
                    displayValue = DisplayFloat(f, "float", "f");
                    return true;
                case double dbl:
                    displayValue = DisplayFloat(dbl, "double", "d");
                    return true;
                case decimal dec:
                    displayValue = DisplayNumeric(dec, "decimal", "m");
                    return true;
                case Half h:
                    displayValue = DisplayFloat(h, "Half", "h");
                    return true;
                case Int128 i128:
                    displayValue = DisplayNumeric(i128, "Int128", "i128");
                    return true;
                case UInt128 ui128:
                    displayValue = DisplayNumeric(ui128, "UInt128", "u128");
                    return true;
                case long l:
                    displayValue = DisplayNumeric(l, "long", "L");
                    return true;
                case ulong ul:
                    displayValue = DisplayNumeric(ul, "ulong", "UL");
                    return true;
                case int i:
                    displayValue = DisplayNumeric(i, "int");
                    return true;
                case uint ui:
                    displayValue = DisplayNumeric(ui, "uint");
                    return true;
                case nint ni:
                    displayValue = DisplayNumeric(ni, "nint");
                    return true;
                case nuint nui:
                    displayValue = DisplayNumeric(nui, "nuint");
                    return true;
                case short s:
                    displayValue = DisplayNumeric(s, "short");
                    return true;
                case ushort us:
                    displayValue = DisplayNumeric(us, "ushort");
                    return true;
                case byte b:
                    displayValue = DisplayNumeric(b, "byte");
                    return true;
                case sbyte sb:
                    displayValue = DisplayNumeric(sb, "sbyte");
                    return true;
                default:
                    displayValue = null;
                    return false;
            }

            static string DisplayNumeric<T>(T arg, string typeName, string? literalSuffix = null)
                where T : INumber<T>, IMinMaxValue<T>
            {
                if (arg.Equals(T.MaxValue))
                {
                    return $"{typeName}.MaxValue";
                }
                else if (arg.Equals(T.MinValue))
                {
                    return $"{typeName}.MinValue";
                }
                else
                {
                    var display = arg.ToString()!;
                    return literalSuffix is null ? display : display + literalSuffix;
                }
            }

            static string DisplayFloat<T>(T arg, string typeName, string? literalSuffix = null)
                where T : IFloatingPoint<T>, IMinMaxValue<T>
            {
                if (T.IsNaN(arg))
                {
                    return $"{typeName}.NaN";
                }
                else if (T.IsPositiveInfinity(arg))
                {
                    return $"{typeName}.PositiveInfinity";
                }
                else if (T.IsNegativeInfinity(arg))
                {
                    return $"{typeName}.NegativeInfinity";
                }
                else if (arg.Equals(T.MaxValue))
                {
                    return $"{typeName}.MaxValue";
                }
                else if (arg.Equals(T.MinValue))
                {
                    return $"{typeName}.MinValue";
                }
                else
                {
                    var display = arg.ToString()!;
                    if (!display.Contains('.'))
                        display += ".0";

                    return literalSuffix is null ? display : display + literalSuffix;
                }
            }
        }
#endif

        private static string FormatException(Exception ex)
        {
            var builder = new StringBuilder();

            for (Exception? e = ex; e is not null; e = e.InnerException)
            {
                builder.Append(e.GetType().Name);
                builder.Append(": ");
                builder.Append(e.Message);

                if (e.InnerException is not null)
                {
                    builder.Append(", ");
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Checks if string contains any character that might need escaping.
        /// </summary>
        private static bool MayNeedEscape(string s)
        {
            foreach (var c in s)
            {
                if (MayNeedEscape(c))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks whether given char *might* need escaping.
        /// </summary>
        /// <returns>False when absolutely no escaping is needed, otherwise true.</returns>
        private static bool MayNeedEscape(char c)
        {
            return !char.IsLetterOrDigit(c) && c != '.' && c != '/' && c != '-' && c != '_';
        }

        private static string EscapeSingleChar(char c)
        {
            if (c == '\'')
                return "\\\'";

            return EscapeControlChar(c);
        }

        private static string EscapeCharInString(char c)
        {
            if (c == '"')
                return "\\\"";

            return EscapeControlChar(c);
        }

        private static string EscapeControlChar(char c)
        {
            switch (c)
            {
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
                    return c.ToString();
            }
        }
    }
}
