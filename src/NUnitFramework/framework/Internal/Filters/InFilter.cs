// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
    /// <summary>
    /// Optimized filter to check in condition using HashSet.
    /// </summary>
    internal sealed class InFilter : TestFilter
    {
        private readonly Func<ITest, string?> _selector;
        private readonly HashSet<string?> _values;
        private readonly string _xmlElementName;

        /// <summary>
        /// Constructs new InFilter.
        /// </summary>
        /// <param name="selector">Selector to get value to check.</param>
        /// <param name="values">Valid values for the filter.</param>
        /// <param name="xmlElementName">The XML element name the original filter used.</param>
        private InFilter(Func<ITest, string?> selector, IEnumerable<string?> values, string xmlElementName)
        {
            _selector = selector;
            _values = new HashSet<string?>(values);
            _xmlElementName = xmlElementName;
        }

        /// <summary>
        /// Tries to optimize OrFilter into InFilter if contained filters are all same and non-regex.
        /// </summary>
        public static bool TryOptimize(OrFilter orFilter, [NotNullWhen(true)] out InFilter? optimized)
        {
            // check if we have all filters accessing same field in OR fashion without regex
            var hashCommonTypeAndNonRegex = orFilter.Filters.Length > 0;
            Type? commonType = null;

            // eagerly build expected value collection
            var expectedValues = new List<string?>(orFilter.Filters.Length);

            // Use foreach-loop against array instead of LINQ for best performance
            foreach (var filter in orFilter.Filters)
            {
                // make sure invariants are valid
                commonType ??= filter.GetType();
                hashCommonTypeAndNonRegex &= commonType == filter.GetType();

                // our supported filter types
                hashCommonTypeAndNonRegex &=
                    filter is ClassNameFilter { IsRegex: false }
                    || filter is FullNameFilter { IsRegex: false }
                    || filter is IdFilter { IsRegex: false }
                    || filter is MethodNameFilter { IsRegex: false }
                    || filter is NamespaceFilter { IsRegex: false }
                    || filter is TestNameFilter { IsRegex: false };

                if (!hashCommonTypeAndNonRegex)
                {
                    break;
                }

                expectedValues.Add(((ValueMatchFilter) filter).ExpectedValue);
            }

            if (hashCommonTypeAndNonRegex)
            {
                // we can transform, all filters in non-empty collection are of same type
                Func<ITest, string?> selector = orFilter.Filters[0] switch
                {
                    ClassNameFilter _ => test => test.ClassName,
                    FullNameFilter _ => test => test.FullName,
                    IdFilter _ => test => test.Id,
                    MethodNameFilter _ => test => test.MethodName,
                    NamespaceFilter _ => test => test.TypeInfo?.Namespace,
                    TestNameFilter _ => test => test.Name,
                    _ => throw new InvalidOperationException("Invalid filter, logic wrong")
                };
                string xmlElementName = orFilter.Filters[0] switch
                {
                    ClassNameFilter _ => ClassNameFilter.XmlElementName,
                    FullNameFilter _ => FullNameFilter.XmlElementName,
                    IdFilter _ => IdFilter.XmlElementName,
                    MethodNameFilter _ => MethodNameFilter.XmlElementName,
                    NamespaceFilter _ => NamespaceFilter.XmlElementName,
                    TestNameFilter _ => TestNameFilter.XmlElementName,
                    _ => throw new InvalidOperationException("Invalid filter, logic wrong")
                };
                optimized = new InFilter(selector, expectedValues, xmlElementName);
                return true;
            }

            optimized = default;
            return false;
        }

        public override bool Match(ITest test)
        {
            return _values.Contains(_selector(test));
        }

        public override TNode AddToXml(TNode parentNode, bool recursive)
        {
            // we transform OrFilter so that's our root
            TNode result = parentNode.AddElement(OrFilter.XmlElementName);

            foreach (var value in _values)
            {
                if (value != null)
                {
                    result.AddElement(_xmlElementName, value);
                }
            }

            return result;
        }
    }
}
