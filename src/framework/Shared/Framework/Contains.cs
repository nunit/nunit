// *****************************************************
// Copyright 2009, Charlie Poole
//
// Licensed under the Open Software License version 3.0
// *****************************************************

using System;
using System.Collections;
using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
    /// <summary>
    /// Static helper class used in the constraint-based syntax
    /// </summary>
    public class Contains
    {
        /// <summary>
        /// Creates a new SubstringConstraint
        /// </summary>
        /// <param name="substring">The value of the substring</param>
        /// <returns>A SubstringConstraint</returns>
        public static Constraint Substring(string substring)
        {
            return new SubstringConstraint(substring);
        }

        /// <summary>
        /// Creates a new CollectionContainsConstraint.
        /// </summary>
        /// <param name="item">The item that should be found.</param>
        /// <returns>A new CollectionContainsConstraint</returns>
        public static Constraint Item(object item)
        {
            return new CollectionContainsConstraint(item);
        }
    }
}
