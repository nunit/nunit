// ***********************************************************************
// Copyright (c) 2008 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************
using System;
using System.Linq;
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
        /// <param name="inherit">if set to <c>true</c> [inherit].</param>
        /// <returns>A list of the given attribute on the given object.</returns>
        public static Attribute[] GetCustomAttributes(object actual, Type attributeType, bool inherit)
        {
#if NETSTANDARD1_4
            if (actual is MemberInfo member)
                return (Attribute[])member.GetCustomAttributes(attributeType, inherit);

            if (actual is Type type)
                return (Attribute[])type.GetTypeInfo().GetCustomAttributes(attributeType, inherit);

            if (actual is ParameterInfo parameter)
                return (Attribute[])parameter.GetCustomAttributes(attributeType, inherit);

            if (actual is Assembly assembly)
                return (Attribute[])assembly.GetCustomAttributes(attributeType);

            var interfaceType = actual?.GetType().GetInterfaces().SingleOrDefault(i => i.FullName == "System.Reflection.ICustomAttributeProvider");
            if (interfaceType != null)
            {
                var method = interfaceType.GetMethod("GetCustomAttributes", new[] { typeof(Type), typeof(bool) });
                return (Attribute[])method.Invoke(actual, new object[] { attributeType, inherit });
            }
#else
            if (actual is ICustomAttributeProvider attrProvider)
                return (Attribute[])attrProvider.GetCustomAttributes(attributeType, inherit);
#endif

            throw new ArgumentException($"Actual value {actual} does not implement ICustomAttributeProvider.", nameof(actual));
        }
    }
}
