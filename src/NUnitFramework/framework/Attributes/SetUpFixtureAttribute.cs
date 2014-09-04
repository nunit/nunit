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
using System.Reflection;

namespace NUnit.Framework
{
    using Interfaces;
    using Internal;

    /// <summary>
    /// SetUpFixtureAttribute is used to identify a SetUpFixture
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class SetUpFixtureAttribute : FixtureBuilderAttribute, IFixtureBuilder
    {
        #region ISuiteBuilder Members

        /// <summary>
        /// Build a SetUpFixture from type provided. Normally called for a Type
        /// on which the attribute has been placed.
        /// </summary>
        /// <param name="type">The type of the fixture to be used.</param>
        /// <returns>A SetUpFixture object as a TestSuite.</returns>
        public TestSuite BuildFrom(Type type)
        {
            SetUpFixture fixture = new SetUpFixture(type);

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

        #endregion

        #region Helper Methods

        private bool IsValidFixtureType(Type fixtureType, ref string reason)
        {
            if (fixtureType.IsAbstract)
            {
                reason = string.Format("{0} is an abstract class", fixtureType.FullName);
                return false;
            }

            if (fixtureType.GetConstructor(new Type[0]) == null)
            {
                reason = string.Format("{0} does not have a valid constructor", fixtureType.FullName);
                return false;
            }

            var invalidAttributes = new Type[] { 
                typeof(SetUpAttribute), 
                typeof(TearDownAttribute),
#pragma warning disable 618 // Obsolete Attributes
                typeof(TestFixtureSetUpAttribute), 
                typeof(TestFixtureTearDownAttribute) };
#pragma warning restore

            foreach (Type invalidType in invalidAttributes)
                if (Reflect.HasMethodWithAttribute(fixtureType, invalidType))
                {
                    reason = invalidType.Name + " attribute not allowed in a SetUpFixture";
                    return false;
                }

            return true;
        }

        #endregion
    }
}
