// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
    /// <summary>
    /// IdFilter selects tests based on their id
    /// </summary>
    internal sealed class IdFilter : ValueMatchFilter
    {
        /// <summary>
        /// Construct an IdFilter for a single value
        /// </summary>
        /// <param name="id">The id the filter will recognize.</param>
        public IdFilter(string id) : base(id, false) { }

        /// <summary>
        /// Match a test against a single value.
        /// </summary>
        public override bool Match(ITest test)
        {
            // We make a direct test here rather than calling ValueMatchFilter.Match
            // because regular expressions are not supported for ID.
            var testId = test.Id;

            // ids usually differ from the end as we have fixed prefix like 0-
            return testId.Length == ExpectedValue.Length
                   && testId[testId.Length - 1] == ExpectedValue[testId.Length - 1]
                   && testId == ExpectedValue;
        }

        /// <summary>
        /// Gets the element name
        /// </summary>
        /// <value>Element name</value>
        protected override string ElementName => "id";
    }
}
