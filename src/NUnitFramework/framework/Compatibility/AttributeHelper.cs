// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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
        /// <remarks>Portable libraries do not have an ICustomAttributeProvider, so we need to cast to each of
        /// it's direct subtypes and try to get attributes off those instead.</remarks>
        /// <param name="actual">The actual.</param>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <param name="inherit">if set to <c>true</c> [inherit].</param>
        /// <returns>A list of the given attribute on the given object.</returns>
        public static Attribute[] GetCustomAttributes(object actual, Type attributeType, bool inherit)
        {
#if !NETSTANDARD1_3
            var attrProvider = actual as ICustomAttributeProvider;
            if (attrProvider == null)
                throw new ArgumentException(string.Format("Actual value {0} does not implement ICustomAttributeProvider.", actual), "actual");
            
            return (Attribute[])attrProvider.GetCustomAttributes(attributeType, inherit);
#else
            var member = actual as MemberInfo;
            if (member != null)
            {
                return (Attribute[])member.GetCustomAttributes(attributeType, inherit);
            }

            var param = actual as ParameterInfo;
            if (param != null)
            {
                return (Attribute[])param.GetCustomAttributes(attributeType, inherit);
            }

            var assembly = actual as Assembly;
            if (assembly != null)
            {
                return (Attribute[])assembly.GetCustomAttributes(attributeType, inherit);
            }

            throw new ArgumentException(string.Format("Actual value {0} must be a MemberInfo, ParameterInfo or Assembly.", actual), "actual");
#endif
            }

        }
}