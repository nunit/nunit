// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
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
            const string emptyFilterXmlWithSpace = "<filter />";
            const string emptyFilterWithoutSpace = "<filter/>";

            // check for fast cases
            if (string.IsNullOrEmpty(xmlText) || xmlText.Length < 11 && xmlText is emptyFilterXmlWithSpace or emptyFilterWithoutSpace)
            {
                return Empty;
            }

            TNode topNode = TNode.FromXml(xmlText);

            if (topNode.Name != "filter")
                throw new Exception("Expected filter element at top level");

            int count = topNode.ChildNodes.Count;

            TestFilter filter = count == 0
                ? Empty
                : count == 1
                    ? FromXml(topNode.FirstChild)
                    : FromXml(topNode);

            return filter;
        }

        /// <summary>
        /// Create a TestFilter from its TNode representation
        /// </summary>
        public static TestFilter FromXml(TNode? node)
        {
            Guard.ArgumentNotNull(node, nameof(node));

            static bool IsRegex(TNode node) => node.Attributes["re"] == "1";

            switch (node.Name)
            {
                case "filter":
                case "and":
                    return new AndFilter(GetChildNodeFilters(node));

                case "or":
                    var orFilter = new OrFilter(GetChildNodeFilters(node));
                    if (InFilter.TryOptimize(orFilter, out var optimized))
                    {
                        return optimized;
                    }
                    return orFilter;

                case "not":
                    return new NotFilter(FromXml(node.FirstChild));

                case "id":
                    return new IdFilter(NodeValue(node));

                case "test":
                    return new FullNameFilter(NodeValue(node), IsRegex(node));

                case "name":
                    return new TestNameFilter(NodeValue(node), IsRegex(node));

                case "method":
                    return new MethodNameFilter(NodeValue(node), IsRegex(node));

                case "class":
                    return new ClassNameFilter(NodeValue(node), IsRegex(node));

                case "namespace":
                    return new NamespaceFilter(NodeValue(node), IsRegex(node));

                case "cat":
                    return new CategoryFilter(NodeValue(node), IsRegex(node));

                case "prop":
                    string? name = node.Attributes["name"];
                    if (name != null)
                        return new PropertyFilter(name, NodeValue(node), IsRegex(node));
                    break;
            }

            throw new ArgumentException("Invalid filter element: " + node.Name, "xmlNode");
        }

        private static string NodeValue(TNode node)
        {
            return node.Value ?? throw new InvalidOperationException("Value is null");
        }

        private static TestFilter[] GetChildNodeFilters(TNode node)
        {
            var count = node.ChildNodes.Count;
            var childFilters = new TestFilter[count];

            for (var i = 0; i < count; i++)
                childFilters[i] = FromXml(node.ChildNodes[i]);

            return childFilters;
        }

        /// <summary>
        /// Nested class provides an empty filter - one that always
        /// returns true when called. It never matches explicitly.
        /// </summary>
        [Serializable]
        private sealed class EmptyFilter : TestFilter
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
