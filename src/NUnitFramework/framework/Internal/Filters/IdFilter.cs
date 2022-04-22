// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
    /// <summary>
    /// IdFilter selects tests based on their id
    /// </summary>
    internal class IdFilter : ValueMatchFilter
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
            return test.Id == ExpectedValue;
        }

        /// <summary>
        /// Gets the element name
        /// </summary>
        /// <value>Element name</value>
        protected override string ElementName
        {
            get { return "id"; }
        }
    }
}
