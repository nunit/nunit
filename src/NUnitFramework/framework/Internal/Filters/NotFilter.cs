// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal.Filters
{
    /// <summary>
    /// NotFilter negates the operation of another filter
    /// </summary>
    internal class NotFilter : TestFilter
    {
        /// <summary>
        /// Construct a not filter on another filter
        /// </summary>
        /// <param name="baseFilter">The filter to be negated</param>
        public NotFilter( TestFilter baseFilter)
        {
            BaseFilter = baseFilter;
        }

        /// <summary>
        /// Gets the base filter
        /// </summary>
        public TestFilter BaseFilter { get; }

        /// <summary>
        /// Determine if a particular test passes the filter criteria.
        /// </summary>
        /// <param name="test">The test to which the filter is applied</param>
        /// <param name="negated">If set to <see langword="true"/> we are carrying a negation through</param>
        /// <returns>True if the test passes the filter, otherwise false</returns>
        public override bool Pass(ITest test, bool negated)
        {
            return BaseFilter.Pass(test, !negated);
        }

        /// <summary>
        /// Check whether the filter matches a test
        /// </summary>
        /// <param name="test">The test to be matched</param>
        /// <returns>True if it matches, otherwise false</returns>
        public override bool Match( ITest test )
        {
            return !BaseFilter.Match( test );
        }

        /// <summary>
        /// Determine if a test matches the filter explicitly. That is, it must
        /// be a direct match of the test itself or one of its children.
        /// </summary>
        /// <param name="test">The test to which the filter is applied</param>
        /// <returns>True if the test matches the filter explicitly, otherwise false</returns>
        public override bool IsExplicitMatch(ITest test)
        {
            return false;
        }

        /// <summary>
        /// Adds an XML node
        /// </summary>
        /// <param name="parentNode">Parent node</param>
        /// <param name="recursive">True if recursive</param>
        /// <returns>The added XML node</returns>
        public override TNode AddToXml(TNode parentNode, bool recursive)
        {
            TNode result = parentNode.AddElement("not");
            if (recursive)
                BaseFilter.AddToXml(result, true);
            return result;
        }
    }
}
