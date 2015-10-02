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

#if NET_2_0
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
    /// <summary>
    /// Provides NUnit specific extensions to aid in Reflection
    /// across multiple frameworks
    /// </summary>
    public static class ReflectionExtensions
    {
#if !NET_4_5 && !NETCORE
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
#elif NETCORE

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
        public static IEnumerable<T> GetAttributes<T>(this Assembly info) where T : class
        {
            return GetAttributesImpl<T>(info.GetCustomAttributes());
        }

        private static IEnumerable<T> GetAttributesImpl<T>(IEnumerable<Attribute> attributes) where T : class
        {
            var attrs = new List<T>();

            attributes.Where(a => typeof(T).IsAssignableFrom(a.GetType()))
                .All(a => { attrs.Add(a as T); return true; });

            return attrs;
        }
#endif
    }
}
