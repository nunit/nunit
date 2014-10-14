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
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Filters;

#if !NETCF && !SILVERLIGHT && !PORTABLE
using XmlNode = System.Xml.XmlNode;
#endif

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Interface to be implemented by filters applied to tests.
    /// The filter applies when running the test, after it has been
    /// loaded, since this is the only time an ITest exists.
    /// </summary>
    [Serializable]
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
        /// Determine if a particular test passes the filter criteria. The default 
        /// implementation checks the test itself, its parents and any descendants.
        /// 
        /// Derived classes may override this method or any of the Match methods
        /// to change the behavior of the filter.
        /// </summary>
        /// <param name="test">The test to which the filter is applied</param>
        /// <returns>True if the test passes the filter, otherwise false</returns>
        public virtual bool Pass( ITest test )
        {
            return Match(test) || MatchParent(test) || MatchDescendant(test);
        }

        /// <summary>
        /// Determine whether the test itself matches the filter criteria, without
        /// examining either parents or descendants.
        /// </summary>
        /// <param name="test">The test to which the filter is applied</param>
        /// <returns>True if the filter matches the any parent of the test</returns>
        public abstract bool Match(ITest test);

        /// <summary>
        /// Determine whether any ancestor of the test matches the filter criteria
        /// </summary>
        /// <param name="test">The test to which the filter is applied</param>
        /// <returns>True if the filter matches the an ancestor of the test</returns>
        protected virtual bool MatchParent(ITest test)
        {
            return (test.RunState != RunState.Explicit && test.Parent != null &&
                (Match(test.Parent) || MatchParent(test.Parent)));
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

#if !NETCF && !SILVERLIGHT && !PORTABLE
        /// <summary>
        /// Create a TestFilter instance from an xml representation.
        /// </summary>
        /// <param name="xmlText"></param>
        /// <returns></returns>
        public static TestFilter FromXml(string xmlText)
        {
            var doc = new System.Xml.XmlDocument();
            doc.LoadXml(xmlText);
            var topNode = doc.FirstChild;

            if (topNode.Name != "filter")
                throw new Exception("Expected filter element at top level");

            switch (topNode.ChildNodes.Count)
            {
                case 0:
                    return TestFilter.Empty;

                case 1:
                    return FromXml(topNode.FirstChild);

                default:
                    return FromXml(topNode);
            }
        }

        private static readonly char[] COMMA = new char[] { ',' };

        private static TestFilter FromXml(XmlNode xmlNode)
        {
            switch (xmlNode.Name)
            {
                case "filter":
                case "and":
                    var andFilter = new AndFilter();
                    foreach (XmlNode childNode in xmlNode.ChildNodes)
                        andFilter.Add(FromXml(childNode));
                    return andFilter;

                case "or":
                    var orFilter = new OrFilter();
                    foreach (System.Xml.XmlNode childNode in xmlNode.ChildNodes)
                        orFilter.Add(FromXml(childNode));
                    return orFilter;

                case "not":
                    return new NotFilter(FromXml(xmlNode.FirstChild));

                case "id":
                    var idFilter = new IdFilter();
                    foreach (string id in xmlNode.InnerText.Split(COMMA))
                        idFilter.Add(int.Parse(id));
                    return idFilter;

                case "tests":
                    var testFilter = new SimpleNameFilter();
                    foreach (XmlNode childNode in xmlNode.SelectNodes("test"))
                        testFilter.Add(childNode.InnerText);
                    return testFilter;

                case "cat":
                    var catFilter = new CategoryFilter();
                    foreach (string cat in xmlNode.InnerText.Split(COMMA))
                        catFilter.AddCategory(cat);
                    return catFilter;

                default:
                    throw new ArgumentException("Invalid filter element: " + xmlNode.Name, "xmlNode");
            }
        }
#endif

        /// <summary>
        /// Nested class provides an empty filter - one that always
        /// returns true when called, unless the test is marked explicit.
        /// </summary>
        [Serializable]
        private class EmptyFilter : TestFilter
        {
            public override bool Match( ITest test )
            {
                return test.RunState != RunState.Explicit;
            }

            public override bool Pass( ITest test )
            {
                return test.RunState != RunState.Explicit;
            }
        }
    }
}
