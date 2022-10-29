// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework
{
    /// <summary>
    /// Identifies the source used to provide test fixture instances for a test class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class TestFixtureSourceAttribute : NUnitAttribute, IFixtureBuilder2
    {
        private readonly NUnitTestFixtureBuilder _builder = new NUnitTestFixtureBuilder();

        /// <summary>
        /// Error message string is public so the tests can use it
        /// </summary>
        public const string MUST_BE_STATIC = "The sourceName specified on a TestCaseSourceAttribute must refer to a static field, property or method.";

        #region Constructors

        /// <summary>
        /// Construct with the name of the method, property or field that will provide data
        /// </summary>
        /// <param name="sourceName">The name of a static method, property or field that will provide data.</param>
        public TestFixtureSourceAttribute(string sourceName)
        {
            this.SourceName = sourceName;
        }

        /// <summary>
        /// Construct with a Type and name
        /// </summary>
        /// <param name="sourceType">The Type that will provide data</param>
        /// <param name="sourceName">The name of a static method, property or field that will provide data.</param>
        public TestFixtureSourceAttribute(Type sourceType, string sourceName)
        {
            this.SourceType = sourceType;
            this.SourceName = sourceName;
        }

        /// <summary>
        /// Construct with a Type
        /// </summary>
        /// <param name="sourceType">The type that will provide data</param>
        public TestFixtureSourceAttribute(Type sourceType)
        {
            this.SourceType = sourceType;
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

        /// <summary>
        /// Gets or sets the category associated with every fixture created from
        /// this attribute. May be a single category or a comma-separated list.
        /// </summary>
        public string? Category { get; set; }

        #endregion

        #region IFixtureBuilder Members

        /// <summary>
        /// Builds any number of test fixtures from the specified type.
        /// </summary>
        /// <param name="typeInfo">The TypeInfo for which fixtures are to be constructed.</param>
        public IEnumerable<TestSuite> BuildFrom(ITypeInfo typeInfo)
        {
            return BuildFrom(typeInfo, PreFilter.Empty);
        }

        #endregion

        #region IFixtureBuilder2 Members

        /// <summary>
        /// Builds any number of test fixtures from the specified type.
        /// </summary>
        /// <param name="typeInfo">The TypeInfo for which fixtures are to be constructed.</param>
        /// <param name="filter">PreFilter used to select methods as tests.</param>
        public IEnumerable<TestSuite> BuildFrom(ITypeInfo typeInfo, IPreFilter filter)
        {
            Type sourceType = SourceType ?? typeInfo.Type;

            var fixtureSuite = new ParameterizedFixtureSuite(typeInfo);
            fixtureSuite.ApplyAttributesToTest(typeInfo.Type);
            var assemblyLifeCycleAttributeProvider = new AttributeProviderWrapper<FixtureLifeCycleAttribute>(typeInfo.Type.Assembly);
            var typeLifeCycleAttributeProvider = new AttributeProviderWrapper<FixtureLifeCycleAttribute>(typeInfo.Type);

            foreach (ITestFixtureData parms in GetParametersFor(sourceType))
            {
                TestSuite fixture = _builder.BuildFrom(typeInfo, filter, parms);
                fixture.ApplyAttributesToTest(assemblyLifeCycleAttributeProvider);
                fixture.ApplyAttributesToTest(typeLifeCycleAttributeProvider);
                fixtureSuite.Add(fixture);
            }

            yield return fixtureSuite;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Returns a set of ITestFixtureData items for use as arguments
        /// to a parameterized test fixture.
        /// </summary>
        /// <param name="sourceType">The type for which data is needed.</param>
        /// <returns></returns>
        public IEnumerable<ITestFixtureData> GetParametersFor(Type sourceType)
        {
            List<ITestFixtureData> data = new List<ITestFixtureData>();

            try
            {
                IEnumerable? source = GetTestFixtureSource(sourceType);

                if (source != null)
                {
                    foreach (object? item in source)
                    {
                        var parms = item as ITestFixtureData;

                        if (parms == null)
                        {
                            object?[]? args = item as object?[];
                            if (args == null)
                            {
                                args = new[] { item };
                            }

                            parms = new TestFixtureParameters(args);
                        }

                        if (this.Category != null)
                            foreach (string cat in this.Category.Split(new[] { ',' }))
                                parms.Properties.Add(PropertyNames.Category, cat);

                        data.Add(parms);
                    }
                }
            }
            catch (Exception ex)
            {
                data.Clear();
                data.Add(new TestFixtureParameters(ex));
            }

            return data;
        }

        private IEnumerable? GetTestFixtureSource(Type sourceType)
        {
            // Handle Type implementing IEnumerable separately
            if (SourceName == null)
                return Reflect.Construct(sourceType) as IEnumerable;

            MemberInfo[] members = sourceType.GetMemberIncludingFromBase(SourceName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            if (members.Length == 1)
            {
                MemberInfo member = members[0];

                var field = member as FieldInfo;
                if (field != null)
                    return field.IsStatic
                        ? (IEnumerable)field.GetValue(null)
                        : SourceMustBeStaticError();

                var property = member as PropertyInfo;
                if (property != null)
                    return property.GetGetMethod(true).IsStatic
                        ? (IEnumerable)property.GetValue(null, null)
                        : SourceMustBeStaticError();

                var m = member as MethodInfo;
                if (m != null)
                    return m.IsStatic
                        ? (IEnumerable)m.Invoke(null, null)
                        : SourceMustBeStaticError();
            }

            return null;
        }

        private static IEnumerable SourceMustBeStaticError()
        {
            var parms = new TestFixtureParameters();
            parms.RunState = RunState.NotRunnable;
            parms.Properties.Set(PropertyNames.SkipReason, MUST_BE_STATIC);
            return new[] { parms };
        }

        #endregion
    }
}
