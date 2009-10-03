// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;

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
		private NUnitTestFixture fixture;

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
			return Reflect.HasAttribute( type, NUnitFramework.TestFixtureAttribute, true ) ||
                   ( type.IsPublic || type.IsNestedPublic ) && 
                   ( !type.IsAbstract || type.IsSealed ) &&
                   ( Reflect.HasMethodWithAttribute(type, NUnitFramework.TestAttribute, true) ||
                     Reflect.HasMethodWithAttribute(type, NUnitFramework.TestCaseAttribute, true) ||
                     Reflect.HasMethodWithAttribute(type, NUnitFramework.TheoryAttribute, true) );
		}

		/// <summary>
		/// Build a TestSuite from type provided.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public Test BuildFrom(Type type)
		{
            Attribute[] attrs = Reflect.GetAttributes(type, NUnitFramework.TestFixtureAttribute, false);

#if CLR_2_0
            if (type.IsGenericType)
                return BuildMultipleFixtures(type, attrs);
#endif

            switch (attrs.Length)
            {
                case 0:
                    return BuildSingleFixture(type, null);
                case 1:
                    object[] args = (object[])Reflect.GetPropertyValue(attrs[0], "Arguments");
                    return args == null || args.Length == 0
                        ? BuildSingleFixture(type, attrs[0])
                        : BuildMultipleFixtures(type, attrs);
                default:
                    return BuildMultipleFixtures(type, attrs);
            }
        }
		#endregion

		#region Helper Methods
        private Test BuildMultipleFixtures(Type type, Attribute[] attrs)
        {
            TestSuite suite = new TestSuite(type.Namespace, TypeHelper.GetDisplayName(type));

            foreach (Attribute attr in attrs)
                suite.Add(BuildSingleFixture(type, attr));

            return suite;
        }

        private Test BuildSingleFixture(Type type, Attribute attr)
        {
            object[] arguments = null;

            if (attr != null)
            {
                arguments = (object[])Reflect.GetPropertyValue(attr, "Arguments");
#if CLR_2_0
                if (type.ContainsGenericParameters)
                {
                    Type[] typeArgs = (Type[])Reflect.GetPropertyValue(attr, "TypeArgs");
                    if( typeArgs.Length > 0 || 
                        TypeHelper.CanDeduceTypeArgsFromArgs(type, arguments, ref typeArgs))
                    {
                        type = TypeHelper.MakeGenericType(type, typeArgs);
                    }
                }
#endif
            }

            this.fixture = new NUnitTestFixture(type, arguments);
            CheckTestFixtureIsValid(fixture);

            NUnitFramework.ApplyCommonAttributes(type, fixture);

            if (fixture.RunState == RunState.Runnable && attr != null)
            {
                object objIgnore = Reflect.GetPropertyValue(attr, "Ignore");
                if (objIgnore != null && (bool)objIgnore == true)
                {
                    fixture.RunState = RunState.Ignored;
                    fixture.IgnoreReason = (string)Reflect.GetPropertyValue(attr, "IgnoreReason");
                }
            }

            AddTestCases(type);

            if (this.fixture.RunState != RunState.NotRunnable && this.fixture.TestCount == 0)
            {
                this.fixture.RunState = RunState.NotRunnable;
                this.fixture.IgnoreReason = fixture.TestName.Name + " does not have any tests";
            }

            return this.fixture;
        }

        /// <summary>
		/// Method to add test cases to the newly constructed fixture.
		/// The default implementation looks at each candidate method
		/// and tries to build a test case from it. It will only need
		/// to be overridden if some other approach, such as reading a 
		/// datafile is used to generate test cases.
		/// </summary>
		/// <param name="fixtureType"></param>
		protected virtual void AddTestCases( Type fixtureType )
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
		/// <param name="method"></param>
		/// <returns></returns>
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
            if (fixtureType.IsAbstract && !fixtureType.IsSealed)
            {
                reason = string.Format("{0} is an abstract class", fixtureType.FullName);
                return false;
            }

#if CLR_2_0
            if ( fixtureType.ContainsGenericParameters )
            {
                reason = "Fixture type contains generic parameters. You must either provide "
                        + "Type arguments or specify constructor arguments that allow NUnit "
                        + "to deduce the Type arguments.";
                return false;
            }
#endif

            return NUnitFramework.CheckSetUpTearDownMethods(fixtureType, NUnitFramework.SetUpAttribute, ref reason)
                && NUnitFramework.CheckSetUpTearDownMethods(fixtureType, NUnitFramework.TearDownAttribute, ref reason)
                && NUnitFramework.CheckSetUpTearDownMethods(fixtureType, NUnitFramework.FixtureSetUpAttribute, ref reason)
                && NUnitFramework.CheckSetUpTearDownMethods(fixtureType, NUnitFramework.FixtureTearDownAttribute, ref reason);
        }
		#endregion
	}
}