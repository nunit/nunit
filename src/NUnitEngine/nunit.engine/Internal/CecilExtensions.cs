// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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
using System.Collections.Generic;
using System.Text;
using Mono.Cecil;

namespace NUnit.Engine.Internal
{
    /// <summary>
    /// Extension methods that make it easier to work with Mono.Cecil.
    /// </summary>
    public static class CecilExtensions
    {
        #region TypeDefinition Extensions

        public static List<CustomAttribute> GetAttributes(this TypeDefinition type, string fullName)
        {
            var attributes = new List<CustomAttribute>();

            foreach (CustomAttribute attr in type.CustomAttributes)
                if (attr.AttributeType.FullName == fullName)
                    attributes.Add(attr);

            return attributes;
        }

        public static CustomAttribute GetAttribute(this TypeDefinition type, string fullName)
        {
            foreach (CustomAttribute attr in type.CustomAttributes)
                if (attr.AttributeType.FullName == fullName)
                    return attr;

            return null;
        }

        #endregion

        #region CustomAttribute Extensions

        public static object GetNamedArgument(this CustomAttribute attr, string name)
        {
            foreach (var property in attr.Properties)
                if (property.Name == name)
                    return property.Argument.Value;

            return null;
        }

        #endregion
    }
}
