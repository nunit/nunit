// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
    /// <summary>
    /// ClassName filter selects tests based on the class FullName
    /// </summary>
    internal class ClassNameFilter : ValueMatchFilter
    {
        /// <summary>
        /// Construct a FullNameFilter for a single name
        /// </summary>
        /// <param name="expectedValue">The name the filter will recognize.</param>
        /// <param name="isRegex">Indicated that the value in <paramref name="expectedValue"/> is a regular expression.</param>
        public ClassNameFilter(string expectedValue, bool isRegex = false) : base(expectedValue, isRegex) { }

        /// <summary>
        /// Match a test against a single value.
        /// </summary>
        public override bool Match(ITest test)
        {
            // tests below the fixture level may have non-null className
            // but we don't want to match them explicitly.
            if (!test.IsSuite || test is ParameterizedMethodSuite || test.ClassName == null)
                return false;

            return Match(test.ClassName);
        }

        /// <summary>
        /// Gets the element name
        /// </summary>
        /// <value>Element name</value>
        protected override string ElementName => "class";
    }
}
