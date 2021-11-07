// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
    /// <summary>
    /// FullName filter selects tests based on their FullName
    /// </summary>
    internal class FullNameFilter : ValueMatchFilter
    {
        /// <summary>
        /// Construct a FullNameFilter for a single name
        /// </summary>
        /// <param name="expectedValue">The name the filter will recognize.</param>
        public FullNameFilter(string expectedValue) : base(expectedValue) { }

        /// <summary>
        /// Match a test against a single value.
        /// </summary>
        public override bool Match(ITest test)
        {
            return Match(test.FullName);
        }

        /// <summary>
        /// Gets the element name
        /// </summary>
        /// <value>Element name</value>
        protected override string ElementName
        {
            get { return "test"; }
        }
    }
}
