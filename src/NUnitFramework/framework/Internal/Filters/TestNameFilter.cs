// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
    /// <summary>
    /// TestName filter selects tests based on their Name
    /// </summary>
    internal sealed class TestNameFilter : ValueMatchFilter
    {
        internal const string XmlElementName = "name";

        /// <summary>
        /// Construct a TestNameFilter for a single name
        /// </summary>
        /// <param name="expectedValue">The name the filter will recognize.</param>
        /// <param name="isRegex">Indicated that the value in <paramref name="expectedValue"/> is a regular expression.</param>
        public TestNameFilter(string expectedValue, bool isRegex = false) : base(expectedValue, isRegex)
        {
        }

        /// <summary>
        /// Match a test against a single value.
        /// </summary>
        public override bool Match(ITest test)
        {
            return Match(test.Name);
        }

        /// <summary>
        /// Gets the element name
        /// </summary>
        /// <value>Element name</value>
        protected override string ElementName => XmlElementName;
    }
}
