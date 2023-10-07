// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Specify the life cycle of a Fixture
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class FixtureLifeCycleAttribute : NUnitAttribute, IApplyToTest
    {
        /// <summary>
        /// Construct a FixtureLifeCycleAttribute with a specified <see cref="LifeCycle"/>.
        /// </summary>
        public FixtureLifeCycleAttribute(LifeCycle lifeCycle)
        {
            LifeCycle = lifeCycle;
        }

        /// <summary>
        /// Defines the life cycle for this test fixture or assembly.
        /// </summary>
        public LifeCycle LifeCycle { get; }

        /// <summary>
        /// Overridden to set a TestFixture's <see cref="LifeCycle"/>.
        /// </summary>
        public void ApplyToTest(Test test)
        {
            if (test is TestFixture testFixture)
            {
                testFixture.LifeCycle = LifeCycle;
            }
        }
    }
}
