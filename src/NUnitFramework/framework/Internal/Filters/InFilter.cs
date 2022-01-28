// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

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

        /// <summary>
        /// Constructs new InFilter.
        /// </summary>
        /// <param name="selector">Selector to get value to check.</param>
        /// <param name="values">Valid values for the filter.</param>
        public InFilter(Func<ITest, string?> selector, IEnumerable<string?> values)
        {
            _selector = selector;
            _values = new HashSet<string?>(values);
        }

        /// <summary>
        /// Tries to optimize OrFilter into InFilter if contained filters are all same and non-regex.
        /// </summary>
        public static bool TryOptimize(OrFilter orFilter, [NotNullWhen(true)] out InFilter? optimized)
        {
            // check if we have all filters accessing same field in OR fashion without regex
            var hashCommonTypeAndNonRegex = orFilter.Filters.Count > 0;
            Type? commonType = null;

            // eagerly build expected value collection
            var expectedValues = new List<string?>(orFilter.Filters.Count);

            // use for for faster traversal without boxed enumerator
            for (var i = 0; i < orFilter.Filters.Count; i++)
            {
                var filter = orFilter.Filters[i];

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
                optimized = new InFilter(selector, expectedValues);
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
            throw new NotImplementedException();
        }
    }
}
