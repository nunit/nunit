// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Attributes
{
    /// <summary>
    /// Indicates the default status of a parameterized test method or test fixture containing no executable child tests.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class NoTestsAttribute : PropertyAttribute, IApplyToTest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoTestsAttribute"/> class
        /// with the specified default <see cref="TestStatus"/>.
        /// </summary>
        /// <param name="defaultStatus">The default <see cref="TestStatus"/> to assign to tests with no executable children.</param>
        public NoTestsAttribute(TestStatus defaultStatus)
        {
            Properties.Add(PropertyNames.NoTests, defaultStatus);
        }
    }
}
