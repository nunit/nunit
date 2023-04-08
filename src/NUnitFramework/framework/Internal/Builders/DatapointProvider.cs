// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Builders
{
    /// <summary>
    /// Provides data from fields marked with the DatapointAttribute or the
    /// DatapointsAttribute.
    /// </summary>
    public class DatapointProvider : IParameterDataProvider
    {
        private readonly bool _searchInDeclaringTypes;

        /// <summary>
        /// Creates a new DatapointProvider.
        /// </summary>
        /// <param name="searchInDeclaringTypes">Determines whether when searching for theory data members of declaring types will also be searched.</param>
        public DatapointProvider(bool searchInDeclaringTypes)
        {
            _searchInDeclaringTypes = searchInDeclaringTypes;
        }

        private static readonly ProviderCache ProviderCache = new ProviderCache();

        #region IDataPointProvider Members

        /// <summary>
        /// Determines whether any data is available for a parameter.
        /// </summary>
        /// <param name="parameter">The parameter of a parameterized test.</param>
        public bool HasDataFor(IParameterInfo parameter)
        {
            var method = parameter.Method;
            if (!method.IsDefined<TheoryAttribute>(true))
                return false;

            Type parameterType = parameter.ParameterType;
            if (parameterType == typeof(bool) || parameterType.IsEnum)
                return true;

            Type containingType = method.TypeInfo.Type;
            foreach (var memberAndOwningType in GetMembersFromType(containingType))
            {
                var member = memberAndOwningType.Item1;

                if (member.IsDefined(typeof(DatapointAttribute), true) &&
                    GetTypeFromMemberInfo(member) == parameterType)
                    return true;
                else if (member.IsDefined(typeof(DatapointSourceAttribute), true) &&
                    GetElementTypeFromMemberInfo(member) == parameterType)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Retrieves data for use with the supplied parameter.
        /// </summary>
        /// <param name="parameter">The parameter of a parameterized test.</param>
        public IEnumerable GetDataFor(IParameterInfo parameter)
        {
            var datapoints = new List<object?>();

            Type parameterType = parameter.ParameterType;
            Type fixtureType = parameter.Method.TypeInfo.Type;

            foreach (var memberAndOwningType in GetMembersFromType(fixtureType))
            {
                var member = memberAndOwningType.Item1;
                var owningType = memberAndOwningType.Item2;

                if (member.IsDefined(typeof(DatapointAttribute), true))
                {
                    var field = member as FieldInfo;
                    if (GetTypeFromMemberInfo(member) == parameterType && field != null)
                    {
                        if (field.IsStatic)
                            datapoints.Add(field.GetValue(null));
                        else
                            datapoints.Add(field.GetValue(ProviderCache.GetInstanceOf(owningType)));
                    }
                }
                else if (member.IsDefined(typeof(DatapointSourceAttribute), true))
                {
                    if (GetElementTypeFromMemberInfo(member) == parameterType)
                    {
                        object? instance;

                        FieldInfo? field = member as FieldInfo;
                        PropertyInfo? property = member as PropertyInfo;
                        MethodInfo? method = member as MethodInfo;
                        if (field != null)
                        {
                            instance = field.IsStatic ? null : ProviderCache.GetInstanceOf(owningType);
                            foreach (object data in (IEnumerable)field.GetValue(instance))
                                datapoints.Add(data);
                        }
                        else if (property != null)
                        {
                            MethodInfo getMethod = property.GetGetMethod(true);
                            instance = getMethod.IsStatic ? null : ProviderCache.GetInstanceOf(owningType);
                            foreach (object data in (IEnumerable)property.GetValue(instance, null))
                                datapoints.Add(data);
                        }
                        else if (method != null)
                        {
                            instance = method.IsStatic ? null : ProviderCache.GetInstanceOf(owningType);
                            foreach (object data in (IEnumerable)method.Invoke(instance, Array.Empty<Type>()))
                                datapoints.Add(data);
                        }
                    }
                }
            }

            if (datapoints.Count == 0)
            {
                var underlyingParameterType = Nullable.GetUnderlyingType(parameterType);
                if (underlyingParameterType != null)
                {
                    parameterType = underlyingParameterType;
                }

                if (parameterType == typeof(bool))
                {
                    datapoints.Add(true);
                    datapoints.Add(false);
                }
                else if (parameterType.IsEnum)
                {
                    foreach (object o in Enum.GetValues(parameterType))
                    {
                        datapoints.Add(o);
                    }
                }

                if (datapoints.Count > 0 && underlyingParameterType != null)
                {
                    datapoints.Add(null);
                }
            }

            return datapoints;
        }

        private IEnumerable<Tuple<MemberInfo, Type>> GetMembersFromType(Type type)
        {
            if (_searchInDeclaringTypes)
            {
                return GetNestedMembersFromType(type);
            }

            return GetDirectMembersOfType(type);
        }

        private static IEnumerable<Tuple<MemberInfo, Type>> GetNestedMembersFromType(Type? type)
        {
            while (type != null)
            {
                foreach (var tuple in GetDirectMembersOfType(type)) yield return tuple;
                type = type.DeclaringType;
            }
        }

        private static IEnumerable<Tuple<MemberInfo, Type>> GetDirectMembersOfType(Type type)
        {
            foreach (var member in type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
            {
                yield return new Tuple<MemberInfo, Type>(member, type);
            }
        }

        private Type? GetTypeFromMemberInfo(MemberInfo member)
        {
            var field = member as FieldInfo;
            if (field != null)
                return field.FieldType;

            var property = member as PropertyInfo;
            if (property != null)
                return property.PropertyType;

            var method = member as MethodInfo;
            if (method != null)
                return method.ReturnType;

            return null;
        }

        private Type? GetElementTypeFromMemberInfo(MemberInfo member)
        {
            Type? type = GetTypeFromMemberInfo(member);

            if (type == null)
                return null;

            if (type.IsArray)
                return type.GetElementType();

            if (type.IsGenericType && type.Name == "IEnumerable`1")
                return type.GetGenericArguments()[0];

            return null;
        }

        #endregion
    }
}
