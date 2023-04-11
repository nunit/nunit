// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
    /// <summary>
    /// ClassName filter selects tests based on the class FullName
    /// </summary>
    internal sealed class NamespaceFilter : ValueMatchFilter
    {
        internal const string XmlElementName = "namespace";

        /// <summary>
        /// Construct a NamespaceFilter for a single namespace
        /// </summary>
        /// <param name="expectedValue">The namespace the filter will recognize.</param>
        /// <param name="isRegex">Indicated that the value in <paramref name="expectedValue"/> is a regular expression.</param>
        public NamespaceFilter(string expectedValue, bool isRegex = false) : base(expectedValue, isRegex) { }

        /// <summary>
        /// Match a test against a single value.
        /// </summary>
        public override bool Match(ITest test)
        {
            string? containingNamespace = null;

            if (test.TypeInfo != null)
            {
                containingNamespace = test.TypeInfo.Namespace;
            }

            return Match(containingNamespace);
        }

        /// <summary>
        /// Gets the element name
        /// </summary>
        /// <value>Element name</value>
        protected override string ElementName => XmlElementName;
    }
}
