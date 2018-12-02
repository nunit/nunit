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

#if NET35 || NET40
using System;
using System.Reflection;

namespace NUnit.Compatibility
{
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
}
#endif
