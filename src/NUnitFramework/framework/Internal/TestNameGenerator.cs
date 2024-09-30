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
        private List<NameFragment>? _fragments;

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
        public string GetDisplayName(TestMethod testMethod, object?[]? args)
        {
            if (_fragments is null)
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

            public virtual void AppendTextTo(StringBuilder sb, TestMethod testMethod, object?[]? args)
            {
                AppendTextTo(sb, testMethod.Method.MethodInfo, args);
            }

            public abstract void AppendTextTo(StringBuilder sb, MethodInfo method, object?[]? args);

            protected static void AppendGenericTypeNames(StringBuilder sb, MethodInfo method)
            {
                sb.Append('<');
                int cnt = 0;
                foreach (Type t in method.GetGenericArguments())
                {
                    if (cnt++ > 0)
                        sb.Append(',');
                    sb.Append(t.Name);
                }
                sb.Append('>');
            }

            protected static string GetDisplayString(object? arg, int stringMax)
            {
                return DisplayName.GetValueString(arg, stringMax);
            }
        }

        private sealed class TestIDFragment : NameFragment
        {
            public override void AppendTextTo(StringBuilder sb, MethodInfo method, object?[]? args)
            {
                sb.Append("{i}"); // No id available using MethodInfo
            }

            public override void AppendTextTo(StringBuilder sb, TestMethod testMethod, object?[]? args)
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

            public override void AppendTextTo(StringBuilder sb, MethodInfo method, object?[]? args)
            {
                sb.Append(_text);
            }
        }

        private sealed class MethodNameFragment : NameFragment
        {
            public override void AppendTextTo(StringBuilder sb, MethodInfo method, object?[]? args)
            {
                sb.Append(method.Name);

                if (method.IsGenericMethod)
                    AppendGenericTypeNames(sb, method);
            }
        }

        private sealed class NamespaceFragment : NameFragment
        {
            public override void AppendTextTo(StringBuilder sb, MethodInfo method, object?[]? args)
            {
                sb.Append(method.DeclaringType!.Namespace);
            }
        }

        private sealed class MethodFullNameFragment : NameFragment
        {
            public override void AppendTextTo(StringBuilder sb, MethodInfo method, object?[]? args)
            {
                sb.Append(method.DeclaringType!.FullName);
                sb.Append('.');
                sb.Append(method.Name);

                if (method.IsGenericMethod)
                    AppendGenericTypeNames(sb, method);
            }
        }

        private sealed class ClassNameFragment : NameFragment
        {
            public override void AppendTextTo(StringBuilder sb, MethodInfo method, object?[]? args)
            {
                sb.Append(method.DeclaringType!.Name);
            }
        }

        private sealed class ClassFullNameFragment : NameFragment
        {
            public override void AppendTextTo(StringBuilder sb, MethodInfo method, object?[]? args)
            {
                sb.Append(method.DeclaringType!.FullName);
            }
        }

        private sealed class ArgListFragment : NameFragment
        {
            private readonly int _maxStringLength;

            public ArgListFragment(int maxStringLength)
            {
                _maxStringLength = maxStringLength;
            }

            public override void AppendTextTo(StringBuilder sb, MethodInfo method, object?[]? arglist)
            {
                if (arglist is not null)
                {
                    sb.Append('(');

                    for (int i = 0; i < arglist.Length; i++)
                    {
                        if (i > 0)
                            sb.Append(",");
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

            public override void AppendTextTo(StringBuilder sb, MethodInfo method, object?[]? args)
            {
                if (args is not null && _index < args.Length)
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

            public override void AppendTextTo(StringBuilder sb, MethodInfo method, object?[]? args)
            {
                if (args is not null)
                {
                    sb.Append('(');

                    var parameters = method.GetParameters();

                    for (int i = 0; i < args.Length; i++)
                    {
                        if (i > 0)
                            sb.Append(", ");

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
