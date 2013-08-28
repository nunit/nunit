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
using NUnit.Framework.Api;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Builders
{
	/// <summary>
	/// SetUpFixtureBuilder knows how to build a SetUpFixture.
	/// </summary>
	public class SetUpFixtureBuilder : Extensibility.ISuiteBuilder
	{	
		#region ISuiteBuilder Members
        /// <summary>
        /// Build a TestSuite from type provided.
        /// </summary>
        /// <param name="type">The type of the fixture to be used</param>
        /// <returns>A TestSuite</returns>
		public Test BuildFrom(Type type)
		{
			SetUpFixture fixture = new SetUpFixture( type );

            if (fixture.RunState != RunState.NotRunnable)
            {
                string reason = null;
                if (!IsValidFixtureType(type, ref reason))
                {
                    fixture.RunState = RunState.NotRunnable;
                    fixture.Properties.Set(PropertyNames.SkipReason, reason);
                }
            }

            return fixture;
		}

        /// <summary>
        /// Examine the type and determine if it is suitable for
        /// this builder to use in building a TestSuite.
        /// Note that returning false will cause the type to be ignored
        /// in loading the tests. If it is desired to load the suite
        /// but label it as non-runnable, ignored, etc., then this
        /// method must return true.
        /// </summary>
        /// <param name="type">The type of the fixture to be used</param>
        /// <returns>
        /// True if the type can be used to build a TestSuite
        /// </returns>
		public bool CanBuildFrom(Type type)
		{
			return type.IsDefined(typeof(SetUpFixtureAttribute), false );
		}
		#endregion

        #region Helper Methods

        private bool IsValidFixtureType(Type type, ref string reason)
        {
            if (type.IsAbstract)
            {
                reason = string.Format("{0} is an abstract class", type.FullName);
                return false;
            }

            if (type.GetConstructor(new Type[0]) == null)
            {
                reason = string.Format("{0} does not have a valid constructor", type.FullName);
                return false;
            }

            if ( Reflect.HasMethodWithAttribute(type, typeof(NUnit.Framework.TestFixtureSetUpAttribute)) )
            {
                reason = "TestFixtureSetUp method not allowed on a SetUpFixture";
                return false;
            }

            if ( Reflect.HasMethodWithAttribute(type, typeof(NUnit.Framework.TestFixtureTearDownAttribute)) )
            {
                reason = "TestFixtureTearDown method not allowed on a SetUpFixture";
                return false;
            }

            return true;
        }
        #endregion
    }
}
