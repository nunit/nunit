// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Extensions;

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
        public ValueSourceAttribute(string? sourceName)
        {
            SourceName = sourceName;
        }

        /// <summary>
        /// Construct with a Type and name - for use with languages
        /// that don't support params arrays.
        /// </summary>
        /// <param name="sourceType">The Type that will provide data</param>
        /// <param name="sourceName">The name of a static method, property or field that will provide data.</param>
        public ValueSourceAttribute(Type? sourceType, string? sourceName)
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

            if (SourceName is null)
            {
                return Reflect.Construct(sourceType) as IEnumerable
                    ?? throw new InvalidDataSourceException($"The value source type '{sourceType}' does not implement IEnumerable.");
            }

            MemberInfo[] members = sourceType.GetMemberIncludingFromBase(SourceName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            return ContextUtils.DoIsolated(() => GetDataSourceValue(members))
                ?? throw CreateSourceNameException();
        }

        private static IEnumerable? GetDataSourceValue(MemberInfo[] members)
        {
            if (members.Length != 1)
                return null;

            MemberInfo member = members[0];

            var field = member as FieldInfo;
            if (field is not null)
            {
                if (field.IsStatic)
                    return (IEnumerable?)field.GetValue(null);

                throw CreateSourceNameException();
            }

            var property = member as PropertyInfo;
            if (property is not null)
            {
                MethodInfo? getMethod = property.GetGetMethod(true);
                if (getMethod?.IsStatic is true)
                    return (IEnumerable?)property.GetValue(null, null);

                throw CreateSourceNameException();
            }

            var m = member as MethodInfo;
            if (m is not null)
            {
                if (m.IsStatic)
                    return AsyncEnumerableAdapter.CoalesceToEnumerable(m.InvokeMaybeAwait<object>());

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
