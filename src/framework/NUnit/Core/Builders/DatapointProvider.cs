// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Reflection;
using System.Collections;
using NUnit.Core.Extensibility;

namespace NUnit.Core.Builders
{
    /// <summary>
    /// Provides data from fields marked with the DatapointAttribute or the
    /// DatapointsAttribute.
    /// </summary>
    public class DatapointProvider : IDataPointProvider
    {
        private static readonly string DatapointAttribute = "NUnit.Framework.DatapointAttribute";
        private static readonly string DatapointsAttribute = "NUnit.Framework.DatapointsAttribute";

        #region IDataPointProvider Members

        public bool HasDataFor(System.Reflection.ParameterInfo parameter)
        {
            Type parameterType = parameter.ParameterType;
            MemberInfo method = parameter.Member;
            Type fixtureType = method.ReflectedType;

            if (!Reflect.HasAttribute(method, NUnitFramework.TheoryAttribute, true))
                return false;

            foreach (FieldInfo field in fixtureType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
            {
                if (field.FieldType == parameterType)
                {
                    if (Reflect.HasAttribute(field, DatapointAttribute, true))
                        return true;
                }
                else if (field.FieldType.IsArray && field.FieldType.GetElementType() == parameterType)
                {
                    if (Reflect.HasAttribute(field, DatapointsAttribute, true))
                        return true;
                }
            }

            return false;
        }

        public System.Collections.IEnumerable GetDataFor(System.Reflection.ParameterInfo parameter)
        {
            ArrayList datapoints = new ArrayList();

            Type parameterType = parameter.ParameterType;
            MemberInfo method = parameter.Member;
            Type fixtureType = method.ReflectedType;

            foreach (FieldInfo field in fixtureType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
            {
                if (field.FieldType == parameterType && Reflect.HasAttribute(field, DatapointAttribute, true))
                {
                    datapoints.Add(field.GetValue(ProviderCache.GetInstanceOf(fixtureType)));
                }
                else if(field.FieldType.IsArray && field.FieldType.GetElementType() == parameterType &&
                    Reflect.HasAttribute(field, DatapointsAttribute, true ))
                {
                    datapoints.AddRange((ICollection)field.GetValue(ProviderCache.GetInstanceOf(fixtureType)));
                }
            }

            return datapoints;
        }

        #endregion
    }
}
