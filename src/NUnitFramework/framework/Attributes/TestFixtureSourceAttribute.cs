// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using NUnit.Framework.Internal.Extensions;

namespace NUnit.Framework
{
    /// <summary>
    /// Identifies the source used to provide test fixture instances for a test class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class TestFixtureSourceAttribute : NUnitAttribute, IFixtureBuilderForNestedGeneric
    {
        private readonly NUnitTestFixtureBuilder _builder = new();

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
            SourceName = sourceName;
        }

        /// <summary>
        /// Construct with a Type and name
        /// </summary>
        /// <param name="sourceType">The Type that will provide data</param>
        /// <param name="sourceName">The name of a static method, property or field that will provide data.</param>
        public TestFixtureSourceAttribute(Type sourceType, string sourceName)
        {
            SourceType = sourceType;
            SourceName = sourceName;
        }

        /// <summary>
        /// Construct with a Type
        /// </summary>
        /// <param name="sourceType">The type that will provide data</param>
        public TestFixtureSourceAttribute(Type sourceType)
        {
            SourceType = sourceType;
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

        /// <summary>
        /// Type arguments used to create a generic fixture instance.
        /// </summary>
        public Type[]? TypeArgs { get; set; }

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

        /// <summary>
        /// Builds any number of test fixtures from the specified type.
        /// </summary>
        /// <param name="typeInfo">The TypeInfo for which fixtures are to be constructed.</param>
        /// <param name="filter">PreFilter used to select methods as tests.</param>
        public IEnumerable<TestSuite> BuildFrom(ITypeInfo typeInfo, IPreFilter filter)
        {
            return BuildFrom(typeInfo, filter, NUnitTestFixtureBuilder.NoOuterTypeArgs);
        }

        /// <summary>
        /// Builds any number of test fixtures from the specified type.
        /// </summary>
        /// <param name="typeInfo">The type info of the fixture to be used.</param>
        /// <param name="filter">PreFilter to be used to select methods.</param>
        /// <param name="outerTypeArgsSets">Sets of Type Arguments for the outer fixture to be used for nested generic classes.</param>
        public IEnumerable<TestSuite> BuildFrom(ITypeInfo typeInfo, IPreFilter filter, Type[]?[] outerTypeArgsSets)
        {
            var fixtureSuite = new ParameterizedFixtureSuite(typeInfo);
            fixtureSuite.ApplyAttributesToTest(typeInfo.Type);
            var assemblyLifeCycleAttributeProvider = typeInfo.Type.Assembly.GetAttributes<FixtureLifeCycleAttribute>();
            var typeLifeCycleAttributeProvider = typeInfo.Type.GetAttributes<FixtureLifeCycleAttribute>(inherit: true);
            var parallelizableAttributeProvider = typeInfo.Type.GetAttributes<ParallelizableAttribute>(inherit: true);
            var noTestsAttributeProvider = typeInfo.Assembly.GetAttributes<NoTestsAttribute>(inherit: true);

            ITestFixtureData[] testFixtureDatas = GetParametersFor(SourceType ?? typeInfo.Type).ToArray();

            foreach (var outerTypeArgs in outerTypeArgsSets)
            {
                foreach (ITestFixtureData testFixtureData in testFixtureDatas)
                {
                    if (typeInfo.Type.ContainsGenericParameters &&
                        testFixtureData is TestFixtureParameters testFixtureParameters)
                    {
                        testFixtureParameters.DeduceTypeArgumentsFromArguments(typeInfo.Type);
                    }

                    TestSuite fixture = _builder.BuildFrom(typeInfo, filter, testFixtureData, outerTypeArgs);
                    fixture.ApplyAttributesToTest(assemblyLifeCycleAttributeProvider);
                    fixture.ApplyAttributesToTest(typeLifeCycleAttributeProvider);
                    fixture.ApplyAttributesToTest(parallelizableAttributeProvider);
                    fixture.ApplyAttributesToTest(noTestsAttributeProvider);
                    fixtureSuite.Add(fixture);
                }
            }

            yield return fixtureSuite;
        }

        /// <inheritdoc/>
        public IEnumerable<ITestFixtureData> GetFixtureData(ITypeInfo typeInfo)
        {
            return GetParametersFor(SourceType ?? typeInfo.Type).OfType<ITestFixtureData>();
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
            List<ITestFixtureData> data = [];

            try
            {
                IEnumerable? source = GetTestFixtureSource(sourceType);

                if (source is not null)
                {
                    foreach (object? item in source)
                    {
                        if (item is not ITestFixtureData parms)
                        {
                            object?[] args = item as object?[] ?? [item];

                            parms = new TestFixtureParameters(args)
                            {
                                TypeArgs = TypeArgs
                            };
                        }

                        if (Category is not null)
                        {
                            foreach (string cat in Category.Tokenize(','))
                                parms.Properties.Add(PropertyNames.Category, cat);
                        }

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
            if (SourceName is null)
                return Reflect.Construct(sourceType) as IEnumerable;

            MemberInfo[] members = sourceType.GetMemberIncludingFromBase(SourceName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            if (members.Length == 1)
            {
                MemberInfo member = members[0];

                var field = member as FieldInfo;
                if (field is not null)
                {
                    return field.IsStatic
                        ? (IEnumerable?)field.GetValue(null)
                        : SourceMustBeStaticError();
                }

                var property = member as PropertyInfo;
                if (property is not null)
                {
                    MethodInfo? getMethod = property.GetGetMethod(true);
                    return getMethod?.IsStatic is true
                        ? (IEnumerable?)property.GetValue(null, null)
                        : SourceMustBeStaticError();
                }

                var m = member as MethodInfo;
                if (m is not null)
                {
                    return m.IsStatic
                        ? AsyncEnumerableAdapter.CoalesceToEnumerable(m.InvokeMaybeAwait<object>())
                        : SourceMustBeStaticError();
                }
            }

            return null;
        }

        private static TestFixtureParameters[] SourceMustBeStaticError()
        {
            var parms = new TestFixtureParameters
            {
                RunState = RunState.NotRunnable
            };
            parms.Properties.Set(PropertyNames.SkipReason, MUST_BE_STATIC);
            return [parms];
        }

        #endregion
    }
}
