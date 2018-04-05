// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
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
using System.Linq;
using System.Reflection;
using NUnit.Compatibility;

namespace NUnit.Framework
{
    /// <summary>
    /// Contains extension methods that do not require a special <c>using</c> directive.
    /// </summary>
    internal static class Extensions
    {
        public static bool IsStatic(this Type type)
        {
            return type.GetTypeInfo().IsAbstract && type.GetTypeInfo().IsSealed;
        }

        public static bool HasAttribute<T>(this ICustomAttributeProvider attributeProvider, bool inherit)
        {
#if NETSTANDARD1_6
            if (!typeof(Attribute).IsAssignableFrom(typeof(T)))
            {
                // Filter manually for any targets that may run on .NET Core 1.1 or lower
                // because only types derived from System.Attribute are allowed.
                return attributeProvider.GetCustomAttributes(inherit).OfType<T>().Any();
            }
#endif
            return attributeProvider.IsDefined(typeof(T), inherit);
        }

        public static bool HasAttribute<T>(this Type type, bool inherit)
        {
            return ((ICustomAttributeProvider)type.GetTypeInfo()).HasAttribute<T>(inherit);
        }

        public static T[] GetAttributes<T>(this ICustomAttributeProvider attributeProvider, bool inherit) where T : class
        {
#if NETSTANDARD1_6
            if (!typeof(Attribute).IsAssignableFrom(typeof(T)))
            {
                // Filter manually for any targets that may run on .NET Core 1.1 or lower
                // because only types derived from System.Attribute are allowed.
                return attributeProvider.GetCustomAttributes(inherit).OfType<T>().ToArray();
            }
#endif
            return (T[])attributeProvider.GetCustomAttributes(typeof(T), inherit);
        }

        public static T[] GetAttributes<T>(this Assembly assembly) where T : class
        {
            return assembly.GetAttributes<T>(inherit: false);
        }

        public static T[] GetAttributes<T>(this Type type, bool inherit) where T : class
        {
            return ((ICustomAttributeProvider)type.GetTypeInfo()).GetAttributes<T>(inherit);
        }
    }
}
