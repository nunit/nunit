// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Specifies that a test must run after another test.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class DependsOnTestAttribute : NUnitAttribute, IApplyToTest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependsOnTestAttribute"/> class.
        /// </summary>
        /// <param name="testName">The name of the test that must finish before this test starts.</param>
        public DependsOnTestAttribute(string testName)
        {
            ArgumentNullException.ThrowIfNull(testName);
            DependantTest = testName;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the dependent test is allowed to fail without affecting this test.
        /// </summary>
        public bool AllowFailure { get; set; } = false;

        /// <summary>
        /// Gets the name of the test that must finish before this test starts.
        /// </summary>
        public string DependantTest { get; }

        /// <summary>
        /// Applies dependency metadata to a test.
        /// </summary>
        /// <param name="test">The test.</param>
        public void ApplyToTest(Test test)
        {
            test.Properties.Set(PropertyNames.DependsOnTest, DependantTest);
            test.Properties.Set(PropertyNames.DependsOnAllowFailure, AllowFailure);
        }
    }
}
