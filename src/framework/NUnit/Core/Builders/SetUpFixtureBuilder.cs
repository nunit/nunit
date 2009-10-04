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
using NUnit.Framework;

namespace NUnit.Core.Builders
{
	/// <summary>
	/// SetUpFixtureBuilder knows how to build a SetUpFixture.
	/// </summary>
	public class SetUpFixtureBuilder : Extensibility.ISuiteBuilder
	{	
		#region ISuiteBuilder Members
		public Test BuildFrom(Type type)
		{
			SetUpFixture fixture = new SetUpFixture( type );

            string reason = null;
            if (!IsValidFixtureType(type, ref reason))
            {
                fixture.RunState = RunState.NotRunnable;
                fixture.IgnoreReason = reason;
            }

            return fixture;
		}

		public bool CanBuildFrom(Type type)
		{
			return type.IsDefined(typeof(SetUpFixtureAttribute), false );
		}
		#endregion

        private bool IsValidFixtureType(Type type, ref string reason)
        {
            if (type.IsAbstract)
            {
                reason = string.Format("{0} is an abstract class", type.FullName);
                return false;
            }

            if (Reflect.GetConstructor(type) == null)
            {
                reason = string.Format("{0} does not have a valid constructor", type.FullName);
                return false;
            }

            if (!NUnitFramework.CheckSetUpTearDownMethods(type, NUnitFramework.SetUpAttribute, ref reason) ||
                !NUnitFramework.CheckSetUpTearDownMethods(type, NUnitFramework.TearDownAttribute, ref reason) )
                    return false;

            if ( Reflect.HasMethodWithAttribute(type, NUnitFramework.FixtureSetUpAttribute, true) )
            {
                reason = "TestFixtureSetUp method not allowed on a SetUpFixture";
                return false;
            }

            if ( Reflect.HasMethodWithAttribute(type, NUnitFramework.FixtureTearDownAttribute, true) )
            {
                reason = "TestFixtureTearDown method not allowed on a SetUpFixture";
                return false;
            }

            return true;
        }
	}
}
