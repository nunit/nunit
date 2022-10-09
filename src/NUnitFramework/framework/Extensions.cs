// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Reflection;

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

        public static T[] GetAttributes<T>(this ICustomAttributeProvider attributeProvider, bool inherit) where T : class
        {
            return (T[])attributeProvider.GetCustomAttributes(typeof(T), inherit);
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
