// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Specifies that a test fixture must run after another fixture.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class DependsOnAttribute : NUnitAttribute, IApplyToTest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependsOnAttribute"/> class.
        /// </summary>
        /// <param name="dependantFixture">The test fixture that must finish before this fixture starts.</param>
        public DependsOnAttribute(Type dependantFixture)
        {
            ArgumentNullException.ThrowIfNull(dependantFixture);
            DependantFixture = dependantFixture;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependsOnAttribute"/> class.
        /// </summary>
        /// <param name="methodName">The name of the test method that must finish before this test method starts.</param>
        public DependsOnAttribute(string methodName)
        {
            ArgumentNullException.ThrowIfNull(methodName);
            DependantMethod = methodName;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the dependent fixture is allowed to fail without affecting this fixture.
        /// </summary>
        public bool AllowFailure { get; set; } = false;

        /// <summary>
        /// Gets the test fixture that must finish before this fixture starts.
        /// </summary>
        public Type? DependantFixture { get; }

        /// <summary>
        /// Gets the test method that must finish before this test method starts.
        /// </summary>
        public string? DependantMethod { get; }

        /// <summary>
        /// Applies dependency metadata to a test.
        /// </summary>
        /// <param name="test">The test.</param>
        public void ApplyToTest(Test test)
        {
            if (DependantFixture is not null)
            {
                if (test is TestMethod)
                {
                    test.MakeInvalid("DependsOnAttribute Type constructor may only be used on fixtures.");
                    return;
                }

                test.Properties.Set(PropertyNames.DependsOnFixture, DependantFixture);
            }
            else if (DependantMethod is not null)
            {
                if (test is not TestMethod)
                {
                    test.MakeInvalid("DependsOnAttribute string constructor may only be used on methods.");
                    return;
                }

                test.Properties.Set(PropertyNames.DependsOnMethod, DependantMethod);
            }

            test.Properties.Set(PropertyNames.DependsOnAllowFailure, AllowFailure);
        }
    }
}
