// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
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

            fixture.ApplyAttributesToTest(typeInfo.Type);

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
                    reason = $"{typeInfo.FullName} is an abstract class";
                    return false;
                }

                if (!typeInfo.HasConstructor(Array.Empty<Type>()))
                {
                    reason = $"{typeInfo.FullName} does not have a default constructor";
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
