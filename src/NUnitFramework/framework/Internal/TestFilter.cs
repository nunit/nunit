// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Filters;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Interface to be implemented by filters applied to tests.
    /// The filter applies when running the test, after it has been
    /// loaded, since this is the only time an ITest exists.
    /// </summary>
    public abstract class TestFilter : ITestFilter
    {
        /// <summary>
        /// Unique Empty filter.
        /// </summary>
        public readonly static TestFilter Empty = new EmptyFilter();

        /// <summary>
        /// Indicates whether this is the EmptyFilter
        /// </summary>
        public bool IsEmpty => this is EmptyFilter;

        /// <summary>
        /// Determine if a particular test passes the filter criteria. The default
        /// implementation checks the test itself, its parents and any descendants.
        ///
        /// Derived classes may override this method or any of the Match methods
        /// to change the behavior of the filter.
        /// </summary>
        /// <param name="test">The test to which the filter is applied</param>
        /// <returns>True if the test passes the filter, otherwise false</returns>
        public virtual bool Pass(ITest test)
        {
            return Pass(test, false);
        }

        /// <summary>
        /// Determine if a particular test passes the filter criteria. The default
        /// implementation checks the test itself, its parents and any descendants.
        ///
        /// Derived classes may override this method or any of the Match methods
        /// to change the behavior of the filter.
        /// </summary>
        /// <param name="test">The test to which the filter is applied</param>
        /// <param name="negated">If set to <see langword="true"/> we are carrying a negation through</param>
        /// <returns>True if the test passes the filter, otherwise false</returns>
        public virtual bool Pass(ITest test, bool negated)
        {
            if (negated)
                return !Match(test) && !MatchParent(test);

            return Match(test) || MatchParent(test) || MatchDescendant(test);
        }

        /// <summary>
        /// Determine if a test matches the filter explicitly. That is, it must
        /// be a direct match of the test itself or one of its children.
        /// </summary>
        /// <param name="test">The test to which the filter is applied</param>
        /// <returns>True if the test matches the filter explicitly, otherwise false</returns>
        public virtual bool IsExplicitMatch(ITest test)
        {
            return Match(test) || MatchDescendant(test);
        }

        /// <summary>
        /// Determine whether the test itself matches the filter criteria, without
        /// examining either parents or descendants. This is overridden by each
        /// different type of filter to perform the necessary tests.
        /// </summary>
        /// <param name="test">The test to which the filter is applied</param>
        /// <returns>True if the filter matches the any parent of the test</returns>
        public abstract bool Match(ITest test);

        /// <summary>
        /// Determine whether any ancestor of the test matches the filter criteria
        /// </summary>
        /// <param name="test">The test to which the filter is applied</param>
        /// <returns>True if the filter matches the an ancestor of the test</returns>
        public bool MatchParent(ITest test)
        {
            return test.Parent != null && (Match(test.Parent) || MatchParent(test.Parent));
        }

        /// <summary>
        /// Determine whether any descendant of the test matches the filter criteria.
        /// </summary>
        /// <param name="test">The test to be matched</param>
        /// <returns>True if at least one descendant matches the filter criteria</returns>
        protected virtual bool MatchDescendant(ITest test)
        {
            var tests = test.Tests;
            // Use for-loop to avoid allocating the enumerator
            int count = tests.Count;
            for (var index = 0; index < count; index++)
            {
                ITest child = tests[index];
                if (Match(child) || MatchDescendant(child))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Create a TestFilter instance from an XML representation.
        /// </summary>
        public static TestFilter FromXml(string xmlText)
        {
            if (string.IsNullOrEmpty(xmlText))
                xmlText = "<filter />";

            TNode topNode = TNode.FromXml(xmlText);

            if (topNode.Name != "filter")
                throw new Exception("Expected filter element at top level");

            int count = topNode.ChildNodes.Count;

            TestFilter filter = count == 0
                ? TestFilter.Empty
                : count == 1
                    ? FromXml(topNode.FirstChild)
                    : FromXml(topNode);

            return filter;
        }

        /// <summary>
        /// Create a TestFilter from its TNode representation
        /// </summary>
        public static TestFilter FromXml(TNode node)
        {
            bool isRegex = node.Attributes["re"] == "1";

            switch (node.Name)
            {
                case "filter":
                case "and":
                    return new AndFilter(GetChildNodeFilters(node));

                case "or":
                    List<TestFilter> orChildFilters = new List<TestFilter>();

                    // check if we have all filters accessing same field in OR fashion without regex
                    var hashCommonTypeAndNonRegex = node.ChildNodes.Count > 0;
                    Type commonType = null;

                    foreach (var childNode in node.ChildNodes)
                    {
                        var filter = FromXml(childNode);

                        // make sure invariants are valid
                        commonType ??= filter.GetType();
                        hashCommonTypeAndNonRegex &= commonType == filter.GetType();

                        // our supported filter types
                        hashCommonTypeAndNonRegex &=
                            filter is ClassNameFilter { IsRegex: false }
                            || filter is FullNameFilter { IsRegex: false }
                            || filter is IdFilter { IsRegex: false }
                            || filter is MethodNameFilter { IsRegex: false }
                            || filter is NamespaceFilter { IsRegex: false }
                            || filter is TestNameFilter { IsRegex: false };

                        orChildFilters.Add(filter);
                    }

                    if (hashCommonTypeAndNonRegex)
                    {
                        // we can transform
                        Func<ITest, string> selector = orChildFilters[0] switch
                        {
                            ClassNameFilter _ => test => test.ClassName,
                            FullNameFilter _ => test => test.FullName,
                            IdFilter _ => test => test.Id,
                            MethodNameFilter _ => test => test.MethodName,
                            NamespaceFilter _ => test => test.TypeInfo?.Namespace,
                            TestNameFilter _ => test => test.Name,
                            _ => throw new InvalidOperationException("Invalid filter, logic wrong")
                        };
                        return new InFilter(selector, orChildFilters.Select(x => ((ValueMatchFilter) x).ExpectedValue));
                    }

                    return new OrFilter(orChildFilters.ToArray());

                case "not":
                    return new NotFilter(FromXml(node.FirstChild));

                case "id":
                    return new IdFilter(node.Value);

                case "test":
                    return new FullNameFilter(node.Value, isRegex);

                case "name":
                    return new TestNameFilter(node.Value, isRegex);

                case "method":
                    return new MethodNameFilter(node.Value, isRegex);

                case "class":
                    return new ClassNameFilter(node.Value, isRegex);

                case "namespace":
                    return new NamespaceFilter(node.Value, isRegex);

                case "cat":
                    return new CategoryFilter(node.Value, isRegex);

                case "prop":
                    string name = node.Attributes["name"];
                    if (name != null)
                        return new PropertyFilter(name, node.Value, isRegex);
                    break;
            }

            throw new ArgumentException("Invalid filter element: " + node.Name, "xmlNode");
        }

        private static TestFilter[] GetChildNodeFilters(TNode node)
        {
            var childFilters = new TestFilter[node.ChildNodes.Count];
            int i = 0;
            foreach (var childNode in node.ChildNodes)
                childFilters[i++] = FromXml(childNode);

            return childFilters;
        }

        /// <summary>
        /// Nested class provides an empty filter - one that always
        /// returns true when called. It never matches explicitly.
        /// </summary>
        [Serializable]
        private class EmptyFilter : TestFilter
        {
            public override bool Match( ITest test )
            {
                return true;
            }

            public override bool Pass( ITest test, bool negated )
            {
                return true;
            }

            public override bool IsExplicitMatch( ITest test )
            {
                return false;
            }

            public override TNode AddToXml(TNode parentNode, bool recursive)
            {
                return parentNode.AddElement("filter");
            }
        }

#region IXmlNodeBuilder Implementation

        /// <summary>
        /// Adds an XML node
        /// </summary>
        /// <param name="recursive">True if recursive</param>
        /// <returns>The added XML node</returns>
        public TNode ToXml(bool recursive)
        {
            return AddToXml(new TNode("dummy"), recursive);
        }

        /// <summary>
        /// Adds an XML node
        /// </summary>
        /// <param name="parentNode">Parent node</param>
        /// <param name="recursive">True if recursive</param>
        /// <returns>The added XML node</returns>
        public abstract TNode AddToXml(TNode parentNode, bool recursive);

#endregion
    }
}
