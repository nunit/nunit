// ***********************************************************************
// Copyright (c) 2008-2015 Charlie Poole, Rob Prouse
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

#nullable enable

using System;
using System.Collections;
using System.Reflection;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Indicates the source used to provide data for one parameter of a test method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = false)]
    public class ValueSourceAttribute : NUnitAttribute, IParameterDataSource
    {
        #region Constructors

        /// <summary>
        /// Construct with the name of the factory - for use with languages
        /// that don't support params arrays.
        /// </summary>
        /// <param name="sourceName">The name of a static method, property or field that will provide data.</param>
        public ValueSourceAttribute(string sourceName)
        {
            SourceName = sourceName;
        }

        /// <summary>
        /// Construct with a Type and name - for use with languages
        /// that don't support params arrays.
        /// </summary>
        /// <param name="sourceType">The Type that will provide data</param>
        /// <param name="sourceName">The name of a static method, property or field that will provide data.</param>
        public ValueSourceAttribute(Type sourceType, string sourceName)
        {
            SourceType = sourceType;
            SourceName = sourceName;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The name of a the method, property or field to be used as a source
        /// </summary>
        public string? SourceName { get; }

        /// <summary>
        /// A Type to be used as a source
        /// </summary>
        public Type? SourceType { get; }

        #endregion

        #region IParameterDataSource Members

        /// <summary>
        /// Retrieves a list of arguments which can be passed to the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter of a parameterized test.</param>
        public IEnumerable GetData(IParameterInfo parameter)
        {
            return GetDataSource(parameter);
        }

        #endregion

        #region Helper Methods

        private IEnumerable GetDataSource(IParameterInfo parameter)
        {
            Type sourceType = SourceType ?? parameter.Method.TypeInfo.Type;

            if (SourceName == null)
            {
                return Reflect.Construct(sourceType) as IEnumerable
                    ?? throw new InvalidDataSourceException($"The value source type '{sourceType}' does not implement IEnumerable.");
            }

            MemberInfo[] members = sourceType.GetMember(SourceName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

            return ContextUtils.DoIsolated(() => GetDataSourceValue(members))
                ?? throw CreateSourceNameException();
        }

        private static IEnumerable? GetDataSourceValue(MemberInfo[] members)
        {
            if (members.Length != 1) return null;

            MemberInfo member = members[0];

            var field = member as FieldInfo;
            if (field != null)
            {
                if (field.IsStatic)
                    return (IEnumerable)field.GetValue(null);

                throw CreateSourceNameException();
            }

            var property = member as PropertyInfo;
            if (property != null)
            {
                if (property.GetGetMethod(true).IsStatic)
                    return (IEnumerable)property.GetValue(null, null);

                throw CreateSourceNameException();
            }

            var m = member as MethodInfo;
            if (m != null)
            {
                if (m.IsStatic)
                    return (IEnumerable)m.Invoke(null, null);

                throw CreateSourceNameException();
            }

            return null;
        }

        private static InvalidDataSourceException CreateSourceNameException()
        {
            return new InvalidDataSourceException("The sourceName specified on a ValueSourceAttribute must refer to a non-null static field, property or method.");
        }

        #endregion
    }
}
