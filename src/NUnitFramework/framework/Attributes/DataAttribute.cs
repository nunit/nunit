// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework
{
    /// <summary>
    /// Abstract base class for all data-providing attributes defined by NUnit.
    /// Used to select all data sources for a method, class or parameter.
    /// </summary>
    [Obsolete("The DataAttribute class has been deprecated and will be removed in a future release. "
        + "Please use " + nameof(IParameterDataSource) + " instead.")]
    public abstract class DataAttribute : NUnitAttribute
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public DataAttribute() { }
    }
}
