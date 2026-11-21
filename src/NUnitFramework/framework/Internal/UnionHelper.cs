// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

            var unionTypeInfo = GetUnionInfo(obj.GetType());
            if (unionTypeInfo is null)
                return false;

            value = unionTypeInfo.ValueAccessor.Invoke(obj, null);
            return true;
        }

        internal static bool TryGetUnionMemberTypes(Type type, [NotNullWhen(true)] out Type[]? memberTypes)
        {
            memberTypes = null;
            UnionTypeContext? unionTypeInfo = GetUnionInfo(type);
            if (unionTypeInfo is null)
                return false;

            memberTypes = unionTypeInfo.MemberTypes;
            return true;
        }

        private static UnionTypeContext? GetUnionInfo(Type type)
        {
            if (type.IsPrimitive || type.IsEnum || type == typeof(string))
                return null;

            return UnionTypeInfoCache.GetValue(type, static type => CreateUnionTypeContext(type));
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

            var candidateTypes = GetMemberTypes(unionType);
            if (candidateTypes.Length == 0)
                return null;

            return new UnionTypeContext(valuePropertyAccessor, candidateTypes);
        }

        private static Type[] GetMemberTypes(Type unionType)
        {
            var constructors = unionType.GetConstructors();

            var memberTypes = new List<Type>(constructors.Length);
            foreach (var constructor in constructors.Where(static o => o.IsPublic))
            {
                var parameters = constructor.GetParameters();
                if (parameters.Length != 1)
                    continue;

                var parameter = parameters[0];
                if (parameter.IsRetval || parameter.IsOut)
                    continue;

                memberTypes.Add(parameter.ParameterType);
            }

            return memberTypes.ToArray();
        }

        private sealed class UnionTypeContext
        {
            public MethodInfo ValueAccessor { get; }
            public Type[] MemberTypes { get; }

            public UnionTypeContext(MethodInfo valueAccessor, Type[] memberTypes)
            {
                ValueAccessor = valueAccessor;
                MemberTypes = memberTypes;
            }
        }
    }
}
