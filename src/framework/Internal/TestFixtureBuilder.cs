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
using System.Reflection;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
	/// <summary>
	/// TestFixtureBuilder contains static methods for building
	/// TestFixtures from types. It uses builtin SuiteBuilders
	/// and any installed extensions to do it.
	/// </summary>
	public class TestFixtureBuilder
	{
        /// <summary>
        /// Determines whether this instance [can build from] the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if this instance [can build from] the specified type; otherwise, <c>false</c>.
        /// </returns>
		public static bool CanBuildFrom( Type type )
		{
#if NUNITLITE
            if (type.IsDefined(typeof(TestFixtureAttribute), true))
                return true;

            if (!type.IsPublic && !type.IsNestedPublic)
                return false;

            if (type.IsAbstract && !type.IsSealed)
                return false;

            foreach (MethodInfo method in type.GetMethods())
            {
                if (method.IsDefined(typeof(TestAttribute), true) ||
                    method.IsDefined(typeof(TestCaseAttribute), true))
                        return true;
            }

            return false;
#else
            return CoreExtensions.Host.SuiteBuilders.CanBuildFrom( type );
#endif
		}

		/// <summary>
		/// Build a test fixture from a given type.
		/// </summary>
		/// <param name="type">The type to be used for the fixture</param>
		/// <returns>A TestSuite if the fixture can be built, null if not</returns>
		public static Test BuildFrom( Type type )
		{
#if NUNITLITE
            TestFixture suite = new TestFixture(type);

            suite.ApplyCommonAttributes(type);

            //object[] attrs = type.GetCustomAttributes(typeof(PropertyAttribute), true);
            //foreach (PropertyAttribute attr in attrs)
            //    foreach( DictionaryEntry entry in attr.Properties )
            //        suite.Properties[entry.Key] = entry.Value;

            //IgnoreAttribute ignore = (IgnoreAttribute)Reflect.GetAttribute(type, typeof(IgnoreAttribute), false);
            //if (ignore != null)
            //{
            //    suite.RunState = RunState.Ignored;
            //    suite.IgnoreReason = ignore.GetReason();
            //}

            if (!Reflect.HasConstructor(type))
            {
                suite.RunState = RunState.NotRunnable;
                suite.IgnoreReason = string.Format("Class {0} has no default constructor", type.Name);
                return suite;
            }

            foreach (MethodInfo method in type.GetMethods())
            {
                if (TestCaseBuilder.IsTestMethod(method))
                    suite.Add(TestCaseBuilder.BuildFrom(method));
            }

            return suite;
#else
            Test suite = CoreExtensions.Host.SuiteBuilders.BuildFrom( type );

			if ( suite != null )
				suite = CoreExtensions.Host.TestDecorators.Decorate( suite, type );

			return suite;
#endif
		}

#if !NUNITLITE
		/// <summary>
		/// Build a fixture from an object. 
		/// </summary>
		/// <param name="fixture">The object to be used for the fixture</param>
		/// <returns>A TestSuite if fixture type can be built, null if not</returns>
		public static Test BuildFrom( object fixture )
		{
			Test suite = BuildFrom( fixture.GetType() );
			if( suite != null)
				suite.Fixture = fixture;
			return suite;
		}
#endif

		/// <summary>
		/// Private constructor to prevent instantiation
		/// </summary>
		private TestFixtureBuilder() { }
	}
}
