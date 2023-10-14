// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Provides the descriptive text relating to the assembly, test fixture or test method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public sealed class DescriptionAttribute : PropertyAttribute
    {
        /// <summary>
        /// Construct a description Attribute
        /// </summary>
        /// <param name="description">The text of the description</param>
        public DescriptionAttribute(string description) : base(PropertyNames.Description, description)
        {
        }
    }
}
