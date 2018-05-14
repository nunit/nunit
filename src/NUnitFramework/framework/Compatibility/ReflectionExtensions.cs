// ***********************************************************************
// Copyright (c) 2015–2018 Charlie Poole, Rob Prouse
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
#if NET20 || NET35 || NET40
    /// <summary>
    /// Provides NUnit specific extensions to aid in Reflection
    /// across multiple frameworks
    /// </summary>
    /// <remarks>
    /// This version of the class supplies GetTypeInfo() on platforms
    /// that don't support it.
    /// </remarks>
    public static class TypeExtensions
    {
        /// <summary>
        /// GetTypeInfo gives access to most of the Type information we take for granted
        /// on .NET Core and Windows Runtime. Rather than #ifdef different code for different
        /// platforms, it is easiest to just code all platforms as if they worked this way,
        /// thus the simple passthrough.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetTypeInfo(this Type type)
        {
            return type;
        }

        /// <summary>
        /// See <see cref="Delegate.CreateDelegate(Type, MethodInfo)"/>.
        /// </summary>
        public static MethodInfo GetMethodInfo(this Delegate @delegate)
        {
            return @delegate.Method;
        }
    }

    /// <summary>
    /// Extensions for Assembly that are not available in pre-4.5 .NET releases
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// An easy way to get a single custom attribute from an assembly
        /// </summary>
        /// <typeparam name="T">The attribute Type</typeparam>
        /// <param name="assembly">The assembly</param>
        /// <returns>An attribute of Type T</returns>
        public static T GetCustomAttribute<T>(this Assembly assembly) where T : Attribute
        {
            T[] attrs = (T[])assembly.GetCustomAttributes(typeof(T), false);
            return attrs.Length > 0 ? attrs[0] : null;
        }
    }

    /// <summary>
    /// Extensions for MethodInfo that are not available in pre-4.5 .NET releases
    /// </summary>
    public static class MethodInfoExtensions
    {
        /// <summary>
        /// See <see cref="Delegate.CreateDelegate(Type, MethodInfo)"/>.
        /// </summary>
        public static Delegate CreateDelegate(this MethodInfo method, Type type)
        {
            return Delegate.CreateDelegate(type, method);
        }
    }

#elif NETSTANDARD1_6

    /// <summary>
    /// Provides NUnit specific extensions to aid in Reflection
    /// across multiple frameworks
    /// </summary>
    /// <remarks>
    /// This version of the class allows direct calls on Type on
    /// those platforms that would normally require use of
    /// GetTypeInfo().
    /// </remarks>
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns an array of <see cref="Type"/> objects that represent the type arguments of a closed generic type or the type parameters of a generic type definition.
        /// </summary>
        public static Type[] GetGenericArguments(this Type type)
        {
            return type.GetTypeInfo().GetGenericArguments();
        }

        /// <summary>
        /// Searches for a public instance constructor whose parameters match the types in the specified array.
        /// </summary>
        public static ConstructorInfo GetConstructor(this Type type, Type[] types)
        {
            return type.GetTypeInfo().GetConstructor(types);
        }

        /// <summary>
        /// Returns all the public constructors defined for the current <see cref="Type"/>.
        /// </summary>
        public static ConstructorInfo[] GetConstructors(this Type type)
        {
            return type.GetTypeInfo().GetConstructors();
        }

        /// <summary>
        /// Determines whether an instance of a specified type can be assigned to an instance of the current type.
        /// </summary>
        public static bool IsAssignableFrom(this Type type, Type c)
        {
            return type.GetTypeInfo().IsAssignableFrom(c?.GetTypeInfo());
        }

        /// <summary>
        /// Determines whether the specified object is an instance of the current <see cref="Type"/>.
        /// </summary>
        public static bool IsInstanceOfType(this Type type, object o)
        {
            return type.GetTypeInfo().IsInstanceOfType(o);
        }

        /// <summary>
        /// When overridden in a derived class, gets all the interfaces implemented or inherited by the current <see cref="Type"/>.
        /// </summary>
        public static Type[] GetInterfaces(this Type type)
        {
            return type.GetTypeInfo().GetInterfaces();
        }

        /// <summary>
        /// Searches for the specified members, using the specified binding constraints.
        /// </summary>
        public static MemberInfo[] GetMember(this Type type, string name, BindingFlags bindingAttr)
        {
            return type.GetTypeInfo().GetMember(name, bindingAttr);
        }

        /// <summary>
        /// Searches for the members defined for the current <see cref="Type"/>, using the specified binding constraints.
        /// </summary>
        public static MemberInfo[] GetMembers(this Type type, BindingFlags bindingAttr)
        {
            return type.GetTypeInfo().GetMembers(bindingAttr);
        }

        /// <summary>
        /// Searches for the public field with the specified name.
        /// </summary>
        public static FieldInfo GetField(this Type type, string name)
        {
            return type.GetTypeInfo().GetField(name);
        }

        /// <summary>
        /// Searches for the public property with the specified name.
        /// </summary>
        public static PropertyInfo GetProperty(this Type type, string name)
        {
            return type.GetTypeInfo().GetProperty(name);
        }

        /// <summary>
        /// Searches for the specified property, using the specified binding constraints.
        /// </summary>
        public static PropertyInfo GetProperty(this Type type, string name, BindingFlags bindingAttr)
        {
            return type.GetTypeInfo().GetProperty(name, bindingAttr);
        }

        /// <summary>
        /// Searches for the public method with the specified name.
        /// </summary>
        public static MethodInfo GetMethod(this Type type, string name)
        {
            return type.GetTypeInfo().GetMethod(name);
        }

        /// <summary>
        /// GSearches for the specified method, using the specified binding constraints.
        /// </summary>
        public static MethodInfo GetMethod(this Type type, string name, BindingFlags bindingAttr)
        {
            return type.GetTypeInfo().GetMethod(name, bindingAttr);
        }

        /// <summary>
        /// Searches for the specified public method whose parameters match the specified argument types.
        /// </summary>
        public static MethodInfo GetMethod(this Type type, string name, Type[] types)
        {
            return type.GetTypeInfo().GetMethod(name, types);
        }

        /// <summary>
        /// Searches for the methods defined for the current <see cref="Type"/>, using the specified binding constraints.
        /// </summary>
        public static MethodInfo[] GetMethods(this Type type, BindingFlags flags)
        {
            return type.GetTypeInfo().GetMethods(flags);
        }
    }
#endif
}
