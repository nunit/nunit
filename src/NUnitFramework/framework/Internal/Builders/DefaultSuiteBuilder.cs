// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Builders
{
    /// <summary>
    /// Built-in SuiteBuilder for all types of test classes.
    /// </summary>
    public class DefaultSuiteBuilder : ISuiteBuilder
    {
        // Builder we use for fixtures without any fixture attribute specified
        private readonly TestFixtureAttribute[] _defaultBuilders = [new()];

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
            {
                return typeInfo.Type.DeclaringType is not null &&
                       typeInfo.Type.DeclaringType.HasAttribute<IFixtureBuilder>(true);
            }

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
            var suites = new List<TestSuite>();

            try
            {
                IFixtureBuilder[] builders = GetFixtureBuilderAttributes(typeInfo);
                Type[]?[] outerTypeArgs = NUnitTestFixtureBuilder.NoOuterTypeArgs;

                if (typeInfo.Type.DeclaringType is Type declaringType && declaringType.IsGenericTypeDefinition)
                {
                    // We need to get the type parameters for the outer class from that class' attributes.
                    outerTypeArgs = GetFixtureBuilderAttributes(new TypeWrapper(declaringType))
                        .OfType<IFixtureBuilderWithParameters>()
                        .SelectMany(b => b.GetFixtureData(typeInfo))
                        .Select(declaringType.GetTypeArgumentsFromArguments)
                        .ToArray();
                }

                foreach (var builder in builders)
                {
                    IEnumerable<TestSuite> fixtures = builder switch
                    {
                        // A nested generic aware builder, accepting builder information from the outer type.
                        IFixtureBuilderForNestedGeneric nestedGenericBuilder => nestedGenericBuilder.BuildFrom(typeInfo, filter, outerTypeArgs),
                        // An enhanced attribute, accepting a filter
                        IFixtureBuilderWithFilter filteringBuilder => filteringBuilder.BuildFrom(typeInfo, filter),
                        _ => builder.BuildFrom(typeInfo),
                    };

                    foreach (var fixture in fixtures)
                        suites.Add(fixture);
                }

                if (typeInfo.IsGenericType || suites.Count > 1)
                    return BuildMultipleFixtures(typeInfo, suites);

                return suites[0];
            }
            catch (Exception ex)
            {
                var fixture = new TestFixture(typeInfo, ex.Unwrap());

                return fixture;
            }
        }

        #endregion

        #region Helper Methods

        private static TestSuite BuildMultipleFixtures(ITypeInfo typeInfo, IEnumerable<TestSuite> fixtures)
        {
            TestSuite suite = new ParameterizedFixtureSuite(typeInfo);

            suite.ApplyAttributesToTestSuite(typeInfo.Type);

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
            while (typeInfo is not null && !typeInfo.IsType(typeof(object)))
            {
                IFixtureBuilder[] attrs = typeInfo.GetCustomAttributes<IFixtureBuilder>(false);

                if (attrs.Length > 0)
                {
                    // We want to eliminate duplicates that have no args.
                    // If there is just one, no duplication is possible.
                    if (attrs.Length == 1)
                        return attrs;

                    // Count how many have arguments
                    int withArgs = 0;
                    foreach (var attr in attrs)
                    {
                        if (HasArguments(attr))
                            withArgs++;
                    }

                    // If all have args, just return them
                    if (withArgs == attrs.Length)
                        return attrs;

                    // If none of them have args, return the first one
                    if (withArgs == 0)
                        return [attrs[0]];

                    // Some of each - extract those with args
                    var result = new IFixtureBuilder[withArgs];
                    int count = 0;
                    foreach (var attr in attrs)
                    {
                        if (HasArguments(attr))
                            result[count++] = attr;
                    }

                    return result;
                }

                typeInfo = typeInfo.BaseType;
            }

            // If no fixture builder attributes were found, we return the default builder,
            // which is a TestFixtureAttribute with no arguments.
            return _defaultBuilders;
        }

        private static bool HasArguments(IFixtureBuilder attr)
        {
            // Only TestFixtureAttribute can be used without arguments
            return attr is not TestFixtureAttribute fixture || fixture.Arguments.Length > 0 || fixture.TypeArgs.Length > 0;
        }

        #endregion
    }
}
