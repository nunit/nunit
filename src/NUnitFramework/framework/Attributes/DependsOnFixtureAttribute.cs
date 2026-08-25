// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Specifies that a test fixture must run after another fixture.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class DependsOnFixtureAttribute : NUnitAttribute, IApplyToTest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependsOnFixtureAttribute"/> class.
        /// </summary>
        /// <param name="dependantFixture">The test fixture that must finish before this fixture starts.</param>
        public DependsOnFixtureAttribute(Type dependantFixture)
        {
            ArgumentNullException.ThrowIfNull(dependantFixture);
            DependantFixture = dependantFixture;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the dependent fixture is allowed to fail without affecting this fixture.
        /// </summary>
        public bool AllowFailure { get; set; } = false;

        /// <summary>
        /// Gets the test fixture that must finish before this fixture starts.
        /// </summary>
        public Type DependantFixture { get; }

        /// <summary>
        /// Applies dependency metadata to a test.
        /// </summary>
        /// <param name="test">The test.</param>
        public void ApplyToTest(Test test)
        {
            test.Properties.Set(PropertyNames.DependsOnFixture, DependantFixture);
            test.Properties.Set(PropertyNames.DependsOnAllowFailure, AllowFailure);
        }
    }
}
