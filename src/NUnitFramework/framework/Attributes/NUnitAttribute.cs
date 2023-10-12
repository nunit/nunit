// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework
{
    /// <summary>
    /// Abstract base class for all custom attributes defined by NUnit.
    /// </summary>
    public abstract class NUnitAttribute : Attribute
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public NUnitAttribute()
        {
        }
    }
}
