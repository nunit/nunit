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
    public sealed class DependsOnAttribute : NUnitAttribute, IApplyToTest, IApplyToTestSuite
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependsOnAttribute"/> class.
        /// </summary>
        /// <param name="dependencyType">The type of fixture that must finish before this fixture starts.</param>
        public DependsOnAttribute(Type dependencyType)
        {
            DependencyType = dependencyType;
        }

        /// <summary>
        /// Gets the type of fixture that must finish before this fixture starts.
        /// </summary>
        public Type DependencyType { get; }

        /// <summary>
        /// Applies dependency metadata to a test.
        /// </summary>
        /// <param name="test">The test.</param>
        public void ApplyToTest(Test test)
        {
            test.Properties.Set(PropertyNames.DependsOn, DependencyType);
        }

        /// <summary>
        /// Applies dependency metadata to the fixture suite.
        /// </summary>
        /// <param name="testSuite">The fixture suite.</param>
        public void ApplyToTestSuite(TestSuite testSuite)
        {
            ApplyToTest(testSuite);
        }
    }
}
