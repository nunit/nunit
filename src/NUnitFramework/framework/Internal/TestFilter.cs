// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using System.Xml;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Filters;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Interface to be implemented by filters applied to tests.
    /// The filter applies when running the test, after it has been
    /// loaded, since this is the only time an ITest exists.
    /// </summary>
#if !PORTABLE && !NETSTANDARD1_6
    [Serializable]
#endif
    public abstract class TestFilter : ITestFilter
    {
        /// <summary>
        /// Unique Empty filter.
        /// </summary>
        public readonly static TestFilter Empty = new EmptyFilter();

        /// <summary>
        /// Indicates whether this is the EmptyFilter
        /// </summary>
        public bool IsEmpty
        {
            get { return this is TestFilter.EmptyFilter; }
        }

        /// <summary>
        /// Indicates whether this is a top-level filter,
        /// not contained in any other filter.
        /// </summary>
        public bool TopLevel { get; set; }

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
            return Match(test) || MatchParent(test) || MatchDescendant(test);
        }

        /// <summary>
        /// Determine if a test matches the filter explicitly. That is, it must
        /// be a direct match of the test itself or one of it's children.
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
            if (test.Tests == null)
                return false;

            foreach (ITest child in test.Tests)
            {
                if (Match(child) || MatchDescendant(child))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Create a TestFilter instance from an xml representation.
        /// </summary>
        public static TestFilter FromXml(string xmlText)
        {
            TNode topNode = TNode.FromXml(xmlText);

            if (topNode.Name != "filter")
                throw new Exception("Expected filter element at top level");

            int count = topNode.ChildNodes.Count;

            TestFilter filter = count == 0
                ? TestFilter.Empty
                : count == 1
                    ? FromXml(topNode.FirstChild)
                    : FromXml(topNode);

            filter.TopLevel = true;

            return filter;
        }

        /// <summary>
        /// Create a TestFilter from it's TNode representation
        /// </summary>
        public static TestFilter FromXml(TNode node)
        {
            bool isRegex = node.Attributes["re"] == "1";

            switch (node.Name)
            {
                case "filter":
                case "and":
                    var andFilter = new AndFilter();
                    foreach (var childNode in node.ChildNodes)
                        andFilter.Add(FromXml(childNode));
                    return andFilter;

                case "or":
                    var orFilter = new OrFilter();
                    foreach (var childNode in node.ChildNodes)
                        orFilter.Add(FromXml(childNode));
                    return orFilter;

                case "not":
                    return new NotFilter(FromXml(node.FirstChild));

                case "id":
                    return new IdFilter(node.Value); 

                case "test":
                    return new FullNameFilter(node.Value) { IsRegex = isRegex };

                case "name":
                    return new TestNameFilter(node.Value) { IsRegex = isRegex };

                case "method":
                    return new MethodNameFilter(node.Value) { IsRegex = isRegex };

                case "class":
                    return new ClassNameFilter(node.Value) { IsRegex = isRegex };

                case "namespace":
                    return new NamespaceFilter(node.Value) { IsRegex = isRegex };

                case "cat":
                    return new CategoryFilter(node.Value) { IsRegex = isRegex };

                case "prop":
                    string name = node.Attributes["name"];
                    if (name != null)
                        return new PropertyFilter(name, node.Value) { IsRegex = isRegex };
                    break;
            }

            throw new ArgumentException("Invalid filter element: " + node.Name, "xmlNode");
        }

        /// <summary>
        /// Nested class provides an empty filter - one that always
        /// returns true when called. It never matches explicitly.
        /// </summary>
#if !PORTABLE && !NETSTANDARD1_6
        [Serializable]
#endif
        private class EmptyFilter : TestFilter
        {
            public override bool Match( ITest test )
            {
                return true;
            }

            public override bool Pass( ITest test )
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
