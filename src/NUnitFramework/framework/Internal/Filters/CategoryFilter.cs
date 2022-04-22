// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
    /// <summary>
    /// CategoryFilter is able to select or exclude tests
    /// based on their categories.
    /// </summary>
    internal class CategoryFilter : ValueMatchFilter
    {
        /// <summary>
        /// Construct a CategoryFilter using a single category name
        /// </summary>
        /// <param name="name">A category name</param>
        /// <param name="isRegex">Indicated that the value in <paramref name="name"/> is a regular expression.</param>
        public CategoryFilter(string name, bool isRegex = false) : base(name, isRegex) { }

        /// <summary>
        /// Check whether the filter matches a test
        /// </summary>
        /// <param name="test">The test to be matched</param>
        /// <returns></returns>
        public override bool Match(ITest test)
        {
            IList testCategories = test.Properties[PropertyNames.Category];

            if ( testCategories != null)
                foreach (string cat in testCategories)
                    if ( Match(cat))
                        return true;

            return false;
        }

        /// <summary>
        /// Gets the element name
        /// </summary>
        /// <value>Element name</value>
        protected override string ElementName
        {
            get { return "cat"; }
        }
    }
}
