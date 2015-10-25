// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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
using System.Reflection;
using System.Text;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// TestNameGenerator is able to create test names according to
    /// a coded pattern.
    /// </summary>
    public class TestNameGenerator
    {
        private List<NameFragment> _fragments = new List<NameFragment>();

        /// <summary>
        /// Construct a TestNameGenerator
        /// </summary>
        /// <param name="pattern">The pattern used by this generator.</param>
        public TestNameGenerator(string pattern)
        {
            // Build a list of actions so this generator can be applied to
            // multiple types and methods.

            int start = 0;
            while (start < pattern.Length)
            {
                int lcurly = pattern.IndexOf('{', start);
                if (lcurly < 0) // No more substitutions in pattern
                    break;

                int rcurly = pattern.IndexOf('}', lcurly);
                if (rcurly < 0)
                    break;

                if (lcurly > start) // Handle fixedixed text before curly brace
                    _fragments.Add(new FixedTextFragment(pattern.Substring(start, lcurly - start)));

                string token = pattern.Substring(lcurly, rcurly - lcurly + 1);

                switch (token)
                {
                    case "{m}":
                        _fragments.Add(new MethodNameFragment());
                        break;
                    case "{n}":
                        _fragments.Add(new NamespaceFragment());
                        break;
                    case "{c}":
                        _fragments.Add(new ClassNameFragment());
                        break;
                    case "{C}":
                        _fragments.Add(new ClassFullNameFragment());
                        break;
                    case "{M}":
                        _fragments.Add(new MethodFullNameFragment());
                        break;
                    case "{a}":
                        _fragments.Add(new ArgListFragment(40));
                        break;
                    case "{0}":
                    case "{1}":
                    case "{2}":
                    case "{3}":
                    case "{4}":
                    case "{5}":
                    case "{6}":
                    case "{7}":
                    case "{8}":
                    case "{9}":
                        int index = token[1] - '0';
                        _fragments.Add(new ArgumentFragment(index, 40));
                        break;
                    default:
                        char c = token[1];
                        if (token.Length >= 5 && token[2] == ':' && (c == 'a' || char.IsDigit(c)))
                        {
                            int length;

                            // NOTE: The code would be much simpler using TryParse. However,
                            // that method doesn't exist in the Compact Framework.
                            try
                            {
                                length = int.Parse(token.Substring(3, token.Length - 4));
                            }
                            catch
                            {
                                length = -1;
                            }
                            if (length > 0)
                            {
                                if (c == 'a')
                                    _fragments.Add(new ArgListFragment(length));
                                else // It's a digit
                                    _fragments.Add(new ArgumentFragment(c - '0', length));
                                break;
                            }
                        }

                        // Output the erroneous token to aid user in debugging
                        _fragments.Add(new FixedTextFragment(token));
                        break;
                }

                start = rcurly + 1;
            }


            // Output any trailing plain text
            if (start < pattern.Length)
                _fragments.Add(new FixedTextFragment(pattern.Substring(start)));
        }

        /// <summary>
        /// Get the display name for a method
        /// </summary>
        /// <param name="method">A MethodInfo</param>
        /// <returns>The display name</returns>
        public string GetDisplayName(MethodInfo method)
        {
            return GetDisplayName(method, null);
        }

        /// <summary>
        /// Get the display name for a method with args
        /// </summary>
        /// <param name="method">A MethodInfo</param>
        /// <param name="args">Argument list for the method</param>
        /// <returns>The display name</returns>
        public string GetDisplayName(MethodInfo method, object[] args)
        {
            var result = new StringBuilder();

            foreach (var fragment in _fragments)
                result.Append(fragment.GetText(method, args));

            return result.ToString();
        }

        #region Nested Classes Representing Name Fragments

        private abstract class NameFragment
        {
            private const string THREE_DOTS = "...";

            public abstract string GetText(MethodInfo method, object[] args);

            protected static void AppendGenericTypeNames(StringBuilder sb, MethodInfo method)
            {
                sb.Append("<");
                int cnt = 0;
                foreach (Type t in method.GetGenericArguments())
                {
                    if (cnt++ > 0) sb.Append(",");
                    sb.Append(t.Name);
                }
                sb.Append(">");
            }

            protected static string GetDisplayString(object arg, int stringMax)
            {
                string display = arg == null
                    ? "null"
                    : Convert.ToString(arg, System.Globalization.CultureInfo.InvariantCulture);

                if (arg is double)
                {
                    double d = (double)arg;

                    if (double.IsNaN(d))
                        display = "double.NaN";
                    else if (double.IsPositiveInfinity(d))
                        display = "double.PositiveInfinity";
                    else if (double.IsNegativeInfinity(d))
                        display = "double.NegativeInfinity";
                    else if (d == double.MaxValue)
                        display = "double.MaxValue";
                    else if (d == double.MinValue)
                        display = "double.MinValue";
                    else
                    {
                        if (display.IndexOf('.') == -1)
                            display += ".0";
                        display += "d";
                    }
                }
                else if (arg is float)
                {
                    float f = (float)arg;

                    if (float.IsNaN(f))
                        display = "float.NaN";
                    else if (float.IsPositiveInfinity(f))
                        display = "float.PositiveInfinity";
                    else if (float.IsNegativeInfinity(f))
                        display = "float.NegativeInfinity";
                    else if (f == float.MaxValue)
                        display = "float.MaxValue";
                    else if (f == float.MinValue)
                        display = "float.MinValue";
                    else
                    {
                        if (display.IndexOf('.') == -1)
                            display += ".0";
                        display += "f";
                    }
                }
                else if (arg is decimal)
                {
                    decimal d = (decimal)arg;
                    if (d == decimal.MinValue)
                        display = "decimal.MinValue";
                    else if (d == decimal.MaxValue)
                        display = "decimal.MaxValue";
                    else
                        display += "m";
                }
                else if (arg is long)
                {
                    long l = (long)arg;
                    if (l == long.MinValue)
                        display = "long.MinValue";
                    else if (l == long.MinValue)
                        display = "long.MaxValue";
                    else
                        display += "L";
                }
                else if (arg is ulong)
                {
                    ulong ul = (ulong)arg;
                    if (ul == ulong.MinValue)
                        display = "ulong.MinValue";
                    else if (ul == ulong.MinValue)
                        display = "ulong.MaxValue";
                    else
                        display += "UL";
                }
                else if (arg is string)
                {
                    var str = (string)arg;
                    bool tooLong = str.Length > stringMax;
                    int limit = stringMax - THREE_DOTS.Length;

                    StringBuilder sb = new StringBuilder();
                    sb.Append("\"");
                    foreach (char c in (string)arg)
                    {
                        sb.Append(EscapeControlChar(c));
                        if (tooLong && sb.Length > limit)
                        {
                            sb.Append(THREE_DOTS);
                            break;
                        }
                    }
                    sb.Append("\"");
                    display = sb.ToString();
                }
                else if (arg is char)
                {
                    display = "\'" + EscapeControlChar((char)arg) + "\'";
                }
                else if (arg is int)
                {
                    int ival = (int)arg;
                    if (ival == int.MaxValue)
                        display = "int.MaxValue";
                    else if (ival == int.MinValue)
                        display = "int.MinValue";
                }
                else if (arg is uint)
                {
                    uint val = (uint)arg;
                    if (val == uint.MaxValue)
                        display = "uint.MaxValue";
                    else if (val == uint.MinValue)
                        display = "uint.MinValue";
                }
                else if (arg is short)
                {
                    short val = (short)arg;
                    if (val == short.MaxValue)
                        display = "short.MaxValue";
                    else if (val == short.MinValue)
                        display = "short.MinValue";
                }
                else if (arg is ushort)
                {
                    ushort val = (ushort)arg;
                    if (val == ushort.MaxValue)
                        display = "ushort.MaxValue";
                    else if (val == ushort.MinValue)
                        display = "ushort.MinValue";
                }
                else if (arg is byte)
                {
                    byte val = (byte)arg;
                    if (val == byte.MaxValue)
                        display = "byte.MaxValue";
                    else if (val == byte.MinValue)
                        display = "byte.MinValue";
                }
                else if (arg is sbyte)
                {
                    sbyte val = (sbyte)arg;
                    if (val == sbyte.MaxValue)
                        display = "sbyte.MaxValue";
                    else if (val == sbyte.MinValue)
                        display = "sbyte.MinValue";
                }

                return display;
            }

            private static string EscapeControlChar(char c)
            {
                switch (c)
                {
                    case '\'':
                        return "\\\'";
                    case '\"':
                        return "\\\"";
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
                        return string.Format("\\x{0:X4}", (int)c);

                    default:
                        return c.ToString();
                }
            }
        }

        private class FixedTextFragment : NameFragment
        {
            private string _text;

            public FixedTextFragment(string text)
            {
                _text = text;
            }

            public override string GetText(MethodInfo method, object[] args)
            {
                return _text;
            }
        }

        private class MethodNameFragment : NameFragment
        {
            public override string GetText(MethodInfo method, object[] args)
            {
                var sb = new StringBuilder();

                sb.Append(method.Name);

                if (method.IsGenericMethod)
                    AppendGenericTypeNames(sb, method);

                return sb.ToString();
            }
        }

        private class NamespaceFragment : NameFragment
        {
            public override string GetText(MethodInfo method, object[] args)
            {
                return method.DeclaringType.Namespace;
            }
        }

        private class MethodFullNameFragment : NameFragment
        {
            public override string GetText(MethodInfo method, object[] args)
            {
                var sb = new StringBuilder();

                sb.Append(method.DeclaringType.FullName);
                sb.Append('.');
                sb.Append(method.Name);

                if (method.IsGenericMethod)
                    AppendGenericTypeNames(sb, method);

                return sb.ToString();
            }
        }

        private class ClassNameFragment : NameFragment
        {
            public override string GetText(MethodInfo method, object[] args)
            {
                return method.DeclaringType.Name;
            }
        }

        private class ClassFullNameFragment : NameFragment
        {
            public override string GetText(MethodInfo method, object[] args)
            {
                return method.DeclaringType.FullName;
            }
        }

        private class ArgListFragment : NameFragment
        {
            private int _maxStringLength;

            public ArgListFragment(int maxStringLength)
            {
                _maxStringLength = maxStringLength;
            }

            public override string GetText(MethodInfo method, object[] arglist)
            {
                var sb = new StringBuilder();

                if (arglist != null)
                {
                    sb.Append('(');

                    for (int i = 0; i < arglist.Length; i++)
                    {
                        if (i > 0) sb.Append(",");
                        sb.Append(GetDisplayString(arglist[i], _maxStringLength));
                    }
                    
                    sb.Append(')');
                }

                return sb.ToString();
            }
        }

        private class ArgumentFragment : NameFragment
        {
            private int _index;
            private int _maxStringLength;

            public ArgumentFragment(int index, int maxStringLength)
            {
                _index = index;
                _maxStringLength = maxStringLength;
            }

            public override string GetText(MethodInfo method, object[] args)
            {
                return _index < args.Length
                    ? GetDisplayString(args[_index], _maxStringLength)
                    : string.Empty;
            }
        }

        #endregion
    }
}
