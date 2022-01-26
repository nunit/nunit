// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
    /// <summary>
    /// Optimized filter to check in condition using HashSet.
    /// </summary>
    internal sealed class InFilter : TestFilter
    {
        private readonly Func<ITest, string> _selector;
        private readonly HashSet<string> _values;

        /// <summary>
        /// Constructs new InFilter.
        /// </summary>
        /// <param name="selector">Selector to get value to check.</param>
        /// <param name="values">Valid values for the filter.</param>
        public InFilter(Func<ITest,string> selector, IEnumerable<string> values)
        {
            _selector = selector;
            _values = new HashSet<string>(values);
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
