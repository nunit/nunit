// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace NUnit.Framework.Internal
{
    internal static class UnionHelper
    {
        private static readonly Type? UnionAttributeType = Type.GetType("System.Runtime.CompilerServices.UnionAttribute");

        internal static bool TryGetUnionValue(object? obj, [NotNullWhen(true)] out object? value)
        {
            value = null;
            if (obj is null)
                return false;

            var unionTypeInfo = GetUnionTypeInfo(obj.GetType());
            if (unionTypeInfo is null)
                return false;

            value = unionTypeInfo.ValueAccessor.Invoke(obj, null);
            return value is not null;
        }

        internal static UnionTypeContext? GetUnionTypeInfo(Type unionType)
        {
            if (UnionAttributeType is null)
                return null;

            var unionAttribute = unionType.GetCustomAttributes(false).FirstOrDefault(o => o.GetType() == UnionAttributeType);
            if (unionAttribute is null)
                return null;

            var valueProperty = unionType.GetProperty("Value", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (valueProperty is null)
                return null;

            var valuePropertyType = valueProperty.PropertyType;
            if (valuePropertyType != typeof(object))
                return null;

            var valuePropertyAccessor = valueProperty.GetGetMethod();
            if (valuePropertyAccessor is null)
                return null;

            var candidateTypes = GetMemberTypes(unionType);
            if (candidateTypes.Length == 0)
                return null;

            return new UnionTypeContext(unionType, candidateTypes, valuePropertyAccessor);
        }

        private static Type[] GetMemberTypes(Type unionType)
        {
            var constructors = unionType.GetConstructors().Where(static o => o.IsPublic);

            var memberTypes = new List<Type>();
            foreach (var constructor in constructors)
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

        internal class UnionTypeContext
        {
            public Type UnionType { get; }
            public Type[] MemberTypes { get; }
            public MethodInfo ValueAccessor { get; }

            public UnionTypeContext(Type unionType, Type[] memberTypes, MethodInfo valueAccessor)
            {
                UnionType = unionType;
                MemberTypes = memberTypes;
                ValueAccessor = valueAccessor;
            }
        }
    }
}
