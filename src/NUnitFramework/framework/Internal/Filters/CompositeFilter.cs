// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
    /// <summary>
    /// A base class for multi-part filters
    /// </summary>
    internal abstract class CompositeFilter : TestFilter
    {
        /// <summary>
        /// Constructs an empty CompositeFilter
        /// </summary>
        public CompositeFilter()
        {
            Filters = new List<TestFilter>();
        }

        /// <summary>
        /// Constructs a CompositeFilter from an array of filters
        /// </summary>
        /// <param name="filters"></param>
        public CompositeFilter( params TestFilter[] filters )
        {
            Filters = new ReadOnlyCollection<TestFilter>(filters);
        }

        /// <summary>
        /// Return a list of the composing filters.
        /// </summary>
        public IList<TestFilter> Filters { get; }

        /// <summary>
        /// Checks whether the CompositeFilter is matched by a test.
        /// </summary>
        /// <param name="test">The test to be matched</param>
        /// <param name="negated">If set to <see langword="true"/> we are carrying a negation through</param>
        public abstract override bool Pass(ITest test, bool negated);

        /// <summary>
        /// Checks whether the CompositeFilter is matched by a test.
        /// </summary>
        /// <param name="test">The test to be matched</param>
        public abstract override bool Match(ITest test);

        /// <summary>
        /// Checks whether the CompositeFilter is explicit matched by a test.
        /// </summary>
        /// <param name="test">The test to be matched</param>
        public abstract override bool IsExplicitMatch(ITest test);

        /// <summary>
        /// Adds an XML node
        /// </summary>
        /// <param name="parentNode">Parent node</param>
        /// <param name="recursive">True if recursive</param>
        /// <returns>The added XML node</returns>
        public override TNode AddToXml(TNode parentNode, bool recursive)
        {
            TNode result = parentNode.AddElement(ElementName);

            if (recursive)
                foreach (ITestFilter filter in Filters)
                    filter.AddToXml(result, true);

            return result;
        }

        /// <summary>
        /// Gets the element name
        /// </summary>
        /// <value>Element name</value>
        protected abstract string ElementName { get; }
    }
}
