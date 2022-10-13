// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Builders
{
    /// <summary>
    /// Built-in SuiteBuilder for all types of test classes.
    /// </summary>
    public class DefaultSuiteBuilder : ISuiteBuilder
    {
        // Builder we use for fixtures without any fixture attribute specified
        private readonly NUnitTestFixtureBuilder _defaultBuilder = new NUnitTestFixtureBuilder();

        #region ISuiteBuilder Methods

        /// <summary>
        /// Examine the type and determine if it is suitable for
        /// this builder to use in building a TestSuite.
        ///
        /// Note that returning false will cause the type to be ignored
        /// in loading the tests. If it is desired to load the suite
        /// but label it as non-runnable, ignored, etc., then this
        /// method must return true.
        /// </summary>
        /// <param name="typeInfo">The fixture type to check</param>
        public bool CanBuildFrom(ITypeInfo typeInfo)
        {
            if (typeInfo.IsAbstract && !typeInfo.IsSealed)
                return false;

            if (typeInfo.IsDefined<IFixtureBuilder>(true))
                return true;

            // Generics must have an attribute in order to provide
            // them with arguments to determine the specific type.
            // TODO: What about automatic fixtures? Should there
            // be some kind of error shown?
            if (typeInfo.IsGenericTypeDefinition)
                return false;

            return typeInfo.HasMethodWithAttribute(typeof(IImplyFixture));
        }

        /// <summary>
        /// Builds a single test suite from the specified type.
        /// </summary>
        /// <param name="typeInfo">The fixture type to build</param>
        public TestSuite BuildFrom(ITypeInfo typeInfo)
        {
            return BuildFrom(typeInfo, PreFilter.Empty);
        }

        /// <summary>
        /// Builds a single test suite from the specified type, subject
        /// to a filter that decides which methods are included.
        /// </summary>
        /// <param name="typeInfo">The fixture type to build</param>
        /// <param name="filter">A PreFilter for selecting methods.</param>
        public TestSuite BuildFrom(ITypeInfo typeInfo, IPreFilter filter)
        {
            var fixtures = new List<TestSuite>();

            try
            {
                IFixtureBuilder[] builders = GetFixtureBuilderAttributes(typeInfo);

                foreach (var builder in builders)
                {
                    // See if this is an enhanced attribute, accepting a filter
                    var builder2 = builder as IFixtureBuilder2;

                    foreach (var fixture in builder2?.BuildFrom(typeInfo, filter) ?? builder.BuildFrom(typeInfo))
                        fixtures.Add(fixture);
                }

                if (typeInfo.IsGenericType)
                    return BuildMultipleFixtures(typeInfo, fixtures);

                switch (fixtures.Count)
                {
                    case 0:
                        return _defaultBuilder.BuildFrom(typeInfo, filter);
                    case 1:
                        return fixtures[0];
                    default:
                        return BuildMultipleFixtures(typeInfo, fixtures);
                }
            }
            catch (Exception ex)
            {
                var fixture = new TestFixture(typeInfo);
                if (ex is System.Reflection.TargetInvocationException)
                    ex = ex.InnerException!;

                fixture.MakeInvalid("An exception was thrown while loading the test." + Environment.NewLine + ex.ToString());

                return fixture;
            }
        }

        #endregion

        #region Helper Methods

        private TestSuite BuildMultipleFixtures(ITypeInfo typeInfo, IEnumerable<TestSuite> fixtures)
        {
            TestSuite suite = new ParameterizedFixtureSuite(typeInfo);

            foreach (var fixture in fixtures)
                suite.Add(fixture);

            return suite;
        }

        /// <summary>
        /// We look for attributes implementing IFixtureBuilder at one level
        /// of inheritance at a time. Attributes on base classes are not used
        /// unless there are no fixture builder attributes at all on the derived
        /// class. This is by design.
        /// </summary>
        /// <param name="typeInfo">The type being examined for attributes</param>
        private IFixtureBuilder[] GetFixtureBuilderAttributes(ITypeInfo? typeInfo)
        {
            IFixtureBuilder[] attrs = Array.Empty<IFixtureBuilder>();

            while (typeInfo != null && !typeInfo.IsType(typeof(object)))
            {
                attrs = typeInfo.GetCustomAttributes<IFixtureBuilder>(false);

                if (attrs.Length > 0)
                {
                    // We want to eliminate duplicates that have no args.
                    // If there is just one, no duplication is possible.
                    if (attrs.Length == 1)
                        return attrs;

                    // Count how many have arguments
                    int withArgs = 0;
                    foreach (var attr in attrs)
                        if (HasArguments(attr))
                            withArgs++;

                    // If all have args, just return them
                    if (withArgs == attrs.Length)
                        return attrs;

                    // If none of them have args, return the first one
                    if (withArgs == 0)
                        return new[] { attrs[0] };

                    // Some of each - extract those with args
                    var result = new IFixtureBuilder[withArgs];
                    int count = 0;
                    foreach (var attr in attrs)
                        if (HasArguments(attr))
                            result[count++] = attr;

                    return result;
                }

                typeInfo = typeInfo.BaseType;
            }

            return attrs;
        }

        private bool HasArguments(IFixtureBuilder attr)
        {
            // Only TestFixtureAttribute can be used without arguments
            var temp = attr as TestFixtureAttribute;

            return temp == null || temp.Arguments.Length > 0 || temp.TypeArgs.Length > 0;
        }

        #endregion
    }
}
