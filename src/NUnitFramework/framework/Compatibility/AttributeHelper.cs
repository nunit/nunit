// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt
using System;
using System.Reflection;

namespace NUnit.Compatibility
{
    /// <summary>
    /// Provides a platform-independent methods for getting attributes
    /// for use by AttributeConstraint and AttributeExistsConstraint.
    /// </summary>
    public static class AttributeHelper
    {
        /// <summary>
        /// Gets the custom attributes from the given object.
        /// </summary>
        /// <param name="actual">The actual.</param>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <param name="inherit">if set to <see langword="true"/> [inherit].</param>
        /// <returns>A list of the given attribute on the given object.</returns>
        public static Attribute[] GetCustomAttributes(object actual, Type attributeType, bool inherit)
        {
            if (actual is ICustomAttributeProvider attrProvider)
                return (Attribute[])attrProvider.GetCustomAttributes(attributeType, inherit);

            throw new ArgumentException($"Actual value {actual} does not implement ICustomAttributeProvider.", nameof(actual));
        }
    }
}
