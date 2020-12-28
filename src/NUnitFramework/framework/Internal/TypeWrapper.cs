// ***********************************************************************
// Copyright (c) 2015 Charlie Poole, Rob Prouse
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

#nullable enable

using System;
using System.Linq;
using System.Reflection;
using NUnit.Compatibility;
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
                var baseType = Type.GetTypeInfo().BaseType;

                return baseType != null
                    ? new TypeWrapper(baseType)
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
            get { return Type.GetTypeInfo().Assembly; }
        }

        /// <summary>
        /// Gets the namespace of the Type
        /// </summary>
        public string Namespace
        {
            get { return Type.Namespace; }
        }

        /// <summary>
        /// Gets a value indicating whether the type is abstract.
        /// </summary>
        public bool IsAbstract
        {
            get { return Type.GetTypeInfo().IsAbstract; }
        }

        /// <summary>
        /// Gets a value indicating whether the Type is a generic Type
        /// </summary>
        public bool IsGenericType
        {
            get { return Type.GetTypeInfo().IsGenericType; }
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
            get { return Type.GetTypeInfo().ContainsGenericParameters; }
        }

        /// <summary>
        /// Gets a value indicating whether the Type is a generic Type definition
        /// </summary>
        public bool IsGenericTypeDefinition
        {
            get { return Type.GetTypeInfo().IsGenericTypeDefinition; }
        }

        /// <summary>
        /// Gets a value indicating whether the type is sealed.
        /// </summary>
        public bool IsSealed
        {
            get { return Type.GetTypeInfo().IsSealed; }
        }

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
            return GetConstructor(argTypes) != null;
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
        public MethodInfo[] GetMethodsWithAttribute<T>(bool inherit) where T : class
        {
            if (!inherit)
            {
                return Type
                                  .GetMethods(Reflect.AllMembers | BindingFlags.DeclaredOnly)
                                  .Where(method => method.IsDefined(typeof(T), inherit: false))
                                  .ToArray();
            }

            var methodsByDeclaringType = Type
                                                    .GetMethods(Reflect.AllMembers | BindingFlags.FlattenHierarchy) // FlattenHierarchy is complex to replicate by looping over base types with DeclaredOnly.
                                                    .Where(method => method.IsDefined(typeof(T), inherit: true))
                                                    .ToLookup(method => method.DeclaringType);

            return Type.TypeAndBaseTypes()
                              .Reverse()
                              .SelectMany(declaringType => methodsByDeclaringType[declaringType])
                              .ToArray();
        }
    }
}
