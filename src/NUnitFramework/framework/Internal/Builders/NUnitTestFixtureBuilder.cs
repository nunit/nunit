// ***********************************************************************
// Copyright (c) 2015 Charlie Poole, Rob Prouse
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
using System.Linq;
using System.Reflection;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Builders
{
    /// <summary>
    /// NUnitTestFixtureBuilder is able to build a fixture given
    /// a class marked with a TestFixtureAttribute or an unmarked
    /// class containing test methods. In the first case, it is
    /// called by the attribute and in the second directly by
    /// NUnitSuiteBuilder.
    /// </summary>
    public class NUnitTestFixtureBuilder
    {
        #region Messages

        const string NO_TYPE_ARGS_MSG =
            "Fixture type contains generic parameters. You must either provide Type arguments or specify constructor arguments that allow NUnit to deduce the Type arguments.";

        const string PARALLEL_NOT_ALLOWED_MSG =
            "ParallelizableAttribute is only allowed on test methods and fixtures";

        #endregion

        #region Instance Fields

        private readonly ITestCaseBuilder _testBuilder = new DefaultTestCaseBuilder();

        #endregion

        #region Public Methods

        /// <summary>
        /// Build a TestFixture from type provided. A non-null TestSuite
        /// must always be returned, since the method is generally called
        /// because the user has marked the target class as a fixture.
        /// If something prevents the fixture from being used, it should
        /// be returned nonetheless, labeled as non-runnable.
        /// </summary>
        /// <param name="typeInfo">An ITypeInfo for the fixture to be used.</param>
        /// <param name="filter">Filter used to select methods as tests.</param>
        /// <returns>A TestSuite object or one derived from TestSuite.</returns>
        // TODO: This should really return a TestFixture, but that requires changes to the Test hierarchy.
        public TestSuite BuildFrom(ITypeInfo typeInfo, IPreFilter filter)
        {
            var fixture = new TestFixture(typeInfo);

            if (fixture.RunState != RunState.NotRunnable)
                CheckTestFixtureIsValid(fixture);

            fixture.ApplyAttributesToTest(new AttributeProviderWrapper<FixtureLifeCycleAttribute>(typeInfo.Type.GetTypeInfo().Assembly));
            fixture.ApplyAttributesToTest(typeInfo.Type.GetTypeInfo());

            AddTestCasesToFixture(fixture, filter);

            return fixture;
        }

        /// <summary>
        /// Overload of BuildFrom called by tests that have arguments.
        /// Builds a fixture using the provided type and information
        /// in the ITestFixtureData object.
        /// </summary>
        /// <param name="typeInfo">The TypeInfo for which to construct a fixture.</param>
        /// <param name="filter">Filter used to select methods as tests.</param>
        /// <param name="testFixtureData">An object implementing ITestFixtureData or null.</param>
        /// <returns></returns>
        public TestSuite BuildFrom(ITypeInfo typeInfo, IPreFilter filter, ITestFixtureData testFixtureData)
        {
            Guard.ArgumentNotNull(testFixtureData, nameof(testFixtureData));

            object?[] arguments = testFixtureData.Arguments;

            if (typeInfo.ContainsGenericParameters)
            {
                Type[]? typeArgs = testFixtureData.TypeArgs;
                if (typeArgs == null || typeArgs.Length == 0)
                {
                    int cnt = 0;
                    foreach (object? o in arguments)
                        if (o is Type) cnt++;
                        else break;

                    typeArgs = new Type[cnt];
                    for (int i = 0; i < cnt; i++)
                        typeArgs[i] = (Type)arguments[i]!;

                    if (cnt > 0)
                    {
                        object?[] args = new object?[arguments.Length - cnt];
                        for (int i = 0; i < args.Length; i++)
                            args[i] = arguments[cnt + i];

                        arguments = args;
                    }
                }

                if (typeArgs.Length > 0 ||
                    TypeHelper.CanDeduceTypeArgsFromArgs(typeInfo.Type, arguments, ref typeArgs))
                {
                    typeInfo = typeInfo.MakeGenericType(typeArgs);
                }
            }

            var fixture = new TestFixture(typeInfo, arguments);

            string name = fixture.Name;

            if (testFixtureData.TestName != null)
            {
                fixture.Name = testFixtureData.TestName;
            }
            else
            {
                var argDisplayNames = (testFixtureData as TestParameters)?.ArgDisplayNames;
                if (argDisplayNames != null)
                {
                    fixture.Name = typeInfo.GetDisplayName();
                    if (argDisplayNames.Length != 0)
                        fixture.Name += '(' + string.Join(", ", argDisplayNames) + ')';
                }
                else if (arguments != null && arguments.Length > 0)
                {
                    fixture.Name = typeInfo.GetDisplayName(arguments);
                }
            }

            if (fixture.Name != name) // name was changed
            {
                string nspace = typeInfo.Namespace;
                fixture.FullName = nspace != null && nspace != ""
                    ? nspace + "." + fixture.Name
                    : fixture.Name;
            }

            if (fixture.RunState != RunState.NotRunnable)
                fixture.RunState = testFixtureData.RunState;

            foreach (string key in testFixtureData.Properties.Keys)
                foreach (object val in testFixtureData.Properties[key])
                    fixture.Properties.Add(key, val);

            if (fixture.RunState != RunState.NotRunnable)
                CheckTestFixtureIsValid(fixture);

            AddTestCasesToFixture(fixture, filter);

            return fixture;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Method to add test cases to the newly constructed fixture.
        /// </summary>
        private void AddTestCasesToFixture(TestFixture fixture, IPreFilter filter)
        {
            // TODO: Check this logic added from Neil's build.
            if (fixture.TypeInfo.ContainsGenericParameters)
            {
                fixture.MakeInvalid(NO_TYPE_ARGS_MSG);
                return;
            }

            var methods = fixture.TypeInfo.GetMethods(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            foreach (IMethodInfo method in methods)
            {
                if (filter.IsMatch(fixture.TypeInfo.Type, method.MethodInfo))
                {
                    Test? test = BuildTestCase(method, fixture);

                    if (test != null)
                        fixture.Add(test);
                    else // it's not a test, check for disallowed attributes
                        if (method.MethodInfo.HasAttribute<ParallelizableAttribute>(false))
                        fixture.MakeInvalid(PARALLEL_NOT_ALLOWED_MSG);
                }
            }
        }

        /// <summary>
        /// Method to create a test case from a MethodInfo and add
        /// it to the fixture being built. It first checks to see if
        /// any global TestCaseBuilder addin wants to build the
        /// test case. If not, it uses the internal builder
        /// collection maintained by this fixture builder.
        ///
        /// The default implementation has no test case builders.
        /// Derived classes should add builders to the collection
        /// in their constructor.
        /// </summary>
        /// <param name="method">The method for which a test is to be created</param>
        /// <param name="suite">The test suite being built.</param>
        /// <returns>A newly constructed Test</returns>
        private Test? BuildTestCase(IMethodInfo method, TestSuite suite)
        {
            return _testBuilder.CanBuildFrom(method, suite)
                ? _testBuilder.BuildFrom(method, suite)
                : null;
        }

        private static void CheckTestFixtureIsValid(TestFixture fixture)
        {
            if (fixture.TypeInfo.ContainsGenericParameters)
            {
                fixture.MakeInvalid(NO_TYPE_ARGS_MSG);
            }
            else if (!fixture.TypeInfo.IsStaticClass)
            {
                Type?[] argTypes = Reflect.GetTypeArray(fixture.Arguments);

                if (!Reflect.GetConstructors(fixture.TypeInfo.Type, argTypes).Any())
                {
                    fixture.MakeInvalid("No suitable constructor was found");
                }
            }
        }

        #endregion
    }
}
