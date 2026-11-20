// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NUnit.Framework.Internal
{
    internal static class UnionHelper
    {
        private const string UnionAttributeFullName = "System.Runtime.CompilerServices.UnionAttribute";
        private static readonly ConditionalWeakTable<Type, UnionTypeContext?> UnionTypeInfoCache = new();

        internal static bool TryGetUnionValue(object? obj, out object? value)
        {
            value = null;
            if (obj is null)
                return false;

            var unionTypeInfo = UnionTypeInfoCache.GetValue(obj.GetType(), static type => CreateUnionTypeContext(type));
            if (unionTypeInfo is null)
                return false;

            value = unionTypeInfo.ValueAccessor.Invoke(obj, null);
            return true;
        }

        private static UnionTypeContext? CreateUnionTypeContext(Type unionType)
        {
            var unionAttribute = unionType.GetCustomAttributes(false).FirstOrDefault(attribute =>
                string.Equals(attribute.GetType().FullName, UnionAttributeFullName, StringComparison.Ordinal));
            if (unionAttribute is null)
                return null;

            var valueProperty = unionType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
            if (valueProperty is null || valueProperty.PropertyType != typeof(object))
                return null;

            var valuePropertyAccessor = valueProperty.GetGetMethod();
            if (valuePropertyAccessor is null)
                return null;

            return new UnionTypeContext(valuePropertyAccessor);
        }

        private sealed class UnionTypeContext
        {
            public MethodInfo ValueAccessor { get; }

            public UnionTypeContext(MethodInfo valueAccessor)
            {
                ValueAccessor = valueAccessor;
            }
        }
    }
}
