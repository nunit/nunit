// ***********************************************************************
// Copyright (c) 2014 Charlie Poole, Rob Prouse
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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Compatibility;

namespace NUnit.Framework
{
    using System.Diagnostics.CodeAnalysis;
    using Interfaces;
    using Internal;

    /// <summary>
    /// Identifies a class as containing <see cref="OneTimeSetUpAttribute" /> or
    /// <see cref="OneTimeTearDownAttribute" /> methods for all the test fixtures
    /// under a given namespace.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SetUpFixtureAttribute : NUnitAttribute, IFixtureBuilder
    {
        #region ISuiteBuilder Members

        /// <summary>
        /// Builds a <see cref="SetUpFixture"/> from the specified type.
        /// </summary>
        /// <param name="typeInfo">The type info of the fixture to be used.</param>
        public IEnumerable<TestSuite> BuildFrom(ITypeInfo typeInfo)
        {
            SetUpFixture fixture = new SetUpFixture(typeInfo);

            if (fixture.RunState != RunState.NotRunnable)
            {
                string? reason = null;
                if (!IsValidFixtureType(typeInfo, ref reason))
                    fixture.MakeInvalid(reason);
            }

            fixture.ApplyAttributesToTest(typeInfo.Type.GetTypeInfo());

            return new TestSuite[] { fixture };
        }

        #endregion

        #region Helper Methods

        private static bool IsValidFixtureType(ITypeInfo typeInfo, [NotNullWhen(false)] ref string? reason)
        {
            if (!typeInfo.IsStaticClass)
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
            }

            var invalidAttributes = new Type[] {
                typeof(SetUpAttribute),
                typeof(TearDownAttribute)
            };

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
