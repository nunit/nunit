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
using System.Reflection;
using NUnit.Compatibility;

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// The ITypeInfo interface is an abstraction of a .NET Type
    /// </summary>
    public interface ITypeInfo : IReflectionInfo
    {
        #region Properties

        /// <summary>
        /// Gets the underlying Type on which this ITypeInfo is based
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Gets the base type of this type as an ITypeInfo
        /// </summary>
        ITypeInfo BaseType { get; }

        /// <summary>
        /// Returns true if the Type wrapped is equal to the argument
        /// </summary>
        bool IsType(Type type);

        /// <summary>
        /// Gets the Name of the Type
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the FullName of the Type
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Gets the assembly in which the type is declared
        /// </summary>
        Assembly Assembly { get; }

        /// <summary>
        /// Gets the Namespace of the Type
        /// </summary>
        string Namespace { get; }

        /// <summary>
        /// Gets a value indicating whether the type is abstract.
        /// </summary>
        bool IsAbstract { get; }

        /// <summary>
        /// Gets a value indicating whether the Type is a generic Type
        /// </summary>
        bool IsGenericType { get; }

        /// <summary>
        /// Gets a value indicating whether the Type has generic parameters that have not been replaced by specific Types.
        /// </summary>
        bool ContainsGenericParameters { get; }

        /// <summary>
        /// Gets a value indicating whether the Type is a generic Type definition
        /// </summary>
        bool IsGenericTypeDefinition { get; }

        /// <summary>
        /// Gets a value indicating whether the type is sealed.
        /// </summary>
        bool IsSealed { get; }

        /// <summary>
        /// Gets a value indicating whether this type is a static class.
        /// </summary>
        bool IsStaticClass { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Get the display name for this typeInfo.
        /// </summary>
        string GetDisplayName();

        /// <summary>
        /// Get the display name for an oject of this type, constructed with specific arguments
        /// </summary>
        string GetDisplayName(object[] args);

        /// <summary>
        /// Returns a Type representing a generic type definition from which this Type can be constructed.
        /// </summary>
        Type GetGenericTypeDefinition();

        /// <summary>
        /// Returns a new ITypeInfo representing an instance of this generic Type using the supplied Type arguments
        /// </summary>
        ITypeInfo MakeGenericType(Type[] typeArgs);

        /// <summary>
        /// Returns a value indicating whether this type has a method with a specified public attribute
        /// </summary>
        bool HasMethodWithAttribute(Type attrType);

        /// <summary>
        /// Returns an array of IMethodInfos for methods of this Type
        /// that match the specified flags.
        /// </summary>
        IMethodInfo[] GetMethods(BindingFlags flags);

        /// <summary>
        /// Gets the public constructor taking the specified argument Types
        /// </summary>
        ConstructorInfo GetConstructor(Type[] argTypes);

        /// <summary>
        /// Returns a value indicating whether this Type has a public constructor taking the specified argument Types.
        /// </summary>
        bool HasConstructor(Type[] argTypes);

        /// <summary>
        /// Construct an object of this Type, using the specified arguments.
        /// </summary>
        object Construct(object[] args);

        #endregion
    }
}
