// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
        private readonly string _pattern;

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
        /// Get the display name for a TestMethod and its arguments
        /// </summary>
        /// <param name="testMethod">A TestMethod</param>
        /// <returns>The display name</returns>
        public string GetDisplayName(TestMethod testMethod)
        {
            return GetDisplayName(testMethod, null);
        }

        /// <summary>
        /// Get the display name for a TestMethod and its arguments
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
                fragment.AppendTextTo(result, testMethod, args);

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

                if (lcurly > start) // Handle fixed text before curly brace
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
                    case "{p}":
                        fragments.Add(new ParamArgListFragment(0));
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
                        fragments.Add(new ArgumentFragment(index, 0));
                        break;
                    default:
                        char c = token[1];
                        if (token.Length >= 5 && token[2] == ':' && (c == 'a' || c == 'p' || char.IsDigit(c)))
                        {
                            if (int.TryParse(token.Substring(3, token.Length - 4), out var length) && length > 0)
                            {
                                if (c == 'a')
                                    fragments.Add(new ArgListFragment(length));
                                else if (c == 'p')
                                    fragments.Add(new ParamArgListFragment(length));
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

            public virtual void AppendTextTo(StringBuilder sb, TestMethod testMethod, object[] args)
            {
                AppendTextTo(sb, testMethod.Method.MethodInfo, args);
            }

            public abstract void AppendTextTo(StringBuilder sb, MethodInfo method, object[] args);

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

                if (arg is Array {Rank: 1} argArray)
                {
                    if (argArray.Length == 0)
                        display = "[]";
                    else
                    {
                        var builder = new StringBuilder();
                        builder.Append("[");

                        const int maxNumItemsToEnumerate = 5;

                        var numItemsToEnumerate = Math.Min(argArray.Length, maxNumItemsToEnumerate);
                        for (int i = 0; i < numItemsToEnumerate; i++)
                        {
                            if (i > 0)
                                builder.Append(", ");

                            var element = argArray.GetValue(i);

                            if (element is Array {Rank: 1} childArray)
                            {
                                builder.Append(childArray.GetType().GetElementType().Name);
                                builder.Append("[]");
                            }
                            else
                            {
                                var elementDisplayString = GetDisplayString(element, stringMax);
                                builder.Append(elementDisplayString);
                            }
                        }

                        if (argArray.Length > maxNumItemsToEnumerate)
                            builder.Append(", ...");

                        builder.Append("]");
                        display = builder.ToString();
                    }
                }
                else if (arg is double dbl)
                {
                    if (double.IsNaN(dbl))
                        display = "double.NaN";
                    else if (double.IsPositiveInfinity(dbl))
                        display = "double.PositiveInfinity";
                    else if (double.IsNegativeInfinity(dbl))
                        display = "double.NegativeInfinity";
                    else if (dbl == double.MaxValue)
                        display = "double.MaxValue";
                    else if (dbl == double.MinValue)
                        display = "double.MinValue";
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
                else if (arg is string str)
                {
                    bool tooLong = stringMax > 0 && str.Length > stringMax;
                    int limit = tooLong ? stringMax - THREE_DOTS.Length : 0;

                    var sb = new StringBuilder();
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
                else if (arg is char c)
                {
                    display = "\'" + EscapeSingleChar(c) + "\'";
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

        private sealed class TestIDFragment : NameFragment
        {
            public override void AppendTextTo(StringBuilder sb, MethodInfo method, object[] args)
            {
                sb.Append("{i}"); // No id available using MethodInfo
            }

            public override void AppendTextTo(StringBuilder sb, TestMethod testMethod, object[] args)
            {
                sb.Append(testMethod.Id);
            }
        }

        private sealed class FixedTextFragment : NameFragment
        {
            private readonly string _text;

            public FixedTextFragment(string text)
            {
                _text = text;
            }

            public override void AppendTextTo(StringBuilder sb, MethodInfo method, object[] args)
            {
                sb.Append(_text);
            }
        }

        private sealed class MethodNameFragment : NameFragment
        {
            public override void AppendTextTo(StringBuilder sb, MethodInfo method, object[] args)
            {
                sb.Append(method.Name);

                if (method.IsGenericMethod)
                    AppendGenericTypeNames(sb, method);
            }
        }

        private sealed class NamespaceFragment : NameFragment
        {
            public override void AppendTextTo(StringBuilder sb, MethodInfo method, object[] args)
            {
                sb.Append(method.DeclaringType.Namespace);
            }
        }

        private sealed class MethodFullNameFragment : NameFragment
        {
            public override void AppendTextTo(StringBuilder sb, MethodInfo method, object[] args)
            {
                sb.Append(method.DeclaringType.FullName);
                sb.Append('.');
                sb.Append(method.Name);

                if (method.IsGenericMethod)
                    AppendGenericTypeNames(sb, method);
            }
        }

        private sealed class ClassNameFragment : NameFragment
        {
            public override void AppendTextTo(StringBuilder sb, MethodInfo method, object[] args)
            {
                sb.Append(method.DeclaringType.Name);
            }
        }

        private sealed class ClassFullNameFragment : NameFragment
        {
            public override void AppendTextTo(StringBuilder sb, MethodInfo method, object[] args)
            {
                sb.Append(method.DeclaringType.FullName);
            }
        }

        private sealed class ArgListFragment : NameFragment
        {
            private readonly int _maxStringLength;

            public ArgListFragment(int maxStringLength)
            {
                _maxStringLength = maxStringLength;
            }

            public override void AppendTextTo(StringBuilder sb, MethodInfo method, object[] arglist)
            {
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
            }
        }

        private sealed class ArgumentFragment : NameFragment
        {
            private readonly int _index;
            private readonly int _maxStringLength;

            public ArgumentFragment(int index, int maxStringLength)
            {
                _index = index;
                _maxStringLength = maxStringLength;
            }

            public override void AppendTextTo(StringBuilder sb, MethodInfo method, object[] args)
            {
                if (_index < args.Length)
                    sb.Append(GetDisplayString(args[_index], _maxStringLength));
            }
        }

        private sealed class ParamArgListFragment : NameFragment
        {
            private readonly int _maxStringLength;

            public ParamArgListFragment(int maxStringLength)
            {
                _maxStringLength = maxStringLength;
            }

            public override void AppendTextTo(StringBuilder sb, MethodInfo method, object[] args)
            {
                if (args != null)
                {
                    sb.Append('(');

                    var parameters = method.GetParameters();
                    
                    for (int i = 0; i < args.Length; i++)
                    {
                        if (i > 0) sb.Append(", ");

                        if (i < parameters.Length)
                        {
                            sb.Append(parameters[i].Name);
                            sb.Append(": ");
                        }

                        sb.Append(GetDisplayString(args[i], _maxStringLength));
                    }

                    sb.Append(')');
                }
            }
        }

        #endregion
    }
}
