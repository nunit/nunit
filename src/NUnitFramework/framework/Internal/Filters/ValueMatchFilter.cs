// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Text.RegularExpressions;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
    /// <summary>
    /// ValueMatchFilter selects tests based on some value, which
    /// is expected to be contained in the test.
    /// </summary>
    internal abstract class ValueMatchFilter : TestFilter
    {
        private readonly Regex? _regex;

        /// <summary>
        /// Returns the value matched by the filter - used for testing
        /// </summary>
        public string ExpectedValue { get; }

        /// <summary>
        /// Indicates whether the value is a regular expression
        /// </summary>
        public bool IsRegex => _regex is not null;

        /// <summary>
        /// Construct a ValueMatchFilter for a single value.
        /// </summary>
        /// <param name="expectedValue">The value to be included.</param>
        /// <param name="isRegex">Indicated that the value in <paramref name="expectedValue"/> is a regular expression.</param>
        protected ValueMatchFilter(string expectedValue, bool isRegex)
        {
            ExpectedValue = expectedValue;
            if (isRegex)
            {
                _regex = new Regex(expectedValue, RegexOptions.Compiled);
            }
        }

        /// <summary>
        /// Match the input provided by the derived class
        /// </summary>
        /// <param name="input">The value to be matched</param>
        /// <returns>True for a match, false otherwise.</returns>
        protected bool Match(string? input)
        {
            if (_regex is null)
                return ExpectedValue == input;
            else
                return input is not null && _regex.IsMatch(input);
        }

        /// <summary>
        /// Adds an XML node
        /// </summary>
        /// <param name="parentNode">Parent node</param>
        /// <param name="recursive">True if recursive</param>
        /// <returns>The added XML node</returns>
        public override TNode AddToXml(TNode parentNode, bool recursive)
        {
            TNode result = parentNode.AddElement(ElementName, ExpectedValue);
            if (_regex is not null)
                result.AddAttribute("re", "1");
            return result;
        }

        /// <summary>
        /// Gets the element name
        /// </summary>
        /// <value>Element name</value>
        protected abstract string ElementName { get; }
    }
}
