// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
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
            return type.IsAbstract && type.IsSealed;
        }

        public static bool HasAttribute<T>(this ICustomAttributeProvider attributeProvider, bool inherit)
        {
            return attributeProvider.IsDefined(typeof(T), inherit);
        }

        public static bool HasAttribute<T>(this Type type, bool inherit)
        {
            return ((ICustomAttributeProvider)type).HasAttribute<T>(inherit);
        }

        public static T[] GetAttributes<T>(this ICustomAttributeProvider attributeProvider, bool inherit) where T : class
        {
            return (T[])attributeProvider.GetCustomAttributes(typeof(T), inherit);
        }

        public static T[] GetAttributes<T>(this Assembly assembly) where T : class
        {
            return assembly.GetAttributes<T>(inherit: false);
        }

        public static T[] GetAttributes<T>(this Type type, bool inherit) where T : class
        {
            return ((ICustomAttributeProvider)type).GetAttributes<T>(inherit);
        }

        public static IEnumerable Skip(this IEnumerable enumerable, long skip)
        {
            var iterator = enumerable.GetEnumerator();
            using (iterator as IDisposable)
            {
                while (skip-- > 0)
                {
                    if (!iterator.MoveNext())
                        yield break;
                }

                while (iterator.MoveNext())
                {
                    yield return iterator.Current;
                }
            }
        }
    }
}
