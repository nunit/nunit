// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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
        private NUnitTestFixtureBuilder _defaultBuilder = new NUnitTestFixtureBuilder();

        #region ISuiteBuilder Methods
        /// <summary>
        /// Checks to see if the provided Type is a fixture.
        /// To be considered a fixture, it must be a non-abstract
        /// class with one or more attributes implementing the
        /// IFixtureBuilder interface or one or more methods
        /// marked as tests.
        /// </summary>
        /// <param name="typeInfo">The fixture type to check</param>
        /// <returns>True if the fixture can be built, false if not</returns>
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
        /// Build a TestSuite from TypeInfo provided.
        /// </summary>
        /// <param name="typeInfo">The fixture type to build</param>
        /// <returns>A TestSuite built from that type</returns>
        public TestSuite BuildFrom(ITypeInfo typeInfo)
        {
            var fixtures = new List<TestSuite>();

            try
            {
                IFixtureBuilder[] builders = GetFixtureBuilderAttributes(typeInfo);

                foreach (var builder in builders)
                    foreach (var fixture in builder.BuildFrom(typeInfo))
                        fixtures.Add(fixture);

                if (typeInfo.IsGenericType)
                    return BuildMultipleFixtures(typeInfo, fixtures);

                switch (fixtures.Count)
                {
                    case 0:
                        return _defaultBuilder.BuildFrom(typeInfo);
                    case 1:
                        return fixtures[0];
                    default:
                        return BuildMultipleFixtures(typeInfo, fixtures);
                }
            }
            catch (Exception ex)
            {
                var fixture = new TestFixture(typeInfo);
                fixture.RunState = RunState.NotRunnable;

                if (ex is System.Reflection.TargetInvocationException)
                    ex = ex.InnerException;
                var msg = "An exception was thrown while loading the test." + Env.NewLine + ex.ToString();
                fixture.Properties.Add(PropertyNames.SkipReason, msg);

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
        /// <returns>A list of the attributes found.</returns>
        private IFixtureBuilder[] GetFixtureBuilderAttributes(ITypeInfo typeInfo)
        {
            IFixtureBuilder[] attrs = new IFixtureBuilder[0];

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
                        return new IFixtureBuilder[] { attrs[0] };
                    
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
