// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable
using System.Collections;
using System.Linq;

namespace NUnit.Framework.Internal.Filters
{
    /// <summary>
    /// PropertyFilter is able to select or exclude tests
    /// based on their properties.
    /// </summary>
    internal class PropertyFilter : ValueMatchFilter
    {
        private readonly string _propertyName;

        /// <summary>
        /// Construct a PropertyFilter using a property name and expected value
        /// </summary>
        /// <param name="propertyName">A property name</param>
        /// <param name="expectedValue">The expected value of the property</param>
        /// <param name="isRegex">Indicated that the value in <paramref name="expectedValue"/> is a regular expression.</param>
        public PropertyFilter(string propertyName, string expectedValue, bool isRegex = false) : base(expectedValue, isRegex)
        {
            _propertyName = propertyName;
        }

        /// <summary>
        /// Check whether the filter matches a test
        /// </summary>
        /// <param name="test">The test to be matched</param>
        public override bool Match(ITest test)
        {
            IList values = test.Properties[_propertyName];

            return values.Cast<string>().Any(Match);
        }

        /// <summary>
        /// Adds an XML node
        /// </summary>
        /// <param name="parentNode">Parent node</param>
        /// <param name="recursive">True if recursive</param>
        /// <returns>The added XML node</returns>
        public override TNode AddToXml(TNode parentNode, bool recursive)
        {
            TNode result = base.AddToXml(parentNode, recursive);
            result.AddAttribute("name", _propertyName);
            return result;
        }

        /// <summary>
        /// Gets the element name
        /// </summary>
        /// <value>Element name</value>
        protected override string ElementName => "prop";
    }
}
