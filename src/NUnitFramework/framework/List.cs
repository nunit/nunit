// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;

namespace NUnit.Framework
{
    /// <summary>
    /// The List class is a helper class with properties and methods
    /// that supply a number of constraints used with lists and collections.
    /// </summary>
    [Obsolete("The List class has been deprecated and will be removed in a future release. "
        + "Please use the extension method System.Linq.Enumerable.Select instead.")]
    public class List
    {
        /// <summary>
        /// List.Map returns a ListMapper, which can be used to map
        /// the original collection to another collection.
        /// </summary>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static ListMapper Map( ICollection actual )
        {
            return new ListMapper( actual );
        }
    }
}
