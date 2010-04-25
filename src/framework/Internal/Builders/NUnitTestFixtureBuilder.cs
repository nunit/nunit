// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using System.Text.RegularExpressions;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Api;
using NUnit.Framework.Internal;

namespace NUnit.Core.Builders
{
	/// <summary>
	/// Built-in SuiteBuilder for NUnit TestFixture
	/// </summary>
	public class NUnitTestFixtureBuilder : Extensibility.ISuiteBuilder
	{
		#region Instance Fields
		/// <summary>
		/// The NUnitTestFixture being constructed;
		/// </summary>
		private TestFixture fixture;

	    private Extensibility.ITestCaseBuilder2 testBuilders = CoreExtensions.Host.TestBuilders;

	    private Extensibility.ITestDecorator testDecorators = CoreExtensions.Host.TestDecorators;

		#endregion

        #region ISuiteBuilder Methods
        /// <summary>
		/// Checks to see if the fixture type has the TestFixtureAttribute
		/// </summary>
		/// <param name="type">The fixture type to check</param>
		/// <returns>True if the fixture can be built, false if not</returns>
		public bool CanBuildFrom(Type type)
		{
            if ( type.IsAbstract && !type.IsSealed )
                return false;

			return type.IsDefined(typeof(TestFixtureAttribute), true ) ||
                   ( type.IsPublic || type.IsNestedPublic ) && 
                   ( Reflect.HasMethodWithAttribute(type, typeof(NUnit.Framework.TestAttribute), true) ||
                     Reflect.HasMethodWithAttribute(type, typeof(NUnit.Framework.TestCaseAttribute), true) ||
                     Reflect.HasMethodWithAttribute(type, typeof(NUnit.Framework.TheoryAttribute), true) );
		}

		/// <summary>
		/// Build a TestSuite from type provided.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public Test BuildFrom(Type type)
		{
            TestFixtureAttribute[] attrs = 
                (TestFixtureAttribute[])type.GetCustomAttributes(typeof(TestFixtureAttribute), false);

#if CLR_2_0
            if (type.IsGenericType)
                return BuildMultipleFixtures(type, attrs);
#endif

            switch (attrs.Length)
            {
                case 0:
                    return BuildSingleFixture(type, null);
                case 1:
                    object[] args = (object[])attrs[0].Arguments;
                    return args == null || args.Length == 0
                        ? BuildSingleFixture(type, attrs[0])
                        : BuildMultipleFixtures(type, attrs);
                default:
                    return BuildMultipleFixtures(type, attrs);
            }
        }
		#endregion

		#region Helper Methods
        private Test BuildMultipleFixtures(Type type, TestFixtureAttribute[] attrs)
        {
            TestSuite suite = new ParameterizedFixtureSuite(type);

            foreach (TestFixtureAttribute attr in attrs)
                suite.Add(BuildSingleFixture(type, attr));

            return suite;
        }

        private Test BuildSingleFixture(Type type, TestFixtureAttribute attr)
        {
            object[] arguments = null;

            if (attr != null)
            {
                arguments = (object[])attr.Arguments;
#if CLR_2_0
                if (type.ContainsGenericParameters)
                {
                    Type[] typeArgs = (Type[])attr.TypeArgs;
                    if( typeArgs.Length > 0 || 
                        TypeHelper.CanDeduceTypeArgsFromArgs(type, arguments, ref typeArgs))
                    {
                        type = TypeHelper.MakeGenericType(type, typeArgs);
                    }
                }
#endif
            }

            this.fixture = new TestFixture(type, arguments);
            CheckTestFixtureIsValid(fixture);

            fixture.ApplyCommonAttributes(type);

            if (fixture.RunState == RunState.Runnable && attr != null)
            {
                if (attr.Ignore)
                {
                    fixture.RunState = RunState.Ignored;
                    fixture.IgnoreReason = attr.IgnoreReason;
                }
            }

            AddTestCases(type);

            return this.fixture;
        }

        /// <summary>
		/// Method to add test cases to the newly constructed fixture.
		/// </summary>
		/// <param name="fixtureType"></param>
		private void AddTestCases( Type fixtureType )
		{
			IList methods = fixtureType.GetMethods( 
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static );

			foreach(MethodInfo method in methods)
			{
				Test test = BuildTestCase(method, this.fixture);

				if(test != null)
				{
					this.fixture.Add( test );
				}
			}
		}

		/// <summary>
		/// Method to create a test case from a MethodInfo and add
		/// it to the fixture being built. It first checks to see if
		/// any global TestCaseBuilder addin wants to build the
		/// test case. If not, it uses the internal builder
		/// collection maintained by this fixture builder. After
		/// building the test case, it applies any decorators
		/// that have been installed.
		/// 
		/// The default implementation has no test case builders.
		/// Derived classes should add builders to the collection
		/// in their constructor.
		/// </summary>
		/// <param name="method">The MethodInfo for which a test is to be created</param>
        /// <param name="suite">The test suite being built.</param>
		/// <returns>A newly constructed Test</returns>
		private Test BuildTestCase( MethodInfo method, TestSuite suite )
		{
            Test test = testBuilders.BuildFrom( method, suite );

			if ( test != null )
				test = testDecorators.Decorate( test, method );

			return test;
		}

        private void CheckTestFixtureIsValid(TestFixture fixture)
        {
            Type fixtureType = fixture.FixtureType;
            string reason = null;

            if (fixture.RunState == RunState.NotRunnable)
                return;

            if (!IsValidFixtureType(fixtureType, ref reason))
            {
                fixture.RunState = RunState.NotRunnable;
                fixture.IgnoreReason = reason;
            }
            else if( !IsStaticClass( fixtureType ) )
            {
                object[] args = fixture.arguments;

                ConstructorInfo ctor = args == null || args.Length == 0
                    ? Reflect.GetConstructor(fixtureType)
                    : Reflect.GetConstructor(fixtureType, Type.GetTypeArray(args));

                if (ctor == null)
                {
                    fixture.RunState = RunState.NotRunnable;
                    fixture.IgnoreReason = "No suitable constructor was found";
                }
            }
        }

        private static bool IsStaticClass(Type type)
        {
            return type.IsAbstract && type.IsSealed;
        }

		/// <summary>
        /// Check that the fixture type is valid. This method ensures that 
        /// the type is not abstract and that there is no more than one of 
        /// each setup or teardown method and that their signatures are correct.
        /// </summary>
        /// <param name="fixtureType">The type of the fixture to check</param>
        /// <param name="reason">A message indicating why the fixture is invalid</param>
        /// <returns>True if the fixture is valid, false if not</returns>
        private bool IsValidFixtureType(Type fixtureType, ref string reason)
        {
#if CLR_2_0
            if ( fixtureType.ContainsGenericParameters )
            {
                reason = "Fixture type contains generic parameters. You must either provide "
                        + "Type arguments or specify constructor arguments that allow NUnit "
                        + "to deduce the Type arguments.";
                return false;
            }
#endif

            return true;
        }
		#endregion
	}
}