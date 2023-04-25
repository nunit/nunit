// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Reflection;

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
        ITypeInfo? BaseType { get; }

        /// <summary>
        /// Returns true if the Type wrapped is equal to the argument
        /// </summary>
        bool IsType(Type type);

        /// <summary>
        /// Gets the name of the Type
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the full name of the Type
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Gets the assembly in which the type is declared
        /// </summary>
        Assembly Assembly { get; }

        /// <summary>
        /// Gets the namespace of the Type
        /// </summary>
        string? Namespace { get; }

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
        /// Get the display name for an object of this type, constructed with specific arguments
        /// </summary>
        string GetDisplayName(object?[]? args);

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
        ConstructorInfo? GetConstructor(Type[] argTypes);

        /// <summary>
        /// Returns a value indicating whether this Type has a public constructor taking the specified argument Types.
        /// </summary>
        bool HasConstructor(Type[] argTypes);

        /// <summary>
        /// Construct an object of this Type, using the specified arguments.
        /// </summary>
        object Construct(object?[]? args);

        #endregion

        /// <summary>
        /// Returns all methods declared by this type that have the specified attribute, optionally
        /// including base classes. Methods from a base class are always returned before methods from a class that
        /// inherits from it.
        /// </summary>
        /// <param name="inherit">Specifies whether to search the fixture type inheritance chain.</param>
        IMethodInfo[] GetMethodsWithAttribute<T>(bool inherit) where T : class;
    }
}
