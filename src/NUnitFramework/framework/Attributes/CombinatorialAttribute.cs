// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework
{
    /// <summary>
    /// Marks a test to use a combinatorial join of any argument data provided.
    /// Since this is the default, the attribute is optional.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CombinatorialAttribute : CombiningStrategyAttribute
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public CombinatorialAttribute() : base(new CombinatorialStrategy(), new ParameterDataSourceProvider()) { }
    }
}
