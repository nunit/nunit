// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
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
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Implements a simplified filter for use in deciding which
    /// Types and Methods should be used to generate tests. It is consructed with a 
    /// list of strings, each of which may end up being interpreted in various ways.
    /// </summary>
    internal class PreFilter : IPreFilter
    {
        private static readonly char[] ARG_START = new char[] { '(', '<' };

        private readonly List<FilterElement> _filters = new List<FilterElement>();

        /// <summary>
        /// Return a new PreFilter, without elements, which is considered
        /// empty and always matches.
        /// </summary>
        public static PreFilter Empty
        {
            get { return new PreFilter(); }
        }

        /// <summary>
        /// Return true if the filter is empty, in which case it
        /// always succeeds. Technically, this is just a filter and
        /// you can add elements but it's best to use Empty when
        /// you need an empty filter and new when you plan to add.
        /// </summary>
        public bool IsEmpty
        {
            get { return _filters.Count == 0; }
        }

        /// <summary>
        /// Add a new filter element to the filter
        /// </summary>
        /// <param name="filterText"></param>
        public void Add(string filterText)
        {
            // First, make sure the new filter is not redundant
            foreach (var filter in _filters)
            {
                if (filter.Text == filterText)
                    return;

                if (filterText.StartsWith(filter.Text + "."))
                    return;
            }

            var newFilter = new FilterElement(filterText);

            // Check to see if it makes any of the existing
            // filter elements redundant.
            for (int index = _filters.Count - 1; index >= 0; index--)
                if (_filters[index].Text.StartsWith(filterText + "."))
                    _filters.RemoveAt(index);

            _filters.Add(new FilterElement(filterText));
        }

        /// <summary>
        /// Use the filter on a Type, returning true if the type matches the filter
        /// and should therefore be included in the discovery process.
        /// </summary>
        public bool IsMatch(Type type)
        {
            if (IsEmpty)
                return true;

            string typeName = type.FullName;

            foreach (FilterElement filter in _filters)
            {
                if (filter.Match(type))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Use the filter on a Type, returning true if the type matches the filter
        /// and should therefore be included in the discovery process.
        /// </summary>
        public bool IsMatch(Type type, MethodInfo method)
        {
            if (IsEmpty)
                return true;

            int filterCount = 0;
            foreach (FilterElement filter in _filters)
            {
                if (type.FullName == filter.ClassName)
                {
                    ++filterCount;
                    if (method.Name == filter.MethodName)
                        return true;
                }
            }

            // If no match, we succeed only if there were no method filters for this fixture
            return filterCount == 0;
        }

        #region Nested FilterElementType Enumeration

        private enum FilterElementType
        {
            Unknown,
            Namespace,
            Fixture,
            Method
        }

        #endregion

        #region Nested FilterElement Class

        private class FilterElement
        {
            private FilterElementType ElementType = FilterElementType.Unknown;
            public string Text;
            public string ClassName;
            public string MethodName;

            public FilterElement(string text)
            {
                int argStart = text.IndexOfAny(ARG_START);
                Text = argStart > 0
                    ? text.Substring(0, argStart)
                    : text;

                // Filter may be the fullname of a test method, if so extract class name
                int lastdot = Text.LastIndexOf('.');
                if (lastdot > 0)
                {
                    ClassName = Text.Substring(0, lastdot);
                    MethodName = Text.Substring(lastdot + 1);
                }
            }

            public bool Match(Type type)
            {
                switch(ElementType)
                {
                    default:
                    case FilterElementType.Unknown:
                        return MatchUnknownElement(type);
                    case FilterElementType.Fixture:
                        return MatchFixtureElement(type);
                    case FilterElementType.Namespace:
                        return MatchNamespaceElement(type);
                    case FilterElementType.Method:
                        return MatchMethodElement(type);
                }
            }

            private bool MatchUnknownElement(Type type)
            {
                if (MatchFixtureElement(type))
                {
                    ElementType = FilterElementType.Fixture;
                    return true;
                }

                if (MatchNamespaceElement(type))
                {
                    ElementType = FilterElementType.Namespace;
                    return true;
                }

                if (MatchMethodElement(type))
                {
                    ElementType = FilterElementType.Method;
                    return true;
                }

                return false;
            }

            private bool MatchFixtureElement(Type type)
            {
                return type.FullName == Text;
            }

            private bool MatchNamespaceElement(Type type)
            {
                return type.FullName.StartsWith(Text + ".");
            }

            private bool MatchMethodElement(Type type)
            {
                return type.FullName == ClassName;
            }
        }

        #endregion
    }
}
