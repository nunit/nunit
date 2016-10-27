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

namespace NUnit.Framework
{
    using Interfaces;
    using Internal;

    /// <summary>
    /// SetUpFixtureAttribute is used to identify a SetUpFixture
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class SetUpFixtureAttribute : NUnitAttribute, IFixtureBuilder
    {
        #region ISuiteBuilder Members

        /// <summary>
        /// Build a SetUpFixture from type provided. Normally called for a Type
        /// on which the attribute has been placed.
        /// </summary>
        /// <param name="typeInfo">The type info of the fixture to be used.</param>
        /// <returns>A SetUpFixture object as a TestSuite.</returns>
        public IEnumerable<TestSuite> BuildFrom(ITypeInfo typeInfo)
        {
            SetUpFixture fixture = new SetUpFixture(typeInfo);

            if (fixture.RunState != RunState.NotRunnable)
            {
                string reason = null;
                if (!IsValidFixtureType(typeInfo, ref reason))
                {
                    fixture.RunState = RunState.NotRunnable;
                    fixture.Properties.Set(PropertyNames.SkipReason, reason);
                }
            }

            return new TestSuite[] { fixture };
        }

        #endregion

        #region Helper Methods

        private bool IsValidFixtureType(ITypeInfo typeInfo, ref string reason)
        {
            if (typeInfo.IsAbstract)
            {
                reason = string.Format("{0} is an abstract class", typeInfo.FullName);
                return false;
            }

            if (!typeInfo.HasConstructor(new Type[0]))
            {
                reason = string.Format("{0} does not have a default constructor", typeInfo.FullName);
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
                if (typeInfo.HasMethodWithAttribute(invalidType))
                {
                    reason = invalidType.Name + " attribute not allowed in a SetUpFixture";
                    return false;
                }

            return true;
        }

        #endregion
    }
}
