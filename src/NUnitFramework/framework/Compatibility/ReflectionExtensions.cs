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

#if PORTABLE
using System.Linq;
#elif NET_2_0
namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Enables compiling extension methods in .NET 2.0
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class ExtensionAttribute : Attribute { }
}
#endif

namespace NUnit.Framework.Compatibility
{
#if !NET_4_5 && !PORTABLE
    /// <summary>
    /// Provides NUnit specific extensions to aid in Reflection
    /// across multiple frameworks
    /// </summary>
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
    }

#elif PORTABLE

    /// <summary>
    /// Provides NUnit specific extensions to aid in Reflection
    /// across multiple frameworks
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns an array of generic arguments for the give type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type[] GetGenericArguments(this Type type)
        {
            var info = type.GetTypeInfo();
            return info.GenericTypeArguments.Concat(info.GenericTypeParameters).ToArray();
        }

        /// <summary>
        /// Gets the constructor with the given parameter types
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ptypes"></param>
        /// <returns></returns>
        public static ConstructorInfo GetConstructor(this Type type, Type[] ptypes)
        {
            return type.GetConstructors()
                .Where(c => c.GetParameters().ParametersMatch(ptypes))
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the constructors for a type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IList<ConstructorInfo> GetConstructors(this Type type)
        {
            return type.GetTypeInfo()
                .DeclaredConstructors
                .Where(c => c.IsPublic && !c.IsStatic)
                .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool IsAssignableFrom(this Type type, Type other)
        {
            return other != null && type.GetTypeInfo().IsAssignableFrom(other.GetTypeInfo());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool IsInstanceOfType(this Type type, object other)
        {
            return other != null && type.IsAssignableFrom(other.GetType());
        }

        /// <summary>
        /// Gets declared or inherited interfaces on this type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type[] GetInterfaces(this Type type)
        {
            return type.GetTypeInfo().ImplementedInterfaces.ToArray();
        }

        /// <summary>
        /// Gets the member on a given type by name. BindingFlags ARE IGNORED.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="ignored"></param>
        /// <returns></returns>
        public static MemberInfo[] GetMember(this Type type, string name, BindingFlags ignored)
        {
            return type.GetMembers(ignored)
                .Where(m => m.Name == name)
                .ToArray();
        }

        /// <summary>
        /// Gets all members on a given type. BindingFlags ARE IGNORED.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ignored"></param>
        /// <returns></returns>
        public static IEnumerable<MemberInfo> GetMembers(this Type type, BindingFlags ignored)
        {
            // We only use this in two places and ask for public, private, static and instance
            // members. Since none of that info is available on MemberInfo, I am skipping and
            // returning all.
            return type.GetTypeInfo().DeclaredMembers;
        }

        /// <summary>
        /// Gets field of the given name on the type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static FieldInfo GetField(this Type type, string name)
        {
            return type.GetTypeInfo()
                .DeclaredFields
                .Where(p => (p.Name == name))
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets property of the given name on the type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PropertyInfo GetProperty(this Type type, string name)
        {
            return type.GetRuntimeProperties()
                .Where(p => ((p.GetMethod != null && p.GetMethod.IsPublic) || (p.SetMethod != null && p.SetMethod.IsPublic)) && p.Name == name)
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets property of the given name on the type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static PropertyInfo GetProperty(this Type type, string name, BindingFlags flags)
        {
            return type.GetRuntimeProperties()
                .ApplyBindingFlags(flags)
                .Where(p => p.Name == name)
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the method with the given name and parameter list
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static MethodInfo GetMethod(this Type type, string name)
        {
            return type.GetMethods()
                .Where(m => m.Name == name)
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the method with the given name and parameter list
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static MethodInfo GetMethod(this Type type, string name, BindingFlags flags)
        {
            return type.GetMethods(flags)
                .Where(m => m.Name == name)
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the method with the given name and parameter list
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="ptypes"></param>
        /// <returns></returns>
        public static MethodInfo GetMethod(this Type type, string name, Type[] ptypes)
        {
            return type.GetMethods()
                .Where(m => m.Name == name && m.GetParameters().ParametersMatch(ptypes))
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets public methods on the given type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MethodInfo[] GetMethods(this Type type)
        {
            return type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
        }

        /// <summary>
        /// Gets methods on a type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static MethodInfo[] GetMethods(this Type type, BindingFlags flags)
        {
            bool declaredOnly = flags.HasFlag(BindingFlags.DeclaredOnly);
            IEnumerable<MethodInfo> methods;
            if (declaredOnly)
                methods = type.GetTypeInfo().DeclaredMethods;
            else
                methods = type.GetRuntimeMethods().Where(m => !m.IsConstructor);

            return methods.ApplyBindingFlags(flags).ToArray();
        }

        static IEnumerable<PropertyInfo> ApplyBindingFlags(this IEnumerable<PropertyInfo> infos, BindingFlags flags)
        {
            bool pub = flags.HasFlag(BindingFlags.Public);
            bool priv = flags.HasFlag(BindingFlags.NonPublic);
            if (pub && !priv)
                infos = infos.Where(p => (p.GetMethod != null && p.GetMethod.IsPublic) || (p.SetMethod != null && p.SetMethod.IsPublic));

            bool stat = flags.HasFlag(BindingFlags.Static);
            bool inst = flags.HasFlag(BindingFlags.Instance);
            if (stat && !inst)
                infos = infos.Where(p => (p.GetMethod != null && p.GetMethod.IsStatic) || (p.SetMethod != null && p.SetMethod.IsStatic));
            else if (inst && !stat)
                infos = infos.Where(p => (p.GetMethod != null && !p.GetMethod.IsStatic) || (p.SetMethod != null && !p.SetMethod.IsStatic));

            return infos;
        }

        static IEnumerable<MethodInfo> ApplyBindingFlags(this IEnumerable<MethodInfo> infos, BindingFlags flags)
        {
            bool pub = flags.HasFlag(BindingFlags.Public);
            bool priv = flags.HasFlag(BindingFlags.NonPublic);
            if (priv && !pub)
                infos = infos.Where(m => m.IsPrivate);
            else if (pub && !priv)
                infos = infos.Where(m => m.IsPublic);

            bool stat = flags.HasFlag(BindingFlags.Static);
            bool inst = flags.HasFlag(BindingFlags.Instance);
            if (stat && !inst)
                infos = infos.Where(m => m.IsStatic);
            else if (inst && !stat)
                infos = infos.Where(m => !m.IsStatic);

            return infos;
        }

        static bool ParametersMatch(this ParameterInfo[] pinfos, Type[] ptypes)
        {
            if (pinfos.Length != ptypes.Length)
                return false;

            for (int i = 0; i < pinfos.Length; i++)
            {
                if (!pinfos[i].ParameterType.Equals(ptypes[i]))
                    return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Extensions to the various MemberInfo derived classes
    /// </summary>
    public static class MemberInfoExtensions
    {
        /// <summary>
        /// Returns the get method for the given property
        /// </summary>
        /// <param name="pinfo"></param>
        /// <param name="nonPublic"></param>
        /// <returns></returns>
        public static MethodInfo GetGetMethod(this PropertyInfo pinfo, bool nonPublic)
        {
            if (pinfo.GetMethod == null)
                return null;

            if (nonPublic == false && pinfo.GetMethod.IsPrivate)
                return null;

            return pinfo.GetMethod;
        }

        /// <summary>
        /// Returns an array of custom attributes of the specified type applied to this member
        /// </summary>
        /// <remarks> Portable throws an argument exception if T does not
        /// derive from Attribute. NUnit uses interfaces to find attributes, thus
        /// this method</remarks>
        public static IEnumerable<T> GetAttributes<T>(this MemberInfo info, bool inherit) where T : class
        {
            return GetAttributesImpl<T>(info.GetCustomAttributes(inherit));
        }

        /// <summary>
        /// Returns an array of custom attributes of the specified type applied to this parameter
        /// </summary>
        public static IEnumerable<T> GetAttributes<T>(this ParameterInfo info, bool inherit) where T : class
        {
            return GetAttributesImpl<T>(info.GetCustomAttributes(inherit));
        }

        /// <summary>
        /// Returns an array of custom attributes of the specified type applied to this assembly
        /// </summary>
        public static IEnumerable<T> GetAttributes<T>(this Assembly asm) where T : class
        {
            return GetAttributesImpl<T>(asm.GetCustomAttributes());
        }

        private static IEnumerable<T> GetAttributesImpl<T>(IEnumerable<Attribute> attributes) where T : class
        {
            var attrs = new List<T>();

            attributes.Where(a => typeof(T).IsAssignableFrom(a.GetType()))
                .All(a => { attrs.Add(a as T); return true; });

            return attrs;
        }
    }

    /// <summary>
    /// Extensions for Assembly that are not available in portable
    /// </summary>
    public static class AssemblyExtensions
    { 
        /// <summary>
        /// DNX does not have a version of GetCustomAttributes on Assembly that takes an inherit
        /// parameter since it doesn't make sense on Assemblies. This version just ignores the 
        /// inherit parameter.
        /// </summary>
        /// <param name="asm">The assembly</param>
        /// <param name="attributeType">The type of attribute you are looking for</param>
        /// <param name="inherit">Ignored</param>
        /// <returns></returns>
        public static object[] GetCustomAttributes(this Assembly asm, Type attributeType, bool inherit)
        {
            return asm.GetCustomAttributes(attributeType).ToArray();
        }

        /// <summary>
        /// Gets the types in a given assembly
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public static IList<Type> GetTypes(this Assembly asm)
        {
            return asm.DefinedTypes.Select(info => info.AsType()).ToList();
        }
    }
#endif
}
