// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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
using NUnit.Framework;
using NUnit.Core.Extensibility;

namespace NUnit.Core.Builders
{
    /// <summary>
    /// Provides data from fields marked with the DatapointAttribute or the
    /// DatapointsAttribute.
    /// </summary>
    public class DatapointProvider : IDataPointProvider
    {
        #region IDataPointProvider Members

        public bool HasDataFor(System.Reflection.ParameterInfo parameter)
        {
            Type parameterType = parameter.ParameterType;
            MemberInfo method = parameter.Member;
            Type fixtureType = method.ReflectedType;

            if (!method.IsDefined(typeof(TheoryAttribute), true))
                return false;

            foreach (FieldInfo field in fixtureType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
            {
                if (field.FieldType == parameterType)
                {
                    if (field.IsDefined(typeof(DatapointAttribute), true))
                        return true;
                }
                else if (field.FieldType.IsArray && field.FieldType.GetElementType() == parameterType)
                {
                    if (field.IsDefined(typeof(DatapointsAttribute), true))
                        return true;
                }
            }

            return false;
        }

        public System.Collections.IEnumerable GetDataFor(System.Reflection.ParameterInfo parameter)
        {
            ObjectList datapoints = new ObjectList();

            Type parameterType = parameter.ParameterType;
            MemberInfo method = parameter.Member;
            Type fixtureType = method.ReflectedType;

            foreach (FieldInfo field in fixtureType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
            {
                if (field.FieldType == parameterType && field.IsDefined(typeof(DatapointAttribute), true))
                {
                    datapoints.Add(field.GetValue(ProviderCache.GetInstanceOf(fixtureType)));
                }
                else if(field.FieldType.IsArray && field.FieldType.GetElementType() == parameterType &&
                    field.IsDefined(typeof(DatapointsAttribute), true ))
                {
                    datapoints.AddRange((ICollection)field.GetValue(ProviderCache.GetInstanceOf(fixtureType)));
                }
            }

            return datapoints;
        }

        #endregion
    }
}
