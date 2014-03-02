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
using System.Collections;
using System.Reflection;
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
    public class NUnitTestFixtureBuilder : IFixtureBuilder
    {
        #region Static Fields

        static readonly string NO_TYPE_ARGS_MSG =
            "Fixture type contains generic parameters. You must either provide " +
            "Type arguments or specify constructor arguments that allow NUnit " +
            "to deduce the Type arguments.";

        #endregion

        #region Instance Fields

        /// <summary>
        /// The NUnitTestFixture being constructed;
        /// </summary>
        private TestFixture fixture;

        private ITestCaseBuilder testBuilder = new DefaultTestCaseBuilder();

        #endregion

        #region IFixtureBuilder Members

        /// <summary>
        /// Build a TestFixture from type provided. A non-null TestSuite
        /// must always be returned, since the method is generally called
        /// because the user has marked the target class as a fixture.
        /// If something prevents the fixture from being used, it should
        /// be returned nonetheless, labelled as non-runnable.
        /// </summary>
        /// <param name="type">The type of the fixture to be used.</param>
        /// <returns>A TestSuite object or one derived from TestSuite.</returns>
        // TODO: This should really return a TestFixture, but that requires changes to the Test hierarchy.
        public TestSuite BuildFrom(Type type)
        {
            this.fixture = new TestFixture(type);

            if (fixture.RunState != RunState.NotRunnable)
                CheckTestFixtureIsValid(fixture);

            fixture.ApplyAttributesToTest(type);


            AddTestCases(type);

            return this.fixture;
        }

        #endregion

        #region Other Public Methods

        /// <summary>
        /// Overload of BuildFrom called by TestFixtureAttribute. Builds
        /// a fixture using the provided type and information in the
        /// properties of the attribute.
        /// </summary>
        /// <param name="type">The Type for which to construct a fixture.</param>
        /// <param name="attr">The attribute marking the fixture Type.</param>
        /// <returns></returns>
        public TestSuite BuildFrom(Type type, TestFixtureAttribute attr)
        {
            object[] arguments = null;

            if (attr != null)
            {
                arguments = attr.Arguments;

#if !NETCF
                if (type.ContainsGenericParameters)
                {
                    Type[] typeArgs = attr.TypeArgs;
                    if (typeArgs.Length == 0)
                    {
                        int cnt = 0;
                        foreach (object o in arguments)
                            if (o is Type) cnt++;
                            else break;

                        typeArgs = new Type[cnt];
                        for (int i = 0; i < cnt; i++)
                            typeArgs[i] = (Type)arguments[i];

                        if (cnt > 0)
                        {
                            object[] args = new object[arguments.Length - cnt];
                            for (int i = 0; i < args.Length; i++)
                                args[i] = arguments[cnt + i];

                            arguments = args;
                        }
                    }

                    if (typeArgs.Length > 0 ||
                        TypeHelper.CanDeduceTypeArgsFromArgs(type, arguments, ref typeArgs))
                    {
                        type = TypeHelper.MakeGenericType(type, typeArgs);
                    }
                }
#endif
            }

            this.fixture = new TestFixture(type);
            
            if (arguments != null)
            {
                string name = fixture.Name = TypeHelper.GetDisplayName(type, arguments);
                string nspace = type.Namespace;
                fixture.FullName = nspace != null && nspace != ""
                    ? nspace + "." + name
                    : name;
                fixture.Arguments = arguments;
            }

            if (fixture.RunState != RunState.NotRunnable)
                CheckTestFixtureIsValid(fixture);

            fixture.ApplyAttributesToTest(type);

            AddTestCases(type);

            return this.fixture;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Method to add test cases to the newly constructed fixture.
        /// </summary>
        /// <param name="fixtureType"></param>
        private void AddTestCases(Type fixtureType)
        {
            IList methods = fixtureType.GetMethods(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            foreach (MethodInfo method in methods)
            {
                Test test = BuildTestCase(method, this.fixture);

                if (test != null)
                {
                    this.fixture.Add(test);
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
        /// <param name="method">The MethodInfo for which a test is to be created</param>
        /// <param name="suite">The test suite being built.</param>
        /// <returns>A newly constructed Test</returns>
        private Test BuildTestCase(MethodInfo method, TestSuite suite)
        {
            return testBuilder.CanBuildFrom(method, suite)
                ? testBuilder.BuildFrom(method, suite)
                : null;
        }

        private void CheckTestFixtureIsValid(TestFixture fixture)
        {
            Type fixtureType = fixture.FixtureType;

            if (fixtureType.ContainsGenericParameters)
            {
                fixture.RunState = RunState.NotRunnable;
                fixture.Properties.Set(PropertyNames.SkipReason, NO_TYPE_ARGS_MSG);
            }
            else if (!IsStaticClass(fixtureType))
            {
                object[] args = fixture.Arguments;

                Type[] argTypes;

                // Note: This could be done more simply using
                // Type.EmptyTypes and Type.GetTypeArray() but
                // they don't exist in all runtimes we support.
                if (args == null)
                    argTypes = new Type[0];
                else
                {
                    argTypes = new Type[args.Length];

                    int index = 0;
                    foreach (object arg in args)
                        argTypes[index++] = arg.GetType();
                }

                ConstructorInfo ctor = fixtureType.GetConstructor(argTypes);

                if (ctor == null)
                {
                    fixture.RunState = RunState.NotRunnable;
                    fixture.Properties.Set(PropertyNames.SkipReason, "No suitable constructor was found");
                }
            }
        }

        private static bool IsStaticClass(Type type)
        {
            return type.IsAbstract && type.IsSealed;
        }

        #endregion
    }
}
