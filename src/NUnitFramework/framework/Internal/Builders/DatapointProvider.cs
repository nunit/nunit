// ***********************************************************************
// Copyright (c) 2008 Charlie Poole, Rob Prouse
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
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Builders
{
    /// <summary>
    /// Provides data from fields marked with the DatapointAttribute or the
    /// DatapointsAttribute.
    /// </summary>
    public class DatapointProvider : IParameterDataProvider
    {
        #region IDataPointProvider Members

        /// <summary>
        /// Determines whether any data is available for a parameter.
        /// </summary>
        /// <param name="parameter">The parameter of a parameterized test</param>
        public bool HasDataFor(IParameterInfo parameter)
        {
            var method = parameter.Method;
            if (!method.IsDefined<TheoryAttribute>(true))
                return false;

            Type parameterType = parameter.ParameterType;
            if (parameterType == typeof(bool) || parameterType.GetTypeInfo().IsEnum)
                return true;

            Type containingType = method.TypeInfo.Type;
            foreach (MemberInfo member in containingType.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
            {
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
        /// <param name="parameter">The parameter of a parameterized test</param>
        public IEnumerable GetDataFor(IParameterInfo parameter)
        {
            var datapoints = new List<object>();

            Type parameterType = parameter.ParameterType;
            Type fixtureType = parameter.Method.TypeInfo.Type;

            foreach (MemberInfo member in fixtureType.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
            {
                if (member.IsDefined(typeof(DatapointAttribute), true))
                {
                    if (GetTypeFromMemberInfo(member) == parameterType && member is FieldInfo field)
                    {
                        if (field.IsStatic)
                            datapoints.Add(field.GetValue(null));
                        else
                            datapoints.Add(field.GetValue(ProviderCache.GetInstanceOf(fixtureType)));
                    }
                }
                else if (member.IsDefined(typeof(DatapointSourceAttribute), true))
                {
                    if (GetElementTypeFromMemberInfo(member) == parameterType)
                    {
                        object instance;
                        if (member is FieldInfo field)
                        {
                            instance = field.IsStatic ? null : ProviderCache.GetInstanceOf(fixtureType);
                            foreach (object data in (IEnumerable)field.GetValue(instance))
                                datapoints.Add(data);
                        }
                        else if (member is PropertyInfo property)
                        {
                            MethodInfo getMethod = property.GetGetMethod(true);
                            instance = getMethod.IsStatic ? null : ProviderCache.GetInstanceOf(fixtureType);
                            foreach (object data in (IEnumerable)property.GetValue(instance, null))
                                datapoints.Add(data);
                        }
                        else if (member is MethodInfo method)
                        {
                            instance = method.IsStatic ? null : ProviderCache.GetInstanceOf(fixtureType);
                            foreach (object data in (IEnumerable)method.Invoke(instance, new Type[0]))
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
                else if (parameterType.GetTypeInfo().IsEnum)
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

        private Type GetTypeFromMemberInfo(MemberInfo member)
        {
            if (member is FieldInfo field)
                return field.FieldType;

            if (member is PropertyInfo property)
                return property.PropertyType;

            if (member is MethodInfo method)
                return method.ReturnType;

            return null;
        }

        private Type GetElementTypeFromMemberInfo(MemberInfo member)
        {
            Type type = GetTypeFromMemberInfo(member);

            if (type == null)
                return null;

            if (type.IsArray)
                return type.GetElementType();

            if (type.GetTypeInfo().IsGenericType && type.Name == "IEnumerable`1")
                return type.GetGenericArguments()[0];

            return null;
        }

        #endregion
    }
}
