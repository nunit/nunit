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
        // TODO: Using a static here is not good it's the easiest
        // way to get a temporary implementation without passing the
        // pattern all the way down the test builder hierarchy

        /// <summary>
        /// Default pattern used to generate names
        /// </summary>
        public static string DefaultTestNamePattern = "{m}{a}";

        // The name pattern used by this TestNameGenerator
        private string _pattern;

        // The list of NameFragments used to generate names
        private List<NameFragment> _fragments;

        /// <summary>
        /// Construct a TestNameGenerator
        /// </summary>
        public TestNameGenerator()
        {
            _pattern = DefaultTestNamePattern;
        }

        /// <summary>
        /// Construct a TestNameGenerator
        /// </summary>
        /// <param name="pattern">The pattern used by this generator.</param>
        public TestNameGenerator(string pattern)
        {
            _pattern = pattern;
        }

        /// <summary>
        /// Get the display name for a TestMethod and it's arguments
        /// </summary>
        /// <param name="testMethod">A TestMethod</param>
        /// <returns>The display name</returns>
        public string GetDisplayName(TestMethod testMethod)
        {
            return GetDisplayName(testMethod, null);
        }

        /// <summary>
        /// Get the display name for a TestMethod and it's arguments
        /// </summary>
        /// <param name="testMethod">A TestMethod</param>
        /// <param name="args">Arguments to be used</param>
        /// <returns>The display name</returns>
        public string GetDisplayName(TestMethod testMethod, object[] args)
        {
            if (_fragments == null)
                _fragments = BuildFragmentList(_pattern);

            var result = new StringBuilder();

            foreach (var fragment in _fragments)
                result.Append(fragment.GetText(testMethod, args));

            return result.ToString();
        }

        #region Helper Methods

        private static List<NameFragment> BuildFragmentList(string pattern)
        {
            var fragments = new List<NameFragment>();

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
                    fragments.Add(new FixedTextFragment(pattern.Substring(start, lcurly - start)));

                string token = pattern.Substring(lcurly, rcurly - lcurly + 1);

                switch (token)
                {
                    case "{m}":
                        fragments.Add(new MethodNameFragment());
                        break;
                    case "{i}":
                        fragments.Add(new TestIDFragment());
                        break;
                    case "{n}":
                        fragments.Add(new NamespaceFragment());
                        break;
                    case "{c}":
                        fragments.Add(new ClassNameFragment());
                        break;
                    case "{C}":
                        fragments.Add(new ClassFullNameFragment());
                        break;
                    case "{M}":
                        fragments.Add(new MethodFullNameFragment());
                        break;
                    case "{a}":
                        fragments.Add(new ArgListFragment(0));
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
                        fragments.Add(new ArgumentFragment(index, 40));
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
                                    fragments.Add(new ArgListFragment(length));
                                else // It's a digit
                                    fragments.Add(new ArgumentFragment(c - '0', length));
                                break;
                            }
                        }

                        // Output the erroneous token to aid user in debugging
                        fragments.Add(new FixedTextFragment(token));
                        break;
                }

                start = rcurly + 1;
            }


            // Output any trailing plain text
            if (start < pattern.Length)
                fragments.Add(new FixedTextFragment(pattern.Substring(start)));

            return fragments;
        }

        #endregion

        #region Nested Classes Representing Name Fragments

        private abstract class NameFragment
        {
            private const string THREE_DOTS = "...";

            public virtual string GetText(TestMethod testMethod, object[] args)
            {
                return GetText(testMethod.Method.MethodInfo, args);
            }

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
                    if (arg.Equals(long.MinValue))
                        display = "long.MinValue";
                    else if (arg.Equals(long.MaxValue))
                        display = "long.MaxValue";
                    else
                        display += "L";
                }
                else if (arg is ulong)
                {
                    ulong ul = (ulong)arg;
                    if (ul == ulong.MinValue)
                        display = "ulong.MinValue";
                    else if (ul == ulong.MaxValue)
                        display = "ulong.MaxValue";
                    else
                        display += "UL";
                }
                else if (arg is string)
                {
                    var str = (string)arg;
                    bool tooLong = stringMax > 0 && str.Length > stringMax;
                    int limit = tooLong ? stringMax - THREE_DOTS.Length : 0;

                    StringBuilder sb = new StringBuilder();
                    sb.Append("\"");
                    foreach (char c in str)
                    {
                        sb.Append(EscapeCharInString(c));
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
                    display = "\'" + EscapeSingleChar((char)arg) + "\'";
                }
                else if (arg is int)
                {
                    if (arg.Equals(int.MaxValue))
                        display = "int.MaxValue";
                    else if (arg.Equals(int.MinValue))
                        display = "int.MinValue";
                }
                else if (arg is uint)
                {
                    if (arg.Equals(uint.MaxValue))
                        display = "uint.MaxValue";
                    else if (arg.Equals(uint.MinValue))
                        display = "uint.MinValue";
                }
                else if (arg is short)
                {
                    if (arg.Equals(short.MaxValue))
                        display = "short.MaxValue";
                    else if (arg.Equals(short.MinValue))
                        display = "short.MinValue";
                }
                else if (arg is ushort)
                {
                    if (arg.Equals(ushort.MaxValue))
                        display = "ushort.MaxValue";
                    else if (arg.Equals(ushort.MinValue))
                        display = "ushort.MinValue";
                }
                else if (arg is byte)
                {
                    if (arg.Equals(byte.MaxValue))
                        display = "byte.MaxValue";
                    else if (arg.Equals(byte.MinValue))
                        display = "byte.MinValue";
                }
                else if (arg is sbyte)
                {
                    if (arg.Equals(sbyte.MaxValue))
                        display = "sbyte.MaxValue";
                    else if (arg.Equals(sbyte.MinValue))
                        display = "sbyte.MinValue";
                }

                return display;
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
                        return string.Format("\\x{0:X4}", (int)c);

                    default:
                        return c.ToString();
                }
            }
        }

        private class TestIDFragment : NameFragment
        {
            public override string GetText(MethodInfo method, object[] args)
            {
                return "{i}"; // No id available using MethodInfo
            }

            public override string GetText(TestMethod testMethod, object[] args)
            {
                return testMethod.Id;
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
