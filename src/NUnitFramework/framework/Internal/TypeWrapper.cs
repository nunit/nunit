// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// The TypeWrapper class wraps a Type so it may be used in
    /// a platform-independent manner.
    /// </summary>
    public class TypeWrapper : ITypeInfo
    {
        /// <summary>
        /// Construct a TypeWrapper for a specified Type.
        /// </summary>
        public TypeWrapper(Type type)
        {
            Guard.ArgumentNotNull(type, nameof(Type));

            Type = type;
        }

        /// <summary>
        /// Gets the underlying Type on which this TypeWrapper is based.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets the base type of this type as an ITypeInfo
        /// </summary>
        public ITypeInfo? BaseType
        {
            get
            {
                var baseType = Type.BaseType;

                return baseType is not null
                    ? new TypeWrapper(baseType)
                    : null;
            }
        }

        /// <summary>
        /// Gets the Name of the Type
        /// </summary>
        public string Name => Type.Name;

        /// <summary>
        /// Gets the FullName of the Type
        /// </summary>
        public string FullName => Type.FullName();

        /// <summary>
        /// Gets the assembly in which the type is declared
        /// </summary>
        public Assembly Assembly => Type.Assembly;

        /// <summary>
        /// Gets the namespace of the Type
        /// </summary>
        public string? Namespace => Type.Namespace;

        /// <summary>
        /// Gets a value indicating whether the type is abstract.
        /// </summary>
        public bool IsAbstract => Type.IsAbstract;

        /// <summary>
        /// Gets a value indicating whether the Type is a generic Type
        /// </summary>
        public bool IsGenericType => Type.IsGenericType;

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
        public bool ContainsGenericParameters => Type.ContainsGenericParameters;

        /// <summary>
        /// Gets a value indicating whether the Type is a generic Type definition
        /// </summary>
        public bool IsGenericTypeDefinition => Type.IsGenericTypeDefinition;

        /// <summary>
        /// Gets a value indicating whether the type is sealed.
        /// </summary>
        public bool IsSealed => Type.IsSealed;

        /// <summary>
        /// Gets a value indicating whether this type represents a static class.
        /// </summary>
        public bool IsStaticClass => Type.IsStatic();

        /// <summary>
        /// Get the display name for this type
        /// </summary>
        public string GetDisplayName()
        {
            return TypeHelper.GetDisplayName(Type);
        }

        /// <summary>
        /// Get the display name for an object of this type, constructed with the specified args.
        /// </summary>
        public string GetDisplayName(object?[]? args)
        {
            return TypeHelper.GetDisplayName(Type, args);
        }

        /// <summary>
        /// Returns a new ITypeInfo representing an instance of this generic Type using the supplied Type arguments
        /// </summary>
        public ITypeInfo MakeGenericType(Type[] typeArgs)
        {
            return new TypeWrapper(Type.MakeGenericType(typeArgs));
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
            return Type.GetAttributes<T>(inherit);
        }

        /// <summary>
        /// Returns a value indicating whether the type has an attribute of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public bool IsDefined<T>(bool inherit) where T : class
        {
            return Type.HasAttribute<T>(inherit);
        }

        /// <summary>
        /// Returns a flag indicating whether this type has a method with an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public bool HasMethodWithAttribute(Type attributeType)
        {
            return Reflect.HasMethodWithAttribute(Type, attributeType);
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

        /// <summary>
        /// Gets the public constructor taking the specified argument Types
        /// </summary>
        public ConstructorInfo? GetConstructor(Type[] argTypes)
        {
            return Type.GetConstructor(argTypes);
        }

        /// <summary>
        /// Returns a value indicating whether this Type has a public constructor taking the specified argument Types.
        /// </summary>
        public bool HasConstructor(Type[] argTypes)
        {
            return GetConstructor(argTypes) is not null;
        }

        /// <summary>
        /// Construct an object of this Type, using the specified arguments.
        /// </summary>
        public object Construct(object?[]? args)
        {
            return Reflect.Construct(Type, args);
        }

        /// <summary>
        /// Override ToString() so that error messages in NUnit's own tests make sense
        /// </summary>
        public override string ToString()
        {
            return Type.ToString();
        }

        /// <summary>
        /// Returns all methods declared by this type that have the specified attribute, optionally
        /// including base classes. Methods from a base class are always returned before methods from a class that
        /// inherits from it.
        /// </summary>
        /// <param name="inherit">Specifies whether to search the fixture type inheritance chain.</param>
        public IMethodInfo[] GetMethodsWithAttribute<T>(bool inherit) where T : class
        {
            if (!inherit)
            {
                return Type
                    .GetMethods(Reflect.AllMembers | BindingFlags.DeclaredOnly)
                    .Where(method => method.IsDefined(typeof(T), inherit: false))
                    .Select(method => new MethodWrapper(Type, method))
                    .ToArray();
            }

            var methodsByDeclaringType = Type
                .GetMethods(Reflect.AllMembers | BindingFlags.FlattenHierarchy) // FlattenHierarchy is complex to replicate by looping over base types with DeclaredOnly.
                .Where(method => method.IsDefined(typeof(T), inherit: true))
                .ToLookup(method => method.DeclaringType);

            return Type.TypeAndBaseTypes()
                .Reverse()
                .SelectMany(declaringType => methodsByDeclaringType[declaringType].Select(method => new MethodWrapper(declaringType, method)))
                .ToArray();
        }
    }
}
