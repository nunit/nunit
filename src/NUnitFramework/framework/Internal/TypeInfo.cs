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
using System.Reflection;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// The TypeInfo class wraps a Type so it may be used in
    /// a platform-independent manner.
    /// </summary>
    public class TypeInfo : ITypeInfo
    {
        /// <summary>
        /// Construct a TypeWrapper for a specified Type.
        /// </summary>
        public TypeInfo(Type type)
        {
            Guard.ArgumentNotNull(type, "Type");

            Type = type;
        }

        /// <summary>
        /// Gets the underlying Type on which this TypeWrapper is based.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets the base type of this type as an ITypeInfo
        /// </summary>
        public ITypeInfo BaseType 
        {
            get 
            {
                var baseType = Type.BaseType;

                return baseType != null
                    ? new TypeInfo(baseType)
                    : null; 
            }
        }

        /// <summary>
        /// Gets the Name of the Type
        /// </summary>
        public string Name
        {
            get { return Type.Name; }
        }

        /// <summary>
        /// Gets the FullName of the Type
        /// </summary>
        public string FullName
        {
            get { return Type.FullName;  }
        }

        /// <summary>
        /// Gets the assembly in which the type is declared
        /// </summary>
        public Assembly Assembly
        {
            get { return Type.Assembly; }
        }

        /// <summary>
        /// Gets the namespace of the Type
        /// </summary>
        public string Namespace
        {
            get { return Type.Namespace; }
        }

        /// <summary>
        /// Gets a value indicating whether the Type is a generic Type
        /// </summary>
        public bool IsGenericType
        {
            get { return Type.IsGenericType; }
        }

        /// <summary>
        /// Returns true if the Type wrapped is T
        /// </summary>
        public bool IsType(Type type)
        {
            return Type == type;
        }

        /// <summary>
        /// Gets a value indicating whether the Type has generic parameters that have not been replaced by specific Types.
        /// </summary>
        public bool ContainsGenericParameters
        {
            get { return Type.ContainsGenericParameters; }
        }

        /// <summary>
        /// Gets a value indicating whether the Type is a generic Type definition
        /// </summary>
        public bool IsGenericTypeDefinition
        {
            get { return Type.IsGenericTypeDefinition; }
        }

        /// <summary>
        /// Returns a Type representing a generic type definition from which this Type can be constructed.
        /// </summary>
        public Type GetGenericTypeDefinition()
        {
            return Type.GetGenericTypeDefinition();
        }

        /// <summary>
        /// Returns an array of custom attributes of the specified type applied to this type
        /// </summary>
        public T[] GetCustomAttributes<T>(bool inherit) where T : class
        {
            return (T[])Type.GetCustomAttributes(typeof(T), inherit);
        }

        /// <summary>
        /// Returns an array of IMethodInfos for methods of this Type
        /// that match the specified flags.
        /// </summary>
        public IMethodInfo[] GetMethods(BindingFlags flags)
        {
            var methods = Type.GetMethods(flags);
            var result = new MethodWrapper[methods.Length];

            for (int i = 0; i < methods.Length; i++)
                result[i] = new MethodWrapper(Type, methods[i]);

            return result;
        }
    }
}
