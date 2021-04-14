// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
    /// <summary>
    /// Combines multiple filters so that a test must pass one
    /// of them in order to pass this filter.
    /// </summary>
    internal class OrFilter : CompositeFilter
    {
        private bool _matchFullName;
        private readonly HashSet<string> _fullNames;

        /// <summary>
        /// Constructs an empty OrFilter
        /// </summary>
        public OrFilter() { }

        /// <summary>
        /// Constructs an OrFilter from an array of filters
        /// </summary>
        /// <param name="filters"></param>
        public OrFilter(params TestFilter[] filters) : base(filters)
        {
            _matchFullName = filters.Length > 0;

            // Try to reduce inner filters to a hash set of full names
            // as it's a common case when running using VSTest
            foreach (var filter in filters)
            {
                if (filter is FullNameFilter {IsRegex: false} fullNameFilter)
                {
                    _fullNames ??= new HashSet<string>();
                    _fullNames.Add(fullNameFilter.ExpectedValue);
                }
                else
                {
                    _matchFullName = false;
                    break;
                }
            }
        }

        /// <summary>
        /// Checks whether the OrFilter is matched by a test
        /// </summary>
        /// <param name="test">The test to be matched</param>
        /// <param name="negated">If set to <see langword="true"/> we are carrying a negation through</param>
        /// <returns>True if any of the component filters pass, otherwise false</returns>
        public override bool Pass( ITest test, bool negated )
        {
            // If we are in optimized matching mode don't delegate to child filters
            if (_matchFullName)
            {
                if (negated)
                    return !Match(test) && !MatchParent(test);

                return Match(test) || MatchParent(test) || MatchDescendant(test);
            }

            if (negated)
            {
                foreach (var filter in Filters)
                {
                    if (!filter.Pass(test, negated: true))
                    {
                        return false;
                    }
                }

                return true;
            }

            foreach (var f in Filters)
            {
                if (f.Pass(test, negated))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks whether the OrFilter is matched by a test
        /// </summary>
        /// <param name="test">The test to be matched</param>
        /// <returns>True if any of the component filters match, otherwise false</returns>
        public override bool Match( ITest test )
        {
            if (_matchFullName)
            {
                return _fullNames.Contains(test.FullName);
            }

            foreach( TestFilter filter in Filters )
                if ( filter.Match( test ) )
                    return true;

            return false;
        }

        /// <summary>
        /// Checks whether the OrFilter is explicit matched by a test
        /// </summary>
        /// <param name="test">The test to be matched</param>
        /// <returns>True if any of the component filters explicit match, otherwise false</returns>
        public override bool IsExplicitMatch( ITest test )
        {
            foreach( TestFilter filter in Filters )
                if ( filter.IsExplicitMatch( test ) )
                    return true;

            return false;
        }

        /// <summary>
        /// Gets the element name
        /// </summary>
        /// <value>Element name</value>
        protected override string ElementName
        {
            get { return "or"; }
        }
    }
}
