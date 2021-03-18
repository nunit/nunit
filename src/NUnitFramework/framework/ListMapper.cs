// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Compatibility;

namespace NUnit.Framework
{
    /// <summary>
    /// ListMapper is used to transform a collection used as an actual argument
    /// producing another collection to be used in the assertion.
    /// </summary>
    [Obsolete("The ListMapper class has been deprecated and will be removed in a future release. "
        + "Please use the extension method System.Linq.Enumerable.Select instead.")]
    public class ListMapper
    {
        readonly ICollection original;

        /// <summary>
        /// Construct a ListMapper based on a collection
        /// </summary>
        /// <param name="original">The collection to be transformed</param>
        public ListMapper( ICollection original )
        {
            this.original = original;
        }

        /// <summary>
        /// Produces a collection containing all the values of a property
        /// </summary>
        /// <param name="name">The collection of property values</param>
        /// <returns></returns>
        public ICollection Property( string name )
        {
            var propList = new List<object>();
            foreach( object item in original )
            {
                PropertyInfo property = item.GetType().GetProperty( name,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
                if ( property == null )
                    throw new ArgumentException( string.Format(
                        "{0} does not have a {1} property", item, name ) );

                propList.Add( property.GetValue( item, null ) );
            }

            return propList;
        }
    }
}
